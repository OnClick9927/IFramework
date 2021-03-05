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
using IFramework;
using IFramework.UI;

namespace IFramework_Demo
{
	public class Panel02View : UIView<Panel02ViewModel, Panel02>
	{
		protected override void BindProperty()
		{
			base.BindProperty();
			//ToDo
		}

		protected override void OnClear()
		{
		}

		protected override void OnLoad()
		{
		}

        protected override void OnPop(UIEventArgs arg)
        {
            Hide();
        }

        protected override void OnPress(UIEventArgs arg)
        {
            Hide();
        }

        protected override void OnTop(UIEventArgs arg)
        {
            Show();
        }

    }
}
