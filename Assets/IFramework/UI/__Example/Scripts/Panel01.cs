/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.4.17f1
 *Date:           2020-02-28
 *Description:    Description
 *History:        2020-02-28--
*********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using IFramework;
using UnityEngine.UI;
using UnityEngine;
using IFramework.Modules.MVVM;
using IFramework.UI;

namespace IFramework_Demo
{
    public class Panel01Model:IModel
    {
        public int count=100;
    }

    public class Panel01 : UIPanel
	{
		public Text Count_Text;
		public Button BTn_ADD;
		public Button BTn_SUB;

	}
}
