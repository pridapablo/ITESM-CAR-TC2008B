/*
Car.cs
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

    // Mesh class to store the mesh of the object
    Mesh mesh;
    Vector3[] carBaseVertices; // Original vertices
    Vector3[] carNewVertices;

    // Create a game object to group everything
    GameObject carObject;

    // Vector of wheel objects
    public List<Wheel> wheels;

    public GameObject wheelPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // Multiply the displacement's z component by -1 to make the car move
        // forward
        // displacement.z *= -1;

        // Create a game object to group everything
        carObject = new GameObject("Car");
        // set the parent of the car to the carObject
        transform.parent = carObject.transform;

        CopyCarVertices();
        InstantiateWheels();
    }

    // Update is called once per frame
    void Update()
    {
        // Compute the composite matrix
        Matrix4x4 composite = CarTransformations();
        // Update the car mesh
        UpdateCarMesh(composite);

        // Move the wheels
        foreach (Wheel wheel in wheels)
        {
            wheel.MoveWheel(composite);
        }
    }

    // Start methods

    void InstantiateWheels()
    {
        // Instantiate the wheels at the correct positions
        InstantiateWheelAtPosition(new Vector3(-0.46f, -0.25f, -0.78f)); // front right
        InstantiateWheelAtPosition(new Vector3(-0.46f, -0.25f, 0.74f)); // back right
        InstantiateWheelAtPosition(new Vector3(0.46f, -0.25f, -0.78f)); // front left
        InstantiateWheelAtPosition(new Vector3(0.46f, -0.25f, 0.74f)); // back left
    }

    void InstantiateWheelAtPosition(Vector3 position)
    {
        GameObject wheelObject = Instantiate(wheelPrefab, new Vector3(0, 0, 0), Quaternion.identity); // At the origin so that rotation is less expensive
        Wheel wheelScript = wheelObject.GetComponent<Wheel>();
        wheelScript.Initialize(position);

        // Set the parent of the wheel to the carObject
        wheelObject.transform.parent = carObject.transform;

        // Add the wheel to the list of wheels
        wheels.Add(wheelScript);
    }

    void CopyCarVertices()
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


    // Update methods

    Matrix4x4 CarTransformations()
    {
        // Translation matrix
        Matrix4x4 move = Transformations.TranslationMat(displacement.x * Time.time,
                                                        displacement.y * Time.time,
                                                        displacement.z * Time.time);

        // Initialize composite matrix as just the translation
        // Matrix4x4 composite = move;

        // Calculate rotation angle based on displacement
        float angle = Mathf.Atan2(displacement.z, displacement.x) * Mathf.Rad2Deg;

        // // Offset the angle by 90 degrees
        angle += 90;

        // Rotation matrix around the Y axis
        Matrix4x4 rotate = Transformations.RotateMat(angle, AXIS.Y);

        Matrix4x4 composite = move * rotate; // Combine rotation and

        // Matrix4x4 composite = move; // Combine rotation and

        return composite;
    }


    void UpdateCarMesh(Matrix4x4 composite)
    {
        // Multiply each vertex in the mesh by the composite matrix
        for (int i = 0; i < carNewVertices.Length; i++)
        {
            Vector4 temp = new Vector4(carBaseVertices[i].x,
                                        carBaseVertices[i].y,
                                        carBaseVertices[i].z,
                                        1);
            carNewVertices[i] = composite * temp;
        }

        // Replace the vertex in the mesh
        mesh.vertices = carNewVertices;
        // Make sure the normals are adapted to the new vertex positions
        mesh.RecalculateNormals();
    }
}