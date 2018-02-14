using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveAtMouse : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 500))
            {
                transform.position = hit.point+ Vector3.up;
            }
        }
    }
}
