/*
CarTransform.cs
Use transformation matrices to modify the vertices of a mesh
Applied to car object which will instantiate wheels and move them along with
the car.

Gilberto Echeverria (professor) - Edited on 2023-11-02
Pablo Banzo Prida (student) - Edited on 2023-13-02
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] Vector3 displacement;
    [SerializeField] float angle;
    [SerializeField] AXIS rotationAxis;

    // Mesh class to store the mesh of the object
    Mesh mesh;
    Vector3[] carBaseVertices; // Original vertices
    Vector3[] carNewVertices;

    // Start is called before the first frame update
    void Start()
    {
        // get the mesh of the car's child (of type MeshFilter)
        mesh = GetComponentInChildren<MeshFilter>().mesh;
        // get the vertices of the mesh
        carBaseVertices = mesh.vertices;

        // Create a copy of the original vertices
        carNewVertices = new Vector3[carBaseVertices.Length];
        for (int i = 0; i < carBaseVertices.Length; i++)
        {
            carNewVertices[i] = carBaseVertices[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveCar();
        MoveWheels();
    }

    void MoveWheels()
    {

    }

    void MoveCar()
    {
        // A matrix to move the object
        Matrix4x4 move = Transformations.TranslationMat(displacement.x * Time.time,
                                                      displacement.y * Time.time,
                                                      displacement.z * Time.time);

        // Matrices to apply a rotation around an arbitrary point
        // Using 'displacement' as the point of rotation
        Matrix4x4 moveOrigin = Transformations.TranslationMat(-displacement.x,
                                                            -displacement.y,
                                                            -displacement.z);

        Matrix4x4 moveObject = Transformations.TranslationMat(displacement.x,
                                                            displacement.y,
                                                            displacement.z);

        // Matrix to generate a rotataion
        Matrix4x4 rotate = Transformations.RotateMat(angle * Time.time, rotationAxis);

        // Combine all the matrices into a single one
        // Rotate around a pivot point
        //Matrix4x4 composite = moveObject * rotate * moveOrigin;
        // Roll and move as a wheel
        Matrix4x4 composite = move * rotate;

        // Multiply each vertex in the mesh by the composite matrix
        for (int i = 0; i < carNewVertices.Length; i++)
        {
            Vector4 temp = new Vector4(carBaseVertices[i].x,
                                        carBaseVertices[i].y,
                                        carBaseVertices[i].z,
                                        1);
            carNewVertices[i] = composite * temp;
        }

        // Replace the vertices in the mesh
        mesh.vertices = carNewVertices;
        // Make sure the normals are adapted to the new vertex positions
        mesh.RecalculateNormals();
    }
}