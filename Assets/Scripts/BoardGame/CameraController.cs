using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // for set the camera focus
    public float smoothTime = 0.3f;
    public Vector3 offset = new Vector3(0, 10, -5); // Adjust this to your needs

    private Vector3 velocity = Vector3.zero;
    private Transform target;

    // for camera movement
    public float moveSpeed = 10;
    private float verticalInput;
    private float horizontalInput;
    private float scrollInput;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        // get the user's vertical input
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (verticalInput != 0)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * (moveSpeed / 2) * verticalInput);
            transform.Translate(Vector3.up * Time.deltaTime * (moveSpeed / 2) * verticalInput);
            target = null;
        }
        if (horizontalInput != 0)
        {
            transform.Translate(Vector3.right * Time.deltaTime * moveSpeed * horizontalInput);
            target = null;
        }
        if (scrollInput != 0)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed * 30 * scrollInput);
            target = null;
        }

        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            transform.LookAt(target);
        }
    }


}
