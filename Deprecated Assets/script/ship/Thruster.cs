using UnityEngine;

public class Thruster : MonoBehaviour
{
    public float thrust;
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
      if (Input.GetKey (KeyCode.Space))
        rb.AddForce(transform.up * thrust);
    }
}
