/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using IFramework.Serialization;
using IFramework.GUITool;
using IFramework.GUITool.ToorbarMenu;
using IFramework.Serialization.DataTable;
using UnityEditor.IMGUI.Controls;

namespace IFramework.Language
{
    [EditorWindowCache("IFramework.Language")]
    partial class LanWindow : EditorWindow
    {
        [CustomPropertyDrawer(typeof(LanguageKeyAttribute))]
        class LanguageKeyDrawer : PropertyDrawer
        {
            private LanGroup _LanGroup { get { return AssetDatabase.LoadAssetAtPath<LanGroup>(EditorEnv.frameworkPath.CombinePath(LanGroup.assetPath)); } }
            private int _hashID;
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (property.type != "string")
                {
                    EditorGUI.PropertyField(position, property, label, true);
                    return;
                }
                if (_hashID == 0) _hashID = "LanguageKeyDrawer".GetHashCode();
                int ctrlId = GUIUtility.GetControlID(_hashID, FocusType.Keyboard, position);
                {
                    label = EditorGUI.BeginProperty(position, label, property);
                    position = EditorGUI.PrefixLabel(position, ctrlId, label);
                    if (DropdownButton(ctrlId, position, new GUIContent(property.stringValue)))
                    {
                        int index = -1;
                        for (int i = 0; i < _LanGroup.keys.Count; i++)
                        {
                            if (_LanGroup.keys[i] == property.stringValue)
                            {
                                index = i;
                                break;
                            }
                        }
                        SearchablePopup.Show(position, _LanGroup.keys.ToArray(), index, (i, str) =>
                        {
                            property.stringValue = str;
                            property.serializedObject.ApplyModifiedProperties();
                        });
                    }
                }
                EditorGUI.EndProperty();
            }

            private static bool DropdownButton(int id, Rect position, GUIContent content)
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
                        EditorStyles.toolbarTextField.Draw(position, content, id, false);
                        break;
                }
                return false;
            }

        }
        private class Styles
        {
            public static GUIStyle EntryBackodd = "CN EntryBackodd";
            public static GUIStyle EntryBackEven = "CN EntryBackEven";
            public static GUIStyle Title = "IN BigTitle";
            public static GUIStyle TitleTxt = "IN BigTitle Inner";
            public static GUIStyle BoldLabel = EditorStyles.boldLabel;
            public static GUIStyle toolbarButton = EditorStyles.toolbarButton;
            public static GUIStyle toolbar = EditorStyles.toolbar;
            public static GUIStyle Fold = GUIStyles.Get("ToolbarDropDown");
            public static GUIStyle FoldOut = EditorStyles.foldout;
            public static GUIStyle CloseBtn = "WinBtnClose";
            public static GUIStyle minus = "OL Minus";
            public static GUIStyle BG = "box";
            public static GUIStyle box = "box";
            public static GUIStyle in_title = new GUIStyle("IN Title") { fixedHeight = 20 + 5 };
            public static GUIStyle settingsHeader = "SettingsHeader";
            public static GUIStyle header = "DD HeaderStyle";
            public static GUIStyle toolbarSeachTextFieldPopup = "ToolbarSeachTextFieldPopup";
            public static GUIStyle searchTextField = new GUIStyle("ToolbarTextField")
            {
                margin = new RectOffset(0, 0, 2, 0)
            };
            public static GUIStyle searchCancelButton = "ToolbarSeachCancelButton";
            public static GUIStyle searchCancelButtonEmpty = "ToolbarSeachCancelButtonEmpty";
            public static GUIStyle foldout = "Foldout";
            public static GUIStyle ToolbarDropDown = "ToolbarDropDown";
            public static GUIStyle selectionRect = "SelectionRect";

            static Styles()
            {
                Fold.fixedHeight = BoldLabel.fixedHeight;
            }
        }
        private class Contents
        {

            public static GUIContent CreateViewTitle = new GUIContent("Create", EditorGUIUtility.IconContent("tree_icon_leaf").image);
            public static GUIContent GroupTitle = new GUIContent("Group", EditorGUIUtility.IconContent("d_tree_icon_frond").image);
            public static GUIContent CopyBtn = new GUIContent("C", "Copy");
            public static GUIContent OK = EditorGUIUtility.IconContent("vcs_add");
            public static GUIContent Warnning = EditorGUIUtility.IconContent("console.warnicon.sml");

        }
        private const string CreateViewNmae = "CreateView";
        private const string Group = "Group";
        private static CreateView createView = new CreateView();
        private GroupView group;

        [SerializeField]
        private string tmpLayout;
        private const float ToolBarHeight = 17;
        private Rect localPosition { get { return new Rect(Vector2.zero, position.size); } }
        private SubWinTree sunwin;
        private ToolBarTree ToolBarTree;


        private abstract class LanwindowItem
        {
            public static LanWindow window;
            public Rect position;

            protected float TitleHeight { get { return Styles.Title.CalcHeight(titleContent, position.width); } }
            protected float smallBtnSize = 20;
            protected float describeWidth = 30;
            protected virtual GUIContent titleContent { get; }
            public void OnGUI(Rect position)
            {
                this.position = position;
                position.DrawOutLine(2, Color.black);
                GUI.BeginClip(position);
                {
                    Rect[] rs = position.HorizontalSplit(TitleHeight);
                    GUI.Box(rs[0], "");
                    GUI.Box(rs[0], titleContent, Styles.Title);
                    DrawContent(rs[1]);
                    GUI.EndClip();
                }

            }
            protected abstract void DrawContent(Rect rect);
        }
    }
    partial class LanWindow : EditorWindow
    {
        private LanGroup _group;
        private List<LanPair> _pairs { get { return _group.pairs; } }
        private List<string> _keys { get { return _group.keys; } }

        private string stoPath;
        private void OnEnable()
        {
            LanwindowItem.window = this;
            stoPath = EditorEnv.frameworkPath.CombinePath(LanGroup.assetPath);
            LoadLanGroup();
            this.titleContent = new GUIContent("Lan", EditorGUIUtility.IconContent("d_WelcomeScreen.AssetStoreLogo").image);
            group = new GroupView();
            SubwinInit();
        }
        private void LoadLanGroup()
        {
            if (File.Exists(stoPath))
                _group = EditorTools.ScriptableObjectTool.Load<LanGroup>(stoPath);
            else
                _group = EditorTools.ScriptableObjectTool.Create<LanGroup>(stoPath);
        }
        private void UpdateLanGroup()
        {
            EditorTools.ScriptableObjectTool.Update(_group);
        }
        private void OnDisable()
        {
            tmpLayout = sunwin.Serialize();
            UpdateLanGroup();
        }

        private void Views(Rect rect)
        {
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < sunwin.allLeafCount; i++)
            {
                SubWinTree.TreeLeaf leaf = sunwin.allLeafs[i];
                menu.AddItem(leaf.titleContent, !sunwin.closedLeafs.Contains(leaf), () =>
                {
                    if (sunwin.closedLeafs.Contains(leaf))
                        sunwin.DockLeaf(leaf, SubWinTree.DockType.Left);
                    else
                        sunwin.CloseLeaf(leaf);
                });
            }
            menu.DropDown(rect);
            Event.current.Use();
        }
        private void SubwinInit()
        {
            sunwin = new SubWinTree();
            sunwin.drawCursorEve += (rect, sp) =>
            {
                if (sp == SplitType.Vertical)
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
                else
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);
            };
            if (string.IsNullOrEmpty(tmpLayout))
            {
                for (int i = 1; i <= 2; i++)
                {
                    string userdata = i == 1 ? "Group" : "CreateView";
                    SubWinTree.TreeLeaf L = sunwin.CreateLeaf(new GUIContent(userdata));
                    L.userData = userdata;
                    sunwin.DockLeaf(L, SubWinTree.DockType.Left);
                }
            }
            else
            {
                sunwin.DeSerialize(tmpLayout);
            }
            sunwin[Group].titleContent = new GUIContent(Group);
            sunwin[Group].minSize = new Vector2(250, 250);
            sunwin[CreateViewNmae].minSize = new Vector2(300, 300);
            sunwin[Group].paintDelegate += group.OnGUI;
            sunwin[CreateViewNmae].paintDelegate += createView.OnGUI;


            ToolBarTree = new ToolBarTree();
            ToolBarTree.DropDownButton(new GUIContent("Views"), Views, 60)
                            .FlexibleSpace()
                            .Toggle(new GUIContent("Title"), (bo) => { sunwin.isShowTitle = bo; }, sunwin.isShowTitle, 60)
                            .Toggle(new GUIContent("Lock"), (bo) => { sunwin.isLocked = bo; }, sunwin.isLocked, 60);

        }
        private void OnGUI()
        {
            var rs = localPosition.Zoom(AnchorType.MiddleCenter, -2).HorizontalSplit(ToolBarHeight, 4);
            ToolBarTree.OnGUI(rs[0]);
            sunwin.OnGUI(rs[1]);
            this.minSize = sunwin.minSize + new Vector2(0, ToolBarHeight);
        }

        private void DeletePairsByKey(string key)
        {
            _group.DeletePairsByKey(key);
            UpdateLanGroup();
        }
        private void DeleteLanPair(LanPair pair)
        {
            _group.DeletePair(pair);
        }
        private void AddLanPair(LanPair pair)
        {
            if (string.IsNullOrEmpty(pair.value.Trim()))
            {
                ShowNotification(new GUIContent("Value Can't be Null"));
                return;
            }
            LanPair tmpPair = new LanPair()
            {
                lan = pair.lan,
                key = pair.key,
                value = pair.value
            };
            LanPair lp = _pairs.Find((p) => { return p.lan == tmpPair.lan && p.key == tmpPair.key; });
            if (lp == null)
            {
                _pairs.Add(tmpPair);
                UpdateLanGroup();
            }
            else
            {
                if (lp.value == tmpPair.value)
                    ShowNotification(new GUIContent("Don't Add Same"));
                else
                {
                    if (EditorUtility.DisplayDialog("Warn",
                        string.Format("Replace Old Value ?\n Old Value  {0}\n New Vlaue  {1}", lp.value, tmpPair.value), "Yes", "No"))
                    {
                        lp.value = tmpPair.value;
                    }
                }
            }
        }

        private void AddLanGroupKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                ShowNotification(new GUIContent("Err: key is Empty " + key));
                return;
            }
            if (!_keys.Contains(key))
            {
                _keys.Add(key);
                UpdateLanGroup();
            }
            else
            {
                ShowNotification(new GUIContent("Err: key Has Exist " + key));
            }
        }
        private void DeleteLanKey(string key)
        {
            if (_keys.Contains(key))
            {
                _keys.Remove(key);
                DeletePairsByKey(key);
            }
        }
        private void CleanData()
        {
            _pairs.Clear();
            _keys.Clear();
            UpdateLanGroup();
        }
        private void WriteXml(string path)
        {
            path.WriteText(Xml.ToXml(_pairs), Encoding.UTF8);
        }
        private void ReadXml(string path)
        {
            List<LanPair> ps = Xml.FromXml<List<LanPair>>(path.ReadText(Encoding.UTF8))
                .Distinct()
                .ToList().FindAll((p) => { return !string.IsNullOrEmpty(p.key) && !string.IsNullOrEmpty(p.value); });
            AddLanPairs(ps);
        }
        private void WriteJson(string path)
        {
            path.WriteText(JsonUtility.ToJson(_pairs), Encoding.UTF8);
        }
        private void ReadJson(string path)
        {
            List<LanPair> ps = JsonUtility.FromJson<List<LanPair>>(path.ReadText(Encoding.UTF8))
               .Distinct()
               .ToList().FindAll((p) => { return !string.IsNullOrEmpty(p.key) && !string.IsNullOrEmpty(p.value); });
            AddLanPairs(ps);
        }
        private bool IsKeyInUse(string key)
        {
            for (int i = 0; i < _pairs.Count; i++)
            {
                if (_pairs[i].key == key)
                {
                    return true;
                }
            }
            return false;
        }
        private void ReadCsv(string path)
        {
            var dw = DataTableTool.CreateReader(new StreamReader(path, System.Text.Encoding.UTF8), new DataRow(), new DataExplainer());
            var pairs = dw.Get<LanPair>().Distinct()
                 .ToList().FindAll((p) => { return !string.IsNullOrEmpty(p.key) && !string.IsNullOrEmpty(p.value); });
            dw.Dispose();
            AddLanPairs(pairs);
        }
        private void ReadScriptableObject(string path)
        {
            var g = AssetDatabase.LoadAssetAtPath<LanGroup>(path.ToAssetsPath());
            if (g == null) return;
            AddLanPairs(g.pairs);
        }
        private void WriteScriptableObject(string path)
        {
            if (!File.Exists(path))
                EditorTools.ScriptableObjectTool.Create<LanGroup>(path.ToAssetsPath());
            var g = EditorTools.ScriptableObjectTool.Load<LanGroup>(path.ToAssetsPath());
            if (g == null) return;
            g.pairs.AddRange(_pairs);
            g.pairs.Distinct();
            EditorTools.ScriptableObjectTool.Update(g);
        }

        private void WriteCsv(string path)
        {
            var w = DataTableTool.CreateWriter(new StreamWriter(path, false),
                           new DataRow(),
                           new DataExplainer());
            w.Write(_pairs);
            w.Dispose();
        }


        private void AddLanPairs(List<LanPair> pairs)
        {
            if (pairs == null || pairs.Count == 0) return;
            for (int i = 0; i < pairs.Count; i++)
            {
                var filePair = pairs[i];
                if (!_keys.Contains(filePair.key)) _keys.Add(filePair.key);
                LanPair oldPair = _pairs.Find((pair) => { return pair.key == filePair.key && pair.lan == filePair.lan; });
                if (oldPair == null) _pairs.Add(filePair);
                else
                {
                    if (oldPair.value != filePair.value)
                    {
                        if (EditorUtility.DisplayDialog("Warning",
                                            "The LanPair Is Same Do You Want Replace \n"
                                            .Append(string.Format("Lan {0}\t\t Key {0}\t \n", oldPair.lan, oldPair.key))
                                            .Append(string.Format("Old  Val\t\t {0}\n", oldPair.value))
                                            .Append(string.Format("New  Val\t\t {0}\n", filePair.value))
                                            , "Yes", "No"))
                        {
                            oldPair.value = filePair.value;
                        }
                    }
                }
            }
            UpdateLanGroup();
        }
        [Serializable]
        private class CreateView : LanwindowItem
        {
            public CreateView()
            {
                searchField = new GUITool.SearchField("", null, 0);

                searchField.onValueChange += (str) =>
                {
                    keySearchStr = str;
                };
            }
            protected override GUIContent titleContent { get { return Contents.CreateViewTitle; } }
            [SerializeField] private bool toolFoldon;
            private void Tool()
            {
                Rect rect = EditorGUILayout.BeginHorizontal(Styles.toolbar);
                {
                    toolFoldon = EditorGUILayout.Foldout(toolFoldon, "Tool", true);
                    EditorGUILayout.EndHorizontal();
                }
                if (!toolFoldon) return;
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Fresh"))
                    {
                        window.LoadLanGroup();

                    }
                    if (GUILayout.Button("Save"))
                    {
                        window.UpdateLanGroup();

                    }
                    if (GUILayout.Button("Clear"))
                    {
                        window.CleanData();

                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Read Xml"))
                    {
                        window.LoadLanGroup();
                        string path = EditorUtility.OpenFilePanel("xml (End with  xml)", Application.dataPath, "xml");
                        if (string.IsNullOrEmpty(path) || !path.EndsWith(".xml")) return;
                        window.ReadXml(path);
                    }
                    if (GUILayout.Button("Write Xml"))
                    {
                        window.UpdateLanGroup();
                        string path = EditorUtility.SaveFilePanel("xml (End with  xml)", Application.dataPath, "LanGroup", "xml");
                        if (string.IsNullOrEmpty(path) || !path.EndsWith(".xml")) return;
                        window.WriteXml(path);
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Read Json"))
                    {
                        string path = EditorUtility.OpenFilePanel("json (End With json)", Application.dataPath, "json");
                        if (string.IsNullOrEmpty(path) || !path.EndsWith(".json")) return;
                        window.ReadJson(path);
                    }
                    if (GUILayout.Button("Write Json"))
                    {
                        string path = EditorUtility.SaveFilePanel("json (End With json)", Application.dataPath, "LanGroup", "json");
                        if (string.IsNullOrEmpty(path) || !path.EndsWith(".json")) return;
                        window.WriteJson(path);
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Read Csv"))
                    {
                        string path = EditorUtility.OpenFilePanel("csv (End With csv)", Application.dataPath, "csv");
                        if (string.IsNullOrEmpty(path) || !path.EndsWith(".csv")) return;
                        window.ReadCsv(path);
                    }
                    if (GUILayout.Button("Write Csv"))
                    {
                        string path = EditorUtility.SaveFilePanel("csv (End With csv)", Application.dataPath, "LanGroup", "csv");
                        if (string.IsNullOrEmpty(path) || !path.EndsWith(".csv")) return;
                        window.WriteCsv(path);
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Read ScriptableObject"))
                    {
                        string path = EditorUtility.OpenFilePanel("ScriptableObject (End With asset)", Application.dataPath, "asset");
                        if (string.IsNullOrEmpty(path) || !path.EndsWith(".asset")) return;
                        window.ReadScriptableObject(path);
                    }
                    if (GUILayout.Button("Write ScriptableObject"))
                    {
                        string path = EditorUtility.SaveFilePanel("ScriptableObject (End With asset)", Application.dataPath, "LanGroup", "asset");
                        if (string.IsNullOrEmpty(path) || !path.EndsWith(".asset")) return;
                        window.WriteScriptableObject(path);
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            [SerializeField] private bool creatingKeyFoldon;
            [SerializeField] private string tmpKey;
            private void CreateLanKey()
            {
                Rect rect = EditorGUILayout.BeginHorizontal(Styles.toolbar);
                {
                    creatingKeyFoldon = EditorGUILayout.Foldout(creatingKeyFoldon, "Create Key", true);

                    EditorGUILayout.EndHorizontal();
                }
                if (!creatingKeyFoldon) return;
                rect = EditorGUILayout.BeginHorizontal();
                {
                    tmpKey = EditorGUILayout.TextField(tmpKey);
                    if (GUILayout.Button(Contents.OK, GUILayout.Width(describeWidth)))
                    {
                        window.AddLanGroupKey(tmpKey);
                        tmpKey = string.Empty;
                    }
                    EditorGUILayout.EndHorizontal();
                }

            }

            [SerializeField] private bool createLanPairFlodon;
            [SerializeField] private LanPair tmpLanPair = new LanPair();
            [SerializeField] private int hashID;
            private void AddLanPairFunc()
            {

                if (window._keys.Count == 0) return;
                Rect rect = EditorGUILayout.BeginHorizontal(Styles.toolbar);
                {
                    createLanPairFlodon = EditorGUILayout.Foldout(createLanPairFlodon, "Create LanPair", true);
                    EditorGUILayout.EndHorizontal();
                }
                if (!createLanPairFlodon) return;
                if (tmpLanPair == null) tmpLanPair = new LanPair() { key = window._keys[0] };
                if (hashID == 0) hashID = "CreateView".GetHashCode();

                GUILayout.BeginVertical();
                {

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Lan", GUILayout.Width(describeWidth));
                    tmpLanPair.lan = (SystemLanguage)EditorGUILayout.EnumPopup(tmpLanPair.lan);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(EditorGUIUtility.IconContent("editicon.sml"), GUILayout.Width(smallBtnSize));
                        GUILayout.EndHorizontal();
                    }

                    Rect pos = GUILayoutUtility.GetLastRect();
                    int ctrlId = GUIUtility.GetControlID(hashID, FocusType.Keyboard, pos);
                    {
                        if (DropdownButton(ctrlId, pos, new GUIContent(string.Format("Key: {0}", tmpLanPair.key))))
                        {
                            int index = -1;
                            for (int i = 0; i < window._keys.Count; i++)
                            {
                                if (window._keys[i] == tmpLanPair.key)
                                {
                                    index = i; break;
                                }
                            }
                            SearchablePopup.Show(pos, window._keys.ToArray(), index, (i, str) =>
                            {
                                tmpLanPair.key = str;
                                window.Repaint();
                            });
                        }
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Val", GUILayout.Width(describeWidth));
                    tmpLanPair.value = EditorGUILayout.TextField(tmpLanPair.value);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(Contents.OK))
                    {
                        window.AddLanPair(tmpLanPair);

                    }

                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }

            }
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

            [SerializeField] private bool keyFoldon;
            [SerializeField] private Vector2 scroll;
            [SerializeField] private string keySearchStr = string.Empty;
            private GUITool.SearchField searchField;
            private void LanGroupKeysView()
            {
                GUILayout.BeginHorizontal(Styles.toolbar);
                {
                    keyFoldon = EditorGUILayout.Foldout(keyFoldon, string.Format("Keys  Count: {0}", window._keys.Count), true);
                    GUILayout.Label("");
                    searchField.OnGUI(GUILayoutUtility.GetLastRect());
                    GUILayout.EndHorizontal();
                }

                if (keyFoldon)
                {
                    scroll = GUILayout.BeginScrollView(scroll);
                    {
                        window._keys.ForEach((index, key) =>
                        {
                            if (key.ToLower().Contains(keySearchStr.ToLower()))
                            {
                                GUILayout.BeginHorizontal(Styles.BG);
                                EditorGUILayout.SelectableLabel(key, GUILayout.Height(20));
                                GUILayout.Label(window.IsKeyInUse(key) ? GUIContent.none : Contents.Warnning, GUILayout.Width(smallBtnSize));
                                if (GUILayout.Button(string.Empty, Styles.CloseBtn, GUILayout.Width(smallBtnSize), GUILayout.Height(smallBtnSize)))
                                {
                                    if (EditorUtility.DisplayDialog("Make sure", "You Will Delete All Pairs with this key", "ok", "no"))
                                        window.DeleteLanKey(key);
                                }

                                GUILayout.EndHorizontal();
                            }
                        });

                        GUILayout.EndScrollView();
                    }

                }
            }

            protected override void DrawContent(Rect rect)
            {
                GUILayout.BeginArea(rect.Zoom(AnchorType.MiddleCenter, -10));
                Tool();
                GUILayout.Space(5);
                AddLanPairFunc();
                GUILayout.Space(5);
                CreateLanKey();
                GUILayout.Space(5);
                LanGroupKeysView();
                GUILayout.EndArea();
            }

        }



        [Serializable]
        private class GroupView : LanwindowItem
        {
            protected override GUIContent titleContent { get { return Contents.GroupTitle; } }
            private GUITool.SearchField search;
            private enum SearchType
            {
                Key, Language, Value
            }
            private SearchType _searchType;
            private SelectTree _tree;
            public GroupView()
            {
                TreeViewState _state=new TreeViewState();
                MultiColumnHeader state = new MultiColumnHeader(new MultiColumnHeaderState(new MultiColumnHeaderState.Column[] {

                    new MultiColumnHeaderState.Column(){
                        width=20,
                        autoResize=false

                    },
                    new MultiColumnHeaderState.Column(){
                        headerContent=new GUIContent("Language"),
                            width=100,
                        autoResize=false
                    },
                    new MultiColumnHeaderState.Column(){
                        headerContent=new GUIContent("Key"),
                        minWidth=100

                    },
                    new MultiColumnHeaderState.Column(){
                        headerContent=new GUIContent("Value"),
                        minWidth=200
                    }
                }));
               _tree = new SelectTree(_state, state, this);
                search = new GUITool.SearchField("", Enum.GetNames(typeof(SearchType)), 0);
                search.onModeChange += (value) => { _searchType = (SearchType)value; };
            }
            protected override void DrawContent(Rect rect)
            {
                var rs = rect.Zoom(AnchorType.MiddleCenter, -10).Split(SplitType.Horizontal, 20);
                _tree.Fresh();
                search.OnGUI(rs[0]);
                _tree.OnGUI(rs[1]);
            }

            private class SelectTree : TreeView
            {
                private struct Index
                {
                    public int id;
                    public LanPair value;
                }
                private readonly GroupView group;
                private List<Index> tmps = new List<Index>();
                public SelectTree(TreeViewState state, MultiColumnHeader multiColumnHeader, GroupView group) : base(state, multiColumnHeader)
                {
                    this.rowHeight = 20;
                    this.group = group;
                    showAlternatingRowBackgrounds = true;
                    SearchChanged("");
                }
                protected override TreeViewItem BuildRoot()
                {
                    var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
                    return root;
                }

                protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
                {
                    List<TreeViewItem> list = new List<TreeViewItem>();
                    for (int i = 0; i < tmps.Count; i++)
                    {
                        list.Add(new TreeViewItem()
                        {
                            id = tmps[i].id,
                            depth = 1
                        });
                    }

                    return list;
                }

                public void Fresh()
                {
                    tmps.Clear();
                    for (int i = 0; i < window._pairs.Count; i++)
                    {
                        bool fit = false;

                        if (string.IsNullOrEmpty(group.search.value))
                            fit= true;
                        switch ((group._searchType))
                        {
                            case SearchType.Key:
                                fit|= window._pairs[i].key.ToLower().Contains(group.search.value.ToLower());
                                break;
                            case SearchType.Language:
                                fit |= window._pairs[i].lan.ToString().ToLower().Contains(group.search.value.ToLower());
                                break;
                            case SearchType.Value:
                                fit |= window._pairs[i].value.ToLower().Contains(group.search.value.ToLower());
                                break;
                        }
                        if (fit)
                        {
                            tmps.Add(new Index() {
                                id = i,
                                value = window._pairs[i]
                            });
                        }
                    }
                    Reload();
                }
                protected override void RowGUI(RowGUIArgs args)
                {
                    var pair = window._pairs[args.item.id];

                    for (int i = 0; i < args.GetNumVisibleColumns(); i++)
                    {
                        switch (i)
                        {
                            case 0:
                                if (GUI.Button(args.GetCellRect(i), "", Styles.minus))
                                {
                                    if (EditorUtility.DisplayDialog("Make Sure", string.Format("Really want delete\n" +
                                       "Key :{0}\n" +
                                       "Language :{1}\n" +
                                       "Value : {2}", pair.key, pair.lan, pair.value), "yes", "no"))
                                    {
                                        window.DeleteLanPair(pair);
                                        window.UpdateLanGroup();
                                    }

                                }
                                break;
                            case 1:
                                EditorGUI.EnumPopup(args.GetCellRect(i), pair.lan);

                                break;
                            case 2:
                                EditorGUI.LabelField(args.GetCellRect(i), pair.key);
                                break;
                            case 3:
                                EditorGUI.LabelField(args.GetCellRect(i), pair.value);
                                break;
                            default:
                                break;
                        }
                    }
                }
                protected override void ContextClicked()
                {
                    var list = this.GetSelection();
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () =>
                    {
                        if (EditorUtility.DisplayDialog("Make Sure", string.Format("Really want delete {0} pairs", list.Count), "yes", "no"))
                        {
                            for (int j = list.Count - 1; j >= 0; j--)
                            {
                                window.DeleteLanPair(window._pairs[tmps[j].id]);
                            }
                            window.UpdateLanGroup();
                        }
                    });

                    menu.ShowAsContext();
                }
      
            }
        }
    }
}