/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.4.24f1
 *Date:           2021-03-23
 *Description:    Description
 *History:        2021-03-23--
*********************************************************************************/

namespace IFramework_Demo
{
    public class UIMap_MVVM 
	{
		public static System.Collections.Generic.Dictionary<System.Type, System.Tuple<System.Type, System.Type, System.Type>> map = 
		new System.Collections.Generic.Dictionary<System.Type, System.Tuple<System.Type, System.Type, System.Type>>()
		{
                        {typeof(IFramework_Demo.Panel02),System.Tuple.Create(typeof(IFramework_Demo.Panel02Model),typeof(IFramework_Demo.Panel02View),typeof(IFramework_Demo.Panel02ViewModel))},

            {typeof(IFramework_Demo.Panel01),System.Tuple.Create(typeof(IFramework_Demo.Panel01Model),typeof(IFramework_Demo.Panel01View),typeof(IFramework_Demo.Panel01ViewModel))},
//ToDo
		}
;	 }
}
