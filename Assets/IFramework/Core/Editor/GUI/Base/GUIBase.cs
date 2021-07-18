/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;

namespace IFramework.GUITool
{
    public abstract class GUIBase :Unit
    {
        public Rect position { get; private set; }

        public virtual void OnGUI(Rect position) 
        {
            this.position = position;
        }
    }

}
