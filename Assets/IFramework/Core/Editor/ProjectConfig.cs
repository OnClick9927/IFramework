/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using IFramework.GUITool;
using System;
using UnityEditor;
using UnityEngine;

namespace IFramework
{

    [OnEnvironmentInit]
    public static class ProjectConfig
    {
        public static string NameSpace { get { return Info.NameSpace; } }
        public static string UserName { get { return Info.UserName; } }
        public static string Version { get { return Info.Version; } }
        public static string Description { get { return Info.Description; } }
        public static bool enable { get { return Info.enable; } }
        public static bool enable_L { get { return Info.enable_L; } }
        public static bool enable_W { get { return Info.enable_W; } }
        public static bool enable_E { get { return Info.enable_E; } }
        public const string configName = "ProjectConfig";
        [Serializable]
        public class ProjectConfigInfo 
        {
            public bool enable = true;
            public bool enable_L = true;
            public bool enable_W = true;
            public bool enable_E = true;



            public string NameSpace;
            public string UserName;
            public string Version;
            public string Description;
            public ProjectConfigInfo()
            {
                UserName = "OnClick";
                NameSpace = "IFramework_Demo";
                Version = "0.0.1";
                Description = "Description";
            }
        }


        private static ProjectConfigInfo __info;
        private static ProjectConfigInfo Info
        {
            get
            {
                if (__info == null)
                {
                    __info = EditorTools.Prefs.GetObject<ProjectConfigInfo, ProjectConfigInfo>(key);
                    if (__info == null)
                    {
                        EditorTools.Prefs.SetObject<ProjectConfigInfo, ProjectConfigInfo>(key, new ProjectConfigInfo());
                        __info = EditorTools.Prefs.GetObject<ProjectConfigInfo, ProjectConfigInfo>(key);
                    }
                }
                return __info;
            }
        }
        private const string key = "ProjectConfig";
        private static void Save()
        {
            EditorTools.Prefs.SetObject<ProjectConfigInfo, ProjectConfigInfo>(key,Info);
        }



        static ProjectConfig()
        {
            Log.loger = new UnityLoger();
            Log.enable_L = ProjectConfig.enable_L;
            Log.enable_W = ProjectConfig.enable_W;
            Log.enable_E = ProjectConfig.enable_E;
            Log.enable = ProjectConfig.enable;
        }


        [EditorWindowCache("ProjectConfig")]
        class ProjectConfigWindow : EditorWindow, ILayoutGUI
        {
          
            private void OnGUI()
            {


                this.Space()
                        .ETextField(new GUIContent("UserName", "Project Author's Name"), ref Info.UserName)
                        .ETextField(new GUIContent("Version", "Version of Project"), ref Info.Version)
                        .LabelField(new GUIContent("NameSpace", "Script's Namespace"))
                        .TextArea(ref Info.NameSpace)
                        .Label("Description of Scripts")
                        .ETextArea(ref Info.Description, GUILayout.Height(100))
                        .Space(10);
                //.FlexibleSpace();

                this.Label("LogSetting in Editor mode", GUIStyles.Get("IN Title"))
                    .Toggle("Enable", ref Info.enable)
                    .Pan(() =>
                    {
                        GUI.enabled = Info.enable;
                        this.Toggle("Log Enable", ref Info.enable_L)
                            .Toggle("Warning Enable", ref Info.enable_W)
                            .Toggle("Error Enable", ref Info.enable_E);

                        GUI.enabled = true;
                    }).FlexibleSpace();


                this.BeginHorizontal()
                            .FlexibleSpace()
                            .Button(Save, "Save")
                        .EndHorizontal();
            }


        }
    }
}
