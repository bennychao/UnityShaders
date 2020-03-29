
using UnityEngine;

using System.Collections;



// This is in fact just the Water script from Pro Standard Assets,

// just with refraction stuff removed.



[ExecuteInEditMode] // Make mirror live-update even when not in play mode

public class MirrorReflection : MonoBehaviour

{

    public bool m_DisablePixelLights = true;

    public int m_TextureSize = 256;

    public float m_ClipPlaneOffset = 0.07f;


    public Camera reflectionCamera;

    public LayerMask m_ReflectLayers = -1;



    private Hashtable m_ReflectionCameras = new Hashtable(); // Camera -> Camera table



    public RenderTexture m_ReflectionTexture = null;

    private int m_OldReflectionTextureSize = 0;



    private static bool s_InsideRendering = false;



    // This is called when it's known that the object will be rendered by some

    // camera. We render reflections and do other updates here.

    // Because the script executes in edit mode, reflections for the scene view

    // camera will just work!

    public void OnWillRenderObject()
    {
        var rend = GetComponent<Renderer>();
        if (!enabled || !rend || !rend.sharedMaterial || !rend.enabled)
            return;

        Camera cam = Camera.current;
        if (!cam)
            return;

        // Safeguard from recursive reflections.   
        if (s_InsideRendering)
            return;

        s_InsideRendering = true;

      //  Camera reflectionCamera;
        CreateMirrorObjects(cam, out reflectionCamera);

        // find out the reflection plane: position and normal in world space
        Vector3 pos = transform.position;
        Vector3 normal = transform.up;

        UpdateCameraModes(cam, reflectionCamera);

        // Render reflection

        // Reflect camera around reflection plane

        float d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;

        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);



        Matrix4x4 reflection = Matrix4x4.zero;

        CameraHelper.CalculateReflectionMatrix(ref reflection, reflectionPlane);

        Vector3 oldpos = cam.transform.position;

        Vector3 newpos = reflection.MultiplyPoint(oldpos);

        reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;



        // Setup oblique projection matrix so that near plane is our reflection

        // plane. This way we clip everything below/above it for free.

        Vector4 clipPlane = CameraHelper.CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);

        //Matrix4x4 projection = cam.projectionMatrix;
        //Matrix4x4 projection = cam.CalculateObliqueMatrix(clipPlane);
        Matrix4x4 clipP = reflectionCamera.projectionMatrix;
       CameraHelper.CalculateObliqueMatrix(ref clipP, clipPlane);

        reflectionCamera.projectionMatrix = clipP;

        reflectionCamera.cullingMask = ~(1 << 4) & m_ReflectLayers.value; // never render water layer

        reflectionCamera.targetTexture = m_ReflectionTexture;

        GL.SetRevertBackfacing(true);

        reflectionCamera.transform.position = newpos;

        Vector3 euler = cam.transform.eulerAngles;

       reflectionCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);

        reflectionCamera.Render();

        //reflectionCamera.transform.position = oldpos;

        GL.SetRevertBackfacing(false);

        Material[] materials = rend.sharedMaterials;

        foreach (Material mat in materials)
        {

            if (mat.HasProperty("_ReflectionTex"))

                mat.SetTexture("_ReflectionTex", m_ReflectionTexture);

        }


        // Restore pixel light count

        s_InsideRendering = false;

    }





    // Cleanup all the objects we possibly have created

    void OnDisable()

    {

        if (m_ReflectionTexture)
        {

            DestroyImmediate(m_ReflectionTexture);

            m_ReflectionTexture = null;

        }

        foreach (DictionaryEntry kvp in m_ReflectionCameras)

            DestroyImmediate(((Camera)kvp.Value).gameObject);

        m_ReflectionCameras.Clear();

    }





    private void UpdateCameraModes(Camera src, Camera dest)

    {

        if (dest == null)

            return;

        // set camera to clear the same way as current camera

        dest.clearFlags = src.clearFlags;

        dest.backgroundColor = src.backgroundColor;

        if (src.clearFlags == CameraClearFlags.Skybox)

        {

            Skybox sky = src.GetComponent(typeof(Skybox)) as Skybox;

            Skybox mysky = dest.GetComponent(typeof(Skybox)) as Skybox;

            if (!sky || !sky.material)

            {

                mysky.enabled = false;

            }

            else

            {

                mysky.enabled = true;

                mysky.material = sky.material;

            }

        }

        // update other values to match current camera.

        // even if we are supplying custom camera&projection matrices,

        // some of values are used elsewhere (e.g. skybox uses far plane)

        dest.farClipPlane = src.farClipPlane;

        dest.nearClipPlane = src.nearClipPlane;

        dest.orthographic = src.orthographic;

        dest.fieldOfView = src.fieldOfView;

        dest.aspect = src.aspect;

        dest.orthographicSize = src.orthographicSize;

    }



    // On-demand create any objects we need

    private void CreateMirrorObjects(Camera currentCamera, out Camera reflectionCamera)

    {

        reflectionCamera = null;



        // Reflection render texture

        if (!m_ReflectionTexture || m_OldReflectionTextureSize != m_TextureSize)

        {

            if (m_ReflectionTexture)

                DestroyImmediate(m_ReflectionTexture);

            m_ReflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);

            m_ReflectionTexture.name = "__MirrorReflection" + GetInstanceID();

            m_ReflectionTexture.isPowerOfTwo = true;

            m_ReflectionTexture.hideFlags = HideFlags.DontSave;

            m_OldReflectionTextureSize = m_TextureSize;

        }



        // Camera for reflection

        reflectionCamera = m_ReflectionCameras[currentCamera] as Camera;

        if (!reflectionCamera) // catch both not-in-dictionary and in-dictionary-but-deleted-GO

        {

            GameObject go = new GameObject("Mirror Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));

            reflectionCamera = go.GetComponent<Camera>();

            reflectionCamera.enabled = true;

            reflectionCamera.transform.position = transform.position;

            reflectionCamera.transform.rotation = transform.rotation;

            reflectionCamera.gameObject.AddComponent<FlareLayer>();

           // go.hideFlags = HideFlags.HideAndDontSave;

            m_ReflectionCameras[currentCamera] = reflectionCamera;

        }
    }


}
