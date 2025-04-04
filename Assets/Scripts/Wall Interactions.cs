using UnityEditor;
using UnityEngine;

public class WallInteractions : MonoBehaviour
{
    [Header("References")]

    public static string speedDown = "Speed Down";
    public static string speedUp = "Speed Up";
    public static string increaseGravity = "Increase Gravity";
    public static string restart = "Restart";

    private WallRunMovement wm;

    [Header("Player Movement Modifier Values")]

    [Range(0f, 1f)]
    public float speedDownMultiplier;
    [Range(0f, 5f)]
    public float speedUpMultiplier;
    [Range(0f, 1f)]
    public float increaseGravityMultiplier;

    [Header("Save Variables")]

    public float savedWallRunSpeed;
    public float savedGravityCounterForce;

    private Vector3 savedStartingPosition;


    private void Start()
    {
        wm = GetComponent<WallRunMovement>();

        // save the current values
        savedStartingPosition = this.transform.position;

        savedWallRunSpeed = wm.wallRunSpeed;
        savedGravityCounterForce = wm.gravityCounterForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(speedDown))
            wm.wallRunSpeed *= speedDownMultiplier;
        else if (other.CompareTag(speedUp)) 
            wm.wallRunSpeed *= speedUpMultiplier;

        // this wont be used in most parts
        else if (other.CompareTag(increaseGravity))
            wm.gravityCounterForce *= (1 - increaseGravityMultiplier);

        // this is not technically a wall but it's meant for city 1 (level 2) and would reset the player back
        else if (other.CompareTag(restart))
            this.transform.position = savedStartingPosition;

    }

    private void OnTriggerExit(Collider other)
    {
        wm.wallRunSpeed = savedWallRunSpeed;
        wm.gravityCounterForce = savedGravityCounterForce;
    }
}
