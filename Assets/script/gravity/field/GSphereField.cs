using UnityEngine;

// a spherical gravity field
public class GSphereField : GField {

  public float radius = 15;

  public override float boundingRadius
    => radius;

  // get the sea level surface position for an arbitrary local space position
  // return value in local space
  public override Vector3 LocalPointToSurface(Vector3 point) {
    Vector3 gravity = LocalPointToGravity(point);
    return - gravity.normalized * radius;
  }

  // get the gravity at a certain point in local space
  // return value in local space
  public override Vector3 LocalPointToGravity(Vector3 point) {
    return -point.normalized * surfaceGravity;
  }

  void OnDrawGizmos() {
    Color c = Gizmos.color;
    Gizmos.color = new Color(0.7f, 1.0f, 0.5f);
    Gizmos.DrawWireSphere(this.position, radius);
    Gizmos.color = c;
  }

}
