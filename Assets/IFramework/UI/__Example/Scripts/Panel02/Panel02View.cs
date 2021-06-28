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
	public partial class Panel02View
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

		protected override void OnPop(IFramework.UI.UIEventArgs arg)
		{
            Hide();
		}

		protected override void OnPress(IFramework.UI.UIEventArgs arg)
		{
            Hide();
		}

		protected override void OnTop(IFramework.UI.UIEventArgs arg)
		{
            Show();
		}

		protected override void OnShow()
		{
		}

		protected override void OnHide()
		{
		}

		protected override void OnPause()
		{
		}

		protected override void OnUnPause()
		{
		}

		protected override void OnClose()
		{
		}

	}
}