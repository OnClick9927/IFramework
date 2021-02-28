/*********************************************************************************
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
    [AddComponentMenu("IFramework/ScriptCreater")]
    public class ScriptCreater : MonoBehaviour
    {
        public string scriptName="newSp.cs";
        public string createDirectory="Assets";
        public ScriptMark[] marks;
        public string description="";
        public int searchIndex;
        private void OnEnable()
        {
            Destroy(this);
        }
    }
}
