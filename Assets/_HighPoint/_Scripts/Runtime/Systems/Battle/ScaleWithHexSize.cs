using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Battle
{
    public class ScaleWithHexSize : MonoBehaviour
    {
        void Start()
        {
            transform.localScale *= HexGrid.Instance.HexSize;
        }
    }
}