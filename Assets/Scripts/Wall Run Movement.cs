using System;
using UnityEngine;

public class WallRunMovement : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask wall;
    public LayerMask ground;
    public float wallRunForce;
    public float wallRunSpeed;

    public float wallJumpUpForce;
    public float wallJumpSideForce;

    public float wallClimbSpeed;
    public float maxWallRunTime;

    private float wallRunTimer;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    private bool upwardsRunning;
    private bool downwardsRunning;
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;

    public float boundaryForce;

    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;

    private bool leftWall, rightWall;

    [Header("Exiting")]
    public float exitWallTime;

    private bool exitingWall;
    private float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("References")]
    public Transform orientation;
    public PlayerCamera cam;
    private BaseMovement bm;
    private Rigidbody rb;




    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bm = GetComponent<BaseMovement>();
    }

    private void Update()
    {
        CheckForWall();
        //Debug.Log(rightWall);
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (bm.wallrunning)
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        rightWall = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, wall);
        leftWall = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, wall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, ground);
    }

    private void StateMachine()
    {
        // Getting Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        // State 1 - Wallrunning
        if ((leftWall || rightWall) && AboveGround() && !exitingWall)
        {
            if (!bm.wallrunning && verticalInput > 0)
                StartWallRun();
            else if (horizontalInput != 0 && verticalInput == 0)
                CreateHorizontalBoundary();
            

            // wall jump 
            if (Input.GetKeyDown(jumpKey))
                WallJump();

            // wallrun timer
            if (wallRunTimer > 0) 
                wallRunTimer -= Time.deltaTime;

            if (wallRunTimer <= 0 && bm.wallrunning)
            {
                exitingWall = true; 
                exitWallTimer = exitWallTime;
            }
        }

        // State 2 - Exiting Wall

        else if (exitingWall)
        {
            if (bm.wallrunning)
                StopWallRun();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;
        }

        // State 3 - None
        else
        {
            if (bm.wallrunning)
                StopWallRun();
        }
    }

    private void StartWallRun()
    {
        bm.wallrunning = true;

        wallRunTimer = maxWallRunTime;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // apply camera effects
        cam.DoFov(90f);
        if (leftWall) cam.DoTilt(-5f);
        if (rightWall) cam.DoTilt(5f);
    }

    private void WallRunningMovement()
    {
        rb.useGravity = false;

        Vector3 wallNormal = rightWall ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        // for wall jumping 360 degrees
        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        // forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // upwards/downwards force
        if (upwardsRunning)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, wallClimbSpeed, rb.linearVelocity.z);
        if (downwardsRunning)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -wallClimbSpeed, rb.linearVelocity.z);

        // push to wall force (prevents falling from curved walls)
        if (!(leftWall && horizontalInput > 0) && !(rightWall && horizontalInput < 0))
            rb.AddForce(-wallNormal * 100, ForceMode.Force);

        // weaken gravity
        if (useGravity)
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);

    }

    private void CreateHorizontalBoundary() // prevents player from just holding horizontal input and sticking on the wall and not moving 
    {
        Vector3 wallNormal = rightWall ? rightWallHit.normal : leftWallHit.normal;

        rb.AddForce(wallNormal * boundaryForce, ForceMode.Impulse);
    }

    private void StopWallRun()
    {
        bm.wallrunning = false;
        rb.useGravity = true;

        // reduce FOV after getting off wall
        cam.DoFov(80f);
        // reset tilt
        cam.DoTilt(0);
    }

    


    private void WallJump()
    {
        // entering exit wall state
        exitingWall = true;
        exitWallTimer = exitWallTime;


        Vector3 wallNormal = rightWall ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        // reset y velocity and apply jump force
        rb.linearVelocity = new Vector3 (rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }


}

