using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sifter.UI
{
    [RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class PreferLayoutGroup : UIBehaviour, ILayoutElement
    {
        ILayoutElement layoutGroup;

        protected override void OnEnable()
        {
            base.OnEnable();
            LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());
        }

        protected override void OnDisable()
        {
            LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());
            base.OnDisable();
        }

        public float minWidth => GetLayoutGroup().minWidth;

        public float preferredWidth => GetLayoutGroup().preferredWidth;

        public float flexibleWidth => GetLayoutGroup().flexibleWidth;

        public float minHeight => GetLayoutGroup().minHeight;

        public float preferredHeight => GetLayoutGroup().preferredHeight;

        public float flexibleHeight => GetLayoutGroup().flexibleHeight;

        public int layoutPriority => 100;

        public void CalculateLayoutInputHorizontal()
        {
        }

        public void CalculateLayoutInputVertical()
        {
        }

        ILayoutElement GetLayoutGroup()
        {
            return layoutGroup ??= GetComponent<HorizontalOrVerticalLayoutGroup>();
        }
    }
}