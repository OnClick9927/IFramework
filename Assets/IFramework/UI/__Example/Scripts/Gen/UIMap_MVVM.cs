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

namespace IFramework_Demo
{
	public class UIMap_MVVM 
	{
		public static Dictionary<Type, Tuple<Type, Type, Type>> map =
		new Dictionary<Type, Tuple<Type, Type, Type>>()
		{
			{typeof(IFramework_Demo.Panel01),Tuple.Create(typeof(IFramework_Demo.Panel01Model),typeof(IFramework_Demo.Panel01View),typeof(IFramework_Demo.Panel01ViewModel))},
			{typeof(IFramework_Demo.Panel02),Tuple.Create(typeof(IFramework_Demo.Panel02Model),typeof(IFramework_Demo.Panel02View),typeof(IFramework_Demo.Panel02ViewModel))},
//ToDo
		};
	}
}
