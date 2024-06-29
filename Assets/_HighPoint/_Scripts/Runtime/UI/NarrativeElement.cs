using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Systems.SceneManagement;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class NarrativeElement : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    public float DisplayTime = 8f;

    void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
    }
}