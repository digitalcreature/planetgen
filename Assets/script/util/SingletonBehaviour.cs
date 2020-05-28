using UnityEngine;

// a behaviour for which only a single object exists in the scene
// use ClassName.instance to reference the instance from anywhere
public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T> {

  static T _instance;
  public static T instance {
    get {
      if (_instance == null) {
        _instance = Object.FindObjectOfType<T>();
        if (_instance == null) {
          throw new MissingSingletonException<T>();
        }
      }
      return _instance;
    }
  }

}

public class MissingSingletonException<T> : System.Exception {

  public MissingSingletonException() : base(string.Format("Missing instance of singleton object {0} in scene!", typeof(T).Name)) {}

}
