using System.Linq;
using UnityEngine;
using DG.Tweening;

public static class MaterialUtils
{
    public static void FlashColor(this MeshRenderer source, Color color, float duration, bool includeChildren)
    {
        var allMeshRenders =
            includeChildren ? source.GetComponentsInChildren<MeshRenderer>() 
                            : new MeshRenderer[] { source };

        foreach (var mr in allMeshRenders)
        {
            mr.material.DOComplete();
            
            var startColor = mr.material.color;
            var tintColor = color * startColor;

            // mr.material.DOColor(color * startColor, duration / 2f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutQuint);

            mr.material.color = tintColor;
            mr.material.DOColor(startColor, duration).SetEase(Ease.InExpo);
        }
    }
}