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

        public class RunTimeView : UIMoudleWindowTab
        {
            private const float typeWith = 200;
            private const float paneltypeWith = 100;
            private SearchField searcher;
            private SearchField searcher2;

            private UIModule moudle;
            [SerializeField] private Vector2 stackSroll, cacheScroll;
            [SerializeField] private string searchText_module = string.Empty;
            [SerializeField] private string searchTxt_panel = string.Empty;

            [SerializeField] private bool ShowBGBG = true;
            [SerializeField] private bool ShowBackGround = true;
            [SerializeField] private bool ShowAnimationUnderPage = true;
            [SerializeField] private bool ShowCommon = true;
            [SerializeField] private bool ShowAnimationOnPage = true;
            [SerializeField] private bool ShowPop = true;
            [SerializeField] private bool ShowGuide = true;
            [SerializeField] private bool ShowToast = true;
            [SerializeField] private bool ShowTop = true;
            [SerializeField] private bool ShowTopTop = true;

            [SerializeField] private bool IsStackOn = true;
            [SerializeField] private bool IsCacheOn = true;
            public EnvironmentType envType= EnvironmentType.Ev1;
            public override string name { get { return "Runtime"; } }

            public override void OnEnable()
            {
                searcher = new SearchField("", null, 0);
                searcher2 = new SearchField(searchTxt_panel, null, 0);
                searcher.onEndEdit += (str) =>
                {
                    searchText_module = str;
                    if (!EditorApplication.isPlaying) return;
                    FrameworkEnvironment env1 = Framework.GetEnv(envType);
                    try
                    {
                        if (env1 != null)
                        {
                            moudle = env1.modules.FindModule<UIModule>(str);
                            if (moudle == null)
                            {
                                EditorWindow.focusedWindow.ShowNotification(new GUIContent("Not Find,Moudle Must Bind Framework First"));
                            }
                        }
                        else
                        {
                            EditorWindow.focusedWindow.ShowNotification(new GUIContent("This Env is Null"));
                        }
                    }
                    catch (Exception)
                    {
                        EditorWindow.focusedWindow.ShowNotification(new GUIContent("Unknow Err"));
                    }
                   

                };
                searcher2.onEndEdit += (str) =>
                {
                    searchTxt_panel = str;
                };
            }
            public override void OnGUI()
            {
                this.BeginHorizontal()
                          .Label("Search UIMoudle ")
                          .FlexibleSpace()
                          .Pan(() => {
                              envType = (EnvironmentType)EditorGUILayout.EnumPopup(envType);
                          })
                          .Label("", GUILayout.MaxWidth(300))
                          .Pan(() =>
                          {

                              searcher.OnGUI(GUILayoutUtility.GetLastRect());
                          })
                      .EndHorizontal();
                this.BeginHorizontal(Styles.toolbar)
                        .Toggle(ref ShowBGBG, Contents.BGBG, Styles.toolbarButton)
                        .Toggle(ref ShowBackGround, Contents.BackGround, Styles.toolbarButton)
                        .Toggle(ref ShowAnimationUnderPage, Contents.AUP, Styles.toolbarButton)
                        .Toggle(ref ShowCommon, Contents.Common, Styles.toolbarButton)
                        .Toggle(ref ShowAnimationOnPage, Contents.AOP, Styles.toolbarButton)
                        .Toggle(ref ShowPop, Contents.Popup, Styles.toolbarButton)
                        .Toggle(ref ShowGuide, Contents.Guide, Styles.toolbarButton)
                        .Toggle(ref ShowToast, Contents.Toast, Styles.toolbarButton)
                        .Toggle(ref ShowTop, Contents.Top, Styles.toolbarButton)
                        .Toggle(ref ShowTopTop, Contents.TopTop, Styles.toolbarButton)
                    .EndHorizontal()
                    .BeginHorizontal(Styles.toolbar)
                        .Label("Search Panel By Name")
                        .FlexibleSpace()
                        .Label("", GUILayout.MaxWidth(300))
                        .Pan(() => {
                            searcher2.OnGUI(GUILayoutUtility.GetLastRect());
                        })
                    .EndHorizontal()
                    .BeginHorizontal(Styles.toolbar)
                        .Label("Name")
                        .Label("Type", GUILayout.MaxWidth(typeWith))
                        .Label("PanelType", GUILayout.Width(paneltypeWith))
                    .EndHorizontal();


                if (moudle == null || moudle.disposed || !EditorApplication.isPlaying) return;
                this.BeginHorizontal(Styles.toolbar)
                    .Label("UI Load Order", Styles.BoldLabel)
                .EndHorizontal()
                .Space(2)
                .BeginVertical()
                    .BeginHorizontal(Styles.Fold, GUILayout.Height(20))
                        .Space(10)
                        .Foldout(ref IsStackOn, string.Format("Stack  Count:  {0}", moudle.stackCount), true, Styles.FoldOut)
                        .FlexibleSpace()
                    .EndHorizontal()
                    .Pan(() =>
                    {
                        if (moudle.stackCount <= 0) return;
                        var o = moudle.stack.ToList();
                        //GUI.enabled = false;
                        if (IsStackOn)
                        {
                            this.DrawScrollView(() =>
                            {
                                for (int i = moudle.stack.Count - 1; i >= 0; i--)
                                {
                                    GUI.enabled = i == 0;
                                    bool canshow = FitCanShow(o[i]);
                                    if (!canshow) continue;
                                    Rect rect;
                                    this.BeginHorizontal()
                                                .Space(10)
                                                .EBeginHorizontal(out rect, EditorStyles.toolbar)
                                                    .Space(10)
                                                    .Label(o[i].name)
                                                    .Label(o[i].GetType().ToString(), GUILayout.MaxWidth(typeWith))
                                                    .Label(o[i].layer.ToString(), GUILayout.Width(paneltypeWith))
                                                .EEndHorizontal()
                                                .Pan(() =>
                                                {
                                                    if (Event.current.clickCount == 2 && rect.Contains(Event.current.mousePosition))
                                                        Selection.activeGameObject = o[i].gameObject;
                                                })
                                            .EndHorizontal();
                                }
                                GUI.enabled = true;
                            }, ref stackSroll);
                        }
                    })

                    .BeginHorizontal(Styles.Fold, GUILayout.Height(20))
                        .Space(10)
                        .Foldout(ref IsCacheOn, string.Format("Cache  Count:  {0}", moudle.memoryCount), true, Styles.FoldOut)
                        .FlexibleSpace()
                    .EndHorizontal()
                    .Pan(() =>
                    {
                        if (moudle.memory.Count <= 0) return;
                        var o = moudle.memory.ToList();
                        if (IsCacheOn)
                        {
                            this.DrawScrollView(() =>
                            {
                                GUI.enabled = false;
                                for (int i = 0; i < moudle.memory.Count; i++)
                                {
                                    bool canshow = FitCanShow(o[i]);
                                    if (!canshow) continue;
                                    Rect rect;
                                    this.BeginHorizontal()
                                                .Space(10)
                                                .EBeginHorizontal(out rect, EditorStyles.toolbar)
                                                    .Space(10)
                                                    .Label(o[i].name)
                                                    .Label(o[i].GetType().ToString(), GUILayout.MaxWidth(typeWith))
                                                    .Label(o[i].layer.ToString(), GUILayout.Width(paneltypeWith))
                                                .EEndHorizontal()
                                                .Pan(() =>
                                                {
                                                    if (Event.current.clickCount == 2 && rect.Contains(Event.current.mousePosition))
                                                        Selection.activeGameObject = o[i].gameObject;
                                                })
                                            .EndHorizontal();

                                }
                                GUI.enabled = true;

                            }, ref cacheScroll);
                        }

                    })
                .EndVertical()
                .FlexibleSpace()
                .Space(20);
                TestButton();
            }
            private bool FitCanShow(UIPanel panel)
            {
                bool canshow = false;
                switch (panel.layer)
                {
                    case UILayer.BelowBackground: canshow = ShowBGBG; break;
                    case UILayer.Background: canshow = ShowBackGround; break;
                    case UILayer.BelowAnimation: canshow = ShowAnimationUnderPage; break;
                    case UILayer.Common: canshow = ShowCommon; break;
                    case UILayer.AboveAnimation: canshow = ShowAnimationOnPage; break;
                    case UILayer.Pop: canshow = ShowPop; break;
                    case UILayer.Guide: canshow = ShowGuide; break;
                    case UILayer.Toast: canshow = ShowToast; break;
                    case UILayer.Top: canshow = ShowTop; break;
                    case UILayer.AboveTop: canshow = ShowTopTop; break;
                }
                canshow &= panel.name.ToLower().Contains(searchTxt_panel.ToLower());
                return canshow;
            }
            private void TestButton()
            {
                if (moudle.loaderCount <= 0)
                    EditorGUILayout.HelpBox("Must Have Loader ", UnityEditor.MessageType.Error);
                EditorGUILayout.LabelField("LoaderCount: " + moudle.loaderCount);
                using (new EditorGUI.DisabledScope(!EditorApplication.isPlaying))
                {
                    GUILayout.BeginHorizontal();
                    using (new EditorGUI.DisabledScope(moudle.stackCount <= 0))
                    {
                        if (GUILayout.Button("GoBack", GUILayout.Height(30)))
                        {
                            moudle.GoBack();
                        }
                    }
                    using (new EditorGUI.DisabledScope(moudle.memoryCount <= 0))
                    {
                        if (GUILayout.Button("GoForWard", GUILayout.Height(30)))
                        {
                            moudle.GoForWard();
                        }
                    }
                    GUILayout.EndHorizontal();
                    using (new EditorGUI.DisabledScope(moudle.memoryCount <= 0))
                    {
                        if (GUILayout.Button("ClearCache", GUILayout.Height(30)))
                        {
                            moudle.ClearCache();
                        }
                    }
                }
            }
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
