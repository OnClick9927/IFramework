/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-30
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Linq;
using UnityEngine;

namespace IFramework
{
    [DisallowMultipleComponent]
    [AddComponentMenu("IFramework/ScriptCreater")]
    public class ScriptCreater : MonoBehaviour
    {
#if UNITY_EDITOR
        public string ns = "";
        public string scriptName = "newSp.cs";
        public string createDirectory = "Assets";
        public ScriptMark[] marks;
        public string description = "";
        public int searchIndex;

        public string type = "";
        public void Colllect()
        {
            marks = GetComponentsInChildren<ScriptMark>(true);
        }
        public bool FieldCheckWithScriptName(out string err)
        {
            for (int i = 0; i < marks.Length; i++)
            {
                var mark = marks[i];
                if (mark.fieldName == scriptName)
                {
                    err = "Field Name Should be diferent With ScriptName";
                    return false;
                }
            }
            err = "";
            return true;
        }
        public bool FieldCheck(out string err)
        {
            for (int i = 0; i < marks.Length; i++)
            {
                var mark = marks[i];
                var sameFields = marks.ToList().FindAll((__sm) => { return mark.fieldName == __sm.fieldName; });
                if (sameFields.Count > 1)
                {
                    err = "Can't Exist Same Name Field";
                    return false;
                }

            }
            err = "";
            return true;
        }

#endif
        private void OnEnable()
        {
            Destroy(this);
        }
    }
}
