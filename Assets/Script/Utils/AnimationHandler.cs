using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour {

    Animator animator;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 velocity = GetComponent<Rigidbody>().velocity;
        animator.SetFloat("Speed", velocity.magnitude);
        velocity.y = 0;
        if (velocity != Vector3.zero)
            transform.forward = velocity;
    }
}
