using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentChase : AgentSeek {


    [SerializeField]Collider roomBounds;
    public override void Start()
    {
        steering.AddBehavior(new Wander(steering));
        steering.AddBehavior(new Separation(steering));
        steering.AddBehavior(new StayWithinBounds(steering, roomBounds.bounds));
        curState = WanderChase;
    }
    public void WanderChase()
    {
        Transform closestEntity = GetClosestAgent("human");
        if (closestEntity != null)
        {
            steering.RemoveBehavior<Wander>();
            steering.AddBehavior(new Pursuit(steering, closestEntity, closestEntity.GetComponent<Rigidbody>()));
            curState = Chase;


            Steering targetSteering = closestEntity.GetComponent<Steering>();
            targetSteering.RemoveBehavior<Wander>();
            //targetSteering.AddBehavior(new Evade(targetSteering, transform, GetComponent<Rigidbody>()));
            targetSteering.AddBehavior(new Flee(targetSteering, transform));
            targetSteering.AddBehavior(new StayWithinBounds(targetSteering, roomBounds.bounds,5,3));
        }
    }
    public void Chase()
    {
        Transform target = steering.FindBehavior<Pursuit>().GetTarget();

        Debug.DrawLine(transform.position, target.position, Color.red);
        //if ((target.position - transform.position).magnitude > targetLostRadius)
        //{
        //    steering.RemoveBehavior<Pursuit>();
        //    steering.AddBehavior(new Wander(steering));

        //    curState = Wander; Steering targetSteering = target.GetComponent<Steering>();
        //    targetSteering.AddBehavior(new Wander(targetSteering));
        //    targetSteering.RemoveBehavior<Evade>();
        //}
    }
}
