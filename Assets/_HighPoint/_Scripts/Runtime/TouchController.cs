using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchController : SingletonPersistent<TouchController>
{
    public static Action<Vector2> OnTouch;
    public static Action<Vector2> OnUiTouch;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                CheckTouch(touch.position);
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            CheckTouch(Input.mousePosition);
        }
#endif
    }

    void CheckTouch(Vector2 touchPosition)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            OnUiTouch?.Invoke(touchPosition);
        }
        else
        {
            OnTouch?.Invoke(touchPosition);
        }
    }
}