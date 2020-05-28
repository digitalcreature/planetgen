using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

// attach to an object that needs to load terrain in a radius
public class TViewer : MonoBehaviour {

  public float minRange = 100f;
  public float maxRange = 250f;
  public float cullSpherePadding = 50;

  public static HashSet<TViewer> all {get; private set; } = new HashSet<TViewer>();

  // what level of detail should an object located at a point in world space be displayed at?
  // objects >= maxRange distance away are at LOD = 0,
  // objects <= minRange distance away are at LOD = 1,
  // NOTE: return value is not clamped to this range; objects closer than minRange will be > 1
  // objects further away than maxRange will be < 0
  public float GetDetailLevel(Vector3 point) {
    float d = (point - transform.position).magnitude;
    float t = (d - maxRange) / (minRange - maxRange);
    return t;
  }

  static int cullSphereCenter_id = -1;
  static int cullSphereRadius_id = -1;

  void Awake() {
    if (cullSphereCenter_id < 0) {
      cullSphereCenter_id = Shader.PropertyToID("_CullSphereCenter");
      cullSphereRadius_id = Shader.PropertyToID("_CullSphereRadius");
    }
  }

  void Start() {
    OnEnable();
  }

  void OnEnable() {
    all.Add(this);
  }

  void OnDisable() {
    all.Remove(this);
  }

  void Update() {
    Vector3 c = transform.position;
    Shader.SetGlobalVector(cullSphereCenter_id, new Vector4(c.x, c.y, c.z, 1));
    Shader.SetGlobalFloat(cullSphereRadius_id, maxRange - cullSpherePadding);
  }

  void OnDrawGizmosSelected() {
    Color c = Gizmos.color;
    Gizmos.color = new Color(1.0f, 0.7f, 0.5f);
    Gizmos.DrawWireSphere(transform.position, minRange);
    Gizmos.DrawWireSphere(transform.position, maxRange);
    Gizmos.color = new Color(1.0f, 0.5f, 0.5f);
    Gizmos.color = c;
  }



}
