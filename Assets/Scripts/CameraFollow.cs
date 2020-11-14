using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    private float distanceToTarget;
    void Awake()
    {
        target = GameObject.FindWithTag("Player").transform;
        distanceToTarget = transform.position.x - target.position.x;
    }
    void Update()
    {
        float targetObjectX = target.position.x;
        Vector3 newCameraPosition = transform.position;
        newCameraPosition.x = targetObjectX + distanceToTarget;
        transform.position = newCameraPosition; 
    }
}
