/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.148
 *UnityVersion:   2018.4.24f1
 *Date:           2021-07-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
namespace IFramework.Tweens
{
    public class DoScaleComponent : TweenComponent<Vector3,Transform>
    {
        protected override Vector3 GetTargetValue()
        {
            return transform.localScale;
        }

        protected override void SetTargetValue(Vector3 value)
        {
            transform.localScale = value;
        }
    }
}
