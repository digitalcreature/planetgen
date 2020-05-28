using UnityEngine;

// utility class for mouse look
// see CameraRig.cs and camera rig prefab for usage example
[System.Serializable]
public class MouseLook {

  public float sensitivity = 5f;  // the sensitivity of the mouse look
  public Transform gimbal;        // the transform to rotate with the mouse
  public float gimbalPitch;
  public float gimbalYaw;
  // update a gimbal transform to follow the mouse
  // aligned with an up vector; use when gravity is a thing
  public void UpdateGimbal(Vector3 up) {
    float inputX = Input.GetAxis("Mouse X") * sensitivity;
    float inputY = Input.GetAxis("Mouse Y") * sensitivity;
    gimbalPitch = Vector3.Angle(gimbal.forward, up);
    gimbalYaw = gimbal.localEulerAngles.y;
    gimbalYaw += inputX;
    gimbalPitch -= inputY;
    if (gimbalPitch < 0) gimbalPitch = 0;
    if (gimbalPitch > 180) gimbalPitch = 180;
    gimbal.localEulerAngles = new Vector3(gimbalPitch - 90, gimbalYaw, 0);
  }

  // update a gimbal transform to follow the mouse
  // unaligned with any particular up vector, use when gravity isnt a thing
  public void UpdateGimbal6DOF() {
    float inputX = Input.GetAxis("Mouse X") * sensitivity;
    float inputY = Input.GetAxis("Mouse Y") * sensitivity;
    gimbal.Rotate(-inputY, inputX, 0);
  }

}
