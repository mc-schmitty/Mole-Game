using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform follow;
    public float maxSpeed = 2;


    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 eh = follow.position;
        eh.z = transform.position.z;
        transform.position = Vector2.MoveTowards(transform.position, eh, maxSpeed*Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
}
