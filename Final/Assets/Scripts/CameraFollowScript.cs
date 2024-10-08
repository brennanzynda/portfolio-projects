using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{

    public Transform target;
    public float lerpSpeed = 1f;
    public float offset = 2f;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    void MoveCamera()
    {
        Vector3 newPos = Vector3.Lerp(transform.position, target.position, lerpSpeed);
        newPos.z = newPos.z - offset;
        transform.position = newPos;
    }
}
