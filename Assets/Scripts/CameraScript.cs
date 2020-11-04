using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float moveSpeed;

    public float zoomSpeed;
    public float targetOrtho;
    public float smoothSpeed;
    public float minOrtho;
    public float maxOrtho;

    void Start()
    {
        targetOrtho = Camera.main.orthographicSize;
    }

    void Update()
    {
        Move();
        Zoom();
    }

    void Move()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 dir = transform.up * zInput + transform.right * xInput;

        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            targetOrtho -= scroll * zoomSpeed;
            targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
        }

        Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetOrtho, smoothSpeed * Time.deltaTime);
    }
}
