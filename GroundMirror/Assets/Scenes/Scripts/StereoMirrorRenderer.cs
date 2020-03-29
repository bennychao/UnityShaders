using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonLib;
using CommomLib;

public class StereoMirrorRenderer : MonoBehaviour {

    public GameObject stereoCameraHead = null;
    public Camera stereoCameraEye = null;
    public float textureResolutionScale = 1.0f;
    public float stereoSeparation = 0.04f;
    public float RTWidth = 1024;

    private Camera mainCamera;

    public Camera MainCamera
    {
        set
        {
            mainCamera = value;
        }
    }

    // render texture for stereo rendering
    private RenderTexture leftEyeTexture = null;
    private RenderTexture rightEyeTexture = null;

    private Matrix4x4 reflectionMat;

    // the materials for displaying render result
    private Material stereoMaterial;

    private BoxCollider curBox;

    private bool bVR = false;

    // Use this for initialization
    void Start () {
        bVR =  UnityEngine.XR.XRSettings.loadedDeviceName == "OpenVR";


        if (stereoCameraHead == null)
            CreateStereoCameraRig();

        SwapStereoShader();

        curBox = GetComponent<BoxCollider>();
        var size = curBox.GetBoxRawSize();
        CreateRenderTextures((int)(size.x * RTWidth), (int)(size.y * RTWidth));

        mainCamera = Camera.main;
        // StereoMirrorMgr.Instance.AddMirror(this);
        //Observable.NextFrame().Subscribe(_ =>
        //{
        //    mainCamera = UserController.Instance.GetPlayerCamera();
        //});
    }
	
	// Update is called once per frame
	void Update () {

        if (mainCamera == null)
        {
            
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            float a = Vector3.Angle(curBox.transform.position - mainCamera.transform.position, mainCamera.transform.forward);
            stereoCameraEye.enabled = a < 45;
            //Debug.Log("stereoCameraEye " + a);
            if (a < 45)
            {
                curBox.DebugShow();
                for (int i = 0; i < 4; i++)
                {
                    Debug.DrawLine(points[i], points[(i + 1) % 4], Color.green);
                }

                Render();

                Debug.DrawLine(mainCamera.transform.position, stereoCameraEye.transform.position);
            }

        }

    }

    public void Render()
    {
        MoveStereoCameraBasedOnHmdPose();

        if (bVR)
            RenderWithTwoTextures();
        else
            RenderWithOneTexture();
    }


    private void SwapStereoShader()
    {
        // swap correct shader for different unity versions
        Renderer renderer = GetComponentInChildren<Renderer>();

        stereoMaterial = renderer.materials[0];

        if (bVR)
            stereoMaterial.shader = Shader.Find("Unlit/StereoMirrorShader");
        else
            stereoMaterial.shader = Shader.Find("Unlit/MirrorShader");
    }

    private void CreateStereoCameraRig()
    {
        stereoCameraHead = new GameObject("Stereo Camera Head [" + gameObject.name + "]");
        stereoCameraHead.transform.parent = transform;

        GameObject stereoCameraEyeObj = new GameObject("Stereo Camera Eye [" + gameObject.name + "]");
        stereoCameraEyeObj.transform.parent = stereoCameraHead.transform;
        stereoCameraEye = stereoCameraEyeObj.AddComponent<Camera>();
       
        stereoCameraEye.enabled = false;
    }


    private void CreateRenderTextures(int sceneWidth, int sceneHeight, int aaLevel = 4)
    {
        int depth = 24;
        int w = (int)(textureResolutionScale * sceneWidth);
        int h = (int)(textureResolutionScale * sceneHeight);

        leftEyeTexture = new RenderTexture(w, h, depth);
        leftEyeTexture.antiAliasing = aaLevel;

#if UNITY_5_4_OR_NEWER
        rightEyeTexture = new RenderTexture(w, h, depth);
        rightEyeTexture.antiAliasing = aaLevel;
#endif
    }

    private void RenderWithTwoTextures()
    {


        // render
        stereoCameraEye.targetTexture = leftEyeTexture;

        Vector3 eyeSeparationLeftDir = stereoCameraHead.transform.right * stereoSeparation;

        stereoCameraEye.transform.position += eyeSeparationLeftDir;
        // set projection matrix
        CalculateCameraMatrix(curBox, stereoCameraEye);
        stereoCameraEye.Render();
        stereoMaterial.SetTexture("_LeftEyeTexture", leftEyeTexture);

        // render
        stereoCameraEye.transform.position -= eyeSeparationLeftDir;

        stereoCameraEye.targetTexture = rightEyeTexture;
        // set projection matrix
        CalculateCameraMatrix(curBox, stereoCameraEye, false);
        //stereoCameraEye.Render();
        stereoMaterial.SetTexture("_RightEyeTexture", rightEyeTexture);
    }

    private void RenderWithOneTexture()
    {
        stereoCameraEye.targetTexture = leftEyeTexture;

        // set projection matrix
        CalculateCameraMatrix(curBox, stereoCameraEye);
        stereoCameraEye.Render();
        stereoMaterial.SetTexture("_LeftEyeTexture", leftEyeTexture);
    }

    Vector3[] points = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
    public void CalculateCameraMatrix(BoxCollider Box, Camera cam, bool bleft = true)
    {
        Vector3 dir = stereoCameraHead.transform.position - Box.transform.position;

        //dl -> dr -> ur -> rl  clockwise 
        points = Box.GetMinProjectBox(ref dir);// new Vector3[] {dl, dr, ur, ul };

        Debug.DrawLine(cam.transform.position - dir, cam.transform.position, Color.yellow);

        Vector3 vecLeft, vecRight;

        Vector3 cameraUp = Vector3.up;


        float aspect = 0;
        for (int i = 0; i < 4; i++)
        {
            int iLeft = (i - 1 + 4) % 4;
            int iRight = (i + 1) % 4;

            vecLeft = points[iLeft] - points[i];
            vecRight = points[iRight] - points[i];

            float angle = Vector3.Angle(vecLeft, vecRight);
            if (angle < 90)
            {
                continue;
            }

            float lenLeft = vecLeft.magnitude;
            float lenRight = vecRight.magnitude;
            if (lenLeft > lenRight)
            {
                Vector3 vec = Vector3.Project(vecRight, vecLeft);

                points[i] = points[i] + vec;

                points[(i + 2) % 4] -= vec;

                cameraUp = points[iRight] - points[i];
                vec = points[iLeft] - points[i];

                aspect = vec.magnitude / cameraUp.magnitude;
            }
            else
            {
                Vector3 vec = Vector3.Project(vecLeft, vecRight);

                points[i] = points[i] + vec;

                points[(i + 2) % 4] -= vec;

                cameraUp = points[iLeft] - points[i];
                vec = points[iRight] - points[i];

                aspect = vec.magnitude / cameraUp.magnitude;
            }

            break;
        }

        cam.transform.rotation = Quaternion.LookRotation(-dir, cameraUp);
        //cam.transform.LookAt(-dir, cameraUp);
        //Vector3 vecDLtoDR = dl - dr;
        //Vector3 vecURtoDR = ur - dr;
        //float lenDLtoDR = vecDLtoDR.magnitude;
        //float lenURtoDR = vecURtoDR.magnitude;
        //if (lenDLtoDR > lenURtoDR)
        //{
        //    //base edge vecDLtoDR

        float fov = Mathf.Atan2(cameraUp.magnitude / 2, dir.magnitude) * 180 / Mathf.PI;

        cam.fieldOfView = fov * 2;

        cam.aspect = aspect;
        Matrix4x4 P = Matrix4x4.Perspective(fov * 2, aspect, 0.1f, 1000);

        Vector4 clipplane = CameraHelper.CameraSpacePlane(cam, Box.transform.position, -Box.transform.forward, 0.01f);

        Matrix4x4 clipP = P;
        CameraHelper.CalculateObliqueMatrix(ref clipP, clipplane);
        ///Matrix4x4 mat = Camera.main.projectionMatrix;
        cam.projectionMatrix = clipP;
        // = Camera.main.CalculateObliqueMatrix(clipplane);

        //calculate the mirror camera's vp
        //var mirrorVPMat = mat * Camera.main.worldToCameraMatrix;
        //var mirrorVPMat = mat.transpose;// * Camera.main.worldToCameraMatrix;
        Matrix4x4 V = cam.worldToCameraMatrix;
        P = GL.GetGPUProjectionMatrix(P, true);
        Matrix4x4 VP = P * V;

        if (bleft)
        {
            Box.GetComponentInChildren<Renderer>().material.SetVector("_matrix01", new Vector4(VP.m00, VP.m01, VP.m02, VP.m03));
            Box.GetComponentInChildren<Renderer>().material.SetVector("_matrix02", new Vector4(VP.m10, VP.m11, VP.m12, VP.m13));
            Box.GetComponentInChildren<Renderer>().material.SetVector("_matrix03", new Vector4(VP.m20, VP.m21, VP.m22, VP.m23));
            Box.GetComponentInChildren<Renderer>().material.SetVector("_matrix04", new Vector4(VP.m30, VP.m31, VP.m32, VP.m33));
        }
        else
        {
            Box.GetComponentInChildren<Renderer>().material.SetVector("_rmatrix01", new Vector4(VP.m00, VP.m01, VP.m02, VP.m03));
            Box.GetComponentInChildren<Renderer>().material.SetVector("_rmatrix02", new Vector4(VP.m10, VP.m11, VP.m12, VP.m13));
            Box.GetComponentInChildren<Renderer>().material.SetVector("_rmatrix03", new Vector4(VP.m20, VP.m21, VP.m22, VP.m23));
            Box.GetComponentInChildren<Renderer>().material.SetVector("_rmatrix04", new Vector4(VP.m30, VP.m31, VP.m32, VP.m33));
        }

        Box.GetComponentInChildren<Renderer>().material.SetFloat("_depth", dir.magnitude - 0.1f);
    }

    public void MoveStereoCameraBasedOnHmdPose()
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
        stereoCameraHead.transform.position = Vector3.Lerp(stereoCameraHead.transform.position,  reflectedPos, 2);
        stereoCameraEye.transform.localPosition = Vector3.zero;

        // set head orientation 
        //Quaternion reflectedRot = reflectionMat.MultiplyPoint()
        Vector3 comForward = reflectionMat.MultiplyVector(mainCamera.transform.forward);
        Vector3 comUp = reflectionMat.MultiplyVector(mainCamera.transform.up);

        stereoCameraHead.transform.rotation = Quaternion.LookRotation(comForward, comUp);

        //for Debug
    }

}
