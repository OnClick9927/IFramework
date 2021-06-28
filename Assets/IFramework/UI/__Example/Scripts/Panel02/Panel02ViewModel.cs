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
	public partial class Panel02ViewModel
	{
		protected override void Initialize()
		{

		}

		protected override void OnDispose()
		{

		}

		void IFramework.Modules.Message.IMessageListener.Listen(IFramework.Modules.Message.IMessage message)
		{

		}

		protected override void SubscribeMessage()
		{
			message.Subscribe<Panel02View>(this);
		}

		protected override void UnSubscribeMessage()
		{
			message.UnSubscribe<Panel02View>(this);
		}

	}
}