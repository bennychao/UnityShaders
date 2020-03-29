using CommonLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorGround : MonoBehaviour
{
    private GameObject mainCamera;
    public Camera mirrorCamera;

    private Matrix4x4 reflectionMat;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMirrorCameraPos();

        UpdateCameraMatrix();
    }

    private void UpdateCameraMatrix()
    {
        Vector3 dir = mirrorCamera.transform.position - transform.position;
        var up = Vector3.Cross(-dir, mirrorCamera.transform.right);

        mirrorCamera.transform.rotation = Quaternion.LookRotation(-dir, up);

        Vector4 clipplane = CameraHelper.CameraSpacePlane(mirrorCamera, transform.position, transform.forward, 0.01f);

        Matrix4x4 clipP = mirrorCamera.projectionMatrix;

        CameraHelper.CalculateObliqueMatrix(ref clipP, clipplane);

        mirrorCamera.projectionMatrix = clipP;

    }

    public void UpdateMirrorCameraPos()
    {
        // get reflection plane -- assume +y as normal
        Vector3 mirrorPos = transform.position;
        Vector3 mirrorForward = transform.forward;

        float offset = 0;
        float d = -Vector3.Dot(mirrorForward, mirrorPos) - offset;
        Vector4 reflectionPlane = new Vector4(mirrorForward.x, mirrorForward.y, mirrorForward.z, d);

        // get reflection matrix
        reflectionMat = Matrix4x4.zero;
        reflectionMat = MatrixExt.CalculateReflectionMatrix(reflectionPlane);

        // set head position
        Vector3 reflectedPos = reflectionMat.MultiplyPoint(mainCamera.transform.position);
        mirrorCamera.transform.position = Vector3.Lerp(mirrorCamera.transform.position, reflectedPos, 2);
        //mirrorCamera.transform.localPosition = Vector3.zero;

        // set head orientation 
        //Quaternion reflectedRot = reflectionMat.MultiplyPoint()
        //Vector3 comForward = reflectionMat.MultiplyVector(mainCamera.transform.forward);
        //Vector3 comUp = reflectionMat.MultiplyVector(mainCamera.transform.up);
        
        //mirrorCamera.transform.rotation = Quaternion.LookRotation(comForward, comUp);

        //for Debug
    }
}
