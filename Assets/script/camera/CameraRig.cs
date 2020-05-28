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

  void Start() {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
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