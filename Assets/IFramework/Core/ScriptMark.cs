﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-30
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
namespace IFramework
{
    [DisallowMultipleComponent]
    [AddComponentMenu("IFramework/ScriptMark")]
    public class ScriptMark:MonoBehaviour
	{
#if UNITY_EDITOR
        public bool isPublic=true;
        public string fieldName;
        public string fieldType;
        public string description;
        public int index;
#endif

        private void OnEnable()
        {
            Destroy(this);
        }
    }
}
