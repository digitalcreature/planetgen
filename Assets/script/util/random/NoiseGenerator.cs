using UnityEngine;

[System.Serializable]
public class NoiseGenerator {

  public Vector3 offset;
  public float frequency = 0.15f;
  public int octaves = 1;
  public float lacunarity = 2f;
  public float gain = 0.5f;

  // sample the generator. result is from ~0 to ~1 (fbm can lead to slightly higher peaks/lower valleys)
  public float Sample(Vector3 v) {
    float x = 0;
    int oct = octaves < 1 ? 1 : octaves;
    float amp = 1;
    float freq = frequency;
    for (int o = 0; o < oct; o ++) {
      x += amp * SNoise((offset + v) * freq);
    	freq *= lacunarity;
    	amp *= gain;
    }
    return (x + 1f) / 2f;
  }

  // unsigned perlin noise from 0 to 1
  static float Noise(Vector3 v) {
    return (SNoise(v) + 1f) / 2f;
  }

  // signed perlin noise from -1 to 1
  static float SNoise(Vector3 v) {
    return Perlin.Noise(v);
  }


}
