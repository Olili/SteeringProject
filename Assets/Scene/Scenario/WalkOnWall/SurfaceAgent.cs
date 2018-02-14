using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceAgent : Agent {

    Transform target;
    public void Start()
    {
        target = new GameObject("target").transform;
        target.position = transform.position + Vector3.forward * 5;
        target.parent = transform;

        steering.AddBehavior(new Seek(steering, target));
        //steering.AddBehavior(new Separation(steering));
    }
    public void Update()
    {
        target.position = transform.position + Vector3.forward * 5 - Vector3.up* transform.position.y;
    }
}
