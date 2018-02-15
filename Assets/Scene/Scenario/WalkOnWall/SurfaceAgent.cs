using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceAgent : Agent {

    [SerializeField] Transform target;
    public void Start()
    {
        //target = new GameObject("target").transform;
        //target.position = transform.position + Vector3.forward * 50;
        //target.parent = transform;

        steering.AddBehavior(new Seek(steering, target));
        steering.AddBehavior(new Separation(steering));
        //steering.AddBehavior(new Separation(steering));
    }
    protected override void FixedUpdate()
    {
        //target.position = transform.position + Vector3.forward * 50 - Vector3.up* transform.position.y;
    }
}
