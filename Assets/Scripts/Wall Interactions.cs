using UnityEditor;
using UnityEngine;

public class WallInteractions : MonoBehaviour
{
    [Header("References")]

    public static string speedDown = "Speed Down";
    public static string speedUp = "Speed Up";
    public static string increaseGravity = "Increase Gravity";

    private WallRunMovement wm;

    [Header("Save Variables")]

    public float savedWallRunSpeed;
    public float savedGravityCounterForce;

    private void Start()
    {
        Debug.Log(speedDown);

        wm = GetComponent<WallRunMovement>();

        // save the current values
        savedWallRunSpeed = wm.wallRunSpeed;
        savedGravityCounterForce = wm.gravityCounterForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(speedDown))
            wm.wallRunSpeed *= 0.5f;
        else if (other.CompareTag(speedUp))
            wm.wallRunSpeed *= 3f;
        else if (other.CompareTag(increaseGravity))
            wm.gravityCounterForce *= 0.3f;
    }

    private void OnTriggerExit(Collider other)
    {
        wm.wallRunSpeed = savedWallRunSpeed;
        wm.gravityCounterForce = savedGravityCounterForce;
    }
}
