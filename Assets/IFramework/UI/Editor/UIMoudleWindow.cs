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
        [CustomEditor(typeof(NamesSto))]
        class NamesStoEditor:Editor
        {
            public override void OnInspectorGUI()
            {
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
        public class UIModuleStackWatch: UIMoudleWindowTab
        {
            private const string key = "UIModuleStackWatch";
            public override string name { get { return "Stack_Watch"; } }
            private UIModule module;
            [SerializeField] private EnvironmentType type= EnvironmentType.Ev1;
            [SerializeField] private string moduleName = "default";
            private SearchField search;
            public override void OnGUI()
            {
                type = (EnvironmentType)EditorGUILayout.EnumPopup("EnvType", type);
                if (!EditorApplication.isPlaying) return;
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("ModuleName");
                    GUILayout.Label("");
                    search.OnGUI(GUILayoutUtility.GetLastRect());
                    GUILayout.EndHorizontal();
                }

                if (module==null)
                {
                    Search_onEndEdit(moduleName);
                }
                if (module == null) return;
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("GoBack"))
                    {
                        module.GoBack();
                    }
                    if (GUILayout.Button("GoForward"))
                    {
                        module.GoForWard();
                    }
                    if (GUILayout.Button("ClearMemory"))
                    {
                        module.ClearMemory();
                    }
                    GUILayout.EndHorizontal();
                }
                var stack = module.GetStack();
                var memory = module.GetMemory();
                int count = stack.Count;
                GUILayout.Label($"StackCount: {count}\t\tMemoryCount: {memory.Count}");
                GUILayout.Label("Name\t\tLayer\t\tLayerOrder");
                scroll = GUILayout.BeginScrollView(scroll);
                {
                    stack.Reverse().ForEach((p) =>
                    {
                        count--;
                        string format = $"S-{p.name}\t\t{p.layer}\t\t{p.layerOrder}";
                        if (count==0)
                            format = $"C-{p.name}\t\t{p.layer}\t\t{p.layerOrder}";
                        GUILayout.Label(format);
                        var rect = GUILayoutUtility.GetLastRect();
                        Event e = Event.current;
                        if (count == 0 && e.type == EventType.Repaint)
                        {
                            GUIStyles.SelectionRect.Draw(rect, false, false, false, false);
                        }
                        if (rect.Contains(e.mousePosition) && e.clickCount == 2)
                        {
                            Selection.activeGameObject = p.gameObject;
                        }

                    });
                    memory.ForEach((p) =>
                    {
                        string format = $"M-{p.name}\t\t{p.layer}\t\t{p.layerOrder}";
                        GUILayout.Label(format);
                        var rect = GUILayoutUtility.GetLastRect();
                        Event e = Event.current;

                        if (rect.Contains(e.mousePosition) && e.clickCount == 2)
                        {
                            Selection.activeGameObject = p.gameObject;
                        }
                    });
                    GUILayout.EndScrollView();
                }
               
            }
            Vector2 scroll;
            public override void OnDisable()
            {
                EditorTools.Prefs.SetObject<UIModuleStackWatch, UIModuleStackWatch>(key, this);
            }
            public override void OnEnable()
            {
                var last = EditorTools.Prefs.GetObject<UIModuleStackWatch, UIModuleStackWatch>(key);
                if (last!=null)
                {
                    this.type = last.type;
                    this.moduleName = last.moduleName;
                }
                search = new SearchField(moduleName, null, 0);
                search.onEndEdit += Search_onEndEdit;
            }

            private void Search_onEndEdit(string obj)
            {
                moduleName = obj;
                var env = Framework.GetEnv(type);
                if (env!=null && env.inited)
                {
                    module = env.modules.GetModule<UIModule>(moduleName);
                }
            }
        }
        [Serializable]
        public class MVVM_GenCodeView : UIMoudleWindowTab
        {
            private const string key = "MVVM_GenCodeView";
            public override string name { get { return "MVVN_GenCode_CS"; } }
            [SerializeField] private string UIMapDir="Assets/Project";
            [SerializeField]
            private string PanelGenDir
            {
                get
                {
                    if (panel == null) return "";
                    string path = UIMapDir.CombinePath(panel.name);
                    return path;
                }
            }
            private string ns
            {
                get
                {
                    if (panel != null)
                    {
                        return panel.GetType().Namespace;
                    }
                    return "";
                }
            }
            [SerializeField] private string UIMapName = "UIMap_MVVM";
            //[SerializeField] private List<string> panelTypes;
            [SerializeField] private UIPanel panel;

            //[SerializeField] private string panelType;
            [SerializeField] private List<string> modelTypes;
            [SerializeField] private string modelType;
            private NamesSto sto;
            string UIMap_CSName { get { return UIMapName.Append(".cs"); } }

            public override void OnEnable()
            {   
                var last = EditorTools.Prefs.GetObject<MVVM_GenCodeView, MVVM_GenCodeView>(key);
                if (last != null)
                {
                    this.panel = last.panel;
                    this.UIMapDir = last.UIMapDir;
                    this.UIMapName = last.UIMapName;
                    this.modelTypes = last.modelTypes;
                    this.modelType = last.modelType;
                }
                modelTypes = typeof(IModel).GetSubTypesInAssemblys()
                  .Where(type => !type.IsAbstract && type.IsClass)
                  .Select((type) =>
                  {
                      return type.FullName;
                  }).ToList();
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
                        GUIStyles.BoldLabel.Draw(position, content, id, false);
                        break;
                }
                return false;
            }
            public override void OnGUI()
            {
                if (EditorApplication.isCompiling)
                {
                    GUILayout.Label("Editor is Compiling");
                    GUILayout.Label("please wait");
                    return;
                }
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Work Directory For Module", GUIStyles.toolbar);
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
                UIMapName = EditorGUILayout.TextField("UI Map Name", UIMapName);
                EditorGUI.BeginChangeCheck();
                panel = EditorGUILayout.ObjectField("UIPanel", panel, typeof(UIPanel), true) as UIPanel;
                EditorGUILayout.LabelField("UIPanelGenPath", PanelGenDir);
                if (EditorGUI.EndChangeCheck())
                {
                    if (panel != null && !Directory.Exists(PanelGenDir))
                    {
                        Directory.CreateDirectory(PanelGenDir);
                        AssetDatabase.Refresh();
                    }
                }

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Create EventArgs"))
                    {
                        if (panel == null)
                        {
                            EditorWindow.focusedWindow.ShowNotification(new GUIContent("Select panel first"));
                        }
                        else
                        {
                            CreateEventArgs(panel.GetType(), ns);
                        }
                    }
                    GUILayout.Space(20);
                    if (GUILayout.Button("Create Model"))
                    {
                        if (panel == null)
                        {
                            EditorWindow.focusedWindow.ShowNotification(new GUIContent("Select panel first"));
                        }
                        else
                        {
                            CreateModel(panel.GetType(), ns);
                        }
                    }
                    GUILayout.Space(20);

                    if (GUILayout.Button("Fresh Model Types"))
                    {
                        modelTypes = typeof(IModel).GetSubTypesInAssemblys()
                            .Where(type => !type.IsAbstract && type.IsClass)
                            .Select((type) =>
                            {
                                return type.FullName;
                            }).ToList();
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(10);
                if (hashID == 0) hashID = "MVVM_GenCodeView".GetHashCode();

                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Click To Select Model Type", GUIStyles.toolbar);
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
                if (GUILayout.Button("Gen"))
                {
                    if (string.IsNullOrEmpty(UIMapDir))
                    {
                        EditorWindow.focusedWindow.ShowNotification(new GUIContent("Set UI Map Gen Dir "));
                        return;
                    }
                    if (panel == null)
                    {
                        EditorWindow.focusedWindow.ShowNotification(new GUIContent("Select UI Panel"));
                        return;
                    }
                    if (string.IsNullOrEmpty(modelType))
                    {
                        EditorWindow.focusedWindow.ShowNotification(new GUIContent("Select UI Model Type "));
                        return;
                    }
                    string _modelType = modelType.Split('.').ToList().Last();
                    string paneltype = panel.GetType().Name;
                    string vmType = paneltype.Append("ViewModel");
                    string viewType = paneltype.Append("View");
                    if (panel != null && !Directory.Exists(PanelGenDir))
                    {
                        Directory.CreateDirectory(PanelGenDir);
                    }
                    WriteView(viewType, vmType, panel.GetType(), paneltype, ns);
                    WriteVM(vmType, viewType, ns);
                    WriteMap(UIMapDir.CombinePath(UIMap_CSName), panel.name, ns, modelType,panel.GetType());
                    AssetDatabase.Refresh();
                }
                GUILayout.Space(10);
                if (GUILayout.Button("Load Panel Names Map"))
                {
                    sto = CheckNameSto(UIMapDir);
                }
                if (sto!=null)
                {
                    scroll = GUILayout.BeginScrollView(scroll);
                    {
                        sto.map.ForEach((index,_nm) =>
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(_nm.panelName);
                                if (GUILayout.Button("",GUIStyles.minus))
                                {
                                    sto.RemoveMap(_nm.panelName);
                                    EditorTools.ScriptableObjectTool.Update(sto);
                                    Directory.Delete(sto.workspace.CombinePath(_nm.panelName), true);
                                    WriteMap(sto.workspace.CombinePath(UIMap_CSName), sto.ns);
                                }
                                GUILayout.EndHorizontal();
                            }

                        });

                        GUILayout.EndScrollView();
                    }
                }

            }
            Vector2 scroll;
            private NamesSto CheckNameSto(string work)
            {
                string path = work.CombinePath("Editor");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = path.CombinePath("NamesSto.asset");
                if (!File.Exists(path))
                {
                    return EditorTools.ScriptableObjectTool.Create<NamesSto>(path);
                }
                return EditorTools.ScriptableObjectTool.Load<NamesSto>(path);
            }
        


            private void CreateEventArgs(Type type, string ns)
            {
                WriteTxt(PanelGenDir.CombinePath($"{type.Name}Args.cs"), argsOrigin, null, ns);
                AssetDatabase.Refresh();
            }
            private void CreateModel(Type type, string ns)
            {
                WriteTxt(PanelGenDir.CombinePath($"{type.Name}Model.cs"), modelOrigin, null, ns);
                AssetDatabase.Refresh();
            }
            private void WriteVM(string vmType, string viewType, string ns)
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
                                             .Replace(".Design", "");
                               }
                           , ns);
                if (!File.Exists(path))
                {
                    WriteTxt(path, vmScriptOrigin, (str) => {
                        return str.Replace("#viewType#", viewType);
                    }, ns);
                }
            }

            private void WriteView(string viewType, string vmType, Type type, string panelType, string ns)
            {
                string designPath = PanelGenDir.CombinePath(viewType.Append(".Design.cs"));
                string path = PanelGenDir.CombinePath(viewType.Append(".cs"));

                WriteTxt(designPath, viewDesignScriptOrigin,
               (str) => {

                   return str.Replace("#VMType#", vmType)
                   .Replace("#PanelType#", panelType)
                   .Replace("#panelfield#", GetPanelField(type))
                   .Replace(".Design", "");
               }, ns
               );
                if (!File.Exists(path))
                {
                    WriteTxt(path, viewScriptOrigin, null, ns);
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
            private void WriteMap(string path,string ns)
            {
                string replace = "";
                string namerp = "";
                var sto = CheckNameSto(UIMapDir);
                sto.map.ForEach(_nm => {
                    namerp = namerp.Append("\t\tpublic const string " + _nm.panelName + " = " + "\"" + _nm.panelName + "\";\n");
                    replace = replace.Append("\t\t\t" + _nm.content + ",\n");
                });
                WriteTxt(path, mapScriptOrigin.Replace("//Names", namerp).Replace("//ToDo", replace), null, ns);
            }
            private void WriteMap(string path, string panelName, string ns,string modelType,Type panelType)
            {
                var sto = CheckNameSto(UIMapDir);
                string content= string.Format("{2} {0} ,System.Tuple.Create(typeof({1}),typeof({4}.{5}View),typeof({4}.{5}ViewModel)){3}", panelName, modelType, "{", "}",ns,panelType.Name);
                sto.AddMap(panelName, content);
                sto.ns = ns;
                sto.workspace = UIMapDir;
                EditorTools.ScriptableObjectTool.Update(sto);
                WriteMap(path,ns);
            }
            private static void WriteTxt(string writePath, string source, Func<string, string> func, string ns)
            {
                source = source.Replace("#User#", EditorTools.ProjectConfig.UserName)
                         .Replace("#UserSCRIPTNAME#", Path.GetFileNameWithoutExtension(writePath))
                           .Replace("#UserNameSpace#", ns)
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

            private const string argsOrigin = head +
            "namespace #UserNameSpace#\n" +
            "{\n" +
            "\tpublic class #UserSCRIPTNAME# : IFramework.IEventArgs\n" +
            "\t{\n" +
            "\t\t//write your args fields here\n" +
            "\t}\n" +
            "}";
            private const string modelOrigin = head +
            "namespace #UserNameSpace#\n" +
            "{\n" +
            "\tpublic class #UserSCRIPTNAME# : IFramework.IModel\n" +
            "\t{\n" +
            "\t\t//write your model fields here\n" +
            "\t}\n" +
            "}";
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

            "\t\tprotected override void OnShow()\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
             "\t\tprotected override void OnHide()\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
             "\t\tprotected override void OnPause()\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
             "\t\tprotected override void OnUnPause()\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
            "\t\tprotected override void OnClose()\n" +
            "\t\t{\n" +
            "\t\t}\n" +
            "\n" +
            "\t}\n" +
            "}";
            private const string mapScriptOrigin = head +
           "namespace #UserNameSpace#\n" +
           "{\n" +
            "\tpartial class #UserSCRIPTNAME#\n" +
            "\t{\n" +
            "//Names\n" +
            "\t}\n" +
           "\tpublic partial class #UserSCRIPTNAME# \n" +
           "\t{\n" +
           "\t\tpublic static System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Type, System.Type>> map = \n" +
           "\t\tnew System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Type, System.Type>>()\n" +
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
            EditorApplication.hierarchyChanged += () =>{ Repaint(); };

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
        }
    }
}
