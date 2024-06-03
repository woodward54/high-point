using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class FpsDisplay : MonoBehaviour
{
    TMP_Text _fpsText;

    const int FPS_SAMPLE_COUNT = 20;
    readonly int[] _fpsSamples = new int[FPS_SAMPLE_COUNT];
    int _sampleIndex;

    void Awake()
    {
        _fpsText = GetComponent<TMP_Text>();

        Application.targetFrameRate = -1;

        InvokeRepeating(nameof(UpdateFps), 0, 0.1f);
    }

    void Update()
    {
        _fpsSamples[_sampleIndex++] = (int)(1.0f / Time.deltaTime);
        if (_sampleIndex >= FPS_SAMPLE_COUNT) _sampleIndex = 0;
    }

    void UpdateFps()
    {
        var sum = 0;
        for (var i = 0; i < FPS_SAMPLE_COUNT; i++)
        {
            sum += _fpsSamples[i];
        }

        _fpsText.text = $"FPS: {sum / FPS_SAMPLE_COUNT}";
    }
}