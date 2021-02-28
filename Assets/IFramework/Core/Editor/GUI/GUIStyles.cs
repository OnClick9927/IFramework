/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1.885
 *UnityVersion:   2018.4.17f1
 *Date:           2020-06-25
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.GUITool
{
	public static class GUIStyles
	{
        private static Dictionary<string, GUIStyle> styles;

        public static GUIStyle Get(string name)
        {
            GUIStyle style;
            if (styles==null)
            {
                styles = new Dictionary<string, GUIStyle>();
            }
            if (!styles.TryGetValue(name,out style))
            {
                style = new GUIStyle(name);
                styles.Add(name, style);
            }
            return style;
        }
	}
}
