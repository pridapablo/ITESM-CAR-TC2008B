/* Use matrices to modify the vertices of a mesh 

Note: the child has the mesh, not the parent object

Pablo Banzo Prida
2023-11-02
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyTransform : MonoBehaviour
{
    [SerializeField] Vector3 displacement;
    [SerializeField] float angle;
    [SerializeField] AXIS rotationAxis;

    // Mesh class to store the mesh of the object
    Mesh mesh;
    Vector3[] baseVertices; // Original vertices
    Vector3[] newVertices;

    void Start()
    {
        // get the mesh of the object's child (of type MeshFilter)
        mesh = GetComponentInChildren<MeshFilter>().mesh;
        // get the vertices of the mesh
        baseVertices = mesh.vertices;

        // Create a copy of the original vertices
        newVertices = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
        {
            newVertices[i] = baseVertices[i];
        }


    }

    // Update is called once per frame
    void Update()
    {
        DoTransform();

    }

    void DoTransform()
    {
        Matrix4x4 move = HW_Transforms.TranslationMat(
            displacement.x * Time.deltaTime, // we add Time.deltaTime to make the movement frame rate independent
            displacement.y * Time.deltaTime,
            displacement.z * Time.deltaTime
        );

        Matrix4x4 moveOrigin = HW_Transforms.TranslationMat( // Move the object to the origin
            -displacement.x * Time.deltaTime,
            -displacement.y * Time.deltaTime,
            -displacement.z * Time.deltaTime
        );

        Matrix4x4 moveObject = HW_Transforms.TranslationMat( // Move the object back to its original position
            displacement.x,
            displacement.y,
            displacement.z
        );

        Matrix4x4 rotate = HW_Transforms.RotateMat( // Rotate the object
            angle * Time.deltaTime,
            rotationAxis
        );

        // Combine all the matrices into one.
        // If move, then rotate, (rotate * move) then the object will rotate around the origin, not around itself 

        Matrix4x4 composite = move * rotate;

        // Matrix4x4 composite = move;


        for (int i = 0; i < newVertices.Length; i++)
        {

            Vector4 temp = new Vector4(
                baseVertices[i].x,
                baseVertices[i].y,
                baseVertices[i].z, 1
                );
            newVertices[i] = composite * temp;
        }

        // Replace the vertices of the mesh with the new ones
        mesh.vertices = newVertices;
        mesh.RecalculateNormals(); // Recalculate the normals of the mesh


    }
}
