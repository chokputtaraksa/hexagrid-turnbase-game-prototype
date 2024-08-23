using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public float moveSpeed = 10;
    public GameObject player;
    private Vector3 offset = new Vector3(0, 5, -6);
    private List<HexCell> adjacentCells = new List<HexCell>();
    private float verticalInput;
    private float horizontalInput;
    private float scrollInput;

    private float mouseClickDragInput;

    // Start is called before the first frame update
    void Start()
    {

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
        }
        if (horizontalInput != 0)
        {
            transform.Translate(Vector3.right * Time.deltaTime * moveSpeed * horizontalInput);
        }
        if (scrollInput != 0)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed * 30 * scrollInput);
        }
    }


}
