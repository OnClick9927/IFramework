/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-21
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using IFramework.Serialization;
using IFramework.GUITool.ToorbarMenu;
using System.Linq;

namespace IFramework.Hotfix.AB
{
    [EditorWindowCache("IFramework.AssetBundle")]
    public partial class AssetBundleWindow : EditorWindow
    {
        public class WindowMemory
        {
            [Serializable]
            public class Collections
            {
                [Serializable]
                public class Collection
                {
                    [Serializable]
                    public class SubFile
                    {
                        public enum FileType
                        {
                            ValidFile,
                            Folder,
                            InValidFile
                        }

                        public bool isOpen;

                        public string path;
                        public string name;
                        public FileType fileType;

                        [NonSerialized]
                        private Texture2D thumbnail;
                        public Texture2D ThumbNail
                        {
                            get
                            {
                                if (thumbnail == null)
                                {
                                    if (fileType == FileType.Folder)
                                        thumbnail = EditorGUIUtility.IconContent("Folder Icon").image as Texture2D;
                                    thumbnail = AssetPreview.GetMiniThumbnail(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path));
                                }
                                return thumbnail;
                            }
                        }
                        [SerializeField]
                        private bool selscted;
                        public bool Selected { get { return selscted; } set { SetSelected(value); } }
                        private void SetSelected(bool sel)
                        {
                            if (this.selscted == sel) return;
                            if (fileType == FileType.InValidFile) return;

                            this.selscted = sel;
                            for (int i = 0; i < SubFiles.Count; i++)
                            {
                                SubFiles[i].SetSelected(sel);
                            }
                        }


                        public List<SubFile> SubFiles = new List<SubFile>();
                        public SubFile() { }
                        public SubFile(bool selected, string path, string name, FileType fileType)
                        {
                            this.selscted = selected;
                            this.path = path;
                            this.name = name;
                            this.fileType = fileType;
                        }
                        public void SetSubAssetPaths()
                        {
                            if (fileType == FileType.Folder)
                            {
                                path.GetSubDirs().ForEach((dirName) =>
                                {
                                    SubFile asset = new SubFile(false,
                                        path.CombinePath(dirName),
                                        dirName,
                                        FileType.Folder);
                                    asset.SetSubAssetPaths();
                                    SubFiles.Add(asset);
                                });
                                path.GetSubFiles(false).ForEach((fileName) =>
                                {
                                    if (fileName.Contains(".meta")) return;
                                    bool valid = fileName.EndsWith(".cs");
                                    SubFile asset = new SubFile(false,
                                        fileName.ToAssetsPath(),
                                        fileName.GetFileNameWithoutExtend(),
                                        valid ? FileType.InValidFile : FileType.ValidFile);
                                    asset.SetSubAssetPaths();
                                    SubFiles.Add(asset);
                                });
                            }
                        }
                    }
                    public enum CollectType
                    {
                        ABName,
                        DirName,
                        FileName,
                        Scene
                    }
                    public CollectType type;
                    public string path;
                    public string bundle;
                    public SubFile subAsset;
                    public Collection() { }
                    public Collection(string path)
                    {
                        this.path = path;
                        SetSubAssetPaths();
                    }
                    private void SetSubAssetPaths()
                    {
                        subAsset = new SubFile(false, path, Path.GetFileName(path), SubFile.FileType.Folder);
                        subAsset.SetSubAssetPaths();
                    }

                    public List<string> GetSubAssetPaths()
                    {
                        List<string> result = new List<string>();
                        GetSubAssetPaths(subAsset, result);
                        return result;
                    }
                    private void GetSubAssetPaths(SubFile subAsset, List<string> result)
                    {
                        if (subAsset.fileType != SubFile.FileType.Folder && subAsset.Selected)
                            result.Add(subAsset.path);
                        for (int i = 0; i < subAsset.SubFiles.Count; i++)
                            GetSubAssetPaths(subAsset.SubFiles[i], result);
                    }
                }
                public List<Collection> collection = new List<Collection>();
                public void Add(string path)
                {
                    for (int i = 0; i < collection.Count; i++)
                    {
                        if (path.Contains(collection[i].path))
                            return;
                    }
                    Collection collect = new Collection(path);
                    collection.Add(collect);
                }
                public void Remove(Collection item)
                {
                    if (collection.Contains(item))
                    {
                        collection.Remove(item);
                    }
                }
            }

            [Serializable]
            public class BuiidCollection
            {
                [Serializable]
                public class AssetBundleBuild_Class
                {
                    public string assetBundleName;
                    public string assetBundleVariant;
                    public List<string> assetNames = new List<string>();
                    public List<string> addressableNames = new List<string>();

                    public bool CrossRefence;
                    public long FileLength;
                    public string Size;
                    public static implicit operator AssetBundleBuild(AssetBundleBuild_Class _class)
                    {
                        AssetBundleBuild build = new AssetBundleBuild();
                        build.assetBundleName = _class.assetBundleName;
                        build.assetBundleVariant = _class.assetBundleVariant;
                        build.assetNames = _class.assetNames.ToArray();
                        return build;
                    }
                    public static implicit operator AssetBundleBuild_Class(AssetBundleBuild _struct)
                    {
                        return new AssetBundleBuild_Class()
                        {
                            assetBundleName = _struct.assetBundleName,
                            assetBundleVariant = _struct.assetBundleVariant,
                            assetNames = _struct.assetNames.ToList(),
                            addressableNames = _struct.addressableNames != null ? _struct.addressableNames.ToList() : null
                        };
                    }
                }
                [Serializable]
                public class Deprndences
                {
                    public string assetPath;
                    public string assetName { get { return assetPath.GetFileNameWithoutExtend(); } }
                    public Texture2D thumbnail { get { return AssetPreview.GetMiniThumbnail(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath)); } }
                    public string bundleName;

                    public string size;
                    public long fileLength;
                    [SerializeField]
                    public List<string> bundles = new List<string>();
                }
                [SerializeField]
                public List<Deprndences> dps = new List<Deprndences>();
                public List<AssetBundleBuild_Class> abbs = new List<AssetBundleBuild_Class>();

                private Deprndences ContainsAsset(string assetpath)
                {
                    for (int i = 0; i < dps.Count; i++)
                    {
                        if (dps[i].assetPath == assetpath)
                        {
                            return dps[i];
                        }
                    }
                    return default(Deprndences);
                }
                private AssetBundleBuild_Class ContainsBundle(string bundleName)
                {
                    for (int i = 0; i < abbs.Count; i++)
                    {
                        if (abbs[i].assetBundleName == bundleName)
                        {
                            return abbs[i];
                        }
                    }
                    return default(AssetBundleBuild_Class);
                }

                public void RemoveBundle(string bundleName)
                {
                    AssetBundleBuild_Class item = ContainsBundle(bundleName);
                    if (item == null)
                        Debug.Log("no Bundle");
                    else
                    {
                        abbs.Remove(item);
                        BuildsToDps();
                    }
                }
                public void RemoveBundles(string[] bundleName)
                {
                    for (int i = 0; i < bundleName.Length; i++)
                    {
                        AssetBundleBuild_Class item = ContainsBundle(bundleName[i]);
                        if (item == null)
                            Debug.Log("no Bundle");
                        else
                            abbs.Remove(item);
                    }
                    BuildsToDps();
                }
                public void RemoveAsset(string bundleName, string AssetName)
                {
                    AssetBundleBuild_Class item = ContainsBundle(bundleName);
                    if (item == null)
                        Debug.Log("no Bundle");
                    else
                        for (int i = item.assetNames.Count - 1; i >= 0; i--)
                            if (item.assetNames[i] == AssetName)
                                item.assetNames.RemoveAt(i);
                    if (item.assetNames.Count == 0)
                        abbs.Remove(item);
                    BuildsToDps();
                }
                public void RemoveAssets(string bundleName, string[] AssetName)
                {
                    AssetBundleBuild_Class item = ContainsBundle(bundleName);
                    if (item == null)
                        Debug.Log("no Bundle");
                    else
                    {
                        AssetName.ForEach((assetname) => {

                            for (int i = item.assetNames.Count - 1; i >= 0; i--)
                                if (item.assetNames[i] == assetname)
                                    item.assetNames.RemoveAt(i);
                        });
                    }
                    BuildsToDps();
                }

                public List<AssetBundleBuild> GetAssetBundleBuilds()
                {
                    return abbs.ConvertAll<AssetBundleBuild>((item) => { return item; });
                }
                public void ReadAssetbundleBuild(List<AssetBundleBuild> list)
                {
                    abbs.Clear();
                    dps.Clear();
                    list.ReverseForEach((item) =>
                    {
                        abbs.Add(item);
                    });
                    BuildsToDps();
                }

                private void BuildsToDps()
                {
                    dps.Clear();
                    abbs.ReverseForEach((buildItem) => {
                        buildItem.CrossRefence = false;
                        buildItem.FileLength = 0;
                        for (int i = 0; i < buildItem.assetNames.Count; i++)
                        {
                            string assetpath = buildItem.assetNames[i];
                            Deprndences info = ContainsAsset(assetpath);
                            if (info == null)
                            {
                                info = new Deprndences();
                                info.assetPath = assetpath;
                                info.fileLength = IO.GetFileLength(assetpath);
                                info.size = IO.GetFileSize(info.fileLength);
                                AssetImporter importer = AssetImporter.GetAtPath(assetpath);
                                info.bundleName = importer.assetBundleName + "." + importer.assetBundleVariant;
                                dps.Add(info);
                            }
                            else
                            {
                                buildItem.CrossRefence = true;
                            }
                            info.bundles.Add(buildItem.assetBundleName);
                            buildItem.FileLength += info.fileLength;
                        }
                        buildItem.Size = IO.GetFileSize(buildItem.FileLength);


                    });

                }
            }

            public Collections colect = new Collections();
            public BuiidCollection build = new BuiidCollection();
        }

        private class ToolGUI : GUIBase
        {
            private WindowMemory.BuiidCollection buildCollection { get { return _window.Info.build; } }

            public override void OnGUI(Rect position)
            {
                base.OnGUI(position);
                GUILayout.BeginArea(position);
                {

                    GUILayout.Label("BuildSetting", GUIStyles.Get("LargeBoldLabel"));
                    GUILayout.Label("", GUIStyles.Get("IN Title"), GUILayout.Height(5));
                    EditorGUILayout.LabelField("Build Target:", EditorUserBuildSettings.activeBuildTarget.ToString());
                    EditorGUILayout.LabelField("Output Path:", ABTool.assetsOutPutPath.ToAbsPath());
                    EditorGUILayout.LabelField("Manifest Path:", ABTool.configPath);
                    EditorGUILayout.LabelField("Version Path:", ABTool.versionPath);
                    ABTool.testmode = EditorGUILayout.Toggle(new GUIContent("Test Mode"), ABTool.testmode);


                    GUILayout.Label("", GUIStyles.Get("IN Title"), GUILayout.Height(5));
                    GUILayout.Space(10);
                    if (GUILayout.Button("Clear Bundle Files"))
                    {
                        Build.DeleteBundleFiles();

                    }
                    if (GUILayout.Button("Build Manifest"))
                    {
                        Build.BuildManifest(ABTool.configPath, buildCollection.GetAssetBundleBuilds());
                    }
                    if (GUILayout.Button("Build AssetBundle"))
                    {
                        Build.BuildManifest(ABTool.configPath, buildCollection.GetAssetBundleBuilds());
                        Build.BuildAssetBundles(buildCollection.GetAssetBundleBuilds(), EditorUserBuildSettings.activeBuildTarget);
                        EditorTools.OpenFloder(ABTool.assetsDir);

                    }
                    if (GUILayout.Button("Clear Versions"))
                    {
                        Build.ClearVersions();
                    }
                    if (GUILayout.Button("Copy Bundles to streamingAssetsPath"))
                    {
                        Build.CopyBundleFilesTo(Application.streamingAssetsPath);
                    }
                    GUILayout.Space(10);
                    GUILayout.EndArea();
                }

            }
        }
        private class DirCollectGUI : GUIBase
        {
            public class AssetChooseWindow : PopupWindowContent
            {
                public WindowMemory.Collections.Collection.SubFile assetinfo;
                public override void OnGUI(Rect rect)
                {
                    if (assetinfo == null) return;
                    scroll = GUILayout.BeginScrollView(scroll);
                    {

                        Draw(assetinfo, 0);

                        GUILayout.EndHorizontal();
                    }

                }
                private Vector2 scroll;

                private void Draw(WindowMemory.Collections.Collection.SubFile assetinfo, float offset)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(offset);
                        if (assetinfo.fileType == WindowMemory.Collections.Collection.SubFile.FileType.InValidFile)
                            GUI.enabled = false;
                        bool s = assetinfo.Selected;
                        s = GUILayout.Toggle(s, new GUIContent(assetinfo.ThumbNail), GUILayout.Height(16), GUILayout.Width(40));
                        assetinfo.Selected = s;

                        if (assetinfo.SubFiles.Count > 0)
                            assetinfo.isOpen = EditorGUILayout.Foldout(assetinfo.isOpen, assetinfo.name);
                        else
                            GUILayout.Label(assetinfo.name);
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();
                    }


                    if (assetinfo.isOpen)
                        for (int i = 0; i < assetinfo.SubFiles.Count; i++)
                            Draw(assetinfo.SubFiles[i], offset + 20);
                }
            }

            private const string CollectType = "CollectType";
            private const string BundleName = "BundleName";
            private const string SearchPath = "SearchPath";
            private const string SelectButton = "Set";
            private const string TitleStyle = "IN BigTitle";
            private const string EntryBackodd = "CN EntryBackodd";
            private const string EntryBackEven = "CN EntryBackEven";
            private const float lineHeight = 20;

            private Vector2 ScrollPos;
            private AssetChooseWindow chosseWindow = new AssetChooseWindow();
            private TableViewCalculator tableViewCalc = new TableViewCalculator();
            private WindowMemory.Collections DirCollect { get { return _window.Info.colect; } }
            private ListViewCalculator.ColumnSetting[] Setting
            {
                get
                {

                    return new ListViewCalculator.ColumnSetting[]
                        {
                        new ListViewCalculator.ColumnSetting()
                        {
                            name = BundleName,
                            width = 100
                        },
                        new ListViewCalculator.ColumnSetting()
                        {
                            name=CollectType,
                            width=80,
                        },
                        new ListViewCalculator.ColumnSetting()
                        {
                            name=SelectButton,
                            width=50,
                            offSetY=-4,
                            offsetX=-10

                        },
                        new ListViewCalculator.ColumnSetting()
                        {
                            name=SearchPath,
                            width=200
                        },
                        };
                }
            }

            public override void OnGUI(Rect position)
            {
                base.OnGUI(position);
                Event e = Event.current;
                ListView(e);
                Eve(e);
            }
            private void ListView(Event e)
            {
                tableViewCalc.Calc(position, new Vector2(position.x, position.y + lineHeight), ScrollPos, lineHeight, DirCollect.collection.Count, Setting);
                if (Event.current.type == EventType.Repaint)
                    GUIStyles.Get(EntryBackodd).Draw(tableViewCalc.position, false, false, false, false);

                bool tog = true;
                tog = EditorGUI.Toggle(tableViewCalc.titleRow.position, tog, GUIStyles.Get(TitleStyle));
                EditorGUI.LabelField(tableViewCalc.titleRow[CollectType].position, CollectType);
                EditorGUI.LabelField(tableViewCalc.titleRow[BundleName].position, BundleName);
                EditorGUI.LabelField(tableViewCalc.titleRow[SearchPath].position, SearchPath);

                ScrollPos = GUI.BeginScrollView(tableViewCalc.view, ScrollPos, tableViewCalc.content, false, false);
                {
                    for (int i = tableViewCalc.firstVisibleRow; i < tableViewCalc.lastVisibleRow + 1; i++)
                    {
                        GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;
                        if (e.type == EventType.Repaint)
                            style.Draw(tableViewCalc.rows[i].position, false, false, tableViewCalc.rows[i].selected, false);
                        if (e.button == 0 && e.clickCount == 1 && e.type == EventType.MouseUp && tableViewCalc.rows[i].position.Contains(e.mousePosition))
                        {
                            switch (e.modifiers)
                            {
                                case EventModifiers.None:
                                    tableViewCalc.SelectRow(i);
                                    e.Use();
                                    break;
                                case EventModifiers.Shift:
                                    tableViewCalc.ShiftSelectRow(i);
                                    e.Use();
                                    break;
                                case EventModifiers.Control:
                                    tableViewCalc.ControlSelectRow(i);
                                    e.Use();
                                    break;
                            }
                        }

                        var item = DirCollect.collection[i];

                        int index = (int)item.type;
                        index = EditorGUI.Popup(tableViewCalc.rows[i][CollectType].position,
                                     index,
                                    Enum.GetNames(typeof(WindowMemory.Collections.Collection.CollectType)));
                        item.type = (WindowMemory.Collections.Collection.CollectType)index;
                        if (GUI.Button(tableViewCalc.rows[i][SelectButton].position, SelectButton))
                        {
                            chosseWindow.assetinfo = item.subAsset;
                            PopupWindow.Show(tableViewCalc.rows[i][SelectButton].position, chosseWindow);
                        }


                        GUI.Label(tableViewCalc.rows[i][SearchPath].position, item.path);
                        if (item.type == WindowMemory.Collections.Collection.CollectType.ABName)
                            item.bundle = EditorGUI.TextField(tableViewCalc.rows[i][BundleName].position, item.bundle);
                    }

                    GUI.EndScrollView();
                }



                Handles.color = Color.black;
                for (int i = 0; i < tableViewCalc.titleRow.columns.Count; i++)
                {
                    var item = tableViewCalc.titleRow.columns[i];

                    if (i != 0)
                        Handles.DrawAAPolyLine(1, new Vector3(item.position.x - 2,
                                                                item.position.y,
                                                                0),
                                                  new Vector3(item.position.x - 2,
                                                                item.position.yMax - 2,
                                                                0));

                }
                tableViewCalc.position.DrawOutLine(2, Color.black);
                Handles.color = Color.white;
            }
            private void Eve(Event e)
            {
                if (e.button == 0 && e.clickCount == 1 && e.type == EventType.MouseUp &&
                        (!tableViewCalc.view.Contains(e.mousePosition) ||
                            (tableViewCalc.view.Contains(e.mousePosition) &&
                             !tableViewCalc.content.Contains(e.mousePosition))))
                {
                    tableViewCalc.SelectNone();
                    e.Use();
                }
                var info = EditorTools.DragAndDropTool.Drag(e, tableViewCalc.view);
                if (info.enterArera && info.compelete)
                {
                    for (int i = 0; i < info.paths.Length; i++)
                    {
                        AddCollectItem(info.paths[i]);
                    }
                }
                if (e.button == 1 && e.clickCount == 1 &&
                          tableViewCalc.content.Contains(e.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () => {
                        for (int i = tableViewCalc.rows.Count - 1; i >= 0; i--)
                        {
                            if (tableViewCalc.rows[i].selected)
                                DirCollect.Remove(DirCollect.collection[i]);
                        }
                        _window.UpdateInfo();
                    });

                    menu.ShowAsContext();
                    if (e.type != EventType.Layout)
                        e.Use();

                }
            }

            private void AddCollectItem(string path)
            {
                if (string.IsNullOrEmpty(path) || !path.Contains("Assets")) return;
                if (!Directory.Exists(path)) return;
                DirCollect.Add(path);
                _window.UpdateInfo();
            }
        }
        private class AssetBundleBulidGUI : GUIBase
        {
            private const string ABBWin = "AssetBundleBuild";
            private const string ABBItemWin = "ABBItemWin";
            private const string ABBContentWin = "ABBContent";
            private const string ABBContentItemWin = "ABBContentItem";
            private const float LineHeight = 20;

            public WindowMemory.BuiidCollection.AssetBundleBuild_Class ChossedABB;
            public WindowMemory.BuiidCollection.Deprndences ChoosedAsset;
            private List<WindowMemory.BuiidCollection.AssetBundleBuild_Class> AssetbundleBuilds { get { return _window.Info.build.abbs; } }

            private const string ABName = "ABName";
            private const string RefCount = "RefCount";
            private const string TitleStyle = "IN BigTitle";
            private const string EntryBackodd = "CN EntryBackodd";
            private const string EntryBackEven = "CN EntryBackEven";
            private ListViewCalculator ABBListViewCalc;
            private Vector2 ABBScrollPos;
            private ListViewCalculator.ColumnSetting[] ABBSetting
            {
                get
                {
                    return new ListViewCalculator.ColumnSetting[]
                    {
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=ABName,
                        width=300
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=RefCount,
                        width=40
                    }
                    };
                }
            }

            private Vector2 ABBContentScrollPos;
            private TableViewCalculator ABBContentTable = new TableViewCalculator();
            private List<WindowMemory.BuiidCollection.Deprndences> dpInfo { get { return _window.Info.build.dps; } }

            private const string Preview = "Preview";
            private const string AssetName = "AssetName";
            private const string Bundle = "Bundle";
            private const string Size = "Size";
            private const string CrossRef = "CrossRef";
            private ListViewCalculator.ColumnSetting[] ABBContentSetting
            {
                get
                {
                    return new ListViewCalculator.ColumnSetting[] {
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=Preview,
                        width=40
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=AssetName,
                        width=320
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=Bundle,
                        width=100
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=Size,
                        width=100
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=CrossRef,
                        width=40
                    },

                };

                }
            }

            private SplitView big = new SplitView();
            private SplitView left = new SplitView() { splitType = SplitType.Horizontal };
            private SplitView right = new SplitView() { splitType = SplitType.Horizontal };

            private ListViewCalculator.ColumnSetting[] ABBContentItemSetting
            {
                get
                {
                    return new ListViewCalculator.ColumnSetting[] {
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=Preview,
                        width=40
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=AssetName,
                        width=400
                    },
                };
                }
            }
            private Vector2 ABBContentItemScrollPos;
            private TableViewCalculator ABBContentItemTableCalc = new TableViewCalculator();


            public AssetBundleBulidGUI()
            {
                big.fistPan += left.OnGUI;
                big.secondPan += right.OnGUI;
                left.fistPan += ABBWinGUI;
                left.secondPan += ABBItemWinGUI;
                right.fistPan += ABBContentWinGUI;
                right.secondPan += ABBContentItemWinGUI;

                ABBListViewCalc = new ListViewCalculator();
            }

            private void ABBWinGUI(Rect rect)
            {
                rect.DrawOutLine(2, Color.black);
                Event e = Event.current;
                ABBListViewCalc.Calc(rect, rect.position, ABBScrollPos, LineHeight, AssetbundleBuilds.Count, ABBSetting);
                ABBScrollPos = GUI.BeginScrollView(ABBListViewCalc.view,
                 ABBScrollPos,
                ABBListViewCalc.content, false, false);
                {
                    for (int i = ABBListViewCalc.firstVisibleRow; i < ABBListViewCalc.lastVisibleRow + 1; i++)
                    {
                        if (e.button == 0 && e.clickCount == 1 && e.type == EventType.MouseUp && ABBListViewCalc.rows[i].position.Contains(e.mousePosition))
                        {
                            switch (e.modifiers)
                            {
                                case EventModifiers.None:
                                    if (rect.Contains(e.mousePosition))
                                    {
                                        ABBListViewCalc.SelectRow(i);
                                        ChossedABB = AssetbundleBuilds[i];
                                        e.Use();
                                    }
                                    break;
                                case EventModifiers.Shift:
                                    if (rect.Contains(e.mousePosition))
                                    {
                                        ABBListViewCalc.ShiftSelectRow(i);
                                        e.Use();
                                    }
                                    break;
                                case EventModifiers.Control:
                                    if (rect.Contains(e.mousePosition))
                                    {
                                        ABBListViewCalc.ControlSelectRow(i);
                                        e.Use();
                                    }
                                    break;

                            }
                        }
                        GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;
                        if (e.type == EventType.Repaint)
                            style.Draw(ABBListViewCalc.rows[i].position, false, false, ABBListViewCalc.rows[i].selected, false);
                        GUI.Label(ABBListViewCalc.rows[i][ABName].position, AssetbundleBuilds[i].assetBundleName);
                        if (AssetbundleBuilds[i].CrossRefence)
                            GUI.Label(ABBListViewCalc.rows[i][RefCount].position, EditorGUIUtility.IconContent("console.warnicon.sml"));
                        else
                            GUI.Label(ABBListViewCalc.rows[i][RefCount].position, EditorGUIUtility.IconContent("Collab"));

                    }

                    GUI.EndScrollView();
                }

                if (e.button == 0 && e.clickCount == 1 && e.type == EventType.MouseUp &&
                            (ABBListViewCalc.view.Contains(e.mousePosition) &&
                             !ABBListViewCalc.content.Contains(e.mousePosition)))
                {
                    ABBListViewCalc.SelectNone();
                    e.Use();
                    ChoosedAsset = null;
                    ChossedABB = null;
                }

                if (e.button == 1 && e.clickCount == 1 &&
                  ABBListViewCalc.content.Contains(e.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () => {

                        ChoosedAsset = null;
                        ChossedABB = null;
                        ABBListViewCalc.selectedRows.ReverseForEach((row) => {

                            int index = row.rowID;
                            _window.DeleteBundle(AssetbundleBuilds[index].assetBundleName);
                        });


                        _window.UpdateInfo();

                    });

                    menu.ShowAsContext();
                    if (e.type != EventType.Layout)
                        e.Use();

                }
            }
            private void ABBItemWinGUI(Rect rect)
            {
                rect.DrawOutLine(2, Color.black);
                if (ChossedABB == null) return;
                GUILayout.BeginArea(rect);
                {
                    GUILayout.Label(ChossedABB.assetBundleName);
                    GUILayout.Label(ChossedABB.Size);
                    if (ChossedABB.CrossRefence)
                        GUILayout.Label(EditorGUIUtility.IconContent("console.warnicon.sml"));
                    else
                        GUILayout.Label(EditorGUIUtility.IconContent("Collab"));
                    GUILayout.EndArea();


                }

            }



            private WindowMemory.BuiidCollection.Deprndences GetDpByName(string AssetPath)
            {
                for (int i = 0; i < dpInfo.Count; i++)
                {
                    if (dpInfo[i].assetPath == AssetPath) return dpInfo[i];
                }
                return default(WindowMemory.BuiidCollection.Deprndences);
            }
            private void ABBContentWinGUI(Rect rect)
            {
                GUI.BeginClip(rect);
                rect = new Rect(Vector2.zero, rect.size);
                rect.DrawOutLine(2, Color.black);
                int lineCount = ChossedABB == null ? 0 : ChossedABB.assetNames.Count;
                ABBContentTable.Calc(rect, new Vector2(rect.x, rect.y + LineHeight), ABBContentScrollPos, LineHeight, lineCount, ABBContentSetting);
                GUI.Label(ABBContentTable.titleRow.position, "", TitleStyle);
                GUI.Label(ABBContentTable.titleRow[AssetName].position, AssetName);
                GUI.Label(ABBContentTable.titleRow[Bundle].position, Bundle);
                GUI.Label(ABBContentTable.titleRow[Size].position, Size);
                Event e = Event.current;
                ABBContentScrollPos = GUI.BeginScrollView(ABBContentTable.view, ABBContentScrollPos, ABBContentTable.content, false, false);
                {

                    for (int i = ABBContentTable.firstVisibleRow; i < ABBContentTable.lastVisibleRow + 1; i++)
                    {
                        var asset = GetDpByName(ChossedABB.assetNames[i]);
                        GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;

                        if (e.type == EventType.Repaint)
                            style.Draw(ABBContentTable.rows[i].position, false, false, ABBContentTable.rows[i].selected, false);

                        GUI.Label(ABBContentTable.rows[i][Size].position, asset.size);
                        GUI.Label(ABBContentTable.rows[i][AssetName].position, asset.assetName);
                        GUI.Label(ABBContentTable.rows[i][Preview].position, asset.thumbnail);
                        GUI.Label(ABBContentTable.rows[i][Bundle].position, asset.bundleName);
                        if (asset.bundles.Count == 1)
                            GUI.Label(ABBContentTable.rows[i][CrossRef].position, EditorGUIUtility.IconContent("Collab"));
                        else
                            GUI.Label(ABBContentTable.rows[i][CrossRef].position, asset.bundles.Count.ToString(), GUIStyles.Get("CN CountBadge"));
                        if (e.button == 0 && e.clickCount == 1 && e.type == EventType.MouseUp &&
                                ABBContentTable.rows[i].position.Contains(e.mousePosition))
                        {
                            switch (e.modifiers)
                            {
                                case EventModifiers.None:
                                    if (rect.Contains(e.mousePosition))
                                    {
                                        ABBContentTable.SelectRow(i);
                                        ChoosedAsset = asset;
                                        e.Use();
                                    }
                                    break;
                                case EventModifiers.Shift:
                                    if (rect.Contains(e.mousePosition))
                                    {
                                        ABBContentTable.ShiftSelectRow(i);
                                        e.Use();
                                    }
                                    break;
                                case EventModifiers.Control:
                                    if (rect.Contains(e.mousePosition))
                                    {
                                        ABBContentTable.ControlSelectRow(i);
                                        e.Use();
                                    }
                                    break;
                            }
                        }
                    }


                    GUI.EndScrollView();
                }


                Handles.color = Color.black;

                for (int i = 0; i < ABBContentTable.titleRow.columns.Count; i++)
                {
                    var item = ABBContentTable.titleRow.columns[i];

                    if (i != 0)
                        Handles.DrawAAPolyLine(1, new Vector3(item.position.x - 2,
                                                                item.position.y,
                                                                0),
                                                  new Vector3(item.position.x - 2,
                                                                item.position.yMax - 2,
                                                                0));

                }
                ABBContentTable.position.DrawOutLine(2, Color.black);

                Handles.color = Color.white;

                if (e.button == 0 && e.clickCount == 1 && e.type == EventType.MouseUp &&
                     (ABBContentTable.view.Contains(e.mousePosition) &&
                      !ABBContentTable.content.Contains(e.mousePosition)))
                {
                    ABBContentTable.SelectNone();
                    ChoosedAsset = null;
                    e.Use();
                }
                if (e.button == 1 && e.clickCount == 1 && ABBContentTable.content.Contains(e.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () => {
                        ABBContentTable.selectedRows.ReverseForEach((row) =>
                        {
                            _window.RemoveAsset(ChossedABB.assetNames[row.rowID], ChossedABB.assetBundleName);
                        });
                        _window.UpdateInfo();
                        ChoosedAsset = null;
                    });

                    menu.ShowAsContext();
                    if (e.type != EventType.Layout)
                        e.Use();
                }
                GUI.EndClip();
            }
            private void ABBContentItemWinGUI(Rect rect)
            {
                rect.DrawOutLine(2, Color.black);
                if (ChoosedAsset == null) return;
                ABBContentItemTableCalc.Calc(rect, new Vector2(rect.x, rect.y + LineHeight), ABBContentItemScrollPos, LineHeight, ChoosedAsset.bundles.Count, ABBContentItemSetting);
                GUI.Label(ABBContentItemTableCalc.titleRow[AssetName].position, ChoosedAsset.assetPath);
                GUI.Label(ABBContentItemTableCalc.titleRow[Preview].position, ChoosedAsset.thumbnail);
                ABBContentItemScrollPos = GUI.BeginScrollView(ABBContentItemTableCalc.view, ABBContentItemScrollPos, ABBContentItemTableCalc.content, false, false);
                {
                    for (int i = 0; i < ABBContentItemTableCalc.rows.Count; i++)
                    {
                        GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;
                        if (Event.current.type == EventType.Repaint)
                            style.Draw(ABBContentItemTableCalc.rows[i].position, false, false, ABBContentItemTableCalc.rows[i].selected, false);
                        GUI.Label(ABBContentItemTableCalc.rows[i][AssetName].position, ChoosedAsset.bundles[i]);
                        //this.Label(table.Rows[i][Preview].Position, choo.ThumbNail);
                    }

                    GUI.EndScrollView();
                }

            }


            public override void OnGUI(Rect position)
            {
                base.OnGUI(position);
                big.OnGUI(position);
            }
        }


        private const float ToolBarHeight = 17;
        enum WindowType
        {
            Tool,
            DirectoryCollect,
            AssetBundleBuild,
        }
        private Rect localPosition { get { return new Rect(Vector2.zero, position.size); } }
        private ToolBarTree ToolBarTree;
        private WindowType _windowType;


        public WindowMemory Info;
        private string EditorInfoPath;
        private AssetBundleBulidGUI abBulidWindow;
        private DirCollectGUI dirCollectWindow;
        private ToolGUI toolWindow;

        private void DeleteBundle(string bundleName)
        {
            Info.build.RemoveBundle(bundleName);
        }
        private void RemoveAsset(string assetPath, string bundleName)
        {
            Info.build.RemoveAsset(bundleName, assetPath);
        }
        private void UpdateInfo()
        {
            File.WriteAllText(EditorInfoPath, Xml.ToXml(Info));
            Info = Xml.FromXml<WindowMemory>(File.ReadAllText(EditorInfoPath));
        }
    }
    partial class AssetBundleWindow
    {
        private void ABInit()
        {
            EditorInfoPath = EditorEnv.memoryPath.CombinePath("AssetBundleEditorInfo.xml");
            if (!File.Exists(EditorInfoPath))
                File.WriteAllText(EditorInfoPath, Xml.ToXml(new WindowMemory()));
            Info = Xml.FromXml<WindowMemory>(File.ReadAllText(EditorInfoPath));


            if (toolWindow == null)
                toolWindow = new ToolGUI();
            if (dirCollectWindow == null)
                dirCollectWindow = new DirCollectGUI();
            if (abBulidWindow == null)
                abBulidWindow = new AssetBundleBulidGUI();
        }
        private List<Collecter> LoadCollecters()
        {
            var collecters = Info.colect.collection.ConvertAll<Collecter>((item) =>
            {
                if (string.IsNullOrEmpty(item.path)) return null;
                switch (item.type)
                {
                    case WindowMemory.Collections.Collection.CollectType.ABName:
                        if (!string.IsNullOrEmpty(item.bundle))
                        {
                            return new ABNameCollecter()
                            {
                                bundleName = item.bundle,
                                searchPath = item.path,
                                MeetFiles = item.GetSubAssetPaths()
                            };
                        }
                        return null;
                    case WindowMemory.Collections.Collection.CollectType.DirName:
                        return new DirNameCollecter()
                        {
                            searchPath = item.path,
                            MeetFiles = item.GetSubAssetPaths()
                        };
                    case WindowMemory.Collections.Collection.CollectType.FileName:
                        return new FileNameCollecter()
                        {
                            searchPath = item.path,
                            MeetFiles = item.GetSubAssetPaths()
                        };
                    case WindowMemory.Collections.Collection.CollectType.Scene:
                        return new ScenesCollecter()
                        {
                            searchPath = item.path,
                            MeetFiles = item.GetSubAssetPaths()
                        };
                    default:
                        return null;
                }

            });
            collecters = collecters.ReverseForEach((col) =>
            {
                if (col == null)
                    collecters.Remove(col);
            });
            return collecters;
        }
        private static AssetBundleWindow _window;
        private void OnEnable()
        {
            _window = this;
            Collect.onLoadBuilders -= LoadCollecters;
            Collect.onLoadBuilders += LoadCollecters;
            ABInit();
            ToolBarTree = new ToolBarTree();
            ToolBarTree.Popup((value) => { _windowType = (WindowType)value; }, Enum.GetNames(typeof(WindowType)))
                            .Button(EditorGUIUtility.IconContent("Refresh"), (rect) =>
                            {
                                abBulidWindow.ChoosedAsset = null;
                                abBulidWindow.ChossedABB = null;
                                var abbs = Collect.GetCollection(ABTool.configPath);
                                Info.build.ReadAssetbundleBuild(abbs);
                                UpdateInfo();
                            }, 20, () => { return _windowType == WindowType.AssetBundleBuild; })
                            //.Button(new GUIContent(), (rect) =>
                            //{
                            //    File.WriteAllText(tmpLayout_ABBuildInfoPath, abBulidWindow.abBuildInfoTree.Serialize());
                            //})
                            ;
        }
        private void OnDisable()
        {
            UpdateInfo();
        }


        private void OnGUI()
        {
            var rs = localPosition.Zoom(AnchorType.MiddleCenter, -2).HorizontalSplit(ToolBarHeight, 4);
            switch (_windowType)
            {
                case WindowType.Tool:
                    toolWindow.OnGUI(rs[1]);
                    break;
                case WindowType.DirectoryCollect:
                    dirCollectWindow.OnGUI(rs[1]);
                    break;
                case WindowType.AssetBundleBuild:
                    abBulidWindow.OnGUI(rs[1]);
                    break;
                default:
                    break;
            }
            ToolBarTree.OnGUI(rs[0]);
        }
    }
}
