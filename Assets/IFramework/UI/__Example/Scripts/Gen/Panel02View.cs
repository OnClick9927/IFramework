/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.4.17f1
 *Date:           2020-02-28
 *Description:    Description
 *History:        2020-02-28--
*********************************************************************************/

namespace IFramework_Demo
{
    public class Panel02View : IFramework.UI.UIView<Panel02ViewModel, Panel02>
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

    }
}
