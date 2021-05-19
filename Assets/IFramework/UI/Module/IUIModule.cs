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
        void CreateCanvas();
        bool Exist(UIPanel panel);
        bool ExistInMemory(UIPanel panel);
        bool ExistInStack(UIPanel panel);
        UIPanel FindPanel(string name);
        UIPanel Get(Type type, string name);
        T Get<T>(string name) where T : UIPanel;
        void GoBack();
        void GoForWard();
        UIPanel Load(Type type, string name);
        T Load<T>(string name) where T : UIPanel;
        UIPanel MemoryPeek();
        void Push(UIPanel ui);
        void PutCamera(Camera camera);
        void PutPanel(UIPanel panel);
        void SetCanvas(Canvas canvas);
        void SetGroups(IGroups groups);
    }
}