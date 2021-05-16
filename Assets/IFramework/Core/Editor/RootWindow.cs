/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.51
 *UnityVersion:   2018.4.24f1
 *Date:           2020-09-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using IFramework.GUITool;
using System;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Collections.Generic;
using IFramework.GUITool.ToorbarMenu;
using System.Linq;
using UnityEditor.IMGUI.Controls;
#pragma warning disable
namespace IFramework
{
    partial class RootWindow : EditorWindow
    {
        public static class PkgKitTool
        {
            public static class Constant
            {
                public enum ErrorCode
                {
                    Sucess = 200,
                    Email = 201,
                    Password = 202,
                    UserName = 203,
                    AuthenticationCode = 204,
                    Token = 205,
                    Author = 206,
                    Version = 207,
                    Package = 208,
                    Exception = 209,
                    Field = 210,
                }
                public abstract class ResponseModel
                {
                    public ErrorCode code;
                    public string err;

                    internal static T Dispose<T>(string text) where T : ResponseModel
                    {
                        try
                        {
                            //Debug.Log(text);
                            T t = JsonUtility.FromJson<T>(text);
                            return t;
                        }
                        catch (Exception)
                        {
                            _window.ShowNotification(new GUIContent("数据解析错误"));
                            return null;
                        }
                    }

                    internal bool CheckCode()
                    {
                        if (code != ErrorCode.Sucess)
                        {
                            _window.ShowNotification(new GUIContent(code.ToString() + err));
                        }
                        return code == ErrorCode.Sucess;
                    }
                }
                public abstract class TokenModel : ResponseModel
                {
                    public string token;
                }

                public class LoginModel : TokenModel
                {
                    public string name;
                }

                public class PutPackageModel : ResponseModel { }
                [Serializable]
                public class PackageListModel : ResponseModel
                {
                    [Serializable]
                    public class PkgShort
                    {
                        public string name;
                        public string author;
                        public string versions;
                        public DateTime time;
                    }
                    public PkgShort[] pkgs = new PkgShort[0];
                }
                [Serializable]
                public class PackageInfosModel : ResponseModel
                {
                    public string name;
                    public string author;

                    public List<VersionInfo> versions = new List<VersionInfo>();
                    [Serializable]
                    public class VersionInfo
                    {
                        public bool preview;
                        public string version;
                        public string describtion;
                        public string dependences;
                        public string assetPath;
                    }

                }
                public class DeletePackageModel : ResponseModel { }

                public const string host = "https://www.aicxz.com/api/v1";

                public const string getSignupCode = host + "/User/GetSinupRandomCode";
                public const string login = host + "/User/Login";
                public const string loginWithToken = host + "/User/LoginWithToken";
                public const string putpackage = host + "/Pkg/PutPkg";
                public const string getpackageList = host + "/Pkg/GetPkglist";
                public const string getpackageInfos = host + "/Pkg/GetPkgInfos";
                public const string getpkgurl = host + "/Pkg/DownLoadPkg";
                public const string deletepackage = host + "/Pkg/DeletePkg";
            }
            [System.Serializable]
            public class LocalPkgVersions
            {
                [System.Serializable]

                public class PkgVersion
                {
                    public string name;
                    public string version;
                }
                public List<PkgVersion> versions = new List<PkgVersion>();
            }
            static class WebRequest
            {
                private abstract class Request
                {
                    public readonly string url;
                    private readonly Action<UnityWebRequest> _callback;
                    protected UnityWebRequest request;
                    public float progress { get { return request.downloadProgress; } }
                    public bool isDone { get { return request.isDone; } }
                    public string error { get { return request.error; } }
                    protected Request(string url, Action<UnityWebRequest> callback)
                    {
                        this.url = url;
                        this._callback = callback;
                    }

                    public void Start()
                    {
                        request.SendWebRequest();
                    }

                    public void Compelete()
                    {
                        EditorUtility.ClearProgressBar();
                        if (!string.IsNullOrEmpty(request.error))
                        {
                            _window.ShowNotification(new GUIContent(request.error));
                            return;
                        }
                        if (_callback != null)
                        {
                            _callback.Invoke(request);
                        }
                        request.Abort();
                        request.Dispose();
                    }
                    public static long GetTimeStamp(bool bflag = true)
                    {
                        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        long ret;
                        if (bflag)
                            ret = Convert.ToInt64(ts.TotalSeconds);
                        else
                            ret = Convert.ToInt64(ts.TotalMilliseconds);
                        return ret;
                    }
                }
                private class Request_Get : Request
                {
                    public Request_Get(string url, Action<UnityWebRequest> callback, Dictionary<string, object> forms) : base(url, callback)
                    {
                        string newUrl = url;
                        if (forms != null && forms.Count > 0)
                        {
                            newUrl += "?";
                            foreach (var item in forms)
                            {
                                newUrl += string.Format("{0}={1}", item.Key, item.Value) + "&";
                            }
                            newUrl += GetTimeStamp();
                        }
                        else
                        {
                            newUrl += "?" + GetTimeStamp();
                        }
                        request = UnityWebRequest.Get(newUrl);
                    }
                }
                private class Request_Post : Request
                {
                    public Request_Post(string url, Action<UnityWebRequest> callback, WWWForm forms) : base(url, callback)
                    {
                        if (forms == null)
                        {
                            forms = new WWWForm();
                        }
                        url += "?" + GetTimeStamp();
                        request = UnityWebRequest.Post(url, forms);
                    }
                }
                private const int maxRequest = 20;
                private static Queue<Request> _waitRequests;
                private static List<Request> _requests;
                private static void Update()
                {
                    if (_waitRequests.Count <= 0 && _requests.Count <= 0) return;
                    while (_requests.Count < maxRequest && _waitRequests.Count > 0)
                    {
                        var _req = _waitRequests.Dequeue();
                        _req.Start();
                        _requests.Add(_req);
                    }

                    for (int i = _requests.Count - 1; i >= 0; i--)
                    {
                        var _req = _requests[i];
                        if (_req.isDone)
                        {
                            _req.Compelete();
                            _requests.Remove(_req);
                            //  break;
                        }
                        else
                        {
                            EditorUtility.DisplayProgressBar("Post Request", _req.url, _req.progress);
                        }
                    }
                }
                private static void Run(Request request)
                {
                    if (_waitRequests == null)
                    {
                        _waitRequests = new Queue<Request>();
                        _requests = new List<Request>();
                        EditorEnv.env.BindUpdate(Update);
                    }
                    _waitRequests.Enqueue(request);
                }



                public static void GetRequest(string url, Dictionary<string, object> forms, Action<UnityWebRequest> callback)
                {
                    Run(new Request_Get(url, callback, forms));
                }
                public static void PostRequest(string url, WWWForm forms, Action<UnityWebRequest> callback)
                {
                    Run(new Request_Post(url, callback, forms));
                }
                private static void GetRequest<T>(string url, Dictionary<string, object> forms, Action<T> callback) where T : Constant.ResponseModel
                {
                    GetRequest(url, forms, (req) =>
                    {
                        T t = Constant.ResponseModel.Dispose<T>(req.downloadHandler.text);
                        if (t == null) return;
                        bool bo = t.CheckCode();
                        if (callback != null) callback.Invoke(t);
                    });
                }
                private static void PostRequest<T>(string url, WWWForm forms, Action<T> callback) where T : Constant.ResponseModel
                {
                    PostRequest(url, forms, (req) =>
                    {
                        T t = Constant.ResponseModel.Dispose<T>(req.downloadHandler.text);
                        if (t == null) return;
                        bool bo = t.CheckCode();
                        if (callback != null) callback.Invoke(t);
                    });
                }



                public static void Login(string email, string password, Action<Constant.LoginModel> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("email", email);
                    www.AddField("password", password);
                    PostRequest<Constant.LoginModel>(Constant.login, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }

                public static void Login(string token, Action<Constant.LoginModel> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("token", token);
                    PostRequest<Constant.LoginModel>(Constant.loginWithToken, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }




                public static void PutPackage(string name, string author, string version, string describtion, bool preview, string dependences, string assetpath, byte[] buffer, Action<Constant.PutPackageModel> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("name", name);
                    www.AddField("author", author);
                    www.AddField("version", version);
                    www.AddField("describtion", describtion);
                    www.AddField("assetpath", assetpath);
                    www.AddField("preview", preview.ToString());
                    www.AddField("dependences", dependences);
                    www.AddBinaryData("buffer", buffer);
                    PostRequest<Constant.PutPackageModel>(Constant.putpackage, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }


                public static void GetPackageList(Action<Constant.PackageListModel> callback)
                {
                    PostRequest<Constant.PackageListModel>(Constant.getpackageList, null, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }

                public static void GetPkgInfos(string name, Action<Constant.PackageInfosModel> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("name", name);
                    PostRequest<Constant.PackageInfosModel>(Constant.getpackageInfos, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }
                public static void DownLoadPkg(string name, string version, Action<UnityWebRequest> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("name", name);
                    www.AddField("version", version);
                    PostRequest(Constant.getpkgurl, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }
                public static void DeletePkg(string author, string name, string version, Action<Constant.DeletePackageModel> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("author", author);
                    www.AddField("name", name);
                    www.AddField("version", version);
                    PostRequest<Constant.DeletePackageModel>(Constant.deletepackage, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }
            }


            public class LoginInfo
            {
                public string email;

                public string password;
            }

            public class UploadInfo
            {
                public string name;
                public string author { get { return userjson.name; } }
                public int[] version = new int[4];
                public string assetPath;
                public string describtion = "No Describtion";
                public bool preview;
                public string[] dependences = new string[0];
                public byte[] buffer;
            }

            private static class Paths
            {
                public static string rootPath { get { return EditorEnv.memoryPath; } }

                public static string userjsonPath { get { return rootPath + "/user.json"; } }
                public static string localVersionsPath { get { return rootPath + "/localVersions.json"; } }
                public static string pkgjsonPath { get { return rootPath + "/pkgs.json"; } }
                public static string pkgversionjsonPath
                {
                    get
                    {
                        string path = rootPath + "/pkgversion";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        return path;
                    }
                }
                public static string pkgPath
                {
                    get
                    {
                        string path = rootPath + "/pkgs";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        return path;
                    }
                }
                public static string localPkgPath
                {
                    get
                    {
                        string path = rootPath + "/localpkgs";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        return path;
                    }
                }
            }
            public static event Action onFreshpkgs;

            private static LocalPkgVersions _versions;
            private static LocalPkgVersions localversion
            {
                get
                {

                    if (_versions == null)
                    {
                        if (!File.Exists(Paths.localVersionsPath))
                        {
                            File.WriteAllText(Paths.localVersionsPath, JsonUtility.ToJson(new LocalPkgVersions(), true));
                        }
                        _versions = JsonUtility.FromJson<LocalPkgVersions>(File.ReadAllText(Paths.localVersionsPath));
                    }
                    return _versions;
                }
            }
            private static void UpdateLocalVersion(string name, string version)
            {
                var versions = localversion;
                var tmp = versions.versions.Find((o) => { return o.name == name; });
                if (tmp == null)
                {
                    versions.versions.Add(new LocalPkgVersions.PkgVersion() { name = name, version = version });
                }
                else
                {
                    tmp.version = version;
                }
                File.WriteAllText(Paths.localVersionsPath, JsonUtility.ToJson(versions, true));
            }
            public static string GetLocalVersion(string name)
            {
                var versions = localversion;
                var tmp = versions.versions.Find((o) => { return o.name == name; });
                if (tmp == null) return string.Empty;
                return tmp.version;
            }


            public static PkgkitInfo.UserJson userjson { get { return _window._windowInfo.userJson; } set { _window._windowInfo.userJson = value; } }
            public static List<PkgKitTool.Constant.PackageInfosModel> pkgs { get { return _window._windowInfo.pkgInfos; } }
            public static bool login { get { return _window._windowInfo.login; } }


            public static void OpenMemory()
            {
                EditorTools.OpenFloder(Paths.rootPath);
            }

            public static void ClearMemory()
            {
                Logout();
                Directory.Delete(Paths.rootPath, true);
            }
            public static void Logout()
            {
                ClearUserJson();
            }
            private static void ClearUserJson()
            {
                userjson = new PkgkitInfo.UserJson();
                if (File.Exists(Paths.userjsonPath))
                {
                    File.Delete(Paths.userjsonPath);
                }
            }
            private static void WriteUserJson(string name, string token)
            {
                userjson = new PkgkitInfo.UserJson()
                {
                    name = name,
                    token = token
                };
                File.WriteAllText(Paths.userjsonPath, JsonUtility.ToJson(userjson, true));
            }

            public static void Init()
            {
               // if (login) return;
                if (File.Exists(Paths.userjsonPath))
                {
                    userjson = JsonUtility.FromJson<PkgkitInfo.UserJson>(File.ReadAllText(Paths.userjsonPath));
                    LoginWithToken();
                }
                //FreshWebPackages();

            }




            private static void LoginWithToken()
            {
                WebRequest.Login(userjson.token, (m) =>
                {
                    if (m.code != Constant.ErrorCode.Sucess)
                    {
                        Logout();
                        return;
                    }
                    else
                    {
                        WriteUserJson(m.name, m.token);
                    }
                    FreshWebPackages();
                });
            }
            public static void FreshWebPackages()
            {
                _window._windowInfo.pkgInfos.Clear();
                Action<Constant.PackageInfosModel, int> freshWindowInfo = (info, max) => {

                    _window._windowInfo.pkgInfos.Add(info);
                    if (max <= _window._windowInfo.pkgInfos.Count)
                    {
                        if (onFreshpkgs != null)
                        {
                            onFreshpkgs();
                        }
                    }
                };
                WebRequest.GetPackageList((m) =>
                {
                    if (m.code != Constant.ErrorCode.Sucess) return;

                    Constant.PackageListModel local = new Constant.PackageListModel();
                    if (File.Exists(Paths.pkgjsonPath))
                    {
                        local = JsonUtility.FromJson<Constant.PackageListModel>(File.ReadAllText(Paths.pkgjsonPath));
                    }
                    File.WriteAllText(Paths.pkgjsonPath, JsonUtility.ToJson(m));
                    var localPkgs = local.pkgs.ToList();
                    if (m.pkgs.Length == 0)
                    {
                        if (onFreshpkgs != null)
                        {
                            onFreshpkgs();
                        }
                    }
                    for (int i = 0; i < m.pkgs.Length; i++)
                    {
                        var p = m.pkgs[i];
                        var tmp = localPkgs.Find((_p) => { return p.versions==_p.versions && _p.name == p.name && _p.time == p.time && p.author==_p.author; });
                        Debug.Log($"{p.author}  {tmp == null}");
                        localPkgs.Remove(tmp);
                        string path = Path.Combine(Paths.pkgversionjsonPath, p.name + ".json");
                        if (tmp !=null)
                        {
                            if (File.Exists(path))
                            {
                                freshWindowInfo(JsonUtility.FromJson<Constant.PackageInfosModel>(File.ReadAllText(path)), m.pkgs.Length);
                            }
                            else
                            {
                                WebRequest.GetPkgInfos(p.name, (model) =>
                                {
                                    freshWindowInfo(model, m.pkgs.Length);
                                    File.WriteAllText(path, JsonUtility.ToJson(model, true));
                                });
                            }
                        }
                        else
                        {
                            WebRequest.GetPkgInfos(p.name, (model) =>
                            {
                                freshWindowInfo(model, m.pkgs.Length);
                                File.WriteAllText(path, JsonUtility.ToJson(model, true));
                            });
                        }
                    }
                    localPkgs.ForEach((_p) =>
                    {
                        string path = Path.Combine(Paths.pkgversionjsonPath, _p.name + ".json");
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    });
                });
            }

            public static void Login(LoginInfo info)
            {
                WebRequest.Login(info.email, info.password, (m) =>
                {
                    if (m.code != Constant.ErrorCode.Sucess)
                    {
                        Logout();
                        return;
                    }
                    else
                    {
                        WriteUserJson(m.name, m.token);
                    }
                    FreshWebPackages();
                });
            }



            public static void UploadPkg(UploadInfo info)
            {
                string file = string.Format("{0}_{1}.unitypackage", info.name, string.Join(".", info.version));
                string path = Path.Combine(Paths.localPkgPath, file);
                AssetDatabase.ExportPackage(info.assetPath, path, ExportPackageOptions.Recurse);
                info.buffer = File.ReadAllBytes(path);
                WebRequest.PutPackage(info.name,
                    info.author,
                    string.Join(".", info.version),
                    info.describtion, info.preview,
                    string.Join("@", info.dependences),
                    info.assetPath, info.buffer,
                    (m) => {
                        if (m.code != Constant.ErrorCode.Sucess) return;

                        _window.ShowNotification(new GUIContent("上传成功"));
                        FreshWebPackages();
                    });
            }

            public static void DownLoadPkg(string name, string version)
            {
                WebRequest.DownLoadPkg(name, version, (req) =>
                {
                    string path = Path.Combine(Paths.pkgPath, string.Format("{0}_{1}.unitypackage", name, version));
                    File.WriteAllBytes(path, req.downloadHandler.data);
                    AssetDatabase.ImportPackage(path, true);
                    AssetDatabase.Refresh();
                    UpdateLocalVersion(name, version);
                });
            }





            public static void DeletePkg(string name, string version)
            {
                if (!login) return;
                if (EditorUtility.DisplayDialog("Make Sure", string.Format("Confirm to delete the pkg in sever \nName:   {0}\nVersion:   {1}", name, version), "Yes", "Cancel"))
                {
                    WebRequest.DeletePkg(userjson.name, name, version, (m) =>
                    {
                        if (m.code != Constant.ErrorCode.Sucess) return;

                        _window.ShowNotification(new GUIContent("删除成功"));
                        FreshWebPackages();
                    });
                }

            }
            public static void RemoveLocalPkg(string name, string assetPath)
            {
                Directory.Delete(assetPath, true);
                AssetDatabase.Refresh();
                UpdateLocalVersion(name, "");
            }

        }

        class UserOptionWindow
        {
            enum UserOperation
            {
                Account, Pkg_Upload, Other
            }

            class SystemGUI
            {
                public void OnGUI()
                {
                    GUILayout.Label("System Information");

                    GUILayout.Label("操作系统：" + SystemInfo.operatingSystem);
                    GUILayout.Label("系统内存：" + SystemInfo.systemMemorySize + "MB");
                    GUILayout.Label("处理器：" + SystemInfo.processorType);
                    GUILayout.Label("处理器数量：" + SystemInfo.processorCount);
                    GUILayout.Label("显卡：" + SystemInfo.graphicsDeviceName);
                    GUILayout.Label("显卡类型：" + SystemInfo.graphicsDeviceType);
                    GUILayout.Label("显存：" + SystemInfo.graphicsMemorySize + "MB");
                    GUILayout.Label("显卡标识：" + SystemInfo.graphicsDeviceID);
                    GUILayout.Label("显卡供应商：" + SystemInfo.graphicsDeviceVendor);
                    GUILayout.Label("显卡供应商标识码：" + SystemInfo.graphicsDeviceVendorID);
                    GUILayout.Label("设备模式：" + SystemInfo.deviceModel);
                    GUILayout.Label("设备名称：" + SystemInfo.deviceName);
                    GUILayout.Label("设备类型：" + SystemInfo.deviceType);
                    GUILayout.Label("设备标识：" + SystemInfo.deviceUniqueIdentifier);
                    GUILayout.Space(10);
                    GUILayout.Label("Screen Information");

                    GUILayout.BeginVertical("Box");
                    GUILayout.Label("DPI：" + Screen.dpi);
                    GUILayout.Label("分辨率：" + Screen.currentResolution.ToString());
                    GUILayout.EndVertical();
                    if (GUILayout.Button("Open Memory Floder"))
                    {
                        PkgKitTool.OpenMemory();
                    }
                    if (GUILayout.Button("Clear Memory Floder"))
                    {
                        PkgKitTool.ClearMemory();
                    }
                }
            }

            class LoginGUI
            {
                private class SelectTree : TreeView
                {
                    class Temp
                    {
                        public string name;
                        public string version;
                    }
                    public SelectTree(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
                    {
                        this.rowHeight = 20;
                        showAlternatingRowBackgrounds = true;

                    }
                    public void Fresh()
                    {
                        var pkgs = PkgKitTool.pkgs.FindAll((p) => { return p.author == PkgKitTool.userjson.name; });
                        tmps.Clear();
                        pkgs.ForEach((p) => {
                            for (int i = 0; i < p.versions.Count; i++)
                            {
                                tmps.Add(new Temp()
                                {
                                    name = p.name,
                                    version = p.versions[i].version
                                });
                            }
                        });
                        Reload();
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

                            list.Add(new TreeViewItem() { depth = 1, id = i, displayName = i.ToString() });
                        }
                        return list;
                    }
                    List<Temp> tmps = new List<Temp>();

                    protected override void RowGUI(RowGUIArgs args)
                    {
                        var info = tmps[args.item.id];

                        for (int i = 0; i < args.GetNumVisibleColumns(); i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    if (GUI.Button(args.GetCellRect(i), "", Styles.minus))
                                    {
                                        PkgKitTool.DeletePkg(info.name, info.version);
                                    }
                                    break;
                                case 1:
                                    GUI.Label(args.GetCellRect(i), info.name);
                                    break;
                                case 2:
                                    GUI.Label(args.GetCellRect(i), info.version);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                private PkgKitTool.LoginInfo _login = new PkgKitTool.LoginInfo();
                private SelectTree _tree;
                public LoginGUI()
                {
                    TreeViewState _state = new TreeViewState();
                    _tree = new SelectTree(_state, new MultiColumnHeader(new MultiColumnHeaderState(new MultiColumnHeaderState.Column[] {
                        new MultiColumnHeaderState.Column(){
                            width=20,
                            maxWidth=20,
                            minWidth=20,
                            autoResize =false
                        },
                        new MultiColumnHeaderState.Column()
                        {
                           headerContent=new GUIContent("name"),
                           width=200
                        },
                        new MultiColumnHeaderState.Column(){
                             headerContent=new GUIContent("version"),
                             width=200
                        }
                    })));
                }
                public void OnGUI(Rect position)
                {
                    if (PkgKitTool.login)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(Contents.accop, GUILayout.Width(Contents.gap * 20)))
                        {
                            Application.OpenURL("https://www.aicxz.com/User");
                        }
                        EditorGUILayout.Space();
                        GUILayout.Label(PkgKitTool.userjson.name);
                        if (GUILayout.Button(Contents.logout, GUILayout.Width(Contents.gap * 6)))
                        {
                            PkgKitTool.Logout();
                        }
                        EditorGUILayout.EndHorizontal();
                        var rect = GUILayoutUtility.GetLastRect();
                        position.position = new Vector2(0, 0);
                        rect = position.HorizontalSplit(rect.height)[1];
                        _tree.Fresh();
                        _tree.OnGUI(rect);
                    }
                    else
                    {
                        _login.email = EditorGUILayout.TextField("Email", _login.email);
                        _login.password = EditorGUILayout.TextField("Password", _login.password);
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(Contents.accop, GUILayout.Width(Contents.gap * 20)))
                        {
                            Application.OpenURL("https://www.aicxz.com/User");
                        }
                        EditorGUILayout.Space();
                        if (GUILayout.Button(Contents.go, GUILayout.Width(Contents.gap * 5)))
                        {
                            PkgKitTool.Login(_login);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                }
            }

            class UploadGUI
            {
                private PkgKitTool.UploadInfo _upload = new PkgKitTool.UploadInfo();
                private Vector2 scroll;
                public void OnGUI()
                {
                    scroll = GUILayout.BeginScrollView(scroll);
                    EditorGUILayout.LabelField("Author", PkgKitTool.userjson.name);
                    _upload.name = EditorGUILayout.TextField("Name", _upload.name);
                    _upload.preview = EditorGUILayout.Toggle("Preview", _upload.preview);
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Version");
                        GUILayout.Space(100);
                        for (int i = 0; i < _upload.version.Length; i++)
                        {
                            _upload.version[i] = EditorGUILayout.IntField(_upload.version[i]);
                            if (i < _upload.version.Length - 1)
                            {
                                GUILayout.Label(".", GUILayout.Width(Contents.gap));
                            }
                        }
                        GUILayout.EndHorizontal();
                    }

                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.TextField("AssetPath", _upload.assetPath);
                        if (GUILayout.Button(Contents.select, GUILayout.Width(Contents.gap * 5)))
                        {
                            var str = EditorUtility.OpenFolderPanel("AssetPath", Application.dataPath, "");
                            if (str.Contains("Assets") && IO.IsDirectory(str))
                            {
                                _upload.assetPath = str.ToAssetsPath();
                                _upload.name = Path.GetFileNameWithoutExtension(_upload.assetPath);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.Label("Dependences", Styles.boldLabel);
                    for (int i = 0; i < _upload.dependences.Length; i++)
                    {
                        GUILayout.BeginHorizontal();
                        _upload.dependences[i] = EditorGUILayout.TextField(_upload.dependences[i]);
                        if (GUILayout.Button("", Styles.minus, GUILayout.Width(18)))
                        {
                            ArrayUtility.RemoveAt(ref _upload.dependences, i);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("", Styles.plus))
                    {
                        ArrayUtility.Add(ref _upload.dependences, "");
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label("Describtion", Styles.boldLabel);
                    _upload.describtion = EditorGUILayout.TextArea(_upload.describtion, GUILayout.MinHeight(Contents.gap * 10));

                    {
                        GUILayout.BeginHorizontal();
                        using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(GUIUtility.systemCopyBuffer)))
                        {
                            if (GUILayout.Button("Paste", GUILayout.Width(Contents.gap * 5)))
                            {
                                try
                                {
                                    var p = JsonUtility.FromJson<PkgKitTool.Constant.PackageInfosModel>(GUIUtility.systemCopyBuffer);
                                    _upload.describtion = p.versions.Last().describtion;
                                    var dps = p.versions.Last().dependences.Split('@');
                                    _upload.dependences = dps;
                                    _upload.name = p.name;
                                    _upload.preview = p.versions.Last().preview;
                                    _upload.assetPath = p.versions.Last().assetPath;
                                    var vs = p.versions.Last().version.Split('.');
                                    for (int i = 0; i < _upload.version.Length; i++)
                                    {
                                        _upload.version[i] = int.Parse(vs[i]);
                                        if (i == _upload.version.Length - 1)
                                        {
                                            _upload.version[i]++;
                                        }
                                    }
                                    for (int i = _upload.version.Length - 1; i > 0; i--)
                                    {
                                        if (_upload.version[i] >= 100)
                                        {
                                            _upload.version[i] = 1;
                                            _upload.version[i - 1]++;
                                        }
                                    }
                                    GUI.FocusControl("");
                                }
                                catch (Exception) { }

                            }
                        }

                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(Contents.go, GUILayout.Width(Contents.gap * 5)))
                        {
                            PkgKitTool.UploadPkg(_upload);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndScrollView();
                }
            }



            public UserOptionWindow()
            {
                _split.fistPan += menu.OnGUI;
                _split.secondPan += ContentGUI;
                menu.ReadTree(Enum.GetNames(typeof(UserOperation)).ToList());
                menu.onCurrentChange += (obj) => {
                    _userOperation = (UserOperation)Enum.Parse(typeof(UserOperation), obj);
                };
            }

            private void ContentGUI(Rect position)
            {
                GUILayout.BeginArea(position);
                switch (_userOperation)
                {

                    case UserOperation.Account:
                        _login.OnGUI(position);
                        break;
                    case UserOperation.Pkg_Upload:
                        GUI.enabled = PkgKitTool.login;
                        _upload.OnGUI();
                        GUI.enabled = true;
                        break;
                    case UserOperation.Other:
                        _sys.OnGUI();
                        break;
                    default:
                        break;
                }
                GUILayout.EndArea();
            }



            private UserOperation _userOperation;
            private MenuTree menu = new MenuTree();
            private SplitView _split = new SplitView();
            private LoginGUI _login = new LoginGUI();
            private UploadGUI _upload = new UploadGUI();
            private SystemGUI _sys = new SystemGUI();
            public void OnGUI(Rect position)
            {
                menu.Fitter(_window.search);
                _split.OnGUI(position);
            }
        }

        class PkgsWindow
        {
            public PkgsWindow()
            {
                _split.fistPan += menu.OnGUI;
                _split.secondPan += ContentGUI;
                menu.onCurrentChange += (obj) => {
                    name = obj;
                    index = 0;
                    p = _window._windowInfo.pkgInfos.Find((_p) => { return _p.name == name; });
                    versions = p.versions.ToList().ConvertAll((v) => { return "v" + v.version; }).ToArray();
                };

                PkgKitTool.onFreshpkgs += () => {

                    menu.Clear();
                    var list = _window._windowInfo.pkgInfos.ConvertAll((p) => { return p.name; });
                    menu.ReadTree(list);
                    p = null;
                };


                {
                    menu.Clear();
                    var list = _window._windowInfo.pkgInfos.ConvertAll((p) => { return p.name; });
                    menu.ReadTree(list);
                    p = null;
                }
            }
            private PkgKitTool.Constant.PackageInfosModel p;
            string name;
            string[] versions;
            int index;
            private void ContentGUI(Rect position)
            {
                if (p == null) return;
                var version = p.versions[index];
                GUILayout.BeginArea(position.Zoom(AnchorType.MiddleCenter, -10));


                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(p.name, Styles.header);
                    if (version.preview)
                    {
                        GUILayout.Label("preview");
                    }
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button(Contents.newest, GUILayout.Width(Contents.gap * 6)))
                    {
                        PkgKitTool.DownLoadPkg(name, p.versions.Last().version);
                    }
                    if (GUILayout.Button(Contents.install, GUILayout.Width(Contents.gap * 6)))
                    {
                        PkgKitTool.DownLoadPkg(name, p.versions[index].version);
                    }
                    index = EditorGUILayout.Popup(index, versions, GUILayout.Width(Contents.gap * 6));
                    if (Directory.Exists(version.assetPath))
                    {
                        if (GUILayout.Button(Contents.remove, GUILayout.Width(Contents.gap * 6)))
                        {
                            PkgKitTool.RemoveLocalPkg(p.name, version.assetPath);
                        }
                    }
                    if (PkgKitTool.login && PkgKitTool.userjson.name == p.author)
                    {
                        if (GUILayout.Button(Contents.delete, GUILayout.Width(Contents.gap * 6)))
                        {
                            PkgKitTool.DeletePkg(p.name, version.version);
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.Label("Author: " + p.author, Styles.boldLabel);
                GUILayout.Label("Local Version: " + PkgKitTool.GetLocalVersion(p.name));

                GUILayout.Space(Contents.gap / 2);
                GUILayout.Label("Dependences", Styles.boldLabel);
                var strs = version.dependences.Split('@');
                for (int i = 0; i < strs.Length; i++)
                {
                    GUILayout.Label(strs[i]);
                }
                GUILayout.Space(Contents.gap / 2);
                GUILayout.Label("Describtion", Styles.boldLabel);
                GUILayout.Label(version.describtion);
                GUILayout.EndArea();

                Event e = Event.current;
                if (position.Contains(e.mousePosition) && e.button == 1)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Copy/Name"), false, () => { GUIUtility.systemCopyBuffer = p.name; });
                    menu.AddItem(new GUIContent("Copy/Author"), false, () => { GUIUtility.systemCopyBuffer = p.author; });
                    menu.AddItem(new GUIContent("Copy/AssetPath"), false, () => { GUIUtility.systemCopyBuffer = p.versions[index].assetPath; });
                    menu.AddItem(new GUIContent("Copy/Dependences"), false, () => { GUIUtility.systemCopyBuffer = p.versions[index].dependences; });
                    menu.AddItem(new GUIContent("Copy/Describtion"), false, () => { GUIUtility.systemCopyBuffer = p.versions[index].describtion; });
                    menu.AddItem(new GUIContent("Copy/All"), false, () => { GUIUtility.systemCopyBuffer = JsonUtility.ToJson(p, true); });

                    menu.ShowAsContext();
                    if (e.type != EventType.Layout && e.type != EventType.Repaint)
                    {
                        e.Use();
                    }
                }
            }

            private MenuTree menu = new MenuTree();
            private SplitView _split = new SplitView();
            public void OnGUI(Rect position)
            {
                menu.Fitter(_window.search);
                _split.OnGUI(position);
            }
        }

        class WindowCollection
        {
            private class SelectTree : TreeView
            {
                private struct Index
                {
                    public int id;
                    public EditorWindowTool.Entity value;
                }
                private List<Index> _show;
                public SelectTree(TreeViewState state) : base(state)
                {
                    EditorWindowTool.windows.FindAll((w) => { return w.searchName.ToLower().Contains(_window.search); }).ToArray();
                    _show = new List<Index>();
                    EditorWindowTool.windows
                        .ForEach((entity) =>
                        {
                            _show.Add(new Index() { value = entity, id = EditorWindowTool.windows.IndexOf(entity) });
                        });
                    showAlternatingRowBackgrounds = true;

                    Reload();
                }
                protected override void DoubleClickedItem(int id)
                {
                    var w = EditorWindowTool.FindOrCreate(_show.Find((index) => { return index.id == id; }).value.searchName);
                    if (w != null)
                    {
                        w.Focus();
                    }
                }
                protected override TreeViewItem BuildRoot()
                {
                    var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };
                    return root;
                }
                protected override void SearchChanged(string newSearch)
                {
                    _show.Clear();
                    EditorWindowTool.windows.FindAll((w) => { return w.searchName.ToLower().Contains(_window.search); })
                        .ForEach((entity) =>
                        {
                            _show.Add(new Index() { value = entity, id = EditorWindowTool.windows.IndexOf(entity) });

                        });
                    Reload();
                }
                protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
                {
                    List<TreeViewItem> list = new List<TreeViewItem>();
                    for (int i = 0; i < _show.Count; i++)
                    {
                        list.Add(new TreeViewItem() { depth = 1, id = _show[i].id, displayName = _show[i].value.searchName });
                    }
                    return list;
                }
                protected override void RowGUI(RowGUIArgs args)
                {
                    var window = EditorWindowTool.windows[args.item.id];
                    if (!string.IsNullOrEmpty(window.type.Namespace) && window.type.Namespace.Contains("UnityEditor"))
                        GUI.Label(args.rowRect, new GUIContent(window.searchName, tx));
                    else
                        GUI.Label(args.rowRect, window.searchName);
                }

            }

            public static Texture tx = EditorGUIUtility.IconContent("BuildSettings.Editor.Small").image;
            private SelectTree _tree;
            public WindowCollection()
            {
                _tree = new SelectTree(new TreeViewState());
            }

            public void OnGUI(Rect position)
            {
                _tree.searchString = _window.search;
                _tree.OnGUI(position);
            }
        }
        enum WindowType
        {
            WindowCollection,
            UserOption,
            Pkgs,
        }
        class Contents
        {
            public const float lineHeight = 20;
            public const float gap = 10;

            public static GUIContent accop = new GUIContent("Account Operation");

            public static GUIContent go = new GUIContent("Go");
            public static GUIContent logout = new GUIContent("Logout");
            public static GUIContent select = new GUIContent("Select");
            public static GUIContent remove = new GUIContent("Remove");
            public static GUIContent install = new GUIContent("Install");
            public static GUIContent delete = new GUIContent("Delete");

            public static GUIContent newest = new GUIContent("Newset");
            public static GUIContent refresh = EditorGUIUtility.IconContent("TreeEditor.Refresh");
        }
        class Styles
        {
            public static GUIStyle titlestyle = GUIStyles.Get("IN BigTitle");

            public static GUIStyle minus = "OL Minus";
            public static GUIStyle plus = "OL Plus";
            public static GUIStyle boldLabel = "BoldLabel";
            public static GUIStyle header = new GUIStyle("BoldLabel")
            {
                fontSize = 20
            };
            public static GUIStyle entryBackodd = GUIStyles.Get("CN EntryBackodd");
            public static GUIStyle entryBackEven = GUIStyles.Get("CN EntryBackEven");
        }

        [Serializable]
        public class PkgkitInfo
        {
            [Serializable]
            public class UserJson
            {
                public string name;
                public string token;
            }
            public List<PkgKitTool.Constant.PackageInfosModel> pkgInfos = new List<PkgKitTool.Constant.PackageInfosModel>();
            public UserJson userJson = new UserJson();
            public bool login
            {
                get
                {
                    if (userJson == null) return false;
                    if (string.IsNullOrEmpty(userJson.name)) return false;
                    return true;
                }
            }
        }
        private PkgkitInfo _windowInfo;
        private static RootWindow _window;
        private UserOptionWindow _userOption;
        private WindowCollection _collection;
        private PkgsWindow _pkgs;
        private ToolBarTree _toolBarTree;
        private WindowType __windowType;
        private WindowType _windowType
        {
            get { return __windowType; }
            set
            {
                if (__windowType != value)
                {
                    __windowType = value;
                }
            }
        }

    }
    partial class RootWindow
    {
        [MenuItem("IFramework/RootWindow", priority = -1000)]
        static void ShowWindow()
        {
            GetWindow<RootWindow>();
        }
        private void OnEnable()
        {
            _window = this;
            __windowType = EditorTools.Prefs.GetObject<RootWindow, WindowType>("__windowType");
            if (_windowInfo == null)
            {
                _windowInfo = new PkgkitInfo();
            }
            _collection = new WindowCollection();
            _pkgs = new PkgsWindow();
            _userOption = new UserOptionWindow();
            _toolBarTree = new ToolBarTree();
            PkgKitTool.Init();
            _toolBarTree.Popup((value) => { _windowType = (WindowType)value; }, typeof(WindowType).GetEnumNames(), (int)_windowType)
                //.Button(Contents.refresh, (r) => {
                //    PkgKitTool.FreshWebPackages();
                //}, 20)
                .Space(20)
                .Label(new GUIContent(PkgKitTool.userjson.name), 100, () => { return PkgKitTool.login; })
                .FlexibleSpace()
                .SearchField((value) => { search = value; }, search, 200)
                ;
        }
        private string search = "";
        private void OnDisable()
        {
            EditorTools.Prefs.SetObject<RootWindow, WindowType>("__windowType", __windowType);
        }
        private void OnGUI()
        {
            var rs = this.LocalPosition().HorizontalSplit(20);

            _toolBarTree.OnGUI(rs[0]);

            var r2 = rs[1].Zoom(AnchorType.UpperCenter, -10);
            switch (_windowType)
            {
                case WindowType.UserOption:
                    _userOption.OnGUI(r2);
                    break;
                case WindowType.Pkgs:
                    _pkgs.OnGUI(r2);
                    break;
                case WindowType.WindowCollection:
                    _collection.OnGUI(r2);
                    break;
                default:
                    break;
            }
        }
    }

}
