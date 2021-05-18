/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.254
 *UnityVersion:   2018.4.24f1
 *Date:           2021-05-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;
namespace IFramework.Hotfix
{
    [RequireComponent(typeof(Assets))]
    public class AAGame : Game
    {
        private void Awake()
        {
            assets = GetComponent<Assets>();
            assets.onPrepareBegin +=()=> { onPrepareBegin?.Invoke(); } ;
            assets.onPrepareCompelete += () => { onPrepareCompelete?.Invoke(); };
            assets.onPrepareProgress += (value) => { onPrepareProgress?.Invoke(value); };

            assets.onUpdateBegin += () => { onUpdateBegin?.Invoke(); };
            assets.onUpdateCompelete += () => { onUpdateCompelete?.Invoke(); };
            assets.onUpdateProgress += (value) => { onUpdateProgress?.Invoke(value); };
        }
        public event Action onUpdateBegin;
        public event Action onUpdateCompelete;
        public event Action<float> onUpdateProgress;

        public event Action onPrepareBegin;
        public event Action onPrepareCompelete;
        public event Action<float> onPrepareProgress;
        [HideInInspector]public Assets assets;
        public override void Init()
        {
        }

        public override void Startup()
        {
            onUpdateCompelete += assets.PrepareDefault;
            onPrepareCompelete += () => {
                GameObject prefab= assets.LoadPreparedAsset<GameObject>("cube");
                Debug.Log(prefab);
                GameObject.Instantiate(prefab);
            };
            assets.UpdateAssets();
        }
    }
}
