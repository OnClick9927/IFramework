/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.319
 *UnityVersion:   2018.4.17f1
 *Date:           2020-06-04
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.Hotfix.Lua
{
	public class XluaMain
	{
		public XluaMain()
		{
			XLuaEnv.AddLoader(new AddressableLoader());
			XLuaEnv.onDispose += LuaDispose;
			XLuaEnv.DoString("require 'Main'" +
			                 " Awake()");
			
		}

		private void LuaDispose()
		{
			XLuaEnv.DoString("require 'Main'" +
			                 "OnDispose()");
		}
	}
}
