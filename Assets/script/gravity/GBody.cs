using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

// a rigidbody that experiences the physics of a gravitational field
public partial class GBody : MonoBehaviour {

  // the radius used when checking if a body should be loaded/unloaded
  // this should contain every collider in the body
  // also used for general size approximations
  public float boundingRadius = 0.5f;

  public static HashSet<GBody> loaded { get; private set; } = new HashSet<GBody>();

  public bool isLoaded => loaded.Contains(this);

  public GField gfield { get; private set; }  // the gravity field this object is in

  public Vector3 gravity =>
    gfield == null ? Vector3.zero : gfield.WorldPointToGravity(transform.position);

  public Rigidbody body { get; private set; }

  void Awake() {
    body = GetComponent<Rigidbody>();
  }

  protected virtual void FixedUpdate() {
    AddGravity();
  }

  protected void AddGravity() {
    if (body.useGravity) {
      body.AddForce(gravity, ForceMode.Acceleration);
    }
  }

  // set the gravity field this body responds to
  public void SetGField(GField gfield) {
    if (gfield != this.gfield) {
      this.gfield = gfield;
      if (gfield != null) {
        transform.parent = gfield.transform;
      }
      else {
        transform.parent = null;
      }
    }
  }

  public void Load() {
    loaded.Add(this);
    body.isKinematic = false;
  }

  public void Unload() {
    loaded.Remove(this);
    body.isKinematic = true;
  }

  protected virtual void OnDrawGizmosSelected() {
    Color c = Gizmos.color;
    Gizmos.color = new Color(1f, 0.5f, 0.75f);
    Gizmos.DrawWireSphere(transform.position, boundingRadius);
    Gizmos.color = c;
  }

}
