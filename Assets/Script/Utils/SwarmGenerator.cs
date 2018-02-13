using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmGenerator : MonoBehaviour {

	[SerializeField]GameObject swarmModel;
	[SerializeField]Transform target;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Steering steering = Instantiate(swarmModel, transform).GetComponent<Steering>();
            steering.AddBehavior(new Swarming(steering, target));
        }
    }


}
