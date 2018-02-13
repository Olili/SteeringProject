using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingSteering : Steering {

    public override Vector3 GetDvOnPlan(Vector3 target)
    {
        return base.GetDvOnPlan(target);

    }
}
