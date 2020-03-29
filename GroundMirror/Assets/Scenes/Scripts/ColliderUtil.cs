using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommomLib
{
    /// <summary>
    /// Collider的工具类
    /// </summary>
    public static class ColliderUtil2
    {
        /// <summary>
        /// 获取BoxCollider自身的Size，不被Rotate所影响
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static Vector3 GetBoxRawSize(this BoxCollider box)
        {
            Transform transform = box.transform;
            return new Vector3(box.size.x * transform.lossyScale.x, box.size.y * transform.lossyScale.y, box.size.z * transform.lossyScale.z);
        }

        /// <summary>
        /// 获取Box前边沿的中心点
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static Vector3 GetForwardCenter(this BoxCollider box)
        {
            return box.transform.position + ((GetBoxRawSize(box).z / 2) * box.transform.forward);
        }

        /// <summary>
        /// 获取Box后边沿的中心点
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static Vector3 GetBackwardCenter(this BoxCollider box)
        {
            return box.transform.position + ((GetBoxRawSize(box).z / 2) * -box.transform.forward);
        }

        /// <summary>
        /// 获取Box左边沿的中心点
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static Vector3 GetLeftCenter(this BoxCollider box)
        {
            return box.transform.position + ((GetBoxRawSize(box).x / 2) * -box.transform.right);
        }

        /// <summary>
        /// 获取Box右边沿的中心点
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static Vector3 GetRightCenter(this BoxCollider box)
        {
            return box.transform.position + ((GetBoxRawSize(box).x / 2) * box.transform.right);
        }

        public static Vector3 GetUpRightCenter(this BoxCollider box)
        {
            return box.transform.position + ((GetBoxRawSize(box).x / 2) * box.transform.right) + ((GetBoxRawSize(box).y / 2) * box.transform.up);
        }

        public static Vector3 GetDownRightCenter(this BoxCollider box)
        {
            return box.transform.position + ((GetBoxRawSize(box).x / 2) * box.transform.right) + ((GetBoxRawSize(box).y / 2) * -box.transform.up);
        }

        public static Vector3 GetUpLeftCenter(this BoxCollider box)
        {
            return box.transform.position + ((GetBoxRawSize(box).x / 2) * -box.transform.right) + ((GetBoxRawSize(box).y / 2) * box.transform.up);
        }

        public static Vector3 GetDownLeftCenter(this BoxCollider box)
        {
            return box.transform.position + ((GetBoxRawSize(box).x / 2) * -box.transform.right) + ((GetBoxRawSize(box).y / 2) * -box.transform.up);
        }



        /// <summary>
        /// 获取8个顶点
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static Vector3[] GetCornerPoints(this BoxCollider box)
        {
            return new Vector3[] { box.transform.position +
                 ((GetBoxRawSize(box).x / 2) * -box.transform.right) +
                 ((GetBoxRawSize(box).y / 2) * box.transform.up) +
                 ((GetBoxRawSize(box).z / 2) * box.transform.forward),

                 box.transform.position +
                 ((GetBoxRawSize(box).x / 2) * -box.transform.right) +
                 ((GetBoxRawSize(box).y / 2) * -box.transform.up) +
                 ((GetBoxRawSize(box).z / 2) * box.transform.forward),

                 box.transform.position +
                 ((GetBoxRawSize(box).x / 2) * -box.transform.right) +
                 ((GetBoxRawSize(box).y / 2) * -box.transform.up) +
                 ((GetBoxRawSize(box).z / 2) * -box.transform.forward),

                 box.transform.position +
                 ((GetBoxRawSize(box).x / 2) * -box.transform.right) +
                 ((GetBoxRawSize(box).y / 2) * box.transform.up) +
                 ((GetBoxRawSize(box).z / 2) * -box.transform.forward),

                 //5 ~8
                box.transform.position +
                ((GetBoxRawSize(box).x / 2) * box.transform.right) +
                 ((GetBoxRawSize(box).y / 2) * box.transform.up) +
                 ((GetBoxRawSize(box).z / 2) * box.transform.forward),

                 box.transform.position +
                 ((GetBoxRawSize(box).x / 2) * box.transform.right) +
                 ((GetBoxRawSize(box).y / 2) * -box.transform.up) +
                 ((GetBoxRawSize(box).z / 2) * box.transform.forward),

                 box.transform.position +
                 ((GetBoxRawSize(box).x / 2) * box.transform.right) +
                 ((GetBoxRawSize(box).y / 2) * -box.transform.up) +
                 ((GetBoxRawSize(box).z / 2) * -box.transform.forward),

                 box.transform.position +
                 ((GetBoxRawSize(box).x / 2) * box.transform.right) +
                 ((GetBoxRawSize(box).y / 2) * box.transform.up) +
                 ((GetBoxRawSize(box).z / 2) * -box.transform.forward)


                 };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static Bounds GetRawBounds(this BoxCollider box)
        {
            //Vector3 maxPoint = box.transform.worldToLocalMatrix * box.GetMaxPoint();
            //Vector3 minPoint = box.transform.worldToLocalMatrix * box.GetMinPoint();
            
            return new Bounds(box.transform.position, box.GetBoxRawSize());
        }

        /// <summary>
        /// 判断是否在当前Box内部
        /// </summary>
        /// <param name="container"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static bool InBox(this BoxCollider container, BoxCollider box)
        {
            Bounds bound = new Bounds(Vector3.zero, container.size);

            return box.GetCornerPoints().Where((a) => bound.Contains(container.transform.worldToLocalMatrix.MultiplyPoint(a))).Count() == 8;
        }

        /// <summary>
        /// Debug显示Box的四个中心
        /// </summary>
        /// <param name="Box"></param>
        static public void DebugShow(this BoxCollider Box)
        {
            Debug.DrawLine(Box.transform.position, Box.GetDownLeftCenter());

            Debug.DrawLine(Box.transform.position, Box.GetDownRightCenter());

            Debug.DrawLine(Box.transform.position, Box.GetUpLeftCenter());

            Debug.DrawLine(Box.transform.position, Box.GetUpRightCenter());
        }

        /// <summary>
        /// 获取Box在以Dir为法线的平面的最小的、最近的Box
        /// </summary>
        /// <param name="Box"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        static public Vector3[] GetMinProjectBox(this BoxCollider Box, ref Vector3 dir)
        {

            float fdirdl = Vector3.Dot(Box.GetDownLeftCenter() - Box.transform.position, dir);
            float fdirdr = Vector3.Dot(Box.GetDownRightCenter() - Box.transform.position, dir);
            float fdirul = Vector3.Dot(Box.GetUpLeftCenter() - Box.transform.position, dir);
            float fdirur = Vector3.Dot(Box.GetUpRightCenter() - Box.transform.position, dir);

            List<float> data = new List<float>();
            data.Add(fdirdl);
            data.Add(fdirdr);
            data.Add(fdirul);
            data.Add(fdirur);

            Vector3 subDir = Vector3.zero;

            float fMax = 0;
            int iCurMax = -1;
            for (int i = 0; i < 4; i++)
            {
                if (fMax > data[i])
                    continue;
                fMax = data[i];
                iCurMax = i;
            }

            switch (iCurMax)
            {
                case 0:
                    subDir = Vector3.Project(Box.GetDownLeftCenter() - Box.transform.position, dir);
                    break;

                case 1:
                    subDir = Vector3.Project(Box.GetDownRightCenter() - Box.transform.position, dir);
                    break;

                case 2:
                    subDir = Vector3.Project(Box.GetUpLeftCenter() - Box.transform.position, dir);
                    break;

                case 3:
                    subDir = Vector3.Project(Box.GetUpRightCenter() - Box.transform.position, dir);
                    break;
            }


            Vector3 pos = Box.transform.position;
            dir -= subDir;
            pos += subDir;

            Vector3 dl = Vector3.ProjectOnPlane(Box.GetDownLeftCenter() - pos, dir) + pos;
            Vector3 dr = Vector3.ProjectOnPlane(Box.GetDownRightCenter() - pos, dir) + pos;
            Vector3 ul = Vector3.ProjectOnPlane(Box.GetUpLeftCenter() - pos, dir) + pos;
            Vector3 ur = Vector3.ProjectOnPlane(Box.GetUpRightCenter() - pos, dir) + pos;

            Debug.DrawLine(Box.transform.position, dl, Color.red);
            Debug.DrawLine(Box.transform.position, dr, Color.red);
            Debug.DrawLine(Box.transform.position, ul, Color.red);
            Debug.DrawLine(Box.transform.position, ur, Color.red);


            //dl -> dr -> ur -> rl  clockwise 
            return new Vector3[] { dl, dr, ur, ul };

        }
    }

}
