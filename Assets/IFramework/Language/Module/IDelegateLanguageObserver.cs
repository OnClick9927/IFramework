using System;
using UnityEngine;

namespace IFramework.Language
{
    public interface IDelegateLanguageObserver:ILanguageObserver,IDisposable
    {
        IDelegateLanguageObserver Listen(Action<string> listen);

        void Pause();
        void Start();
        void UnPause();
    }
}