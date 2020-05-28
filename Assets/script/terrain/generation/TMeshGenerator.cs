using UnityEngine;

public abstract class TMeshGenerator : MonoBehaviour {

  public TerrainBase terrain { get; private set; }

  public GField gfield => terrain.gfield;

  protected virtual void Awake() {}

  // initialize the generator so that it can be used
  public virtual void Initialize() {
    terrain = GetComponent<TerrainBase>();
  }

  // use the generator to generate a mesh (based on a previous iteration)
  public abstract TMesh Generate(TMesh mesh);

}


// intermidiate stage mesh generation utility
public struct TMesh {

  public Vector3[] vert;
  public int[] tri;
  public Vector2[] uv;

  public bool isInvalid => vert == null;

  public TMesh(Vector3[] vert, int[] tri, Vector2[] uv = null) {
    this.vert = vert;
    this.tri = tri;
    this.uv = uv;
  }

  public TMesh(Mesh mesh) {
    vert = mesh.vertices;
    tri = mesh.triangles;
    uv = mesh.uv;
  }

  // remove data that corresponds to triangles that dont exist
  // also splits all triangles
  public void Clean() {
    int v = 0;
    Vector3[] vert = new Vector3[tri.Length];
    Vector2[] uv = new Vector2[tri.Length];
    for (int t = 0; t < tri.Length; t ++) {
      int i = tri[t];
      vert[v] = this.vert[tri[t]];
      if (this.uv != null && i < this.uv.Length) {
        uv[v] = this.uv[tri[t]];
      }
      tri[t] = v;
      v ++;
    }
    this.vert = vert;
    this.uv = uv;
  }

  public Mesh ToMesh(Mesh mesh = null) {
    if (mesh == null) {
      mesh = new Mesh();
      mesh.name = "terrain mesh";
    }
    mesh.vertices = vert;
    mesh.triangles = tri;
    mesh.uv = uv;
    Vector3[] norm = new Vector3[tri.Length];
    for (int t = 0; t < tri.Length;) {
      Vector3 a = vert[tri[t + 0]];
      Vector3 b = vert[tri[t + 1]];
      Vector3 c = vert[tri[t + 2]];
      b -= a;
      c -= a;
      Vector3 normal = Vector3.Cross(b, c).normalized;
      norm[t++] = normal;
      norm[t++] = normal;
      norm[t++] = normal;
    }
    mesh.normals = norm;
    return mesh;
  }

  // create a new TMesh by subdividing a triangle
  public static TMesh CreateSubGrid(Vector3 a, Vector3 b, Vector3 c, int n = 2) {
    int tLength = (n - 1) * (n - 1) * 3;
    Vector3[] vs = new Vector3[tLength];
    int[] ts = new int[tLength];
    for (int i = 0; i < tLength; i ++) {
      ts[i] = i;
    }
    Vector3 p = (b - a);
    Vector3 q = (c - a);
    int v = 0;
    for (int i = 0; i < n; i ++) {
      int colHeight = n - i;
      for (int j = 0; j < colHeight; j ++) {
        if (i > 0) {
          if (j > 0) {
            vs[v ++] = SubVert(a, p, q, n, i, j);
            vs[v ++] = SubVert(a, p, q, n, i - 1, j);
            vs[v ++] = SubVert(a, p, q, n, i, j - 1);
          }
          vs[v ++] = SubVert(a, p, q, n, i, j);
          vs[v ++] = SubVert(a, p, q, n, i - 1, j + 1);
          vs[v ++] = SubVert(a, p, q, n, i - 1, j);
        }
      }
    }
    return new TMesh(vs, ts);
  }

  // return a version of this mesh where all triangles have been subdivided
  public TMesh SubGrid(int n = 2) {
    int tLength = (n - 1) * (n - 1) * tri.Length;
    Vector3[] vs = new Vector3[tLength];
    int[] ts = new int[tLength];
    for (int i = 0; i < tLength; i ++) {
      ts[i] = i;
    }
    int v = 0;
    for (int t = 0; t < tri.Length;) {
      Vector3 a = vert[tri[t ++]];
      Vector3 b = vert[tri[t ++]];
      Vector3 c = vert[tri[t ++]];
      Vector3 p = (b - a);
      Vector3 q = (c - a);
      for (int i = 0; i < n; i ++) {
        int colHeight = n - i;
        for (int j = 0; j < colHeight; j ++) {
          if (i > 0) {
            if (j > 0) {
              vs[v ++] = SubVert(a, p, q, n, i, j);
              vs[v ++] = SubVert(a, p, q, n, i - 1, j);
              vs[v ++] = SubVert(a, p, q, n, i, j - 1);
            }
            vs[v ++] = SubVert(a, p, q, n, i, j);
            vs[v ++] = SubVert(a, p, q, n, i - 1, j + 1);
            vs[v ++] = SubVert(a, p, q, n, i - 1, j);
          }
        }
      }
    }
    // for right now, were only going to be calling this on meshes without uvs
    return new TMesh(vs, ts);
  }

  // helper for subgrid functions
  static Vector3 SubVert(Vector3 a, Vector3 p, Vector3 q, int n, int i, int j) {
    return (a +
      (p * ((float) i / (n - 1))) +
      (q * ((float) j / (n - 1)))
    );
  }

}
