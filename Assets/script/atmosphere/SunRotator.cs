using UnityEngine;

public class SunRotator : MonoBehaviour {

    public float cycleTime = 120;   // how many seconds a full rotation takes

    void Update() {
        var euler = transform.eulerAngles;
        euler.y += 360 / cycleTime * Time.deltaTime;
        transform.eulerAngles = euler;
    }

}