/*********************************************************************************
 *Author:         爱吃水蜜桃
 *Version:        1.0
 *UnityVersion:   2018.4.24f1
 *Date:           2021-06-27
 *Description:    Description
 *History:        2021-06-27--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using IFramework;
using UnityEngine;
namespace IFramework.UI.Example
{
    public class UI_Game : Game,IPanelLoader
    {
        UIModule module;
        public override void Init()
        {
            module = modules.GetModule<UIModule>("Example");
            module.CreateCanvas();
            module.AddLoader(this);
            module.SetGroups(new MvvmGroups(UIMap_MVVM.map));
        }

        public UIPanel Load(ref string name)
        {
            return Resources.Load<GameObject>(name).GetComponent<UIPanel>();
        }

        public override void Startup()
        {
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                module.Get(UIMap_MVVM.Panel01);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                module.Get(UIMap_MVVM.Panel02);

            }
        }
    }
}
