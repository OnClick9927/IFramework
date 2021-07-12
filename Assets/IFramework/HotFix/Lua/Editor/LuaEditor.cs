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
using System.Collections.Generic;
using UnityEditor.Experimental.AssetImporters;
using IFramework.UI;

namespace IFramework.Hotfix.Lua
{
    static class LuaEditorTools
    {
        public class FilePostProcessor : AssetPostprocessor
        {
            private static Queue<string> _changed = new Queue<string>();
            private static Action<string> _reload;
            static string frameworkpath
            {
                get
                {
                    return EditorEnv.frameworkPath.CombinePath("Hotfix/Lua/Resources");
                }
            }
            const string projectpath = "Assets/Project/Lua";

            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                if (!EditorApplication.isPlaying || !Application.isPlaying || !XLuaEnv.available) return;

                if (_reload == null)
                    _reload = XLuaEnv.gtable.Get<Action<string>>("UpdateFunctions");

                for (int index = 0; index < importedAssets.Length; index++)
                {
                    var path = importedAssets[index];
                    if (path.EndsWith(".lua"))
                    {
                        if (path.Contains(frameworkpath))
                        {
                            path = path.Replace(frameworkpath, "");
                        }
                        else if (path.Contains(projectpath))
                        {
                            path = path.Replace(projectpath, "");
                        }
                        path = path.Replace(".lua", "").Replace("/", ".").Remove(0, 1);
                        _changed.Enqueue(path);
                    }
                }

                if (_changed.Count > 0)
                {
                    EditorEnv.delayCall += () =>
                    {
                        if (!EditorApplication.isPlaying || !Application.isPlaying || !XLuaEnv.available) return;
                        while (_changed.Count > 0)
                        {
                            string str = _changed.Dequeue();
                            if (_reload != null)
                            {
                                _reload(str);
                                Debug.Log($"<color=#00A0A0> 重载 Lua 代码 :{str}</color>");
                            }
                        }
                        _reload = null;
                    };
                }
            }
        }
#if UNITY_2018_1_OR_NEWER
        [ScriptedImporter(1, "lua")]
        class LuaImporter : ScriptedImporter
        {
            public override void OnImportAsset(AssetImportContext ctx)
            {
                var text = File.ReadAllText(ctx.assetPath);
                TextAsset asset = new TextAsset(text);
                ctx.AddObjectToAsset("text", asset);
                ctx.SetMainObject(asset);
            }
        }
#endif

        [Serializable]
        class MVVM_GenCodeView_Lua : UIMoudleWindow.UIMoudleWindowTab
        {
            private static string hotFixScriptPath { get { return Application.dataPath.CombinePath("Project/Lua").ToAssetsPath(); } }
            [SerializeField] private string UIMapDir;
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
            [SerializeField]
            private string panelName
            {
                get
                {
                    if (panel == null)
                    {
                        return "";
                    }
                    return panel.name;
                }
            }
            private const string UIMapName = "MVVMMap";
            private const string PanelNamesName = "PanelNames";

            [SerializeField] private UIPanel panel;

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
            private NamesSto sto;
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

            public override void OnGUI()
            {
                if (EditorApplication.isCompiling)
                {
                    GUILayout.Label("Editor is Compiling");
                    GUILayout.Label("please wait");
                    return;
                }
                EditorGUILayout.LabelField("UIMap Name", UIMapName);
                EditorGUILayout.LabelField("Panel Names", PanelNamesName);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Work Directory For Module", GUIStyles.Get("toolbar"));
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
                                if (!UIMapDir.Contains(hotFixScriptPath))
                                    UIMapDir = string.Empty;
                            }

                        }
                    }
                    GUILayout.EndHorizontal();
                }
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


                GUILayout.Space(30);

                if (GUILayout.Button("Gen"))
                {
                    if (string.IsNullOrEmpty(panelName))
                    {
                        EditorWindow.focusedWindow.ShowNotification(new GUIContent("Set UI Panel "));
                        return;
                    }

                    ScriptCreater creater;
                    if (panel != null)
                    {
                        creater = panel.GetComponent<ScriptCreater>();
                        if (creater == null)
                        {
                            creater = panel.gameObject.AddComponent<ScriptCreater>();
                        }
                        creater.Colllect();
                        string err;
                        if (!creater.FieldCheck(out err))
                        {
                            EditorUtility.DisplayDialog("Err", err, "ok");
                        }
                        else
                        {
                            if (panel != null && !Directory.Exists(PanelGenDir))
                            {
                                Directory.CreateDirectory(PanelGenDir);
                            }
                            CreateView(PanelGenDir.CombinePath(ViewName).Append(".lua"), ViewName, creater);
                            CreateVM(PanelGenDir.CombinePath(VMName).Append(".lua"), VMName);
                            WriteMap(uimapPath, panelName);
                            AssetDatabase.Refresh();
                        }
                    }

                }

                GUILayout.Space(10);
                if (GUILayout.Button("Load Panel Names Map"))
                {
                    sto = CheckNameSto(UIMapDir);
                }
                if (sto != null)
                {
                    scroll = GUILayout.BeginScrollView(scroll);
                    {
                        sto.map.ForEach((index, _nm) =>
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(_nm.panelName);
                                if (GUILayout.Button("", GUIStyles.minus))
                                {
                                    sto.RemoveMap(_nm.panelName);
                                    EditorTools.ScriptableObjectTool.Update(sto);
                                    Directory.Delete(sto.workspace.CombinePath(_nm.panelName), true);
                                    WriteMap(sto.workspace.CombinePath(UIMapName).Append(".lua"));
                                }
                                GUILayout.EndHorizontal();
                            }

                        });

                        GUILayout.EndScrollView();
                    }
                }

            }
            Vector2 scroll;
            private void WriteMap(string path)
            {
                string namsPath = path.Replace(UIMapName, PanelNamesName);

                string replace = "";
                string namerp = "";
                var sto = CheckNameSto(UIMapDir);
                sto.map.ForEach(_nm => {
                    namerp = namerp.Append(string.Format("\t{0} = \"{0}\",\n", _nm.panelName));
                    replace = replace.Append("\t" + _nm.content + ",\n");
                });
                string PanelNamesNameSub = namsPath.Replace(hotFixScriptPath, "").Replace(".lua", "").Replace("/", ".").Remove(0, 1);

                File.WriteAllText(namsPath, NameMapSource.Replace(nameFlag, namerp).ToUnixLineEndings());
                File.WriteAllText(path, mapSource.Replace(mapFlag, replace)
                         .Replace("#requirePath#", PanelNamesNameSub).ToUnixLineEndings());
            }
            private void WriteMap(string path, string panelName)
            {
                string sub = PanelGenDir.Replace(hotFixScriptPath, "").Replace("/", ".").Remove(0, 1);
                string content = string.Format("{0} Name = PanelNames.{2},ViewType = require(\"{3}\"), VMType = require(\"{4}\") {1},", "{", "}", panelName, sub.Append("." + ViewName), sub.Append("." + VMName));
                var sto = CheckNameSto(UIMapDir);
                sto.AddMap(panelName, content);
                sto.workspace = UIMapDir;
                EditorTools.ScriptableObjectTool.Update(sto);
                WriteMap(path);
            }

            class tmp
            {
                public string fn;
                public string path;
                public string type;
            }

            private void CreateVM(string path, string VMName)
            {
                bool create = false;
                if (File.Exists(path))
                {
                    if (EditorUtility.DisplayDialog("Make sure", "the file exist  ,do you want replace ?\n" + path, "yes", "no"))
                    {
                        create = true;
                    }
                }
                else
                {
                    create = true;
                }

                if (!create) return;
                File.WriteAllText(path, vmSOurce.Replace("#VMName#", VMName).ToUnixLineEndings());
            }

            private void CreateView(string path, string ViewName, ScriptCreater creater)
            {
                bool create = false;
                if (File.Exists(path))
                {
                    if (EditorUtility.DisplayDialog("Make sure", "the file exist  ,do you want replace ?\n" + path, "yes", "no"))
                    {
                        create = true;
                    }
                }
                else
                {
                    create = true;
                }

                if (!create) return;
                List<string> usings = new List<string>();
                List<tmp> fs = new List<tmp>();

                if (creater.marks != null)
                {
                    for (int i = 0; i < creater.marks.Length; i++)
                    {
                        string ns = creater.marks[i].fieldType;
                        if (!usings.Contains(ns))
                        {
                            usings.Add(ns);
                        }
                        fs.Add(new tmp()
                        {
                            fn = creater.marks[i].fieldName,
                            type = ns.Split('.').Last(),
                            path = creater.marks[i].transform.GetPath().Replace(creater.transform.GetPath(), "").Remove(0, 1)
                        });
                    }
                }
                string use = "";
                for (int i = 0; i < usings.Count; i++)
                {
                    use = use.Append(string.Format("local {2} = StaticUsing(\"{0}\"){1}", usings[i], i == usings.Count - 1 ? "" : "\n", usings[i].Split('.').Last()));
                }
                string f = "";
                for (int i = 0; i < fs.Count; i++)
                {
                    f = f.Append(string.Format("\t\t{0} = self:GetComponent(\"{1}\", typeof({2})),{3}", fs[i].fn, fs[i].path, fs[i].type, i == fs.Count - 1 ? "" : "\n"));
                }
                string result = vSource.Replace("#ViewName#", ViewName)
                    .Replace(ViewUseFlag, use)
                     .Replace(ViewFeildFlag, f);
                File.WriteAllText(path, result.ToUnixLineEndings());
            }



            public override void OnEnable()
            {
                var last = EditorTools.Prefs.GetObject<MVVM_GenCodeView_Lua, MVVM_GenCodeView_Lua>(key);
                if (last != null)
                {
                    this.UIMapDir = last.UIMapDir;
                    this.panel = last.panel;
                }
            }
            private const string key = "MVVM_GenCodeView";

            public override void OnDisable()
            {
                EditorTools.Prefs.SetObject<MVVM_GenCodeView_Lua, MVVM_GenCodeView_Lua>(key, this);
            }


            static string head = "--*********************************************************************************\n" +
            "--Author:         " + EditorTools.ProjectConfig.NameSpace + "\n" +
            "--Version:        " + EditorTools.ProjectConfig.Version + "\n" +
            "--UnityVersion:   " + Application.unityVersion + "\n" +
            "--Date:           " + DateTime.Now.ToString("yyyy-MM-dd") + "\n" +
            "--Description:    " + EditorTools.ProjectConfig.Description + "\n" +
            "--History:        " + DateTime.Now.ToString("yyyy-MM-dd") + "\n" +
            "--*********************************************************************************\n";

            static string vSource = head + "--super Fields " + "\n" +
             "----self.message : publish Event" + "\n" +
             "----self.context : ViewModel" + "\n" +
             "----self.panel :  UIpanel From C#" + "\n" +
             "--super Function " + "\n" +
             "----self:Show()" + "\n" +
             "----self:Hide()" + "\n" +
             "----self:Pause()" + "\n" +
             "----self:UnPause()" + "\n" +
             "----self:BindButton(btn, func)" + "\n" +
             "----self:UnBindButton(btn)" + "\n" +
             "----self:GetComponent(path, type)" + "\n" +
             "----self:BindContextField(field, func)" + "\n" +
             "----self:PublishViewEvent(code,...)\n\n" +
             ViewUseFlag + "\n\n" +
              "---@class " + "#ViewName#" + " : UIView" + "\n" +
             "local " + "#ViewName#" + " = class(\"" + "#ViewName#" + "\",require(\"UI.UIView\"))\n" + "\n" +
             "function " + "#ViewName#" + ":OnLoad()" + "\n" +
             "\tself.Controls = {" + "\n" +
              ViewFeildFlag + "\n" +
              "\t}" + "\n" +
              "\t--BindUIEvent" + "\n\n" +
             "end\n" + "\n" +
             "--Bind ViewModel Fields" + "\n" +
             "function " + "#ViewName#" + ":BindProperty()" + "\n" +
              "\t--BindContextField" + "\n\n" +

             "end\n" + "\n" +
             "function " + "#ViewName#" + ":OnTop(arg)" + "\n" +
             "" + "\n" +
             "end\n" + "\n" +
             "function " + "#ViewName#" + ":OnPress(arg)" + "\n" +
             "" + "\n" +
             "end\n" + "\n" +
             "function " + "#ViewName#" + ":OnPop(arg)" + "\n" +
             "" + "\n" +
             "end\n" + "\n" +

             "function " + "#ViewName#" + ":OnShow()" + "\n" +
             "" + "\n" +
             "end\n" + "\n" +
             "function " + "#ViewName#" + ":OnHide()" + "\n" +
             "" + "\n" +
             "end\n" + "\n" +
             "function " + "#ViewName#" + ":OnPause()" + "\n" +
             "" + "\n" +
             "end\n" + "\n" +
             "function " + "#ViewName#" + ":OnUnPause()" + "\n" +
             "" + "\n" +
             "end\n" + "\n" +
             "function " + "#ViewName#" + ":OnClose()" + "\n" +
             "" + "\n" +
             "end\n" + "\n" +



             "function " + "#ViewName#" + ":OnClear()" + "\n" +
             "\tself.Controls = nil" + "\n" +
             "end\n" + "\n" +
             "function " + "#ViewName#" + ":Dispose()" + "\n" +
             "" + "\n" +
             "end\n" + "\n" +
             "return " + "#ViewName#";
            static string vmSOurce = head + "\n" +
             "--super Fields " + "\n" +
            "--super Function " + "\n" +
            "----self:Subscribe( key,func )" + "\n" +
            "----self:UnSubscribe(key,func)" + "\n" +
            "----self:Invoke( key )\n\n" +
            "---@class " + "#VMName#" + " : ViewModel" + "\n" +
            "local #VMName# = class(\"#VMName#\",require(\"UI.ViewModel\"))\n" + "\n" +
            "--return #VMName#'s Fields By table" + "\n" +
            "--Example return { myCount = 666 }" + "\n" +
            "function #VMName#:GetFieldTable()" + "\n" +
            "" + "\n" +
            "end\n" + "\n" +
            "function #VMName#:OnInitialize()" + "\n" +
            "" + "\n" +
            "end\n" + "\n" +
            "--View's  Event " + "\n" +
            "function #VMName#:ListenViewEvent(code,...)" + "\n" +
            "" + "\n" +
            "end\n" + "\n" +
            "function #VMName#:OnDispose()" + "\n" +
            "" + "\n" +
            "end\n" + "\n" +
            "return #VMName#\n";
            static string NameMapSource = head +
                "" + "\n" +
            "local PanelNames =" + "\n" +
            "{" + "\n" +
            nameFlag + "\n" +
            "}" + "\n" +
            "return PanelNames" + "\n";
            static string mapSource = head +
            "local PanelNames = require(\"#requirePath#\")" + "\n" +
            "local map =" + "\n" +
            "{" + "\n" +
           mapFlag + "\n" +
            "}" + "\n" +
            "return map";
            const string mapFlag = "--Todo";
            const string ViewUseFlag = "--using";
            const string ViewFeildFlag = "--Find";
            const string nameFlag = "--Name";
            private class FormatUserCSScript : UnityEditor.AssetModificationProcessor
            {
                public static void OnWillCreateAsset(string metaPath)
                {
                    if (!EditorPrefs.GetBool(key, false)) return;

                    string filePath = metaPath.Replace(".meta", "");
                    if (!filePath.EndsWith(".lua")) return;
                    string realPath = filePath.ToAbsPath();
                    if (!filePath.Contains(hotFixScriptPath))
                    {
                        File.Delete(realPath);
                        return;
                    }
                    string txt = File.ReadAllText(realPath);

                    if (!txt.Contains("#User#")) return;
                    //这里实现自定义的一些规则
                    txt = txt.Replace("#User#", EditorTools.ProjectConfig.UserName)
                             .Replace("#UserSCRIPTNAME#", Path.GetFileNameWithoutExtension(filePath))
                             .Replace("#UserNameSpace#", EditorTools.ProjectConfig.NameSpace)
                             .Replace("#UserVERSION#", EditorTools.ProjectConfig.Version)
                             .Replace("#UserDescription#", EditorTools.ProjectConfig.Description)
                             .Replace("#UserUNITYVERSION#", Application.unityVersion)
                             .Replace("#UserDATE#", DateTime.Now.ToString("yyyy-MM-dd")).ToUnixLineEndings();

                    File.WriteAllText(realPath, txt, System.Text.Encoding.UTF8);
                    EditorPrefs.SetBool(key, false);
                    AssetDatabase.Refresh();

                }

                private static string newScriptName = "newScript.lua";
                private static string originScriptPathWithNameSpace = EditorEnv.formatScriptsPath.CombinePath("UserLuaScript.txt");

                [MenuItem("Assets/IFramework/Create/FormatLuaScript")]
                public static void Create()
                {
                    CreateOriginIfNull();
                    CopyAsset.Copy(newScriptName, originScriptPathWithNameSpace);
                    EditorPrefs.SetBool(key, true);
                }
                private static void CreateOriginIfNull()
                {
                    if (File.Exists(originScriptPathWithNameSpace)) return;
                    using (FileStream fs = new FileStream(originScriptPathWithNameSpace, FileMode.Create, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            fs.Lock(0, fs.Length);
                            sw.WriteLine("--*********************************************************************************");
                            sw.WriteLine("--Author:         #User#");
                            sw.WriteLine("--Version:        #UserVERSION#");
                            sw.WriteLine("--UnityVersion:   #UserUNITYVERSION#");
                            sw.WriteLine("--Date:           #UserDATE#");
                            sw.WriteLine("--Description:    #UserDescription#");
                            sw.WriteLine("--History:        #UserDATE#--");
                            sw.WriteLine("--*********************************************************************************");
                            sw.WriteLine("local #UserSCRIPTNAME# = class(\"#UserSCRIPTNAME#\")");
                            sw.WriteLine("");
                            sw.WriteLine("function #UserSCRIPTNAME#:ctor(...)");
                            sw.WriteLine("end");
                            sw.WriteLine("");
                            sw.WriteLine("return #UserSCRIPTNAME#");
                            fs.Unlock(0, fs.Length);
                            sw.Flush();
                            fs.Flush();
                        }
                    }
                    AssetDatabase.Refresh();
                }
            }

        }
    }


}

