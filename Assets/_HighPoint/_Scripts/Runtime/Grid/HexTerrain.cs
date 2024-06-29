using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class HexTerrain : MonoBehaviour
{
    // public event Action OnMouseEnterAction;
    // public event Action OnMouseExitAction;

    public event Action OnSelect;
    public event Action OnDeselect;

    // private Collider parentCollider;

    // void OnEnable()
    // {
    //     TouchController.Instance.OnTouch += HandleTouch;
    // }

    // void OnDisable()
    // {
    //     TouchController.Instance.OnTouch -= HandleTouch;
    // }

    private void Start()
    {
        // This was to disable all grass, tree, ect colliders
        // parentCollider = GetComponent<Collider>();

        // Disable collisions between the parent collider and all child colliders
        // Collider[] childColliders = GetComponentsInChildren<Collider>();
        // foreach (Collider childCollider in childColliders)
        // {
        //     childCollider.enabled = false;
        // }
        // parentCollider.enabled = true;
    }

    // private void HandleTouch(RaycastHit hit)
    // {
    //     if (hit.transform.gameObject == gameObject)
    //     {
    //         Select();
    //     }
    //     else
    //     {
    //         Deselect();
    //     }
    // }

    public void Select()
    {
        OnSelect?.Invoke();
    }

    public void Deselect()
    {
        OnDeselect?.Invoke();
    }

    // private void OnMouseEnter()
    // {
    //     OnMouseEnterAction?.Invoke();
    // }

    // private void OnMouseExit()
    // {
    //     OnMouseExitAction?.Invoke();
    // }
}