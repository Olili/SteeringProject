using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTeleporter : MonoBehaviour {

    enum Direction { ZAxe,Xaxe};
    [SerializeField]Direction dir;

    public void OnTriggerEnter(Collider collider)
    {
        Vector3 colliderPosition = collider.transform.position;
        switch (dir)
        {
            case Direction.ZAxe:
                colliderPosition.z *= -0.9f;
                break;
            case Direction.Xaxe:
                colliderPosition.x *= -0.9f;
                break;
            default:
                break;
        }
        //colliderPosition.y += 0.5f;
        collider.transform.position = colliderPosition;
    }
}
