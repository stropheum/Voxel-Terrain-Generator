using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dolly : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    private void FixedUpdate()
    {
        transform.position += new Vector3(0, 0, Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<MeshRenderer>().enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<MeshRenderer>().enabled = false;
    }
}
