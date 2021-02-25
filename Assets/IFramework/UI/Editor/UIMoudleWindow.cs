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
using IFramework.Modules.MVVM;
using IFramework.Serialization;

namespace IFramework.UI
{
    [EditorWindowCache("IFramework.UIModule")]
	public partial class UIMoudleWindow:EditorWindow,ILayoutGUI
	{
        private class Contents
        {
            public static GUIContent BGBG = new GUIContent("BBG", "BelowBackground");
            public static GUIContent BackGround = new GUIContent("BG", "BackGround");
            public static GUIContent AUP = new GUIContent("BA", "BelowAnimation");
            public static GUIContent Common = new GUIContent("Com", "Common");
            public static GUIContent AOP = new GUIContent("AA", "AboveAnimation");
            public static GUIContent Popup = new GUIContent("Pop", "Pop");
            public static GUIContent Guide = new GUIContent("Guide", "Guide");
            public static GUIContent Toast = new GUIContent("Toast", "Toast");
            public static GUIContent Top = new GUIContent("Top", "Top");
            public static GUIContent TopTop = new GUIContent("ATop", "AboveTop");
        }
        private class Styles
        {
            public static GUIStyle BoldLabel = EditorStyles.boldLabel;
            public static GUIStyle toolbarButton = EditorStyles.toolbarButton;
            public static GUIStyle toolbar = EditorStyles.toolbar;
            public static GUIStyle searchField = GUIStyles.Get("ToolbarSeachTextField");
            public static GUIStyle cancelBtn = GUIStyles.Get("ToolbarSeachCancelButton");
            public static GUIStyle Fold = GUIStyles.Get("ToolbarDropDown");
            public static GUIStyle FoldOut = EditorStyles.foldout;

            static Styles()
            {
                Fold.fixedHeight = BoldLabel.fixedHeight;
                BoldLabel.fontSize = 10;
            }
        }
        public class UIMoudleWindowTab : ILayoutGUI
        {
            public virtual string name { get; }
            public virtual void OnGUI() { }
            public virtual void OnEnable() { }
            public virtual void OnDisable() { }

        }

        [System.Serializable]
        public class MVVM_GenCodeView : UIMoudleWindowTab
        {
            private const string key= "MVVM_GenCodeView";
            public override string name { get { return "MVVN_GenCode_CS"; } }
            [SerializeField] private string UIMapDir;
            [SerializeField] private string PanelGenDir;
            [SerializeField] private string UIMapName = "UIMap_MVVM";
            [SerializeField] private List<string> panelTypes;
            [SerializeField] private string panelType;
            [SerializeField] private List<string> modelTypes;
            [SerializeField] private string modelType;

            string genpath;
            string uimapGenPath { get { return genpath.CombinePath("MapGen_MVVM.txt"); } }
            string viewGenPath { get { return genpath.CombinePath("View_MVVM.txt"); } }
            string VMGenPath { get { return genpath.CombinePath("VM_MVVM.txt"); } }
            string UIMap_CSName { get { return UIMapName.Append( ".cs"); } }

            public override void OnEnable()
            {
                var last = EditorTools.Prefs.GetObject<MVVM_GenCodeView, MVVM_GenCodeView>(key);
                if (last!=null)
                {
                    this.UIMapDir = last.UIMapDir;
                    this.PanelGenDir = last.PanelGenDir;
                    this.UIMapName = last.UIMapName;

                    this.panelTypes = last.panelTypes;
                    this.panelType = last.panelType;
                    this.modelTypes = last.modelTypes;
                    this.modelType = last.modelType;
                }
                genpath = EditorEnv.memoryPath.CombinePath("UI");
                if (!Directory.Exists(genpath))
                {
                    Directory.CreateDirectory(genpath);
                }
            }
            public override void OnDisable()
            {
                EditorTools.Prefs.SetObject<MVVM_GenCodeView, MVVM_GenCodeView>(key,this);
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
                this.Space(5)
                    .DrawHorizontal(() => {
                    this.Label("Check UIMap Script Name", Styles.toolbar);
                    this.TextField(ref UIMapName);
                });
                this.DrawHorizontal(() =>
                {
                    this.Label("Drag UIMap Gen Directory", Styles.toolbar);
                    this.Label(UIMapDir);
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

                })
                  .DrawHorizontal(() =>
                  {
                      this.FlexibleSpace()
                          .Button(() =>
                          {
                              if (string.IsNullOrEmpty(UIMapDir))
                              {
                                  EditorWindow.focusedWindow.ShowNotification(new GUIContent("Set UI Map Gen Dir "));
                                  return;
                              }
                           //   string uimapGenPath = genpath.CombinePath("MapGen_MVVM.txt");
                              CreateUIMapGen(uimapGenPath);
                              WriteTxt(UIMapDir.CombinePath(UIMap_CSName), uimapGenPath,null);
                              AssetDatabase.Refresh();

                          }, "Copy UIMap From Source");

                  }).Space(30);
                if (hashID == 0) hashID = "MVVM_GenCodeView".GetHashCode();
                this.DrawHorizontal(() =>
                {
                    this.Label("Click Choose Panel Type", Styles.toolbar);
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
                })
                    .Space(10)
                    .DrawHorizontal(() =>
                    {
                        this.Label("Click Choose Model Type", Styles.toolbar);
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
                    })
                    .Space(10)
                    .DrawHorizontal(() =>
                    {
                        this.Label("Drag Panel Gen Directory", Styles.toolbar);
                        this.Label(PanelGenDir);
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

                    })
                    .Space(10)
                    .DrawHorizontal(() =>
                    {
                        this.Button(() =>
                        {
                            panelTypes = typeof(UIPanel).GetSubTypesInAssemblys().Select((type) =>
                            {
                                return type.FullName;
                            }).ToList();
                            modelTypes = typeof(IDataModel).GetSubTypesInAssemblys().Select((type) =>
                            {
                                return type.FullName;
                            }).ToList();
                        }, "Fresh Panel && Model Types")
                        .Space(20)
                        .Button(() =>
                        {
                            if (string.IsNullOrEmpty(UIMapDir))
                            {
                                EditorWindow.focusedWindow.ShowNotification(new GUIContent("Set UI Map Gen Dir "));
                                return;
                            }
                            if (!File.Exists(UIMapDir.CombinePath(UIMap_CSName)))
                            {
                                EditorWindow.focusedWindow.ShowNotification(new GUIContent("Copy UI Map"));
                                return;
                            }
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
                            string _modelType= modelType.Split('.').ToList().Last();
                            string paneltype = panelType.Split('.').ToList().Last();
                            string vmType = paneltype.Append("ViewModel");
                            string viewType = paneltype.Append("View");

                            //string viewPath = genpath.CombinePath("View_MVVM.txt");
                            //string VMPath = genpath.CombinePath("VM_MVVM.txt");
                                              

                            CreateViewGen(viewGenPath);
                            CreateVMGen(VMGenPath);

                            //v
                            WriteTxt(PanelGenDir.CombinePath(viewType.Append(".cs")), viewGenPath,
                                (str)=>{


                                    return str.Replace("#VMType#", vmType).Replace("#PanelType#", paneltype);
                                }
                                );
                            //vm
                            WriteTxt(PanelGenDir.CombinePath(vmType.Append(".cs")), VMGenPath,
                                    (str) => {
                                        string fieldStr = " ";
                                        string syncStr = " ";
                                        Type t = AppDomain.CurrentDomain.GetAssemblies()
                                                    .SelectMany((a)=> { return a.GetTypes(); })
                                                    .ToList().Find((type)=> { return type.FullName == modelType; });
                                        var fs= t.GetFields();
                                        for (int i = 0; i < fs.Length; i++)
                                        {
                                            var info = fs[i];
                                            fieldStr = WriteField(fieldStr, info.FieldType, info.Name);
                                            syncStr = WriteSyncStr(syncStr, info.Name);
                                        }
                                        
                                        return str.Replace("#ModelType#", _modelType)
                                                  .Replace("#FieldString#", fieldStr)
                                                  .Replace("#SyncModelValue#", syncStr);
                                    }
                                );

                               WriteMap(UIMapDir.CombinePath(UIMap_CSName), UIMapDir.CombinePath(UIMap_CSName));
                            AssetDatabase.Refresh();
                        }, "Gen");
                    });

            }
            private void WriteMap(string writePath, string sourcePath)
            {
                var txt = File.ReadAllText(sourcePath);
                string flag = "//ToDo";

                string tmp = string.Format("typeof({0}),Tuple.Create(typeof({1}),typeof({0}View),typeof({0}ViewModel))", panelType,modelType);
                tmp = tmp.Append("},\n").AppendHead("\t\t\t{").Append(flag);
                txt = txt.Replace(flag, tmp);
                File.WriteAllText(writePath, txt, System.Text.Encoding.UTF8);
            }

            private string WriteField(string result ,Type ft,string fn)
            {
                return result.Append(string.Format("\t\tprivate {0} _{1};\n", ft.Name, fn))
                                .Append(string.Format("\t\tpublic {0} {1}\n", ft.Name, fn))
                                .Append(string.Format("\t\t{0}\n","{"))
                                //.Append(string.Format("\t\t\tget {0} return GetProperty(ref _{1}, this.GetPropertyName(() => _{1})); {2}\n", "{", fn, "}"))
                                .Append(string.Format("\t\t\tget {0} return GetProperty(ref _{1}); {2}\n", "{", fn, "}"))

                                .Append(string.Format("\t\t\tprivate set"))
                                .Append(string.Format("\t\t\t{0}\n","{"))
                                .Append(string.Format("\t\t\t\tTmodel.{0} = value;\n", fn))
                                //.Append(string.Format("\t\t\t\tSetProperty(ref _{0}, value, this.GetPropertyName(() => _{0}));\n", fn))
                                .Append(string.Format("\t\t\t\tSetProperty(ref _{0}, value);\n", fn))

                                .Append(string.Format("\t\t\t{0}\n","}"))
                                .Append(string.Format("\t\t{0}\n\n","}"));
            }
            private string WriteSyncStr(string result, string fn)
            {
                return result.Append(string.Format("\t\t\tthis.{0} = Tmodel.{0};\n", fn));

            }

            private void CreateVMGen(string genSourcePath)
            {
                if (File.Exists(genSourcePath)) return;
                using (FileStream fs = new FileStream(genSourcePath, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        fs.Lock(0, fs.Length);
                        sw.WriteLine("/*********************************************************************************");
                        sw.WriteLine(" *Author:         #User#");
                        sw.WriteLine(" *Version:        #UserVERSION#");
                        sw.WriteLine(" *UnityVersion:   #UserUNITYVERSION#");
                        sw.WriteLine(" *Date:           #UserDATE#");
                        sw.WriteLine(" *Description:    #UserDescription#");
                        sw.WriteLine(" *History:        #UserDATE#--");
                        sw.WriteLine("*********************************************************************************/");
                        sw.WriteLine("using System;");
                        sw.WriteLine("using System.Collections;");
                        sw.WriteLine("using System.Collections.Generic;");
                        sw.WriteLine("using IFramework;");
                        sw.WriteLine("using IFramework.UI;");

                        sw.WriteLine("");
                        sw.WriteLine("namespace #UserNameSpace#");
                        sw.WriteLine("{");
                        sw.WriteLine("\tpublic class #UserSCRIPTNAME# : UIViewModel<#ModelType#>");
                        sw.WriteLine("\t{");

                        sw.WriteLine("#FieldString#");
                        sw.WriteLine("\t\tprotected override void SyncModelValue()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("#SyncModelValue#");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine("");

                      

                        sw.WriteLine("\t}");
                        sw.WriteLine("}");
                        fs.Unlock(0, fs.Length);
                        sw.Flush();
                        fs.Flush();
                    }

                }
                AssetDatabase.Refresh();
            }

            private void CreateViewGen(string genSourcePath)
            {
                if (File.Exists(genSourcePath)) return;
                using (FileStream fs = new FileStream(genSourcePath, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        fs.Lock(0, fs.Length);
                        sw.WriteLine("/*********************************************************************************");
                        sw.WriteLine(" *Author:         #User#");
                        sw.WriteLine(" *Version:        #UserVERSION#");
                        sw.WriteLine(" *UnityVersion:   #UserUNITYVERSION#");
                        sw.WriteLine(" *Date:           #UserDATE#");
                        sw.WriteLine(" *Description:    #UserDescription#");
                        sw.WriteLine(" *History:        #UserDATE#--");
                        sw.WriteLine("*********************************************************************************/");
                        sw.WriteLine("using System;");
                        sw.WriteLine("using System.Collections;");
                        sw.WriteLine("using System.Collections.Generic;");
                        sw.WriteLine("using IFramework;");
                        sw.WriteLine("using IFramework.UI;");

                       
                        sw.WriteLine("");
                        sw.WriteLine("namespace #UserNameSpace#");
                        sw.WriteLine("{");
                        sw.WriteLine("\tpublic class #UserSCRIPTNAME# : UIView<#VMType#, #PanelType#>");
                        sw.WriteLine("\t{");
                        sw.WriteLine("\t\tprotected override void BindProperty()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tbase.BindProperty();");
                        sw.WriteLine("\t\t\t//ToDo");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine("");

                        sw.WriteLine("\t\tprotected override void OnClear()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine("");

                        sw.WriteLine("\t\tprotected override void OnLoad()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine("");

                        sw.WriteLine("\t\tprotected override void OnPop(UIEventArgs arg)");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine("");

                        sw.WriteLine("\t\tprotected override void OnPress(UIEventArgs arg)");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine("");

                        sw.WriteLine("\t\tprotected override void OnTop(UIEventArgs arg)");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine("");
                        sw.WriteLine("\t}");
                        sw.WriteLine("}");
                        fs.Unlock(0, fs.Length);
                        sw.Flush();
                        fs.Flush();
                    }

                }
                AssetDatabase.Refresh();
            }

            private static void WriteTxt(string writePath, string sourcePath,Func<string ,string> func)
            {
                var txt = File.ReadAllText(sourcePath);
                txt = txt.Replace("#User#", ProjectConfig.UserName)
                         .Replace("#UserSCRIPTNAME#", Path.GetFileNameWithoutExtension(writePath))
                           .Replace("#UserNameSpace#", ProjectConfig.NameSpace)
                           .Replace("#UserVERSION#", ProjectConfig.Version)
                           .Replace("#UserDescription#", ProjectConfig.Description)
                           .Replace("#UserUNITYVERSION#", Application.unityVersion)
                           .Replace("#UserDATE#", DateTime.Now.ToString("yyyy-MM-dd"));
                if (func!=null)
                    txt = func.Invoke(txt);
                File.WriteAllText(writePath, txt, System.Text.Encoding.UTF8);
            }

            private void CreateUIMapGen(string genSourcePath)
            {
                if (File.Exists(genSourcePath)) return;
                using (FileStream fs = new FileStream(genSourcePath, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        fs.Lock(0, fs.Length);
                        sw.WriteLine("/*********************************************************************************");
                        sw.WriteLine(" *Author:         #User#");
                        sw.WriteLine(" *Version:        #UserVERSION#");
                        sw.WriteLine(" *UnityVersion:   #UserUNITYVERSION#");
                        sw.WriteLine(" *Date:           #UserDATE#");
                        sw.WriteLine(" *Description:    #UserDescription#");
                        sw.WriteLine(" *History:        #UserDATE#--");
                        sw.WriteLine("*********************************************************************************/");
                        sw.WriteLine("using System;");
                        sw.WriteLine("using System.Collections;");
                        sw.WriteLine("using System.Collections.Generic;");
                        sw.WriteLine("using IFramework;");
                        sw.WriteLine("using IFramework.UI;");


                        sw.WriteLine("");
                        sw.WriteLine("namespace #UserNameSpace#");
                        sw.WriteLine("{");
                        sw.WriteLine("\tpublic class #UserSCRIPTNAME# ");
                        sw.WriteLine("\t{");
                        sw.WriteLine("\t\tpublic static Dictionary<Type, Tuple<Type, Type, Type>> map =");
                        sw.WriteLine("\t\tnew Dictionary<Type, Tuple<Type, Type, Type>>()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("//ToDo");
                        sw.WriteLine("\t\t};");
                        sw.WriteLine("\t}");
                        sw.WriteLine("}");
                        fs.Unlock(0, fs.Length);
                        sw.Flush();
                        fs.Flush();
                    }

                }
                AssetDatabase.Refresh();
            }
        }
    }

    partial class UIMoudleWindow 
    {
        private Dictionary<string,UIMoudleWindowTab> _tabs;
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
            this.Toolbar(ref viewIndex, _names)
                .Pan(() =>
                {
                    _tabs[_names[viewIndex]].OnGUI();
                });
          
          //  this.Repaint();
        }
    }
}
