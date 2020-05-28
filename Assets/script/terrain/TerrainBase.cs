using UnityEngine;
using UnityEngine.Networking;

public abstract class TerrainBase : MonoBehaviour {

  public GField gfield { get; private set; }

  public float maxElevation = 100f; // the elevation of terrain (raycasting is not done above this elevation)
  public LayerMask terrainMask;

  public TMeshGenerator generator { get; private set; } // optional procedural generator script

  protected virtual void Awake() {
    generator = GetComponent<TMeshGenerator>();
    gfield = GetComponent<GField>();
  }

  // get the terrain surface position for an arbitrary world position
  // return value is in world space
  public virtual Vector3 WorldPointToSurface(Vector3 position) {
    Vector3 gravity = gfield.WorldPointToGravity(position).normalized;
    Vector3 pos = gfield.WorldPointToSurface(position);
    RaycastHit hit;
    if (this.Raycast(pos + (gravity.normalized * - maxElevation), out hit)) {
      return hit.point;
    }
    else {
      return pos;
    }
  }

  // raycast towards terrain from a position in world space
  public bool Raycast(Vector3 position, out RaycastHit hit) {
    Vector3 direction = gfield.WorldPointToGravity(position);
    return Physics.Raycast(position, direction, out hit, gfield.RaycastLimit(position), terrainMask);
  }

  // spherecast towards terrain from a position in world space
  public bool SphereCast(Vector3 position, float radius, out RaycastHit hit) {
    Vector3 direction = gfield.WorldPointToGravity(position);
    return Physics.SphereCast(position, radius, direction, out hit, gfield.RaycastLimit(position), terrainMask);
  }

  public int SphereCastNonAlloc(Vector3 position, float radius, RaycastHit[] hits) {
    Vector3 direction = gfield.WorldPointToGravity(position);
    return Physics.SphereCastNonAlloc(position, radius, direction.normalized, hits, gfield.RaycastLimit(position), terrainMask);
  }

  // get the highest allowed point above a certain position
  public Vector3 WorldPointToTop(Vector3 position) {
    Vector3 gravity = gfield.WorldPointToGravity(position).normalized;
    return gfield.WorldPointToSurface(position) - (gravity * maxElevation);
  }

  public abstract void Generate();

}
