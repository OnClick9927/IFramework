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

namespace IFramework.Hotfix
{
    public class Assets : MonoBehaviour
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

        private PrepareState _prepareState;
        private Dictionary<string, AsyncOperationHandle> handles = new Dictionary<string, AsyncOperationHandle>();
        private AsyncOperationHandle downloadHandle;
        private UpdateState _updateState;

        public event Action onPrepareBegin;
        public event Action onPrepareCompelete;
        public event Action<float> onPrepareProgress;

        public event Action onUpdateBegin;
        public event Action onUpdateCompelete;
        public event Action<float> onUpdateProgress;
        public string currentPrepare { get; private set; }
        public UpdateState updateState
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
        public PrepareState prepareState
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
        public async void UpdateAssets()
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
        private void FrshProgress()
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



        public void PrepareDefault()
        {
            Debug.Log("加载默认资源");
            PrepareAssets(defaultKey);
        }
        public void PrepareAssets(string key)
        {
            if (handles.ContainsKey(key)) return;
            prepareState = PrepareState.Preparing;
            currentPrepare = key;
            IEnumerable<string> ie = new string[] { key };
            var handler = Addressables.LoadAssetsAsync<Object>(ie, null, Addressables.MergeMode.Union);
            handles.Add(key, handler);
            handler.Completed += Handler_Completed;
        }
        public void ReleaseAssets(string key)
        {
            if (!handles.ContainsKey(key)) return;
            Addressables.Release(handles[key]);
            handles.Remove(key);
        }
        public T LoadPreparedAsset<T>(string key)
        {
            return Addressables.LoadAssetAsync<T>(key).WaitForCompletion();
        }



        public void Release<T>(T t)
        {
            Addressables.Release(t);
        }
        public AsyncOperationHandle<T> Load<T>(string key)
        {
            return Addressables.LoadAssetAsync<T>(key);
        }

        public AsyncOperationHandle<SceneInstance> LoadScene(object key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            return Addressables.LoadSceneAsync(key, loadMode, activateOnLoad, priority);
        }

        public AsyncOperationHandle<SceneInstance> ReleaseScene(AsyncOperationHandle<SceneInstance> scene, bool autoReleaseHandle = true)
        {
            return Addressables.UnloadSceneAsync(scene.Result, autoReleaseHandle);
        }


        private void Handler_Completed(AsyncOperationHandle<IList<Object>> obj)
        {
            prepareState = PrepareState.None;
        }
    }
}
