using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Camera Cam;
    public Truck Truck;

    public float XOffset = -1.0f;
    public float YOffset = 2.0f;
    public float LerpSpeed = 25.0f;

    private void FixedUpdate()
    {
        var currentPosition = transform.position;
        var truckPosition = Truck.transform.position;

        var newPosition = Vector3.Lerp(currentPosition, truckPosition, LerpSpeed * Time.fixedDeltaTime);
        newPosition.x += XOffset;
        newPosition.y += YOffset;
        newPosition.z = -10;
        transform.position = newPosition;
    }
}
