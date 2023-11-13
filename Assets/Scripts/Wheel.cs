using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    // Wheel properties
    public Vector3 localPosition;
    public float wheelRadius = 0.5f;
    public float rotationSpeed = 0f;

    // Method to initialize the wheel
    public void Initialize(Vector3 position)
    {
        this.localPosition = position;
        this.transform.localPosition = position;
        // Additional initialization steps can be added here
    }

    // Update is called once per frame
    void Update()
    {
        RotateWheel();
    }

    // Method to rotate the wheel
    private void RotateWheel()
    {
        // Calculate the rotation amount
        float rotationAmount = rotationSpeed * Time.deltaTime;
        // Apply rotation around the wheel's axis
        this.transform.Rotate(Vector3.right, rotationAmount);
    }

    // Method to set the wheel's rotation speed
    public void SetRotationSpeed(float speed)
    {
        this.rotationSpeed = speed;
    }
}
