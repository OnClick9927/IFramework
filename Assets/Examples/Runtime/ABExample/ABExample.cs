/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections;
using UnityEngine;
using IFramework.Hotfix.AB;
using IFramework;
namespace IFramework_Demo
{
	public class ABExample:Game
	{
  
        [SerializeField] string assetPath = "Assets/Examples/Runtime/ABExample/Logo.prefab";
        IEnumerator Load()
        {
            var asset = Assets.LoadAsync<GameObject>(assetPath);

            if (asset != null)
            {
                while (!asset.isDone)
                {
                    yield return 0;
                }

                var prefab = asset.asset;
                if (prefab != null)
                {
                    var go = Instantiate(prefab) as GameObject;
                    ReleaseAssetOnDestroy.Register(go, asset);
                    GameObject.Destroy(go, 10);
                }
            }

            yield return new WaitForSeconds(11);

            asset = Assets.Load<GameObject>(assetPath);
            if (asset != null)
            {
                var prefab = asset.asset;
                if (prefab != null)
                {
                    var go = Instantiate(prefab) as GameObject;
                    ReleaseAssetOnDestroy.Register(go, asset);
                    GameObject.Destroy(go, 3);
                }
            }
        }


        public override void Startup()
        {
            if (Assets.Init())
            {
                StartCoroutine(Load());

            }
            //Assets.InitAsync(() =>
            //{
            //    StartCoroutine(Load());
            //});

        }

        public override void Init()
        {
            throw new System.NotImplementedException();
        }
    }
}
