using UnityEngine;

// a planar gravity field with gravity aligned with -transform.up
public class GPlaneField : GField {

  // get the sea level surface position for an arbitrary local space position
  // return value in local space
  public override Vector3 LocalPointToSurface(Vector3 point) {
    point.y = 0;
    return point;
  }

  // get the gravity at a certain point in local space
  // return value in local space
  public override Vector3 LocalPointToGravity(Vector3 point) {
    return -Vector3.up * surfaceGravity;
  }

}
