/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.146
 *UnityVersion:   2018.4.17f1
 *Date:           2020-04-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.Tweens
{
    [ScriptVersion(33)]
    public abstract class Tween : RecyclableObject, ITween
    {
        private bool _autoRecyle = true;
        private TweenDirection _direction = TweenDirection.Forward;
        public TweenDirection direction { get { return _direction; } protected set { _direction = value; } }

        public event Action onCompelete;
        public float duration;
        public bool autoRecyle { get { return _autoRecyle; }set { _autoRecyle = value; } }
        public bool snap { get; set; }
        public LoopType loopType { get; set; }
        public abstract int loop { get; set; }
        protected static IPercentConverter defaultConverter = EaseCoverter.Default;
        public abstract IPercentConverter converter { get; set; }


        public abstract void Run();
        public abstract void ReStart();
        public abstract void Rewind(float duration,bool snap=false);
        public abstract void Complete(bool invoke);

        protected void InvokeCompelete()
        {
            if (onCompelete!=null)
            {
                onCompelete.Invoke();
            }
        }
        protected override void OnDataReset()
        {
            snap = false;
            onCompelete = null;
        }

        public ITween SetConverter(IPercentConverter converter)
        {
            if (recyled) return this;
            var last = this.converter;
            this.converter = converter;
            if (last != null && last != defaultConverter)
                last.Recyle();
            return this;
        }
    }
}
