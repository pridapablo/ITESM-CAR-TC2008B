using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyTransforms : MonoBehaviour
{
[SerializeField] Vector3 displacement;
[SerializeField] float angle;
[SerializeField] AXIS rotationAxis;

Mesh mesh;
Vector3[] baseVertices;
Vector3[] newVertices;
void Start(){
mesh = GetComponentInChildren<MeshFilter>().mesh;
baseVertices = mesh.vertices;

newVertices = new Vector3[baseVertices.Length];
for (int i = 0; i < baseVertices.Length; i++){
newVertices[i] = baseVertices[i];
}


}

// Update is called once per frame
void Update(){
DoTransform();
}

void DoTransform(){
Matrix4x4 move = HW_Transforms.TranslationMat(displacement.x*Time.deltaTime,
displacement.y*Time.deltaTime,
displacement.z*Time.deltaTime);

Matrix4x4 rotate = HW_Transforms.RotateMat(angle * Time.deltaTime,
rotationAxis);

Matrix4x4 posOrigin = HW_Transforms.TranslationMat(-displacement.x,
-displacement.y,
-displacement.z);

Matrix4x4 posObject = HW_Transforms.TranslationMat(displacement.x,
displacement.y,
displacement.z);
Matrix4x4 composite = posObject*rotate*posOrigin;

for (int i = 0; i<newVertices.Length; i++){
Vector4 temp = new Vector4(newVertices[i].x,newVertices[i].y,newVertices[i].z,1);
newVertices[i] = composite * temp;
}
mesh.vertices = newVertices;
}
}