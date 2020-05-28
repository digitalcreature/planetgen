using UnityEngine;
using System.Collections.Generic;

public class TFragment : MonoBehaviour {

  public GridTerrain terrain { get; private set; }

  public Vector3 center { get; private set; }       // the center of this fragment, local to terrain

  public Vector3 worldCenter => terrain.transform.TransformPoint(center);

  public MeshFilter filter { get; private set; }
  public MeshCollider hull { get; private set; }
  public MeshRenderer render { get; private set; }

  public int maxGridSize { get; private set; }   // the size of the grid at the highest level of detail

  public bool isLoaded { get; private set; } = false;
  public bool isVisible { get; private set; } = true;

  public bool hasGenerated { get; private set; } = false;

  public float detailFactor { get; private set; }

  public Vector3 a { get; private set; }
  public Vector3 b { get; private set; }
  public Vector3 c { get; private set; }

  // a set of all unloaded surfacebodies that should be loaded when this fragment is
  public HashSet<GBody> unloadedBodies { get; private set; } = new HashSet<GBody>();

  DetailMesh[] detailMeshes;    // collection of detail meshes for each detail level

  void Awake() {
    filter = GetComponent<MeshFilter>();
    render = GetComponent<MeshRenderer>();
    hull = GetComponent<MeshCollider>();
  }

  public TFragment Instantiate(GridTerrain terrain, Vector3 a, Vector3 b, Vector3 c, int maxGridSize) {
    TFragment frag = Instantiate(this);
    frag.name = name;
    frag.Initialize(terrain, a, b, c, maxGridSize);
    return frag;
  }

  void Initialize(GridTerrain terrain, Vector3 a, Vector3 b, Vector3 c, int maxGridSize) {
    gameObject.SetActive(true);
    this.terrain = terrain;
    this.a = a;
    this.b = b;
    this.c = c;
    this.maxGridSize = maxGridSize;
    center = (a + b + c) / 3;
    render.sharedMaterial = terrain.terrainMaterial;
    detailMeshes = new DetailMesh[terrain.detailLevelCount];
    for (int i = 0; i < detailMeshes.Length; i ++) {
      detailMeshes[i] = new DetailMesh();
    }
  }

  void Update() {
    if (isLoaded) {
      // always use the highest detail level mesh for the hull
      Mesh hullMesh = GetDetailMesh(1, - detailFactor);
      hull.enabled = hullMesh != null;
      if (hullMesh != null) {
        hull.sharedMesh = hullMesh;
      }
      UpdateDetailLevel(this.detailFactor);
    }
  }

  public float EvaluateDetailFactorCascades(float detailFactor, out int detailLevel)
    => terrain.detailFactorCascades.Evaluate(detailFactor, out detailLevel);


  public void Load() {
    if (!isLoaded) {
      isLoaded = true;
      gameObject.SetActive(true);
      // if (terrain.isServer) {
      //   foreach (GBody body in unloadedBodies) {
      //     body.Load();
      //   }
      //   unloadedBodies.Clear();
      // }
    }
  }

  public void Unload() {
    if (isLoaded) {
      isLoaded = false;
      gameObject.SetActive(false);
      // if (terrain.isServer) {
      //   foreach (GBody body in GBody.loaded) {
      //     if (SphereIsOverFragment(body.transform.position, body.boundingRadius)) {
      //       unloadedBodies.Add(body);
      //     }
      //   }
      //   foreach (GBody body in unloadedBodies) {
      //     body.Unload();
      //   }
      // }
      // hull.enabled = false;
    }
  }

  RaycastHit[] hits = new RaycastHit[16];

  public bool SphereIsOverFragment(Vector3 position, float radius) {
    position = terrain.WorldPointToTop(position);
    int hitCount = terrain.SphereCastNonAlloc(position, radius, hits);
    for (int i = 0; i < hitCount; i ++) {
      if (hits[i].collider == hull) {
        return true;
      }
    }
    return false;
  }

  public void SetDetailFactor(float detailFactor) {
    this.detailFactor = detailFactor;
  }

  void UpdateDetailLevel(float detailFactor) {
    Mesh mesh = GetDetailMesh(detailFactor);
    if (mesh != null) {
      // only change the mesh if there is a mesh to change to
      filter.mesh = mesh;
    }
  }

  // get the mesh for certain detail level
  // if it isnt done generating, returns null
  // priority is used as the priority for the threaded task if the mesh needs to be generated
  public Mesh GetDetailMesh(float detailFactor, float priority) {
    int detailLevel;
    EvaluateDetailFactorCascades(detailFactor, out detailLevel);
    DetailMesh detailMesh = detailMeshes[detailLevel];
    Mesh mesh = detailMesh.mesh;
    if (mesh == null) {
      Task<TMesh> meshTask = StartMeshGeneration(detailFactor, priority);
      if (meshTask.isDone) {
        mesh = meshTask.result.ToMesh();
        detailMesh.mesh = mesh;
      }
    }
    return mesh;
  }
  public Mesh GetDetailMesh(float detailFactor) => GetDetailMesh(detailFactor, - detailFactor);

  // get the task responsible for generating the detail mesh
  // if the task isnt running yet, start it
  // pririty is used to set the priority of the generation task
  Task<TMesh> StartMeshGeneration(float detailFactor, float priority) {
    int detailLevel;
    detailFactor = EvaluateDetailFactorCascades(detailFactor, out detailLevel);
    DetailMesh detailMesh = detailMeshes[detailLevel];
    Task<TMesh> task = detailMesh.task;
    if (task == null) {
      task = new Task<TMesh>(() => {
        return GenerateMesh(detailFactor);
      }, priority);
      task.AddDependency(terrain.gridMeshTask);
      task.Schedule();
      detailMesh.task = task;
    }
    return task;
  }
  Task<TMesh> StartMeshGeneration(float detailFactor) => StartMeshGeneration(detailFactor, 1 - detailFactor);

  // mesh generation function
  // this is run in a worker thread
  public TMesh GenerateMesh(float detailFactor) {
    int gridSize = (int) (maxGridSize * detailFactor);
    if (gridSize < 2) gridSize = 2;
    TMesh data = TMesh.CreateSubGrid(a, b, c, gridSize);
    if (terrain.generator != null) {
      data = terrain.generator.Generate(data);
    }
    data.Clean();
    return data;
  }

  public void SetVisible(bool isVisible) {
    this.isVisible = isVisible;
    // render.enabled = isVisible;
  }

  class DetailMesh {

    public Mesh mesh;
    public Task<TMesh> task;

  }

}
