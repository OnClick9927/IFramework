/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IFramework.Language
{
    public class LanguageModule : FrameworkModule, ILanguageModule
    {
        class DelegateLanguageObserver : IDelegateLanguageObserver, ILanguageObserver
        {
            public SystemLanguage fallbackLanguage { get; set; }
            public string languageKey { get; private set; }
            public ILanguageModule module { get { return _module; } }

            private bool _disposed;
            private bool _paused;
            private string value;
            private Action<string> _onValueChange;
            private SystemLanguage _last = SystemLanguage.Unknown;
            public void Listen(SystemLanguage languageType, string value)
            {
                this.value = value;
                if (_disposed || _paused) return;
                if (_last == languageType) return;
                if (_onValueChange != null)
                {
                    _onValueChange.Invoke(value);
                }
                _last = languageType;
            }

            private ILanguageModule _module;
            void ILanguageObserver.SetValue(ILanguageModule module, string key, SystemLanguage fallback, bool autoStart)
            {
                this._module = module;
                this.languageKey = key;
                this.fallbackLanguage = fallback;
                _disposed = false;
                _paused = !autoStart;
                _module.Subscribe(this);
            }
            public IDelegateLanguageObserver Listen(Action<string> listen)
            {
                _onValueChange += listen;
                return this;
            }


            public void Start() { UnPause(); }

            public void Pause()
            {
                _paused = true;
            }
            public void UnPause()
            {
                _paused = false;
                if (!string.IsNullOrEmpty(this.value))
                {
                    if (_onValueChange != null)
                    {
                        _onValueChange.Invoke(value);
                    }
                }
            }
            public void Dispose()
            {
                Pause();
                _disposed = true;
                _onValueChange = null;
                _module.UnSubscribe(this);
            }


        }
        private class LanguageObserverPool : BaseTypePool<ILanguageObserver> { }
        private List<LanPair> _pairs;
        private Dictionary<string, List<LanPair>> _keyDic;
        private List<ILanguageObserver> _observers;
        private LanguageObserverPool _pool;
        private SystemLanguage _lan = SystemLanguage.Unknown;

        public SystemLanguage language
        {
            get { return _lan; }
            set
            {
                if (_lan == value) return;
                _lan = value;
                Publish(value);
            }
        }

        public void Subscribe(ILanguageObserver observer)
        {
            _observers.Add(observer);
            SystemLanguage type;
            var value = GetValue(observer.languageKey, language, observer.fallbackLanguage, out type);
            observer.Listen(type, value);
        }
        public void UnSubscribe(ILanguageObserver observer)
        {
            _observers.Remove(observer);
            _pool.Set(observer);
        }
        public T CreateObserver<T>(string key, SystemLanguage fallback, bool autoStart = true) where T: class,ILanguageObserver,new ()
        {
            var o = _pool.Get<T>();
            o.SetValue(this, key, fallback, autoStart);
            Publish(o, _lan);
            return o;
        }
        public IDelegateLanguageObserver CreateDelegateObserver(string key, SystemLanguage fallback, bool autoStart = true)
        {
           return CreateObserver<DelegateLanguageObserver>(key, fallback, autoStart);
        }

        public void Load(List<LanPair> pairs, bool rewrite = true)
        {
            pairs.ForEach((tmpPair) => {
                LanPair pair = _pairs.Find((p) => { return p.lan == tmpPair.lan && p.key == tmpPair.key; });
                if (pair != null && rewrite && pair.value != tmpPair.value)
                    pair.value = tmpPair.value;
                else
                    _pairs.Add(tmpPair);
            });
            //pairs.Clear();
            _keyDic = _pairs.GroupBy(lanPair => { return lanPair.key; }, (key, list) => { return new { key, list }; })
                     .ToDictionary((v) => { return v.key; }, (v) => { return v.list.ToList(); });
        }


        public string GetValue(string key, SystemLanguage language, SystemLanguage fallback, out SystemLanguage type)
        {
            List<LanPair> pairs;
            if (!_keyDic.TryGetValue(key, out pairs))
            {
                type = SystemLanguage.Unknown;
                return null;
            }

            LanPair _fallback = null;
            for (int j = 0; j < pairs.Count; j++)
            {
                var pair = pairs[j];
                if (pair.lan == language)
                {
                    type = this.language;
                    return pair.value;
                }
                else if (pair.lan == fallback)
                {
                    _fallback = pair;
                }
            }
            if (_fallback == null)
            {
                type = SystemLanguage.Unknown;
                return null;
            }

            type = fallback;
            return _fallback.value;
        }
        private void Publish(ILanguageObserver o, SystemLanguage value)
        {
            List<LanPair> pairs;
            if (!_keyDic.TryGetValue(o.languageKey, out pairs)) return;
            LanPair fallback = null;
            bool ok = false;
            for (int j = 0; j < pairs.Count; j++)
            {
                var pair = pairs[j];
                if (pair.lan == value)
                {
                    o.Listen(value, pair.value);
                    ok = true;
                    break;
                }
                else if (pair.lan == o.fallbackLanguage)
                {
                    fallback = pair;
                }
            }
            if (!ok && fallback != null)
            {
                o.Listen(o.fallbackLanguage, fallback.value);
            }
        }
        public void Publish(SystemLanguage value)
        {
            for (int i = 0; i < _observers.Count; i++)
            {
                Publish(_observers[i], value);
            }
        }

        public override int priority { get { return 90; } }
        protected override void Awake()
        {
            _pool = new LanguageObserverPool();
            _pairs = new List<LanPair>();
            _observers = new List<ILanguageObserver>();
        }
        protected override void OnDispose()
        {
            _pairs.Clear();
            _observers.Clear();
            _pool.Dispose();
        }
    }
}
