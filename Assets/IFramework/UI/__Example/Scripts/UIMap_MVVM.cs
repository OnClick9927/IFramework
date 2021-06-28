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
	partial class UIMap_MVVM
	{
		public const string Panel01 = "Panel01";
		public const string Panel02 = "Panel02";

	}
	public partial class UIMap_MVVM 
	{
		public static System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Type, System.Type>> map = 
		new System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Type, System.Type>>()
		{

			{ Panel01 ,System.Tuple.Create(typeof(IFramework.UI.Example.Panel01Model),typeof(Panel01View),typeof(Panel01ViewModel))},
			{ Panel02 ,System.Tuple.Create(typeof(IFramework.UI.Example.Panel02Model),typeof(Panel02View),typeof(Panel02ViewModel))},

		}
;	 }
}
