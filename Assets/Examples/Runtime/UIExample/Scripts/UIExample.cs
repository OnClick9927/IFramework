/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.4.17f1
 *Date:           2020-02-28
 *Description:    Description
 *History:        2020-02-28--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IFramework;
using IFramework.UI;

namespace IFramework_Demo
{
	public class UIExample : MonoBehaviour, IPanelLoader
    {
        UIModule module;
        private void Start()
        {
            Framework.CreateEnv("Game_RT",  EnvironmentType.Ev1).InitWithAttribute();
            module = Framework.env1.modules.CreateModule<UIModule>();
            module.AddLoader(this);
            module.SetGroups(new MvvmGroups(UIMap_MVVM.map));
            module.CreateCanvas();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                module.Get<Panel01>("Panel01");
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                module.Get<Panel02>("Panel02");
            }
            Framework.env1.Update();
        }

        public UIPanel Load(Type type, string name)
        {
            GameObject go = Resources.Load<GameObject>(name);
            return go.GetComponent<UIPanel>();
        }
    }
}
