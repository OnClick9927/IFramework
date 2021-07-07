/*********************************************************************************
 *Author:         爱吃水蜜桃
 *Version:        1.0
 *UnityVersion:   2018.4.24f1
 *Date:           2021-06-28
 *Description:    Description
 *History:        2021-06-28--
*********************************************************************************/
namespace IFramework.UI.Example
{
	public partial class Panel01View : IFramework.UI.UIView<Panel01ViewModel, Panel01> 
	{
		private UnityEngine.UI.Text Count_Text { get { return Tpanel.Count_Text; } }
		private UnityEngine.UI.Button BTn_ADD { get { return Tpanel.BTn_ADD; } }
		private UnityEngine.UI.Button BTn_SUB { get { return Tpanel.BTn_SUB; } }
        private UnityEngine.UI.Button BTn_Next { get { return Tpanel.BTn_Next; } }

    }
}