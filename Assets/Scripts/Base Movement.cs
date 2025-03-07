using Unity.VisualScripting;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask Ground;
    //private bool grounded;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        GetInput();

        if (Grounded())
        {
            rb.linearDamping = groundDrag;


        }
        else
            rb.linearDamping = 0f;

        if (Input.GetKeyDown(KeyCode.LeftShift))
            Walk();
        else
            Sprint();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void Walk()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 100f * Time.deltaTime, ForceMode.Force);
    }

    private void Sprint()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 125f * Time.deltaTime, ForceMode.Force);
    }

    private bool Grounded()
    {
        if (Physics.Raycast(this.transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground))
            return true;
        else return false;
    }

    

}
