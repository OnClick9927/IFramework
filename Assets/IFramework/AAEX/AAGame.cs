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
        public event Action onUpdateBegin { add { Assets.onUpdateBegin += value; } remove { Assets.onUpdateBegin -= value; } }
        public event Action onUpdateCompelete { add { Assets.onUpdateCompelete += value; } remove { Assets.onUpdateCompelete -= value; } }
        public event Action<float> onUpdateProgress { add { Assets.onUpdateProgress += value; } remove { Assets.onUpdateProgress -= value; } }

        public event Action onPrepareBegin { add { Assets.onPrepareBegin += value; } remove { Assets.onPrepareBegin -= value; } }
        public event Action onPrepareCompelete { add { Assets.onPrepareCompelete += value; } remove { Assets.onPrepareCompelete -= value; } }
        public event Action<float> onPrepareProgress { add { Assets.onPrepareProgress += value; } remove { Assets.onPrepareProgress -= value; } }
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
