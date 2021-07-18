/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.151
 *UnityVersion:   2018.4.24f1
 *Date:           2021-07-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
namespace IFramework.Tweens
{
    [CustomEditor(typeof(DoScaleComponent))]
    public class DoScaleComponentEditor : TweenComponentEditor<Vector3,Transform> { }
}
