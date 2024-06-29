using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ToastMessage : Singleton<ToastMessage>
{
    [SerializeField] TMP_Text _mainText;

    readonly float _fadeTime = 0.5f;
    float _nextMsgTime;
    readonly Queue<(string msg, float time)> _msgQueue = new();

    void Start()
    {
        _nextMsgTime = Time.time;
    }

    void Update()
    {
        if (Time.time > _nextMsgTime && _msgQueue.Count > 0)
        {
            var (msg, time) = _msgQueue.Dequeue();

            if (time > 0)
            {
                _nextMsgTime = Time.time + time + _fadeTime;
                ShowTimedMessage(msg, time);
            }
            else
            {
                _nextMsgTime = Time.time;
                ShowMessage(msg);
            }
        }
    }

    void ShowMessage(string msg)
    {
        _mainText.text = msg;
        _mainText.alpha = 1.0f;
    }

    void ClearText()
    {
        _mainText.text = "";
        _mainText.alpha = 0f;
    }

    void ShowTimedMessage(string msg, float visibleTime)
    {
        ShowMessage(msg);

        _mainText.DOFade(0f, _fadeTime).SetDelay(visibleTime).OnComplete(() =>
        {
            ClearText();
        });
    }

    public void EnqueueMessage(string msg, float displayTime = 2f, bool showInstantly = false)
    {
        // Avoid Duplicates
        if (_msgQueue.Any(m => m.msg == msg)) return;
        if (_mainText.text == msg) return;
        
        if (showInstantly)
        {
            _msgQueue.Clear();
            _nextMsgTime = Time.time;
        }

        _msgQueue.Enqueue((msg, displayTime));
    }
}