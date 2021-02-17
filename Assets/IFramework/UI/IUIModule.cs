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
        RectTransform common { get; }
        RectTransform guide { get; }
        RectTransform pop { get; }
        RectTransform toast { get; }
        RectTransform top { get; }
        RectTransform camera { get; }

        Canvas canvas { get; }
        UIPanel current { get; }
        int loaderCount { get; }
        int memoryCount { get; }
        int stackCount { get; }

        void AddLoader(IPanelLoader loader);
        void SetGroups(IGroups groups);
        void SetCanvas(Canvas canvas);
        void CreateCanvas();


        UIPanel Get(Type type, string name, UILayer layer = UILayer.Common);
        T Get<T>(string name, UILayer layer = UILayer.Common) where T : UIPanel;
        void GoBack();
        void GoForWard();


        void ClearCache();

        bool Exist(UIPanel panel);
        bool ExistInMemory(UIPanel panel);
        bool ExistInStack(UIPanel panel);
        bool HaveLoad(string panelName);
        UIPanel Load(Type type, string name, UILayer layer = UILayer.Common);
        T Load<T>(string name, UILayer layer = UILayer.Common) where T : UIPanel;
        UIPanel MemoryPeek();
        void Push(UIPanel ui);
        void SetCamera(Camera ca, bool isLast = true, int index = -1);
        void SetLayer(UIPanel ui, bool isLast = true, int index = -1);
    }
}