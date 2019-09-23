using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseController : MonoBehaviour
{
    [SerializeField] private float sensitivity = 5.0f;
    [SerializeField] private float smoothing = 2.0f;
    private Vector2 mouseLook;
    private Vector2 smoothV;
    private Vector2 mouseDelta;
    private GameObject character;

    private void Start()
    {
        character = transform.parent.gameObject;
    }

    private void Update()
    {
        mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, mouseDelta.x, 1.0f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, mouseDelta.y, 1.0f / smoothing);
        mouseLook += smoothV;
        mouseLook.y = Mathf.Clamp(mouseLook.y, -90.0f, 90.0f);
    }

    private void FixedUpdate()
    {
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector2.right);
        character.GetComponent<Rigidbody>().rotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
    }
}
