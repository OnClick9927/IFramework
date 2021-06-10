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
namespace IFramework.AAEX
{
    public class AAGame : Game
    {
        private void Awake()
        {
            Assets.onPrepareBegin +=()=> { onPrepareBegin?.Invoke(); } ;
            Assets.onPrepareCompelete += () => { onPrepareCompelete?.Invoke(); };
            Assets.onPrepareProgress += (value) => { onPrepareProgress?.Invoke(value); };

            Assets.onUpdateBegin += () => { onUpdateBegin?.Invoke(); };
            Assets.onUpdateCompelete += () => { onUpdateCompelete?.Invoke(); };
            Assets.onUpdateProgress += (value) => { onUpdateProgress?.Invoke(value); };
        }
        public event Action onUpdateBegin;
        public event Action onUpdateCompelete;
        public event Action<float> onUpdateProgress;

        public event Action onPrepareBegin;
        public event Action onPrepareCompelete;
        public event Action<float> onPrepareProgress;
        public override void Init()
        {
        }

        public override void Startup()
        {
            onUpdateCompelete += Assets.PrepareDefault;
            onPrepareCompelete += () => {
                GameObject prefab= Assets.LoadPreparedAsset<GameObject>("cube");
                Debug.Log(prefab);
                GameObject.Instantiate(prefab);
            };
            Assets.UpdateAssets();
        }
    }
}
