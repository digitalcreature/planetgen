using UnityEngine;

public class FPSMonitor : MonoBehaviour {

  void OnGUI() {
    float fps;
    if (Time.timeScale > 0) {
      fps = 1f / (Time.smoothDeltaTime / Time.timeScale);
    }
    else {
      fps = 1f / Time.unscaledDeltaTime;
    }
    GUILayout.Label(string.Format("{0} fps", (int) (fps)));
  }
}
