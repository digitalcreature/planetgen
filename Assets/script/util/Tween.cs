using UnityEngine;

[System.Serializable]
public class Tween {

  public float duration = 1;
  public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

  float _t = 0f;

  public float t {                         // the current progress of the tween from 0 to 1
    get { return _t; }
    set {
      _t = value;
      x = curve.Evaluate(value);
    }
  }
  public float time => t * duration;
  public float speed { get; set; }        // the current speed of the tween's playback
  public float x { get; private set; }    // t with the curve applied

  public bool isAtStart => t <= 0f;
  public bool isAtEnd => t >= 1f;

  public void Update(float delta) {
    t = Mathf.Clamp01(t + speed * (delta / duration));
  }
  public void Update() => Update(Time.deltaTime);

  public float Lerp(float a, float b)
    => Mathf.Lerp(a, b, x);
  public Vector2 Lerp(Vector2 a, Vector2 b)
    => Vector2.Lerp(a, b, x);
  public Vector3 Lerp(Vector3 a, Vector3 b)
    => Vector3.Lerp(a, b, x);
  public Vector4 Lerp(Vector4 a, Vector4 b)
    => Vector4.Lerp(a, b, x);
  public Quaternion Lerp(Quaternion a, Quaternion b)
    => Quaternion.Lerp(a, b, x);

  public Vector3 Slerp(Vector3 a, Vector3 b)
    => Vector3.Slerp(a, b, x);
  public Quaternion Slerp(Quaternion a, Quaternion b)
    => Quaternion.Slerp(a, b, x);

  //  lerp a transform between two other transforms
  public void LerpPos(Transform x, Transform a, Transform b)
    => x.position = Lerp(a.position, b.position);
  public void SlerpPos(Transform x, Transform a, Transform b)
    => x.position = Slerp(a.position, b.position);
  public void LerpRot(Transform x, Transform a, Transform b)
    => x.rotation = Lerp(a.rotation, b.rotation);
  public void SlerpRot(Transform x, Transform a, Transform b)
    => x.rotation = Slerp(a.rotation, b.rotation);

}
