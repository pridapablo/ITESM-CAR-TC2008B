using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    // Wheel properties
    public Vector3 localPosition;

    // Wheel mesh
    Mesh mesh;
    Vector3[] wheelBaseVertices; // Original vertices
    Vector3[] wheelNewVertices;
    public float wheelRadius = 0.5f;
    public float rotationSpeed = 0f;

    // Method to initialize the wheel
    public void Initialize(Vector3 position)
    {
        this.localPosition = position; // Store the local position
        CopyWheelVertices(); // Copy the wheel vertices
    }

    void CopyWheelVertices()
    {
        // Get the mesh of the wheel
        mesh = GetComponentInChildren<MeshFilter>().mesh;
        // Get the vertices of the wheel
        wheelBaseVertices = mesh.vertices;
        // Create a new array to store the transformed vertices
        wheelNewVertices = new Vector3[wheelBaseVertices.Length];
        for (int i = 0; i < wheelBaseVertices.Length; i++)
        {
            wheelNewVertices[i] = wheelBaseVertices[i];
        }
    }
    Matrix4x4 WheelTransformations(Matrix4x4 carComposite)
    {
        return carComposite;
    }

    public void MoveWheel(Matrix4x4 carComposite)
    {
        // Compute the composite matrix
        Matrix4x4 composite = WheelTransformations(carComposite);
        // Update the mesh
        UpdateWheelMesh(composite);
    }

    void UpdateWheelMesh(Matrix4x4 composite)
    {
        for (int i = 0; i < wheelNewVertices.Length; i++)
        {
            Vector4 temp = new Vector4(wheelBaseVertices[i].x,
                                        wheelBaseVertices[i].y,
                                        wheelBaseVertices[i].z,
                                        1);
            wheelNewVertices[i] = composite * temp;

            // Replace the vertex in the mesh
            mesh.vertices = wheelNewVertices;
            // Recalculate the normals
            mesh.RecalculateNormals();
        }
    }

    // Method to set the wheel's rotation speed
    public void SetRotationSpeed(float speed)
    {
        this.rotationSpeed = speed;
    }
}
