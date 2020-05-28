using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

public static class TaskManager {

  public static bool workersAreRunning => workers != null;

  static ConcurrentPriorityQueue<float, Task> tasks = new ConcurrentPriorityQueue<float, Task>();

  static ManualResetEvent tasksAvailableEvent = new ManualResetEvent(false);

  static bool joinRequested;

  static Thread[] workers;
  static object[] workerLocks;

  public static Task<T> Schedule<T>(Func<T> f, float priority = 0) {
    Task<T> task = new Task<T>(f, priority);
    task.Schedule();
    return task;
  }

  public static Task Schedule(Action f, float priority = 0) {
    Task task = new Task(f, priority);
    task.Schedule();
    return task;
  }

  public static T Schedule<T>(T task, float priority = 0) where T : Task {
    if (task == null) throw new ArgumentException();
    tasks.Enqueue(priority, task);
    tasksAvailableEvent.Set();
    return task;
  }

  public static void StartWorkers(int n = 8) {
    JoinWorkers();
    workers = new Thread[n];
    workerLocks = new object[n];
    joinRequested = false;
    tasksAvailableEvent.Reset();
    for (int i = 0; i < n; i ++) {
      Thread worker = new Thread(Worker);
      workers[i] = worker;
      workerLocks[i] = new object();
      worker.Start();
    }
  }

  public static void JoinWorkers() {
    if (workers != null) {
      joinRequested = true;
      for (int i = 0; i < workers.Length; i ++) {
        lock(workerLocks[i]) {
          workers[i].Join();
        }
      }
      workers = null;
      workerLocks = null;
    }
  }

  public static void AbortWorkers() {
    if (workers != null) {
      for (int i = 0; i < workers.Length; i ++) {
        lock(workerLocks[i]) {
          workers[i].Abort();
        }
      }
      workers = null;
      workerLocks = null;
    }
  }

  static void Worker() {
    while (true) {
      KeyValuePair<float, Task> pair;
      while (tasks.TryDequeue(out pair)) {
        Task task = pair.Value;
        task.Run();
        foreach (Task dependant in task.dependants) {
          dependant.dependencies.Remove(task);
          dependant.Schedule();
        }
      }
      if (joinRequested) {
        return;
      }
      else {
        tasksAvailableEvent.Reset();
        tasksAvailableEvent.WaitOne();
      }
    }
  }

}

public class Task<T> : Task {

  public T result { get; private set; }

  public Task(Func<T> task, float priority = 0) : base(() => {}, priority) {
    if (task == null) throw new ArgumentException();
    action = () => result = task();
  }

}

public class Task {

  public bool isDone { get; private set; } = false;

  public HashSet<Task> dependencies { get; private set; }  // the set of all tasks that need to finish before this one
  public HashSet<Task> dependants { get; private set; }    // the set of all tasks waiting for this one to complete

  protected Action action;    // the task to perform

  public float priority { get; private set; }

  public Task(Action action, float priority = 0) {
    if (action == null) throw new ArgumentException();
    this.action = action;
    this.priority = priority;
    dependencies = new HashSet<Task>();
    dependants = new HashSet<Task>();
  }

  public void AddDependency(Task dependency) {
    if (dependency != null && !dependency.isDone) {
      dependency.dependants.Add(this);
      dependencies.Add(dependency);
    }
  }

  public void Run() {
    action();
    isDone = true;
  }

  public Task Schedule() {
    if (dependencies.Count == 0) {
      TaskManager.Schedule(this, priority);
    }
    return this;
  }
}
