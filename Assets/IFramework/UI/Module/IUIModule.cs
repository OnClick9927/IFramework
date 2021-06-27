using System;
using UnityEngine;

namespace IFramework.UI
{
    public interface IUIModule
    {
        RectTransform aboveAnimation { get; }
        RectTransform aboveTop { get; }
        RectTransform background { get; }
        RectTransform belowAnimation { get; }
        RectTransform belowBackground { get; }
        RectTransform camera { get; }
        Canvas canvas { get; }
        RectTransform common { get; }
        UIPanel current { get; }
        RectTransform guide { get; }
        int loaderCount { get; }
        int memoryCount { get; }
        RectTransform pop { get; }
        int priority { get; }
        int stackCount { get; }
        RectTransform toast { get; }
        RectTransform top { get; }

        void AddLoader(IPanelLoader loader);
        void ClearMemory();
        void Close(string name);
        void CreateCanvas();
        bool Exist(UIPanel panel);
        bool ExistInMemory(UIPanel panel);
        bool ExistInStack(UIPanel panel);
        UIPanel FindPanel(string name);
        UIPanel Get(string name);
        bool GoBack();
        bool GoForWard();
        void Hide(string name);
        UIPanel Load(string name);
        UIPanel MemoryPeek();
        void Pause(string name);
        void Push(UIPanel ui);
        void PutCamera(Camera camera);
        void SetCanvas(Canvas canvas);
        void SetGroups(IGroups groups);
        void Show(string name);
        void UnPause(string name);
    }
}