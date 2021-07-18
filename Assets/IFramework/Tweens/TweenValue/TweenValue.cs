/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.146
 *UnityVersion:   2018.4.17f1
 *Date:           2020-04-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using IFramework.NodeAction;
using UnityEngine;

namespace IFramework.Tweens
{
    [ScriptVersion(22)]
    abstract class TweenValue : RecyclableObject
    {
        private IPercentConverter _converter = EaseCoverter.Default;
        private ISequenceNode _node;
        private float _time;

        protected event Action onCompelete;
        protected float percent { get { return ((_time) / duration).Clamp01(); } }
        protected float convertPercent { get { return _converter.Convert(percent, _time, duration); } }
        protected float deltaPercent { get { return delta + (1 - delta) * percent; } }

        public IPercentConverter converter { get { return _converter; } set { _converter = value; } }
        public abstract float duration { get; }
        public bool compeleted { get; private set; }

        public static float delta = 0.618f;
        public static float deltaTime = 0.02f;
        public static float timeScale = 1;
        private static Dictionary<Type, Type> map = new Dictionary<Type, Type>()
        {
            {typeof(bool),typeof(BoolTweenValue) },
            {typeof(int),typeof(IntTweenValue) },
            {typeof(float),typeof(FloatTweenValue) },
            {typeof(Vector2),typeof(Vector2TweenValue) },
            {typeof(Vector3),typeof(Vector3TweenValue) },
            {typeof(Vector4),typeof(Vector4TweenValue) },
            {typeof(Color),typeof(ColorTweenValue) },
            {typeof(Rect),typeof(RectTweenValue) },
            {typeof(Quaternion),typeof(QuaternionTweenValue) },
        };
        public static TweenValue<T> Get<T>(EnvironmentType envType) where T : struct
        {
            Type type;
            if (map.TryGetValue(typeof(T), out type))
                return Allocate(type, envType) as TweenValue<T>;
            throw new Exception(string.Format("Do not Have TweenValue<{0}>  with Type {0}", typeof(T)));
        }

        protected abstract void MoveNext();
        protected override void OnDataReset()
        {
            if (_node != null && !_node.recyled)
            {
                _node.Recyle();
                _node = null;
            }
            onCompelete = null;
            _time = 0;
            _converter = EaseCoverter.Default;
            compeleted = false;
        }


        public void Run()
        {
            _node = this.Sequence(this.env)
                         .DoWhile(IsFinish, LoopEvent)
                         .OnCompelete(OnCompelete)
                         .Run();
        }
        private bool IsFinish()
        {
            return percent<1;
        }
        private void LoopEvent()
        {
            if (recyled) return;
            _time += deltaTime * timeScale;
            MoveNext();

        }
        private void OnCompelete()
        {
            _time = 0;
            compeleted = true;
            if (onCompelete != null)
                onCompelete();
        }
    }

    [ScriptVersion(4)]
    abstract class TweenValue<T> : TweenValue where T : struct
    {
        private IPlugin<T> _plugin;
        private T _current;

        protected T pluginValue { get { return _plugin.getter.Invoke(); } }

        public T current
        {
            get { return _current; }
            set
            {
                if (_plugin.snap)
                    _current = Snap(value);
                else
                    _current = value;
                if (_plugin.setter != null)
                {
                    _plugin.setter(_current);
                }
            }
        }
        public T end { get { return _plugin.end; } }
        public T start { get { return _plugin.start; } }
        public override float duration { get { return _plugin.duration; } }

        protected virtual T Snap(T value) { return value; }
        protected override void OnDataReset()
        {
            base.OnDataReset();
            _plugin.Recyle();
            _plugin = null;
            _current = default(T);
        }

        public void Config(IPlugin<T> plugin, Action onCompelete)
        {
            this._plugin = plugin;
            this._current = plugin.start;
            this.onCompelete += onCompelete;
            SetDataDirty();
        }
    }
}
