using UnityEngine;
using UnityEngine.Networking;

// an abstract gravity field. to define a field with a certain shape, extend this class.
// all gravity fields should have a defined "sea level"
public abstract class GField : MonoBehaviour {

  public float surfaceGravity = 20;

  public Vector3 position { get; private set; }
  public Quaternion rotation { get; private set; }

  public virtual float boundingRadius => 1f;

  // get the sea level surface position for an arbitrary local space position
  // return value in local space
  public abstract Vector3 LocalPointToSurface(Vector3 point);

  // get the gravity at a certain point in local space
  // return value in local space
  public abstract Vector3 LocalPointToGravity(Vector3 point);

  // get the sea level surface position for an arbitrary world position
  // return value is in world space
  // never call from terrain generation code, use LocalPointToSurface instead
  public Vector3 WorldPointToSurface(Vector3 position) {
    Vector3 local = transform.InverseTransformPoint(position);
    return transform.TransformPoint(LocalPointToSurface(local));
  }

  // get the gravity at a certain point in world space
  // return value is in world space
  // never call from terrain generation code, use LocalPointToGravity instead
  public Vector3 WorldPointToGravity(Vector3 position) {
    Vector3 local = transform.InverseTransformPoint(position);
    return transform.TransformDirection(LocalPointToGravity(local));
  }


  // return the raycast distance limit when casting from a world point
  // (override to keep casts in check)
  public virtual float RaycastLimit(Vector3 point) {
    return Mathf.Infinity;
  }

  // align a transform's rotation to the gravity of this planet
  public void AlignTransformToGravity(Transform t) {
    Vector3 gravity = WorldPointToGravity(t.position);
    Vector3 forward = t.forward;
    forward = Vector3.ProjectOnPlane(forward, -gravity);
    t.rotation = Quaternion.LookRotation(forward, -gravity);
  }

  // align a ray to be perpindicular to the gravity of this planet
  // all values are in world space
  public Ray AlignRayToGravity(Ray ray) {
    Vector3 gravity = WorldPointToGravity(ray.origin);
    ray.direction = Vector3.ProjectOnPlane(ray.direction, -gravity).normalized;
    return ray;
  }

  protected virtual void Update() {
    position = transform.position;
    rotation = transform.rotation;
  }

}
