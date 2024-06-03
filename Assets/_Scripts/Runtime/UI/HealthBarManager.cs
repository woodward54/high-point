using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.Entities;
using Unity.Transforms;

public class HealthBar
{
    readonly Unit _unit;
    readonly CanvasGroup _healthBar;
    readonly Image _barImage;
    Vector3 _offset;

    float _currentHealthPercent;
    float _lastUpdateTime;

    public HealthBar(Unit unit, CanvasGroup healthBar, Image barImage, float yOffset)
    {
        _unit = unit;
        _healthBar = healthBar;
        _barImage = barImage;
        _offset = new Vector3(0f, yOffset, 0f);

        _currentHealthPercent = 1f;

        _lastUpdateTime = Time.time;
    }

    public void SetHealthPercent(float percent)
    {
        _currentHealthPercent = math.clamp(percent, 0f, 1f);

        var s = _barImage.transform.localScale;
        s.x = _currentHealthPercent;
        _barImage.transform.localScale = s;

        // Only fade in the health bar if the percent is above 0
        if (_currentHealthPercent <= 0f) return;

        _lastUpdateTime = Time.time;
    }

    public void UpdatePosition(Transform camera)
    {
        _healthBar.transform.position = _unit.transform.position + _offset;

        var target = camera.position;
        target.y = _healthBar.transform.position.y;
        _healthBar.transform.LookAt(target);
    }

    public void UpdateFadeOut(float currentTime, float fadeOutTime)
    {
        var t = currentTime - (_lastUpdateTime + fadeOutTime);
        _healthBar.alpha = Mathf.Clamp(Mathf.SmoothStep(1f, 0f, t), 0, 1);
    }

    public void Destroy()
    {
        UnityEngine.Object.Destroy(_healthBar.gameObject);
    }
}

public class HealthBarManager : Singleton<HealthBarManager>
{
    [SerializeField] Transform _healthBarPrefab;
    [SerializeField] Transform _camera;
    [SerializeField] float _fadeOutTime = 1.0f;

    EventBinding<UnitSpawnEvent> SpawnedUnitBinding;
    EventBinding<UnitDeathEvent> DeadUnitBinding;

    readonly Dictionary<Unit, HealthBar> _healthBars = new();

    void OnEnable()
    {
        SpawnedUnitBinding = new EventBinding<UnitSpawnEvent>(HandleUnitSpawn);
        Bus<UnitSpawnEvent>.Register(SpawnedUnitBinding);

        DeadUnitBinding = new EventBinding<UnitDeathEvent>(HandleUnitDead);
        Bus<UnitDeathEvent>.Register(DeadUnitBinding);
    }

    void OnDisable()
    {
        Bus<UnitSpawnEvent>.Unregister(SpawnedUnitBinding);
        Bus<UnitDeathEvent>.Unregister(DeadUnitBinding);
    }

    private void HandleUnitDead(UnitDeathEvent @event)
    {
        DestroyHealthBar(@event.Unit);
    }

    private void HandleUnitSpawn(UnitSpawnEvent @event)
    {
        CreateHealthBar(@event.Unit);
    }

    public void CreateHealthBar(Unit owner)
    {
        var healthBar = Instantiate(_healthBarPrefab, Vector3.zero, Quaternion.identity, transform);
        var healthBarCanvas = healthBar.GetComponent<CanvasGroup>();
        healthBar.localScale *= HexGrid.Instance.HexSize;
        var healthBarChildImg = healthBar.GetComponentsInChildren<Image>().First(c => c.transform != healthBar);

        float yOffset = HexGrid.Instance.HexSize * owner.UnitConfig.HealthBarOffset;

        var bar = new HealthBar(owner, healthBarCanvas, healthBarChildImg, yOffset);

        owner.RegisterHealthBar(bar);

        _healthBars.Add(owner, bar);
    }

    private void DestroyHealthBar(Unit Unit)
    {
        _healthBars[Unit].Destroy();
        _healthBars.Remove(Unit);
    }

    void Update()
    {
        var curTime = Time.time;
        foreach (var bar in _healthBars.Values)
        {
            bar.UpdatePosition(_camera);
            bar.UpdateFadeOut(curTime, _fadeOutTime);
        }
    }

    void ClearHealthBars()
    {
        foreach (var bar in _healthBars.Values)
        {
            bar.Destroy();
        }

        _healthBars.Clear();
    }
}