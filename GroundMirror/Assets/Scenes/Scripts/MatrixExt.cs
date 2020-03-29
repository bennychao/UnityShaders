using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CommonLib
{
    public static class MatrixExt
    {
        static public Matrix4x4 CalculateReflectionMatrix(Vector4 normal)
        {
            Matrix4x4 reflectionMat;
            reflectionMat.m00 = (1.0f - 2.0f * normal[0] * normal[0]);
            reflectionMat.m01 = (-2.0f * normal[0] * normal[1]);
            reflectionMat.m02 = (-2.0f * normal[0] * normal[2]);
            reflectionMat.m03 = (-2.0f * normal[3] * normal[0]);

            reflectionMat.m10 = (-2.0f * normal[1] * normal[0]);
            reflectionMat.m11 = (1.0f - 2.0f * normal[1] * normal[1]);
            reflectionMat.m12 = (-2.0f * normal[1] * normal[2]);
            reflectionMat.m13 = (-2.0f * normal[3] * normal[1]);

            reflectionMat.m20 = (-2.0f * normal[2] * normal[0]);
            reflectionMat.m21 = (-2.0f * normal[2] * normal[1]);
            reflectionMat.m22 = (1.0f - 2.0f * normal[2] * normal[2]);
            reflectionMat.m23 = (-2.0f * normal[3] * normal[2]);

            reflectionMat.m30 = 0.0f;
            reflectionMat.m31 = 0.0f;
            reflectionMat.m32 = 0.0f;
            reflectionMat.m33 = 1.0f;

            return reflectionMat;
        }
    }
}


