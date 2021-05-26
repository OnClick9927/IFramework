using System.Collections.Generic;
using UnityEngine;

namespace IFramework.Language
{
    public interface ILanguageModule
    {
        SystemLanguage language { get; set; }
        T CreateObserver<T>(string key, SystemLanguage fallback, bool autoStart = true) where T : class, ILanguageObserver, new();

        IDelegateLanguageObserver CreateDelegateObserver(string key, SystemLanguage fallback, bool autoStart = true);
        string GetValue(string key, SystemLanguage language, SystemLanguage fallback, out SystemLanguage type);
        void Load(List<LanPair> pairs, bool rewrite = true);
        void Publish(SystemLanguage value);
        void Subscribe(ILanguageObserver observer);
        void UnSubscribe(ILanguageObserver observer);
    }
}