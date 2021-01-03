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
using System.IO;

namespace IFramework.UI
{
    public partial class UIMoudleWindow
    {
        [System.Serializable]
        public class MVVM_GenCodeView_Lua : UIMoudleWindowTab
        {
            private string hotFixScriptPath { get { return Application.dataPath.CombinePath("Project/Lua").ToAssetsPath(); } }
            [SerializeField] private string UIMapDir;
            [SerializeField] private string PanelGenDir;
            [SerializeField] private string panelName;
            [SerializeField] private string UIMapName = "UIMap_MVVM";
            string uimapPath { get { return UIMapDir.CombinePath(UIMapName).Append(".lua"); } }

            string ViewName
            {
                get
                {
                    return panelName.Append("View");
                }
            }
            string VMName
            {
                get
                {
                    return panelName.Append("ViewModel");
                }
            }

            public override string name { get { return "MVVN_GenCode_Lua"; } }

            public override void OnGUI()
            {

                this.Space(5)
                    .DrawHorizontal(() =>
                    {
                        this.Label("Check UIMap Script Name", Styles.toolbar);
                        this.TextField(ref UIMapName);
                    });
                this.DrawHorizontal(() =>
                {
                    this.Label("UIMap Gen Directory", Styles.toolbar);
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
                                if (!UIMapDir.Contains(hotFixScriptPath))
                                    UIMapDir = string.Empty;
                            }

                        }
                    }

                })
                .Space(30);

                this.ETextField("Panel Name", ref panelName);

                this.DrawHorizontal(() =>
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
                            if (!string.IsNullOrEmpty(UIMapDir))
                            {
                                string path = drag.paths[0];
                                if (path.Contains("Assets"))
                                {
                                    if (path.IsDirectory())
                                        PanelGenDir = path;
                                    else
                                        PanelGenDir = path.GetDirPath();
                                    if (!PanelGenDir.Contains(UIMapDir))
                                        PanelGenDir = string.Empty;
                                }
                            }
                            else
                            {
                                EditorWindow.focusedWindow.ShowNotification(new GUIContent("Choose UI Map Directory First"));
                            }
                          
                        }
                    }

                })
                 .Space(10)
                 .DrawHorizontal(() =>
                 {
                     this
                     .Button(() =>
                     {
                         if (File.Exists(uimapPath))
                         {
                             EditorWindow.focusedWindow.ShowNotification(new GUIContent("UI Map Have Exist "));
                             return;
                         }
                         CreateUIMap(uimapPath);
                         AssetDatabase.Refresh();

                     }, "Create UIMap")
                     .Space(20)
                     .Button(() =>
                     {
                         if (!File.Exists(uimapPath))
                         {
                             EditorWindow.focusedWindow.ShowNotification(new GUIContent("Create UI Map"));
                             return;
                         }
                         if (string.IsNullOrEmpty(panelName))
                         {
                             EditorWindow.focusedWindow.ShowNotification(new GUIContent("Set UI Panel Name "));
                             return;
                         }
                         if (string.IsNullOrEmpty(PanelGenDir))
                         {
                             EditorWindow.focusedWindow.ShowNotification(new GUIContent("Set UI Panel Gen Dir "));
                             return;
                         }

                     //                            string paneltype = panelType.Split('.').Last();


                     CreateView(PanelGenDir.CombinePath(ViewName).Append(".lua"));
                         CreateVM(PanelGenDir.CombinePath(VMName).Append(".lua"));
                         WriteMap(uimapPath, panelName);
                         AssetDatabase.Refresh();
                     }, "Gen");
                 });

            }
            private void WriteMap(string Path, string panelName)
            {
                var txt = File.ReadAllText(Path);
                string flag = "--ToDo";

                string sub = PanelGenDir.Replace(hotFixScriptPath, "").Replace("/", ".");
                if (sub[0] == '.')
                {
                    sub = sub.Remove(0, 1);
                }



                string tmp = string.Format("{0} Name = \"{2}\",ViewType =require(\"{3}\"), VMType=require(\"{4}\"){1}", "{", "}", panelName, sub.Append("." + ViewName), sub.Append("." + VMName));
                tmp = tmp.Append(",\n").AppendHead("\t").Append(flag);
                txt = txt.Replace(flag, tmp);
                File.WriteAllText(Path, txt, System.Text.Encoding.UTF8);
            }



            private void CreateVM(string path)
            {
                if (File.Exists(path)) return;
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        fs.Lock(0, fs.Length);
                        sw.WriteLine("--*********************************************************************************");
                        sw.WriteLine(" --Author:         " + ProjectConfig.NameSpace);
                        sw.WriteLine(" --Version:        " + ProjectConfig.Version);
                        sw.WriteLine(" --UnityVersion:   " + Application.unityVersion);
                        sw.WriteLine(" --Date:           " + DateTime.Now.ToString("yyyy-MM-dd"));
                        sw.WriteLine(" --Description:    " + ProjectConfig.Description);
                        sw.WriteLine(" --History:        " + DateTime.Now.ToString("yyyy-MM-dd") + "--");
                        sw.WriteLine("--*********************************************************************************/");

                        sw.WriteLine("--super Fields ");
                        sw.WriteLine("--super Function ");
                        sw.WriteLine("----self:Subscribe( key,func )");
                        sw.WriteLine("----self:UnSubscribe(key,func)");
                        sw.WriteLine("----self:Invoke( key )\n\n");


                        sw.WriteLine("");
                        sw.WriteLine("local " + VMName + "=Class(\"" + VMName + "\",require(\"UI.ViewModel\"))\n");

                        sw.WriteLine("--return " + VMName + "'s Fields By table");
                        sw.WriteLine("--Example return { myCount = 666 }");
                        sw.WriteLine("function " + VMName + ":GetFieldTable()");
                        sw.WriteLine("");
                        sw.WriteLine("end\n");

                        sw.WriteLine("function " + VMName + ":OnDispose()");
                        sw.WriteLine("");
                        sw.WriteLine("end\n");

                        sw.WriteLine("function " + VMName + ":OnInitialize()");
                        sw.WriteLine("");
                        sw.WriteLine("end\n");


                        sw.WriteLine("--View's  Event ");
                        sw.WriteLine("function " + VMName + ":ListenViewEvent( code,... )");
                        sw.WriteLine("");
                        sw.WriteLine("end\n");


                        sw.WriteLine("return " + VMName);




                        fs.Unlock(0, fs.Length);
                        sw.Flush();
                        fs.Flush();
                    }
                }
                AssetDatabase.Refresh();
            }

            private void CreateView(string genSourcePath)
            {
                if (File.Exists(genSourcePath)) return;
                using (FileStream fs = new FileStream(genSourcePath, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        fs.Lock(0, fs.Length);
                        sw.WriteLine("--*********************************************************************************");
                        sw.WriteLine(" --Author:         " + ProjectConfig.NameSpace);
                        sw.WriteLine(" --Version:        " + ProjectConfig.Version);
                        sw.WriteLine(" --UnityVersion:   " + Application.unityVersion);
                        sw.WriteLine(" --Date:           " + DateTime.Now.ToString("yyyy-MM-dd"));
                        sw.WriteLine(" --Description:    " + ProjectConfig.Description);
                        sw.WriteLine(" --History:        " + DateTime.Now.ToString("yyyy-MM-dd") + "--");
                        sw.WriteLine("--*********************************************************************************/");
                        sw.WriteLine("");

                        sw.WriteLine("--super Fields ");
                        sw.WriteLine("----self.message : publish Event");
                        sw.WriteLine("----self.context : ViewModel");
                        sw.WriteLine("----self.panel :  UIpanel From C#");
                        sw.WriteLine("--super Function ");
                        sw.WriteLine("----self:PublishViewEvent(code,...)\n\n");


                        sw.WriteLine("local " + ViewName + "=Class(\"" + ViewName + "\",require(\"UI.UIView\"))\n");


                        sw.WriteLine("--Bind ViewModel Fields");
                        sw.WriteLine("function " + ViewName + ":BindProperty()");
                        sw.WriteLine("");
                        sw.WriteLine("end\n");

                        sw.WriteLine("function " + ViewName + ":Dispose()");
                        sw.WriteLine("");
                        sw.WriteLine("end\n");

                        sw.WriteLine("function " + ViewName + ":OnLoad()");
                        sw.WriteLine("");
                        sw.WriteLine("end\n");

                        sw.WriteLine("function " + ViewName + ":OnTop( arg )");
                        sw.WriteLine("");
                        sw.WriteLine("end\n");

                        sw.WriteLine("function " + ViewName + ":OnPress( arg )");
                        sw.WriteLine("");
                        sw.WriteLine("end\n");

                        sw.WriteLine("function " + ViewName + ":OnPop( arg )");
                        sw.WriteLine("");
                        sw.WriteLine("end\n");

                        sw.WriteLine("function " + ViewName + ":OnClear()");
                        sw.WriteLine("");
                        sw.WriteLine("end\n");

                        sw.WriteLine("return " + ViewName);




                        fs.Unlock(0, fs.Length);
                        sw.Flush();
                        fs.Flush();
                    }

                }
                AssetDatabase.Refresh();
            }


            private void CreateUIMap(string path)
            {
                if (File.Exists(path)) return;
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        fs.Lock(0, fs.Length);
                        sw.WriteLine("--*********************************************************************************");
                        sw.WriteLine(" --Author:         " + ProjectConfig.NameSpace);
                        sw.WriteLine(" --Version:        " + ProjectConfig.Version);
                        sw.WriteLine(" --UnityVersion:   " + Application.unityVersion);
                        sw.WriteLine(" --Date:           " + DateTime.Now.ToString("yyyy-MM-dd"));
                        sw.WriteLine(" --Description:    " + ProjectConfig.Description);
                        sw.WriteLine(" --History:        " + DateTime.Now.ToString("yyyy-MM-dd") + "--");
                        sw.WriteLine("--*********************************************************************************/");

                        //sw.WriteLine("-- {PanelType =CS.???,type={ViewType=77,VMType=66},}");


                        sw.WriteLine("");
                        sw.WriteLine("local map=");
                        sw.WriteLine("{");

                        sw.WriteLine("--ToDo");
                        sw.WriteLine("}");
                        sw.WriteLine("return map");
                        fs.Unlock(0, fs.Length);
                        sw.Flush();
                        fs.Flush();
                    }

                }
            }



            public override void OnEnable()
            {
                var last = EditorTools.Prefs.GetObject<MVVM_GenCodeView_Lua, MVVM_GenCodeView_Lua>(key);
                if (last != null)
                {
                    this.UIMapDir = last.UIMapDir;
                    this.PanelGenDir = last.PanelGenDir;
                    this.UIMapName = last.UIMapName;
                    this.panelName = last.panelName;
                }
            }
            private const string key = "MVVM_GenCodeView";

            public override void OnDisable()
            {
                EditorTools.Prefs.SetObject<MVVM_GenCodeView_Lua, MVVM_GenCodeView_Lua>(key, this);
            }
        }
    }
}
