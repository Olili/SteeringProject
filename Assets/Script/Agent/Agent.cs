using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Agent : MonoBehaviour {

    protected Steering steering;
    protected Rigidbody rb;

    void Start () {
        steering = GetComponent<Steering>();
        rb = GetComponent<Rigidbody>();
    }
}
