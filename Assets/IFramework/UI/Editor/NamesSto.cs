/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.135
 *UnityVersion:   2018.4.24f1
 *Date:           2021-06-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace IFramework.UI
{
	public class NamesSto:ScriptableObject
	{
        [System.Serializable]
	    public class NameMap
        {
            public string panelName;
            public string content;
        }
        public List<NameMap> map = new List<NameMap>();
        public string ns = "";
        public string workspace = "";
        public void AddMap(string name,string content)
        {
            var temp= map.Find(item => item.panelName == name);
            if (temp!=null)
            {
                map.Remove(temp);
            }
            map.Add(new NameMap() { panelName = name, content = content });
        }
        public void RemoveMap(string name)
        {
            map.RemoveAll(item => item.panelName == name);
        }
	}
}
