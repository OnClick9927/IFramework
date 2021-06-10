/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.252
 *UnityVersion:   2018.4.24f1
 *Date:           2021-05-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using System;
using Object = UnityEngine.Object;
using System.Threading.Tasks;

namespace IFramework.AAEX
{
    public static class Assets
    {
        public const string defaultKey = "default";
        public enum PrepareState
        {
            None,
            Preparing
        }
        public enum UpdateState
        {
            None,
            Updating,
            Compelete
        }

        private static PrepareState _prepareState;
        private static Dictionary<string, AsyncOperationHandle> handles = new Dictionary<string, AsyncOperationHandle>();
        private static AsyncOperationHandle downloadHandle;
        private static UpdateState _updateState;

        public static event Action onPrepareBegin;
        public static event Action onPrepareCompelete;
        public static event Action<float> onPrepareProgress;
        public static event Action onUpdateBegin;
        public static event Action onUpdateCompelete;
        public static event Action<float> onUpdateProgress;
        public static string currentPrepare { get; private set; }
        public static UpdateState updateState
        {
            get { return _updateState; }
            private set
            {
                if (_updateState != value)
                {
                    _updateState = value;
                    switch (_updateState)
                    {
                        case UpdateState.None:
                            break;
                        case UpdateState.Updating:
                            onUpdateBegin?.Invoke();
                            break;
                        case UpdateState.Compelete:
                            onUpdateCompelete?.Invoke();
                            break;
                        default:
                            break;
                    }

                }
            }
        }
        public static PrepareState prepareState
        {
            get { return _prepareState; }
            private set
            {
                if (_prepareState != value)
                {
                    _prepareState = value;
                    switch (value)
                    {
                        case PrepareState.None:
                            onPrepareCompelete?.Invoke();
                            break;
                        case PrepareState.Preparing:
                            currentPrepare = "";
                            onPrepareBegin?.Invoke();
                            break;
                        default:
                            break;
                    }

                }
            }
        }
        public static async void UpdateAssets()
        {
            updateState = UpdateState.Updating;
            Debug.Log("开始更新资源");
            downloadHandle =  Addressables.InitializeAsync();
            await downloadHandle.Task;
            var locators = Addressables.ResourceLocators;

            foreach (var item in locators)
            {
                var sizeHandle = Addressables.GetDownloadSizeAsync(item.Keys);
                await sizeHandle.Task;
                if (sizeHandle.Result > 0)
                {
                    downloadHandle = Addressables.DownloadDependenciesAsync(item.Keys, Addressables.MergeMode.Union);
                    await downloadHandle.Task;
                }
            }
            Debug.Log("更新完成");
            updateState = UpdateState.Compelete;
            Launcher.env.BindUpdate(FrshProgress);
        }
        private static void FrshProgress()
        {
            try
            {
                if (updateState == UpdateState.Updating && !downloadHandle.Equals(default(AsyncOperationHandle)))
                {
                    onUpdateProgress?.Invoke(downloadHandle.PercentComplete);
                }
                if (prepareState == PrepareState.Preparing)
                {
                    onPrepareProgress?.Invoke(handles[currentPrepare].PercentComplete);
                }
            }
            catch (Exception)
            {
            }
        }



        public static void PrepareDefault()
        {
            Debug.Log("加载默认资源");
            PrepareAssets(defaultKey);
        }
        public static void PrepareAssets(string key)
        {
            if (handles.ContainsKey(key)) return;
            prepareState = PrepareState.Preparing;
            currentPrepare = key;
            IEnumerable<string> ie = new string[] { key };
            var handler = Addressables.LoadAssetsAsync<Object>(ie, null, Addressables.MergeMode.Union);
            handles.Add(key, handler);
            handler.Completed += Handler_Completed;
        }
        public static void ReleaseAssets(string key)
        {
            if (!handles.ContainsKey(key)) return;
            Addressables.Release(handles[key]);
            handles.Remove(key);
        }
        public static T LoadPreparedAsset<T>(string key)
        {
            return Addressables.LoadAssetAsync<T>(key).WaitForCompletion();
        }



        public static void Release<T>(T t)
        {
            Addressables.Release(t);
        }
        public static AsyncOperationHandle<T> Load<T>(string key)
        {
            return Addressables.LoadAssetAsync<T>(key);
        }

        public static AsyncOperationHandle<SceneInstance> LoadScene(object key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            return Addressables.LoadSceneAsync(key, loadMode, activateOnLoad, priority);
        }

        public static AsyncOperationHandle<SceneInstance> ReleaseScene(AsyncOperationHandle<SceneInstance> scene, bool autoReleaseHandle = true)
        {
            return Addressables.UnloadSceneAsync(scene.Result, autoReleaseHandle);
        }


        private static async void Handler_Completed(AsyncOperationHandle<IList<Object>> obj)
        {
            //处理1.18.4
            await Task.Delay(1);
            prepareState = PrepareState.None;
        }
    }
}
