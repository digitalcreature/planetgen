using UnityEngine;

public class TPerlinGenerator : TMeshGenerator {

  public NoiseGenerator heightMap;
  public NoiseGenerator splitMap;
  public AnimationCurve splitCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
  public HeightmapSteps steps;

  public AnimationCurve elevationCurve1 = AnimationCurve.Linear(0f, 0f, 1f, 1f);
  public AnimationCurve elevationCurve2 = AnimationCurve.Linear(0f, 0f, 1f, 1f);
  public AnimationCurve texUCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
  public int textureResolution = 8;     // used to quantize uv values, avoidng artifacts

  public float maxHeight = 50;

  public float jitterFrequency = 1f;
  public float jitterRadius = 0.1f;

  public override void Initialize() {
    base.Initialize();
  }

  public override TMesh Generate(TMesh mesh) {
    // we have to clone stuff using AnimationCurve because its not threadsafe for some fuckign reason
    HeightmapSteps steps = this.steps.Clone();
    AnimationCurve elevationCurve1 = this.elevationCurve1.Clone();
    AnimationCurve elevationCurve2 = this.elevationCurve2.Clone();
    AnimationCurve texUCurve = this.texUCurve.Clone();
    AnimationCurve splitCurve = this.splitCurve.Clone();
    Vector3[] vs = mesh.vert;
    int[] ts = mesh.tri;
    Vector2[] uvs = new Vector2[vs.Length];
    for (int t = 0; t < ts.Length;) {
      int a = ts[t++];
      int b = ts[t++];
      int c = ts[t++];
      float ha, hb, hc;
      Vector3 va = vs[a]; ApplyMap(elevationCurve1, elevationCurve2, steps, splitCurve, ref va, out ha); vs[a] = va;
      Vector3 vb = vs[b]; ApplyMap(elevationCurve1, elevationCurve2, steps, splitCurve, ref vb, out hb); vs[b] = vb;
      Vector3 vc = vs[c]; ApplyMap(elevationCurve1, elevationCurve2, steps, splitCurve, ref vc, out hc); vs[c] = vc;
      float h = (ha + hb + hc) / 3f;
      float u = texUCurve.Evaluate(h);
      u = Mathf.Floor(u * textureResolution) / ((float) textureResolution) + (1f / (textureResolution * 2f));
      Vector2 uv = new Vector2(u, 0.5f);
      uvs[a] = uv;
      uvs[b] = uv;
      uvs[c] = uv;
    }
    return new TMesh(vs, ts, uvs);
  }

  private void ApplyMap(AnimationCurve elevationCurve1, AnimationCurve elevationCurve2, HeightmapSteps steps,
                        AnimationCurve splitCurve, ref Vector3 vert, out float height) {
    Vector3 v = vert;
    float h = heightMap.Sample(v);
    float split = splitMap.Sample(v);
    split = splitCurve.Evaluate(split);
    float h1 = elevationCurve1.Evaluate(h);
    float h2 = elevationCurve2.Evaluate(h);
    h = Mathf.Lerp(h1, h2, split);
    height = h;
    h = steps.Apply(h * maxHeight);
    v = VertexHeight(v, h);
    v += new Vector3(
      Perlin.Noise(jitterFrequency * (v + Vector3.right)) * jitterRadius,
      Perlin.Noise(jitterFrequency * (v + Vector3.up)) * jitterRadius,
      Perlin.Noise(jitterFrequency * (v + Vector3.forward)) * jitterRadius
    );
    vert = v;
  }

  private Vector3 VertexHeight(Vector3 v, float h) {
    Vector3 surface = gfield.LocalPointToSurface(v);
    Vector3 gravity = gfield.LocalPointToGravity(v);
    return surface - (gravity.normalized * h);
  }

  private float NoiseMap(Vector3 v) {
    // float h = Perlin.Fbm(offset + (v * tiling), octave);
    // float h = Perlin.Noise(offset + (v * tiling));
    float h = heightMap.Sample(v);
    return h;
  }

}
