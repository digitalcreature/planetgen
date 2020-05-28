
// DEPRECATED: we arent using this and i dont feel like updating it

// using UnityEngine;
// using System.Collections.Generic;
//
// public class THeightmapGenerator : TMeshGenerator {
//
//   public Texture2D heightmap;
//
//   public Vector3 heightmapPostion;
//   public float heightMapScale = 4.13f;
//   public float heightScale = 1f;
//   public float heightMergeRadius = 0.1f;
//   public AnimationCurve tierCurve = AnimationCurve.Linear(0, 0, 1, 1);
//   public float tierHeight = 1f;
//
//   public override Mesh Generate(Mesh mesh) {
//     return GenerateTerrainFragmentMesh(mesh, (Vector3 pos) => {
//       if (tierHeight <= 0)
//         return 0;
//       pos += heightmapPostion;
//       pos *= heightMapScale;
//       float xy = heightmap.GetPixelBilinear(pos.x, pos.y).r;
//       float yz = heightmap.GetPixelBilinear(pos.y, pos.z).r;
//       float xz = heightmap.GetPixelBilinear(pos.x, pos.z).r;
//       float height = xy * yz * xz * heightScale;
//       // return height;
//       float h = Mathf.Floor(height / tierHeight) * tierHeight;
//       return tierCurve.Evaluate((height - h) / tierHeight) * tierHeight + h;
//     }, heightMergeRadius);
//   }
//
//   public Mesh GenerateTerrainFragmentMesh(Mesh mesh, System.Func<Vector3, float> heightmap, float heightMergeRadius) {
//     Vector3[] vs = mesh.vertices;
//     int[] ts = mesh.triangles;
//     List<Vert>[] stacks = new List<Vert>[vs.Length];
//     List<Tri> tris = new List<Tri>();
//     for (int v = 0; v < stacks.Length; v ++) {
//       stacks[v] = new List<Vert>();
//     }
//     for (int t = 0; t < ts.Length;) {
//       Tri tri = new Tri {
//         a = new Vert(ts[t], vs[ts[t++]]),
//         b = new Vert(ts[t], vs[ts[t++]]),
//         c = new Vert(ts[t], vs[ts[t++]]),
//       };
//       // Vector3 center = (tri.a.p + tri.b.p + tri.c.p) / 3f;
//       // float h = heightmap(center);
//       // tri.a.h = h;
//       // tri.b.h = h;
//       // tri.c.h = h;
//       tri.a.h = heightmap(tri.a.p);
//       tri.b.h = heightmap(tri.b.p);
//       tri.c.h = heightmap(tri.c.p);
//       stacks[tri.a.i].Add(tri.a);
//       stacks[tri.b.i].Add(tri.b);
//       stacks[tri.c.i].Add(tri.c);
//       tris.Add(tri);
//     }
//     // foreach (List<Vert> stack in stacks) {
//     //   stack.Sort((a, b) => a.h.CompareTo(b.h));
//     //   for (int i = 0; i < stack.Count - 1; i ++) {
//     //     Vert low = stack[i];
//     //     Vert high = stack[i + 1];
//     //     if ((low.h + heightMergeRadius) >= high.h) {
//     //       high.h = (high.h + low.h) / 2f;
//     //       low.h = float.NaN;
//     //     }
//     //   }
//     //   float h = 0;
//     //   for (int i = stack.Count - 1; i >=0; i --) {
//     //     Vert v = stack[i];
//     //     if (float.IsNaN(v.h)) {
//     //       v.h = h;
//     //     }
//     //     else {
//     //       h = v.h;
//     //     }
//     //   }
//     // }
//     return Tri.BuildMesh(gfield, tris);
//   }
//
//   private struct Tri {
//     public Vert a;
//     public Vert b;
//     public Vert c;
//
//     public static Mesh BuildMesh(GField gfield, List<Tri> tris) {
//       Mesh mesh = new Mesh();
//       Vector3[] vs = new Vector3[tris.Count * 3];
//       int[] ts = new int[tris.Count * 3];
//       int v = 0;
//       int t = 0;
//       foreach (Tri tri in tris) {
//         // Debug.Log(tri.a.h);
//         vs[ts[t ++] = v++] = tri.a.GetHeightMappedPosition(gfield);
//         vs[ts[t ++] = v++] = tri.b.GetHeightMappedPosition(gfield);
//         vs[ts[t ++] = v++] = tri.c.GetHeightMappedPosition(gfield);
//       }
//       mesh.vertices = vs;
//       mesh.triangles = ts;
//       mesh.RecalculateNormals();
//       return mesh;
//     }
//   }
//
//   private class Vert {
//     public int i;
//     public float h;
//     public Vector3 p;
//
//     public Vert(int i, Vector3 p) {
//       this. i = i;
//       this.p = p;
//       this.h = 0;
//     }
//
//     public Vector3 GetHeightMappedPosition(GField gfield) {
//       Vector3 surface = gfield.LocalPointToSurface(p);
//       Vector3 gravity = gfield.LocalPointToGravity(p);
//       return surface - (gravity.normalized * h);
//     }
//
//   }
//
// }
