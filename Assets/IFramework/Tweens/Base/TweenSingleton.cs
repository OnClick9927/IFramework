/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.278
 *UnityVersion:   2018.4.24f1
 *Date:           2020-12-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Singleton;
using UnityEngine;

namespace IFramework.Tweens
{
    [MonoSingletonPath("IFramework/Tweens")]
    class TweenSingleton:MonoSingletonPropertyClass<TweenSingleton>
    {
        public const EnvironmentType envType = EnvironmentType.Extra0;
        public static TweenUpdateType updateType = TweenUpdateType.Update;


        public static bool Initialized()
        {
            return Instance == null;
        }


        protected override void OnSingletonInit()
        {
            Framework.CreateEnv(envType).InitWithAttribute();
        }
        private void Update()
        {
            if (updateType== TweenUpdateType.Update)
            {
                TweenValue.deltaTime = Time.deltaTime;

                Framework.extra0.Update();
            }
        }
        private void FixedUpdate()
        {
            if (updateType == TweenUpdateType.FixedUpdate)
            {
                TweenValue.deltaTime = Time.fixedDeltaTime;

                Framework.extra0.Update();
            }
        }
        private void LateUpdate()
        {
            if (updateType == TweenUpdateType.LateUpdate)
            {
                Framework.extra0.Update();
            }
        }
        private void OnDisable()
        {
            Framework.extra0.Dispose();
        }
      
    }
}
