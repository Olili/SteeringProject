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
    }
    private void Update()
    {
        if (transform.position.z > 24)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -24);
        }
    }
}
