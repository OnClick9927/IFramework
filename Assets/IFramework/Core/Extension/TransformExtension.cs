/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-12
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework
{
    public static partial class TransformExtension
    {
        public static T MakeComponentExist<T>(this T selfComponent) where T : Component
        {
            T t;
            t = selfComponent.GetComponent<T>();
            return t == null ? selfComponent.gameObject.AddComponent<T>() : t;
        }
        public static void RmoveComponent<T>(this T selfComponent) where T : Component
        {
            T t;
            t = selfComponent.GetComponent<T>();
            if (t!=null)
            {
                Object.Destroy(t);
            }
        }
        public static T LocalIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localPosition = Vector3.zero;
            selfComponent.transform.localRotation = Quaternion.identity;
            selfComponent.transform.localScale = Vector3.one;
            return selfComponent;
        }
        public static string GetPath(this Transform transform)
        {
            var sb = new System.Text.StringBuilder();
            var t = transform;
            while (true)
            {
                sb.Insert(0, t.name);
                t = t.parent;
                if (t)
                {
                    sb.Insert(0, "/");
                }
                else
                {
                    return sb.ToString();
                }
            }
        }


    }
}