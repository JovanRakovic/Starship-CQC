using UnityEngine;
using UnityEngine.UI;

public static class UITools
{
    public static void ToggleCanvasGroup(CanvasGroup group, bool toggle)
    {
        group.alpha = toggle? 1 : 0;
        group.blocksRaycasts = toggle;
        group.interactable = toggle;
    }
}