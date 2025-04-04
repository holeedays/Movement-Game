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

    private bool jumped;

    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;

    public float boundaryForce;

    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;

    private bool leftWall, rightWall;
    private bool wasOnLeftWall, wasOnRightWall;

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
    public SoundManager sm;

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

            // checks if the jumpkey was pressed
        if (Input.GetKeyDown(jumpKey))
            jumped = true;

        if ((leftWall || rightWall) && AboveGround() && !exitingWall && jumped) 
        {
            if (!bm.wallrunning && verticalInput > 0)
                StartWallRun();
            else if (horizontalInput != 0 && verticalInput == 0)
                CreateBoundary();
            

            // wall jump 
            if (Input.GetKeyDown(jumpKey))
            {
                WallJump();
                sm.PlayJumpSound();

                wasOnLeftWall = false;
                wasOnRightWall = false;
            }


            // determine the wall we've previously been on
            SaveWallState();


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
            // we could be jumping when exitingwall or we're simply falling
            // the bool "jumped" is not really meaning what it means

            jumped = false;

            if (bm.wallrunning)
                StopWallRun();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
            {
                if (leftWall && wasOnLeftWall && AboveGround())
                {
                    // push off the player from getting on wall
                    CreateBoundary();
                    return;
                }
                else if (rightWall && wasOnRightWall && AboveGround())
                {
                    CreateBoundary();
                    return;
                }
                else
                {
                    exitingWall = false;
                    jumped = true;

                    // reset all saved wall states

                    wasOnLeftWall = false;
                    wasOnRightWall = false;
                }
            }
                

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
        cam.DoFov(120f);
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
        rb.AddForce(wallForward * wallRunSpeed, ForceMode.Force);

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

    private void CreateBoundary() // prevents player from just holding horizontal input and sticking on the wall and not moving 
    {
        Vector3 wallNormal = rightWall ? rightWallHit.normal : leftWallHit.normal;

        rb.AddForce(wallNormal * boundaryForce, ForceMode.Force);
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


    // save the wall state: which wall were we on previously? technically this relies on the side our character
    // is relative to the wall and not the uniqueness of the wall
    private void SaveWallState()
    {
        if (leftWall)
        {
            wasOnLeftWall = true;
            wasOnRightWall = false;
        }
        else if (rightWall)
        {
            wasOnRightWall = true;
            wasOnLeftWall = false;
        }
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

