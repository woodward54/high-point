using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Linq;
using Drawing;
using DG.Tweening;

public class DisableInBuild : MonoBehaviour
{
#if !UNITY_EDITOR
    void Start()
    {
        gameObject.SetActive(false);
    }
#endif
}