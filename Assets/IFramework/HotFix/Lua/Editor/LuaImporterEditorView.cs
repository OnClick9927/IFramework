/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-30
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
#if UNITY_2018_1_OR_NEWER

namespace IFramework.Hotfix.Lua
{
    [CustomEditor(typeof(LuaImporter))]
    public class LuaImporterEditorView: ScriptedImporterEditor
    {
        LuaImporter im { get { return this.target as LuaImporter; } }
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Edit"))
            {
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(im.assetPath, 0);

            }
        }
    }
}
#endif