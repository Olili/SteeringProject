using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentFlee : Agent {
    Transform monster;
    float fleeRadius;
    public void Start()
    {
        //steering.AddBehavior(new Wander(steering));
    }
    //public void Wander()
    //{
    //    Transform closestAgent = GetClosestAgent();
    //    if (closestAgent != null && closestAgent.CompareTag("Monster"))
    //    {
    //        monster = closestAgent;
    //        curState = Flee;
    //        steering.AddBehavior(new Flee(steering, closestAgent));
    //        steering.RemoveBehavior<Wander>();
    //    }
    //}
    //public void Flee()
    //{
    //    if (Vector2.Distance(monster.position,transform.position)> fleeRadius)
    //    {
    //        curState = Wander;
    //        steering.AddBehavior(new Wander(steering));
    //        steering.RemoveBehavior<Flee>();
    //    }
    //}
}
