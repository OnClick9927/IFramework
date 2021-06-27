/*********************************************************************************
 *Author:         爱吃水蜜桃
 *Version:        1.0
 *UnityVersion:   2018.4.24f1
 *Date:           2021-06-27
 *Description:    Description
 *History:        2021-06-27--
*********************************************************************************/
namespace IFramework.UI.Example
{
	public partial class Panel01ViewModel : IFramework.UI.UIViewModel<IFramework.UI.Example.Panel01Model>,IFramework.Modules.Message.IMessageListener
	{
 		private System.Int32 _count;
		public System.Int32 count
		{
			get { return GetProperty(ref _count); }
			private set			{
				Tmodel.count = value;
				SetProperty(ref _count, value);
			}
		}


		protected override void SyncModelValue()
		{
 			this.count = Tmodel.count;

		}

	}
}