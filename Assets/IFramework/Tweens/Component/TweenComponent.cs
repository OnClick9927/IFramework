/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.148
 *UnityVersion:   2018.4.24f1
 *Date:           2021-07-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IFramework.Tweens
{
    public abstract class TweenComponent<T,Target> : MonoBehaviour where T : struct where Target :Object
    {
        public Target[] targets;
        public enum TweenType
        {
            Single,
            Array
        }
        public bool autoPlay = true;
        public float duration = 1;
        public bool snap = false;
        public TweenType type;
        public T start, end;
        public T[] array;
        public AnimationCurve curve;
        public int loop=-1;
        public LoopType LoopType = LoopType.PingPong;
        public bool autoRcyle;
        public Action onCompelete;
        public ITween tween;
        private void OnEnable()
        {
            if (autoPlay)
            {
                Play();
            }
        }
        [ContextMenu("Play")]
        public void Play()
        {
            if (tween!=null)
            {
                tween.Complete(false);
            }
            switch (type)
            {
                case TweenType.Single:
                    tween= TweenEx.DoGoto<T>(start, end, duration, GetTargetValue, SetTargetValue, snap)
                        .SetAnimationCurve(curve)
                        .SetLoop(loop, LoopType)
                        .SetRecyle(autoRcyle)
                        .OnCompelete(() => { tween = null; onCompelete?.Invoke(); });
                    break;
                case TweenType.Array:
                    tween= TweenEx.DoGoto<T>(array, duration, GetTargetValue, SetTargetValue, snap)
                        .SetAnimationCurve(curve)
                        .SetLoop(loop, LoopType)
                        .SetRecyle(autoRcyle)
                        .OnCompelete(() => { tween = null; onCompelete?.Invoke(); });
                    break;
                default:
                    break;
            }

        }

        protected abstract void SetTargetValue(T value);

        protected abstract T GetTargetValue();
    }
}
