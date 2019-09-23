using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Vuforia;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpPower = 10.0f;
    private Rigidbody rigidBody;
    private Vector3 translation;
    private Vector3 strafe;
    private bool jumping = false;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
//        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime * transform.forward;
//        strafe = Input.GetAxis("Horizontal") * speed * Time.deltaTime * transform.right;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.None) ? CursorLockMode.Locked : CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumping = true;
        }
    }

    private void FixedUpdate()
    {
        var translation = Input.GetAxis("Horizontal") * speed * transform.right +
                          Input.GetAxis("Vertical") * speed * transform.forward;
        rigidBody.velocity = new Vector3(
            translation.x,
            rigidBody.velocity.y,
            translation.z);

//        rigidBody.AddForce(Input.GetAxis("Horizontal") * speed * transform.right);
//        rigidBody.AddForce(Input.GetAxis("Vertical") * speed * transform.forward);

        if (jumping)
        {
            jumping = false;
            var upForce = Vector3.up * jumpPower;
            rigidBody.AddForce(upForce);
            Debug.Log($"Up Force: {upForce}");
        }
    }
}
