using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using Niantic.Lightship.AR;
using Niantic.Lightship.AR.VpsCoverage;
using UnityEngine;
using UnityEngine.Networking;

public class VpsMarker : MapMarker, ISelectable
{
    [field: SerializeField] public BaseConfig BaseConfig { get; private set; }
    [SerializeField] MeshRenderer _castle;
    [SerializeField] MeshRenderer _hexFrame;
    [SerializeField] MeshRenderer _hexImg;
    [SerializeField] List<MeshRenderer> _tiles;

    public LocalizationTarget VpsTarget { get; private set; }
    public bool IsLocationCaptured { get; private set; }

    public string Identifier => VpsTarget.Identifier;

    bool _hasDownloadedTexture;

    void Awake()
    {
        FadeHexFrame(0f, 0f);
    }

    public void Setup(LocalizationTarget vpsTarget)
    {
        VpsTarget = vpsTarget;
        _hasDownloadedTexture = false;

        if (Player.Instance.DoesPlayerControlWayspot(vpsTarget.Identifier))
        {
            SetMarkerTeamColor(Player.Instance.Color);
            IsLocationCaptured = true;
        }
        else
        {
            // Default Barbarian Color
            var barbColor = new Color(0.70f, 0.09f, 0f);
            SetMarkerTeamColor(barbColor);
        }
    }

    public void Select()
    {
        StartCoroutine(SetTexture(VpsTarget.ImageURL));

        if (_hasDownloadedTexture)
            FadeHexFrame(1f);
    }

    public void Unselect()
    {
        FadeHexFrame(0f);
    }

    void FadeHexFrame(float alpha, float fadeTime = 0.5f)
    {
        _hexFrame.material.DOFade(alpha, fadeTime);
        _hexImg.material.DOFade(alpha, fadeTime);
    }

    public void SetMarkerTeamColor(Color color)
    {
        _castle.materials[1].color = color;
        _hexFrame.material.color = color;

        _tiles.ForEach(t => t.material.color = color);
    }

    IEnumerator SetTexture(string url)
    {
        if (_hasDownloadedTexture) yield break;

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            yield break;
        }

        Texture myTexture = DownloadHandlerTexture.GetContent(www);
        _hexImg.material.mainTexture = myTexture;
        _hasDownloadedTexture = true;
        FadeHexFrame(1f);
    }
}