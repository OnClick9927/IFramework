using System;
using UnityEngine;

namespace IFramework.Language
{
    public interface IDelegateLanguageObserver:IDisposable
    {
        IDelegateLanguageObserver Listen(Action<string> listen);
        void SetValue(ILanguageModule moudle, string key, SystemLanguage fallback, bool autoStart);


        void Pause();
        void Start();
        void UnPause();
    }
}