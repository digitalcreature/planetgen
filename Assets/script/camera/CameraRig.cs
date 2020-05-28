using UnityEngine;

// the main camera rig
public class CameraRig : SingletonBehaviour<CameraRig> {

  public MouseLook mouseLook;
  public GField gravity;
  public float moveSpeed = 10;
  public FloatRange moveSpeedRange = new FloatRange(10, 100);
  public float moveSpeedInterval = 10;

  [Header("Controls")]
  public KeyCode forward = KeyCode.W;
  public KeyCode backward = KeyCode.S;
  public KeyCode left = KeyCode.A;
  public KeyCode right = KeyCode.D;
  public KeyCode up = KeyCode.Space;
  public KeyCode down = KeyCode.LeftShift;

  public Camera cam { get; private set; }
  public Transform gimbal => mouseLook.gimbal;

  void Awake() {
    cam = GetComponentInChildren<Camera>();
  }

  void Update() {
    gravity.AlignTransformToGravity(transform);
    mouseLook.UpdateGimbal(transform.up);
    var move = Vector3.zero;
    if (Input.GetKey(forward)) move.z ++;
    if (Input.GetKey(backward)) move.z --;
    if (Input.GetKey(right)) move.x ++;
    if (Input.GetKey(left)) move.x --;
    if (Input.GetKey(up)) move.y ++;
    if (Input.GetKey(down)) move.y --;
    move = gimbal.TransformDirection(move);
    move *= moveSpeed;
    transform.position += move * Time.deltaTime;
    moveSpeed = moveSpeedRange.Clamp(moveSpeed + Input.mouseScrollDelta.y * moveSpeedInterval);
  }

}
//   public MouseLook mouseOrbit; // the mouse look
//   public LayerMask distanceAdjustMask;  // the mask to use when adjusting the camera distance
//   public float distance = 10;
//   public float distanceBias = 0.5f;
//   public float maxDistance = 15;
//   public float zoomRate = 3f;
//   public float zoomSmoothTime = 0.1f;
//   public float firstPersonDistanceThreshold = 0.01f; // camera is considered first person when distance is smaller than this value

//   public MouseButton mouseOrbitButton = MouseButton.Middle; // button to use for orbiting

//   public Vector3 lookDirection => gimbal.forward;

//   public bool isFirstPerson { get; private set; }
//   public bool isThirdPerson {
//     get { return !isFirstPerson; }
//     private set { isFirstPerson = !value; }
//   }

//   public CameraRigFocusable focus { get; private set; }
//   public GField gfield => focus == null ? null : focus.gfield;     // the gravity field to align to

//   public Camera cam { get; private set; }         // one of the cameras this rig uses (any should do for most things)
//   public Transform gimbal => mouseOrbit.gimbal;

//   float targetDistance;
//   float zoomVelocity;

//   // temp buffer
//   RaycastHit[] hits = new RaycastHit[8];

//   void Awake() {
//     cam = GetComponentInChildren<Camera>();
//     targetDistance = distance;
//   }

//   void Update() {
//     // check to see if the target is a character that is driving
//     bool isDriving = false;
//     if (focus != null) {
//       transform.position = focus.focusTarget.position;
//       isDriving = (focus.character != null && focus.character.isDriving);
//     }
//     if (isDriving) {
//       // if the target is driving, force them into first person
//       cam.transform.localPosition = Vector3.zero;
//       isFirstPerson = true;
//       focus.UpdateRenderers(isFirstPerson);
//       // align the camerarig to the vehicle
//       Vehicle vehicle = focus.character.vehicle;
//       // Vector3 up = vehicle.transform.up;
//       // transform.rotation = Quaternion.LookRotation(transform.forward, up);
//       transform.rotation = vehicle.transform.rotation;
//       // update the mouselook gimbal
//       mouseOrbit.UpdateGimbal(transform.up);
//     }
//     else {
//       if (gfield != null) {
//         gfield.AlignTransformToGravity(transform);
//       }
//       if (isFirstPerson || Input.GetMouseButton((int) mouseOrbitButton)) {
//         // update the gimbal based on mouse input
//         // this is only done what the mouse button is being held down, or if the player is in first person
//         mouseOrbit.UpdateGimbal(transform.up);
//       }
//       // handle scroll zoom
//       targetDistance -= Input.mouseScrollDelta.y * zoomRate;
//       targetDistance = Mathf.Clamp(targetDistance, 0f, maxDistance);
//       distance = Mathf.SmoothDamp(distance, targetDistance, ref zoomVelocity, zoomSmoothTime);
//       distance = Mathf.Clamp(distance, 0f, maxDistance);
//       // make sure the focus knows were in first person
//       isFirstPerson = distance < firstPersonDistanceThreshold;
//       if (focus != null) {
//         focus.UpdateRenderers(isFirstPerson);
//       }
//       // adjust the distance of the camera
//       float d = distance;
//       int count = Physics.RaycastNonAlloc(gimbal.position, -gimbal.forward, hits, d + distanceBias, distanceAdjustMask);
//       System.Array.Sort(hits, 0, count, new RaycastHitComparer());
//       if (count > 0) {
//         for (int i = 0; i < count; i ++) {
//           RaycastHit hit = hits[i];
//           if (hit.rigidbody == null || !hit.rigidbody.CompareTag("Carried")) {
//             d = hit.distance - distanceBias;
//             if (d < 0f) {
//               d = 0f;
//             }
//             break;
//           }
//         }
//       }
//       cam.transform.localPosition = -Vector3.forward * d;
//     }
//     // lock/unlock the cursor as needed
//     if (!Application.isEditor) {
//       if (isFirstPerson) {
//         Cursor.lockState = CursorLockMode.Confined;
//       }
//       else {
//         Cursor.lockState = CursorLockMode.None;
//       }
//     }
//   }


//   public void SetFocus(CameraRigFocusable focus) {
//     if (this.focus != null) {
//       // if we are already focusing something, make sure it's renderers are in third person mode
//       this.focus.UpdateRenderers(false);
//     }
//     this.focus = focus;
//   }

// }
