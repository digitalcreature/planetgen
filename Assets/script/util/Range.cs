using UnityEngine;

public interface IRange<T> {

  T range { get; }

  T GetRandom(System.Random rng);
  T GetRandom();
  bool Contains(T value);
  T Clamp(T value);

}


// represent a range from min to max, including max
[System.Serializable]
public struct FloatRange : IRange<float> {

  public float min;
  public float max;

  public float range => max - min;

  public FloatRange(float min, float max) {
    this.min = min;
    this.max = max;
  }

  public float GetRandom(System.Random rng) {
    return ((float) rng.NextDouble() * range) + min;
  }

  public float Lerp(float t) {
    return Mathf.Lerp(min, max, t);
  }

  public float InverseLerp(float value) {
    return Mathf.InverseLerp(min, max, value);
  }

  public float GetRandom() {
    return Random.value * range + min;
  }

  public bool Contains(float value) {
    return value >= min && value <= max;
  }

  public float Clamp(float value) {
    return value <= min ? min : (value >= max ? max : value);
  }

}

// represent a range from min to max, excluding max
[System.Serializable]
public struct IntRange : IRange<int> {

  public int min;
  public int max;

  public int range => max - min;

  public IntRange(int min, int max) {
    this.min = min;
    this.max = max;
  }

  public int GetRandom(System.Random rng) {
    return (int) (rng.NextDouble() * range) + min;
  }

  public int GetRandom() {
    return (int) (Random.value * range + min);
  }

  public bool Contains(int value) {
    return value >= min && value < max;
  }

  public int Clamp(int value) {
    return value <= min ? min : (value > max ? max : value);
  }

}

public class MinMaxAttribute : PropertyAttribute {

	public float min;
	public float max;

	public MinMaxAttribute(float min, float max) {
		this.min = min;
		this.max = max;
	}
	public MinMaxAttribute(int min, int max) {
		this.min = min;
		this.max = max;
	}

}
