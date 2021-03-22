/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Serialization;
using IFramework.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IFramework.Hotfix.AB
{
	public class Assets:SingletonPropertyClass<Assets>
	{
        public class Bundles
        {
            private string _rootPath;
            private AssetBundleManifest _manifest;
            private Dictionary<string, Bundle> _bundles = new Dictionary<string, Bundle>();

            public bool Init(string path)
            {
                _rootPath = path;
                Bundle manifestBundle = Load(ABTool.platformName, true, false);
                if (manifestBundle == null || manifestBundle.error != null) return false;
                _manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                return _manifest != null;
            }
            public IEnumerator InitAsync(string path, Action<Bundle> OnComplete)
            {
                _rootPath = path;
                Bundle manifestBundle = Load(ABTool.platformName, true, true);
                yield return manifestBundle;
                if (manifestBundle == null || manifestBundle.error != null)
                {
                    Log.E("Manifest Load Err");
                    yield break;
                }
                _manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (_manifest == null)
                    yield break;
                Bundle bundle = Load(ABTool.helpABName, false, true);
                yield return bundle;
                if (bundle == null || bundle.error != null)
                    yield break;
                if (OnComplete != null) OnComplete(bundle);
            }


            private Bundle Load(string bundleName, bool loadManifest, bool isAsync)
            {
                if (!loadManifest && _manifest == null)
                    throw new Exception("Bundles , Please initialize AssetBundleManifest by calling Bundles.Initialize()");
                string bundlePath = _rootPath.CombinePath(bundleName);
                Bundle bundle;
                if (!_bundles.TryGetValue(bundleName, out bundle))
                {
                    Hash128 version = loadManifest ? new Hash128(1, 0, 0, 0) : _manifest.GetAssetBundleHash(bundleName);
                    if (bundle == null)
                    {
                        List<Bundle> dpdenceBundles=new List<Bundle>();
                        if (!loadManifest)
                        {
                            string[] dps = _manifest.GetAllDependencies(bundleName);
                            if (dps!=null)
                                for (int i = 0; i < dps.Length; i++)
                                    dpdenceBundles.Add(Load(dps[i], false, isAsync));
                        }
                        if (bundlePath.StartsWith("http://") ||
                          bundlePath.StartsWith("https://") ||
                          bundlePath.StartsWith("file://") ||
                          bundlePath.StartsWith("ftp://"))
                        {
                            bundle = new WebRequestBundle(bundlePath, bundleName, dpdenceBundles, version);
                        }
                        else
                        {
                            if (isAsync)
                                bundle = new AsyncBundle(bundlePath, bundleName, dpdenceBundles ,version);
                            else
                                bundle = new Bundle(bundlePath, bundleName, dpdenceBundles, version);
                        }
                        _bundles.Add(bundleName, bundle);
                        bundle.Load();
                    }
                }
                bundle.Retain();
                return bundle;
            }
            public Bundle LoadAsync(string abName)
            {
                return Load(abName, false, true);
            }
            public Bundle LoadSync(string abName)
            {
                return Load(abName, false, false);
            }



            private Queue<Bundle> destoryQueue = new Queue<Bundle>();
            public void ClearUnUseBundles()
            {
                foreach (var item in _bundles)
                    if (item.Value.isDone && !item.Value.useful)
                        destoryQueue.Enqueue(item.Value);
                while (destoryQueue.Count > 0)
                {
                    Bundle bundle = destoryQueue.Dequeue();
                    _bundles.Remove(bundle.name);
                    bundle.UnLoad();
                    bundle.Dispose();
                    bundle = null;
                }
            }
        }

        private class Manifest
        {
            //存储asset对应allBundle的index
            private readonly Dictionary<string, int> amap;
            //存储assetbundleBuid所有asset在allAssets的index
            private readonly Dictionary<string, List<int>> bmap;
            //所有asset 名字
            public List<string> assetNames;
            //所有bundle 名字
            public List<string> bundleNames;

            public Manifest()
            {
                amap = new Dictionary<string, int>();
                bmap = new Dictionary<string, List<int>>();
                assetNames = new List<string>();
                bundleNames = new List<string>();
            }
            public void Load(string txt)
            {
                amap.Clear();
                bmap.Clear();

                assetNames.Clear();
                bundleNames.Clear();
                List<BundleGroup> list = Xml.FromXml<List<BundleGroup>>(txt);
                foreach (var content in list)
                {
                    bundleNames.Add(content.assetBundleName);
                    bmap.Add(content.assetBundleName, new List<int>());
                    bmap[content.assetBundleName].Add(assetNames.Count - 1);
                    foreach (var asset in content.assetNames)
                    {
                        assetNames.Add(asset);
                        bmap[content.assetBundleName].Add(assetNames.Count - 1);
                        amap[asset] = bundleNames.Count - 1;
                    }
                }
            }


            public bool ContainsBundle(string bundle) { return bmap.ContainsKey(bundle); }
            public bool ContainsAsset(string assetPath) { return amap.ContainsKey(assetPath); }
            public string[] GetBundleAssets(string bundleName)
            {
                return Array.ConvertAll<int, string>(bmap[bundleName].ToArray(), input =>
                {
                    return assetNames[input];
                });
            }
            public string GetBundleName(string assetPath) { return bundleNames[amap[assetPath]]; }
            public string GetAssetName(string assetPath) { return Path.GetFileName(assetPath); }
        }

        private Manifest _manifestXML;
        private List<string> _assetNames { get { return instance._manifestXML.assetNames; } }
        private List<string> _bundleNames { get { return instance._manifestXML.bundleNames; } }
        private string initPath
        {
            get
            {
                string relativePath = ABTool.assetsOutPutPath;
                var url =
#if UNITY_EDITOR
                relativePath;
#else
				Path.Combine(Application.streamingAssetsPath, relativePath) ;
#endif
                return relativePath;
            }
        }

        public static string GetBundleName(string assetPath) { return instance._manifestXML.GetBundleName(assetPath); }
        public static string GetAssetName(string assetPath) { return instance._manifestXML.GetAssetName(assetPath); }
        public static Bundles bundles { get; private set; }

        private bool InitializeBundle()
        {
            if (!bundles.Init(initPath))
                throw new FileNotFoundException("bundle manifest not exist.");
            var bundle = bundles.LoadSync(ABTool.helpABName);
            if (bundle == null)
                throw new FileNotFoundException("assets manifest not exist.");
            TextAsset xml = bundle.LoadAsset<TextAsset>(ABTool.configPath.GetFileName());
            if (xml == null)
                throw new FileNotFoundException("assets manifest not exist.");
            using (var reader = new StringReader(xml.text))
            {
                instance._manifestXML.Load(reader.ReadToEnd());
                reader.Close();
            }
            bundle.Release();
            Resources.UnloadAsset(xml);
            xml = null;
            return true;
        }
        public static bool Init()
        {
#if UNITY_EDITOR
            if (!ABTool.testmode) return instance.InitializeBundle();
            return true;
#else
			return instance.InitializeBundle();
#endif
        }
        public static void InitAsync(Action onComplete)
        {
            Launcher.instance.StartCoroutine(bundles.InitAsync(instance.initPath, bundle =>
            {
                if (bundle == null)
                {
                    if (onComplete != null) onComplete();
                    return;
                }
                var asset = bundle.LoadAsset<TextAsset>(ABTool.configPath.GetFileName());
                if (asset != null)
                {
                    using (var reader = new StringReader(asset.text))
                    {
                        instance._manifestXML.Load(reader.ReadToEnd());
                        reader.Close();
                    }
                    bundle.Release();
                    Resources.UnloadAsset(asset);
                    asset = null;
                }
                if (onComplete != null) onComplete();

            }));
        }

        public static void Unload(Asset asset)
        {
            asset.Release();
        }


        public static Asset Load<T>(string path) where T : UnityEngine.Object
        {
            return Load(path, typeof(T));
        }
        public static Asset LoadAsync<T>(string path)
        {
            return LoadAsync(path, typeof(T));
        }
        public static Asset Load(string path, Type type)
        {
            return Load(path, type, false);
        }
        public static Asset LoadAsync(string path, Type type)
        {
            return Load(path, type, true);
        }
        private static Asset Load(string path, Type type, bool isAsynnc)
        {
            Asset asset = instance.assets.Find(obj => { return obj.assetPath == path; });
            if (asset == null)
            {
#if UNITY_EDITOR
                if (ABTool.testmode)
                {
                    asset = new Asset(path, type);
                }
#endif
                if (asset==null)
                {
                    if (isAsynnc)
                        asset = new AsyncBundleAsset(path, type);
                    else
                        asset = new BundleAsset(path, type);
                }
                instance.assets.Add(asset);
                asset.Load();
            }
            asset.Retain();
            return asset;
        }

        
        private Assets(){}
        protected override void OnSingletonInit()
        {
            _manifestXML = new Manifest();
            bundles = new Bundles();
            Framework.BindEnvUpdate(Update, EnvironmentType.Ev1);
            Framework.BindEnvDispose(()=> { SingletonCollection.Dispose<Assets>(); }, EnvironmentType.Ev1);
        }

        private List<Asset> assets = new List<Asset>();
        private IEnumerator gc;
        IEnumerator GC()
        {
            System.GC.Collect();
            yield return 0;
            yield return Resources.UnloadUnusedAssets();
        }
        private void Update()
        {
            bool removed = false;
            for (int i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                if (!asset.isDone && !asset.useful)
                {
                    asset.UnLoad();
                    asset = null;
                    assets.RemoveAt(i);
                    i--;
                    removed = true;
                }
            }

            if (removed)
            {
                if (gc != null)
                {
                    Launcher.instance. StopCoroutine(gc);
                }
                gc = GC();
                Launcher.instance.StartCoroutine(gc);
            }

            bundles.ClearUnUseBundles();
        }

        protected override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
