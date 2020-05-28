using UnityEngine;

[System.Serializable]
public class HeightmapSteps {

  public float stepHeight = 3f;
  public AnimationCurve stepCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

  public float Apply(float height) {
    float h = height / stepHeight;
    float step = Mathf.Floor(h);
    h = step + stepCurve.Evaluate(h - step);
    return h * stepHeight;
  }

  public HeightmapSteps Clone() {
    HeightmapSteps clone = new HeightmapSteps();
    clone.stepHeight = stepHeight;
    clone.stepCurve = stepCurve.Clone();
    return clone;
  }

}
