using UnityEngine;

public static class AnimationCurveE {

  public static AnimationCurve Clone(this AnimationCurve curve) {
    AnimationCurve clone = new AnimationCurve();
    clone.keys = curve.keys;
    return clone;
  }

}
