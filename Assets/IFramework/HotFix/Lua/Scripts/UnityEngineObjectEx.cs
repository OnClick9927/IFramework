/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.314
 *UnityVersion:   2018.4.17f1
 *Date:           2020-05-25
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/


namespace IFramework.Hotfix.Lua
{
    public static class UnityEngineObjectEx
	{
        public static bool IsNull(this UnityEngine.Object o) 
        {
            return o == null;
        }
    }
}
