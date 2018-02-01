using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    Steering steering;
    Rigidbody rb;
	void Start () {
        steering = GetComponent<Steering>();
	}
	
	void FixedUpdate () {
        steering.Seek(transform.position + transform.forward);
        steering.Move();
        transform.forward = GetComponent<Rigidbody>().velocity;
    }
    private void Update()
    {
        if (transform.position.z > 24.5f)
            transform.position = new Vector3(transform.position.x, transform.position.y, -24);
        else if (transform.position.z < -24.5f)
            transform.position = new Vector3(transform.position.x, transform.position.y, 24);
        else if (transform.position.x > 24.5f)
            transform.position = new Vector3(-24, transform.position.y, transform.position.z);
        if (transform.position.x < -24.5f)
            transform.position = new Vector3(24, transform.position.y, transform.position.z);
    }
}
