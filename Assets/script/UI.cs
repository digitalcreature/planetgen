using UnityEngine;

public class UI : MonoBehaviour {

    public GameObject helpPanel;

    void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            helpPanel.SetActive(!helpPanel.activeSelf);
        }
    }


}