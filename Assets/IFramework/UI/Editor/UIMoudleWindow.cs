/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using IFramework.GUITool;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace IFramework.UI
{
    [EditorWindowCache("IFramework.UIModule")]
    public partial class UIMoudleWindow : EditorWindow
    {
        private class Styles
        {
            public static GUIStyle BoldLabel = EditorStyles.boldLabel;
            public static GUIStyle toolbar = EditorStyles.toolbar;
            static Styles()
            {
                BoldLabel.fontSize = 10;
            }
        }
        public class UIMoudleWindowTab
        {
            public virtual string name { get; }
            public virtual void OnGUI() { }
            public virtual void OnEnable() { }
            public virtual void OnDisable() { }
        }

        [Serializable]
        public class MVVM_GenCodeView : UIMoudleWindowTab
        {
            private const string key = "MVVM_GenCodeView";
            public override string name { get { return "MVVN_GenCode_CS"; } }
            [SerializeField] private string UIMapDir;
            [SerializeField] private string PanelGenDir;
            [SerializeField] private string UIMapName = "UIMap_MVVM";
            [SerializeField] private List<string> panelTypes;
            [SerializeField] private string panelType;
            [SerializeField] private List<string> modelTypes;
            [SerializeField] private string modelType;

            string UIMap_CSName { get { return UIMapName.Append(".cs"); } }

            public override void OnEnable()
            {
                var last = EditorTools.Prefs.GetObject<MVVM_GenCodeView, MVVM_GenCodeView>(key);
                if (last != null)
                {
                    this.UIMapDir = last.UIMapDir;
                    this.PanelGenDir = last.PanelGenDir;
                    this.UIMapName = last.UIMapName;

                    this.panelTypes = last.panelTypes;
                    this.panelType = last.panelType;
                    this.modelTypes = last.modelTypes;
                    this.modelType = last.modelType;
                }
            }
            public override void OnDisable()
            {
                EditorTools.Prefs.SetObject<MVVM_GenCodeView, MVVM_GenCodeView>(key, this);
            }
            private int hashID;
            private bool DropdownButton(int id, Rect position, GUIContent content)
            {
                Event e = Event.current;
                switch (e.type)
                {
                    case EventType.MouseDown:
                        if (position.Contains(e.mousePosition) && e.button == 0)
                        {
                            Event.current.Use();
                            return true;
                        }
                        break;
                    case EventType.KeyDown:
                        if (GUIUtility.keyboardControl == id && e.character == '\n')
                        {
                            Event.current.Use();
                            return true;
                        }
                        break;
                    case EventType.Repaint:
                        Styles.BoldLabel.Draw(position, content, id, false);
                        break;
                }
                return false;
            }
            public override void OnGUI()
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Check UIMap Script Name", Styles.toolbar);
                    UIMapName = EditorGUILayout.TextField(UIMapName);

                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Drag UIMap Gen Directory", Styles.toolbar);
                    GUILayout.Label(UIMapDir);
                    Rect rect = GUILayoutUtility.GetLastRect();
                    if (string.IsNullOrEmpty(UIMapDir))
                        rect.DrawOutLine(10, Color.red);
                    else
                        rect.DrawOutLine(2, Color.black);
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        var drag = EditorTools.DragAndDropTool.Drag(Event.current, rect);
                        if (drag.compelete && drag.enterArera && drag.paths.Length == 1)
                        {
                            string path = drag.paths[0];
                            if (path.Contains("Assets"))
                            {
                                if (path.IsDirectory())
                                    UIMapDir = path;
                                else
                                    UIMapDir = path.GetDirPath();
                            }

                        }
                    }

                    GUILayout.EndHorizontal();
                }
                //GUILayout.BeginHorizontal();
                //{
                //    GUILayout.FlexibleSpace();
                //    if (GUILayout.Button("Create UIMap"))
                //    {
                //        if (string.IsNullOrEmpty(UIMapDir))
                //        {
                //            EditorWindow.focusedWindow.ShowNotification(new GUIContent("Set UI Map Gen Dir "));
                //            return;
                //        }
                //        WriteTxt(UIMapDir.CombinePath(UIMap_CSName), mapScriptOrigin, null);
                //        AssetDatabase.Refresh();
                //    }

                //    GUILayout.EndHorizontal();
                //}
                GUILayout.Space(30);
                if (hashID == 0) hashID = "MVVM_GenCodeView".GetHashCode();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Click Choose Panel Type", Styles.toolbar);
                    GUILayout.Label("");
                    Rect pos = GUILayoutUtility.GetLastRect();

                    int ctrlId = GUIUtility.GetControlID(hashID, FocusType.Keyboard, pos);
                    {
                        if (DropdownButton(ctrlId, pos, new GUIContent(string.Format("PanelType: {0}", panelType))))
                        {
                            if (panelTypes == null)
                            {
                                EditorWindow.focusedWindow.ShowNotification(new GUIContent("Fresh Panel Types"));
                                return;
                            }
                            int index = -1;
                            for (int i = 0; i < panelTypes.Count; i++)
                            {
                                if (panelTypes[i] == panelType)
                                {
                                    index = i; break;
                                }
                            }
                            SearchablePopup.Show(pos, panelTypes.ToArray(), index, (i, str) =>
                            {
                                panelType = str;
                            });
                        }
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Click Choose Model Type", Styles.toolbar);
                    GUILayout.Label("");
                    Rect pos = GUILayoutUtility.GetLastRect();

                    int ctrlId = GUIUtility.GetControlID(hashID, FocusType.Keyboard, pos);
                    {
                        if (DropdownButton(ctrlId, pos, new GUIContent(string.Format("ModelType: {0}", modelType))))
                        {
                            if (modelTypes == null)
                            {
                                EditorWindow.focusedWindow.ShowNotification(new GUIContent("Fresh Model Types"));
                                return;
                            }
                            int index = -1;
                            for (int i = 0; i < modelTypes.Count; i++)
                            {
                                if (modelTypes[i] == modelType)
                                {
                                    index = i; break;
                                }
                            }
                            SearchablePopup.Show(pos, modelTypes.ToArray(), index, (i, str) =>
                            {
                                modelType = str;
                            });
                        }
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                {

                    GUILayout.Label("Drag Panel Gen Directory", Styles.toolbar);
                    GUILayout.Label(PanelGenDir);
                    Rect rect = GUILayoutUtility.GetLastRect();
                    if (string.IsNullOrEmpty(PanelGenDir))
                        rect.DrawOutLine(10, Color.red);
                    else
                        rect.DrawOutLine(2, Color.black);
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        var drag = EditorTools.DragAndDropTool.Drag(Event.current, rect);
                        if (drag.compelete && drag.enterArera && drag.paths.Length == 1)
                        {
                            string path = drag.paths[0];
                            if (path.Contains("Assets"))
                            {
                                if (path.IsDirectory())
                                    PanelGenDir = path;
                                else
                                    PanelGenDir = path.GetDirPath();
                            }

                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                {

                    if (GUILayout.Button("Fresh Panel && Model Types"))
                    {
                        panelTypes = typeof(UIPanel).GetSubTypesInAssemblys().Select((type) =>
                        {
                            return type.FullName;
                        }).ToList();
                        modelTypes = typeof(IModel).GetSubTypesInAssemblys().Select((type) =>
                        {
                            return type.FullName;
                        }).ToList();
                    }
                    GUILayout.Space(20);
                    if (GUILayout.Button("Gen"))
                    {
                        if (string.IsNullOrEmpty(UIMapDir))
                        {
                            EditorWindow.focusedWindow.ShowNotification(new GUIContent("Set UI Map Gen Dir "));
                            return;
                        }
                        //if (!File.Exists(UIMapDir.CombinePath(UIMap_CSName)))
                        //{
                        //    EditorWindow.focusedWindow.ShowNotification(new GUIContent("Copy UI Map"));
                        //    return;
                        //}
                        if (string.IsNullOrEmpty(panelType))
                        {
                            EditorWindow.focusedWindow.ShowNotification(new GUIContent("Choose UI Panel Type "));
                            return;
                        }
                        if (string.IsNullOrEmpty(modelType))
                        {
                            EditorWindow.focusedWindow.ShowNotification(new GUIContent("Choose UI Model Type "));
                            return;
                        }
                        if (string.IsNullOrEmpty(PanelGenDir))
                        {
                            EditorWindow.focusedWindow.ShowNotification(new GUIContent("Set UI Panel Gen Dir "));
                            return;
                        }
                        string _modelType = modelType.Split('.').ToList().Last();
                        string paneltype = panelType.Split('.').ToList().Last();
                        string vmType = paneltype.Append("ViewModel");
                        string viewType = paneltype.Append("View");

                        WriteView(viewType, vmType);
                        WriteVM(vmType, viewType);
                        WriteMap(UIMapDir.CombinePath(UIMap_CSName), UIMapDir.CombinePath(UIMap_CSName));
                        AssetDatabase.Refresh();
                    }

                    GUILayout.EndHorizontal();
                }


            }
            private void WriteVM(string vmType, string viewType)
            {
                string designPath = PanelGenDir.CombinePath(vmType.Append(".Design.cs"));
                string path = PanelGenDir.CombinePath(vmType.Append(".cs"));

                WriteTxt(designPath, vmDesignScriptOrigin,
                               (str) => {
                                   string fieldStr = " ";
                                   string syncStr = " ";
                                   Type t = AppDomain.CurrentDomain.GetAssemblies()
                                               .SelectMany((a) => { return a.GetTypes(); })
                                               .ToList().Find((type) => { return type.FullName == modelType; });
                                   var fs = t.GetFields();
                                   for (int i = 0; i < fs.Length; i++)
                                   {
                                       var info = fs[i];
                                       fieldStr = WriteField(fieldStr, info.FieldType, info.Name);
                                       syncStr = WriteSyncStr(syncStr, info.Name);
                                   }

                                   return str.Replace("#ModelType#", t.FullName)
                                             .Replace("#FieldString#", fieldStr)
                                             .Replace("#viewType#", viewType)
                                             .Replace("#SyncModelValue#", syncStr)
                                             .Replace(".Design","");
                               }
                           );
                if (!File.Exists(path))
                {
                    WriteTxt(path, vmScriptOrigin, (str)=> {
                        return str.Replace("#viewType#", viewType);
                    });
                }
            }

            private void WriteView(string viewType, string vmType)
            {
                string designPath = PanelGenDir.CombinePath(viewType.Append(".Design.cs"));
                string path = PanelGenDir.CombinePath(viewType.Append(".cs"));

                WriteTxt(designPath, viewDesignScriptOrigin,
               (str) => {
                   Type t = AppDomain.CurrentDomain.GetAssemblies()
                             .SelectMany((a) => { return a.GetTypes(); })
                             .ToList().Find((type) => { return type.FullName == panelType; });

                   return str.Replace("#VMType#", vmType)
                   .Replace("#PanelType#", panelType)
                   .Replace("#panelfield#", GetPanelField(t))
                   .Replace(".Design", "");
               }
               );
                if (!File.Exists(path))
                {
                    WriteTxt(path, viewScriptOrigin, null);
                }
            }
            private string GetPanelField(Type panel)
            {
                string result = "";
                panel.GetFields().ForEach((field) => {
                    result += string.Format("\t\tprivate {0} {2} {1} get {1} return Tpanel.{2}; {3} {3}\n", field.FieldType.FullName, "{", field.Name, "}");
                });
                return result;
            }
            private string WriteField(string result, Type ft, string fn)
            {
                return result.Append(string.Format("\t\tprivate {0} _{1};\n", ft.FullName, fn))
                                .Append(string.Format("\t\tpublic {0} {1}\n", ft.FullName, fn))
                                .Append(string.Format("\t\t{0}\n", "{"))
                                //.Append(string.Format("\t\t\tget {0} return GetProperty(ref _{1}, this.GetPropertyName(() => _{1})); {2}\n", "{", fn, "}"))
                                .Append(string.Format("\t\t\tget {0} return GetProperty(ref _{1}); {2}\n", "{", fn, "}"))

                                .Append(string.Format("\t\t\tprivate set"))
                                .Append(string.Format("\t\t\t{0}\n", "{"))
                                .Append(string.Format("\t\t\t\tTmodel.{0} = value;\n", fn))
                                //.Append(string.Format("\t\t\t\tSetProperty(ref _{0}, value, this.GetPropertyName(() => _{0}));\n", fn))
                                .Append(string.Format("\t\t\t\tSetProperty(ref _{0}, value);\n", fn))

                                .Append(string.Format("\t\t\t{0}\n", "}"))
                                .Append(string.Format("\t\t{0}\n\n", "}"));
            }
            private string WriteSyncStr(string result, string fn)
            {
                return result.Append(string.Format("\t\t\tthis.{0} = Tmodel.{0};\n", fn));

            }

            private void WriteMap(string writePath, string sourcePath)
            {
                string txt = "";
                if (File.Exists(sourcePath))
                {
                    txt = File.ReadAllText(sourcePath);
                }
                var strs = txt.Replace("\r\n", "\n").Split('\n');
                string flag = "//ToDo";
                List<string> fits = new List<string>();

                for (int i = 0; i < strs.Length; i++)
                {
                    if (strs[i].Contains("typeof") || strs[i].Contains(flag))
                    {
                        fits.Add(strs[i]);
                    }
                }
                if (fits.Count==0)
                {
                    fits.Add(flag);
                }
                string panelfitString = string.Format("typeof({0})", panelType);
                for (int i = 0; i < fits.Count; i++)
                {
                    if (fits[i].Contains(panelfitString))
                    {
                        fits.RemoveAt(i);
                        break;
                    }
                }
                string tmp = string.Format("\t\t\t{2}typeof({0}),System.Tuple.Create(typeof({1}),typeof({0}View),typeof({0}ViewModel)){3},", panelType, modelType, "{", "}");
                fits.Insert(0, tmp);
                string replace = "";
                for (int i = 0; i < fits.Count-1; i++)
                {
                    replace= replace.Append(string.Format("{0}\n", fits[i]));
                }

                txt = mapScriptOrigin.Replace(flag, replace.Append(string.Format("\t\t\t{0}\n",flag)));
                WriteTxt(writePath, txt, null);
            }
            private static void WriteTxt(string writePath, string source, Func<string, string> func)
            {
                source = source.Replace("#User#", EditorTools.ProjectConfig.UserName)
                         .Replace("#UserSCRIPTNAME#", Path.GetFileNameWithoutExtension(writePath))
                           .Replace("#UserNameSpace#", EditorTools.ProjectConfig.NameSpace)
                           .Replace("#UserVERSION#", EditorTools.ProjectConfig.Version)
                           .Replace("#UserDescription#", EditorTools.ProjectConfig.Description)
                           .Replace("#UserUNITYVERSION#", Application.unityVersion)
                           .Replace("#UserDATE#", DateTime.Now.ToString("yyyy-MM-dd")).ToUnixLineEndings();
                if (func != null)
                    source = func.Invoke(source);
                File.WriteAllText(writePath, source, System.Text.Encoding.UTF8);
            }


            private const string head = "/*********************************************************************************\n" +
            " *Author:         #User#\n" +
            " *Version:        #UserVERSION#\n" +
            " *UnityVersion:   #UserUNITYVERSION#\n" +
            " *Date:           #UserDATE#\n" +
            " *Description:    #UserDescription#\n" +
            " *History:        #UserDATE#--\n" +
            "*********************************************************************************/\n";

            private const string vmDesignScriptOrigin = head +
            "namespace #UserNameSpace#\n" +
            "{\n" +
            "\tpublic partial class #UserSCRIPTNAME# : IFramework.UI.UIViewModel<#ModelType#>,IFramework.Modules.Message.IMessageListener\n" +
            "\t{\n" +
            "#FieldString#\n" +
            "\t\tprotected override void SyncModelValue()\n" +
            "\t\t{\n" +
            "#SyncModelValue#\n" +
            "\t\t}\n" +
            "\n" +
            "\t}\n" +
            "}";
            private const string vmScriptOrigin = head +
            "namespace #UserNameSpace#\n" +
            "{\n" +
            "\tpublic partial class #UserSCRIPTNAME#\n" +
            "\t{\n" +

            "\t\tprotected override void Initialize()\n" +
            "\t\t{\n" +
            "\n" +
            "\t\t}\n" +
            "\n" +
            "\t\tprotected override void OnDispose()\n" +
            "\t\t{\n" +
            "\n" +
            "\t\t}\n" +
            "\n" +
            "\t\tvoid IFramework.Modules.Message.IMessageListener.Listen(IFramework.Modules.Message.IMessage message)\n" +
            "\t\t{\n" +
            "\n" +
            "\t\t}\n" +
            "\n" +

            "\t\tprotected override void SubscribeMessage()\n" +
            "\t\t{\n" +
            "\t\t\tmessage.Subscribe<#viewType#>(this);" +
            "\n" +
            "\t\t}\n" +
            "\n" +
           "\t\tprotected override void UnSubscribeMessage()\n" +
            "\t\t{\n" +
            "\t\t\tmessage.UnSubscribe<#viewType#>(this);" +
            "\n" +
            "\t\t}\n" +
            "\n" +


            "\t}\n" +

            "}";
            private const string viewDesignScriptOrigin = head +
            "namespace #UserNameSpace#\n" +
            "{\n" +
            "\tpublic partial class #UserSCRIPTNAME# : IFramework.UI.UIView<#VMType#, #PanelType#> \n" +
            "\t{\n" +
            "#panelfield#\n" +
            "\t}\n" +
            "}";
            private const string viewScriptOrigin = head +
            "namespace #UserNameSpace#\n" +
            "{\n" +
            "\tpublic partial class #UserSCRIPTNAME#\n" +
            "\t{\n" +
            "\t\tprotected override void BindProperty()\n" +
            "\t\t{\n" +
            "\t\t\tbase.BindProperty();\n" +
            "\t\t\t//ToDo\n" +
            "\t\t}\n" +
            "\n" +
            "\t\tprotected override void OnClear()\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
            "\t\tprotected override void OnLoad()\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
            "\t\tprotected override void OnPop(IFramework.UI.UIEventArgs arg)\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
            "\t\tprotected override void OnPress(IFramework.UI.UIEventArgs arg)\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
            "\t\tprotected override void OnTop(IFramework.UI.UIEventArgs arg)\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
            "\t}\n" +
            "}";
            private const string mapScriptOrigin = head +
           "namespace #UserNameSpace#\n" +
           "{\n" +
           "\tpublic class #UserSCRIPTNAME# \n" +
           "\t{\n" +
           "\t\tpublic static System.Collections.Generic.Dictionary<System.Type, System.Tuple<System.Type, System.Type, System.Type>> map = \n" +
           "\t\tnew System.Collections.Generic.Dictionary<System.Type, System.Tuple<System.Type, System.Type, System.Type>>()\n" +
           "\t\t{\n" +
           "\n" +
           "//ToDo\n" +
           "\t\t}\n;" +
           "\t }\n" +
           "}\n";

        }
    }

    partial class UIMoudleWindow
    {
        private Dictionary<string, UIMoudleWindowTab> _tabs;
        private string[] _names;
        private int viewIndex;
        private const string key = "UIMoudleWindow";


        private void OnEnable()
        {
            _tabs = typeof(UIMoudleWindowTab).GetSubTypesInAssemblys()
                                     .ToList()
                                     .ConvertAll((type) => { return Activator.CreateInstance(type) as UIMoudleWindowTab; })
                                     .ToDictionary((tab) => { return tab.name; });

            _names = _tabs.Keys.ToArray();
            foreach (var item in _tabs.Values)
            {
                item.OnEnable();
            }
            viewIndex = EditorTools.Prefs.GetInt<UIMoudleWindow>(key, viewIndex);
        }
        private void OnDisable()
        {
            foreach (var item in _tabs.Values)
            {
                item.OnDisable();
            }
            EditorTools.Prefs.SetInt<UIMoudleWindow>(key, viewIndex);
        }
        private void OnGUI()
        {
            viewIndex = GUILayout.Toolbar(viewIndex, _names);
            _tabs[_names[viewIndex]].OnGUI();

            //  this.Repaint();
        }
    }
}
