/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.4.17f1
 *Date:           2020-04-04
 *Description:    Description
 *History:        2020-04-04--
*********************************************************************************/
using UnityEngine;
using IFramework.Tweens;
using UnityEngine.UI;
using IFramework.NodeAction;
using System;

namespace IFramework_Demo
{
    public class TweenTest : MonoBehaviour
    {
        public Transform cube;
        public Text text;
        ITween tc;
        public AnimationCurve curve;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                tc.Rewind(1);
            }
            if (Input.GetKey(KeyCode.A))
            {
                tc.ReStart();
            }
            if (Input.GetKey(KeyCode.Q))
            {
                tc.Complete(false);
            }
        }




        public void Start()
        {
            //tc = cube.DoMove(new Vector3[] {
            //    Vector3.zero,
            //    Vector3.one,
            //    Vector3.one * 2,
            //    Vector3.one * 3,
            //    Vector3.one * -4,
            //    Vector3.one * 5,
            //    Vector3.one * 6,
            //}, 5, false)
            //.SetRecyle(false);

            Debug.Log(Time.time);
            tc = cube.DoMove(cube.transform.position + Vector3.right * 5, 2, false)
                  .SetLoop(4, LoopType.PingPong)
                  .SetAnimationCurve(curve)
                  .SetRecyle(false)
                  .OnCompelete(() => {
                      Debug.Log(Time.time);
                  })
                  ;
            //cube.DoMove(cube.transform.position + Vector3.up * 2, 2)
            //         .SetLoop(-1, LoopType.PingPong)
            //         .SetCurve(TweenCurves.scurve)
            //         .SetRecyle(false);



            //cube.DoScale(Vector3.one * 10, 0.5f,true)
            //    .SetLoop(3, LoopType.PingPong);
            //cube.DoRota(new Vector3(0, 360, 0), 5f,false)
            //    .SetLoop(-1, LoopType.ReStart)
            //    .SetCurve(TweenCurves.hugeCCurve)
            //    .SetRecyle(false);
            //cube.GetComponent<Renderer>().material.DoColor(Color.cyan, 0.6f)
            //     .SetLoop(-1, LoopType.PingPong)
            //     .SetRecyle(false);


            //text.DoText(0, 10f, 2f).SetLoop(-1, LoopType.PingPong);
            //text.DoText("123456789", 2)
            //        .SetLoop(-1, LoopType.PingPong)
            //        .SetCurve(TweenCurves.scurve);
        }
    }
}
