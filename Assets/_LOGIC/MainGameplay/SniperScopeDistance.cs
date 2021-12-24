using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SniperScopeDistance : MonoBehaviour
{
    [Serializable]
    private class SniperScopePlashka
    {
        public RectTransform Point;
        public TextMeshProUGUI DistanceText;
    }
    
    [SerializeField] private List<SniperScopePlashka> _sniperScopePlashkas = new List<SniperScopePlashka>();
    [SerializeField] private RectTransform _aimCenter;
    [SerializeField] private RectTransform _realTargetCenter;

    private Camera _camera;
    private Gun _targetGun;
    private List<float> _plashkasDistances;
    private float? _aimOffset;
    private Vector2 _aimInitialPos;

    private void Awake()
    {
        _aimInitialPos = _aimCenter.anchoredPosition;
    }

    public void SetCamera(Camera camera)
    {
        _camera = camera;
    }

    public void SetGunSettings(Gun gun, List<float> plashkasDistances)
    {
        _targetGun = gun;
        _plashkasDistances = plashkasDistances;
        _aimOffset = 0;
        _aimCenter.anchoredPosition = _aimInitialPos;
    }

    private void Update()
    {
        var plashkasCount = _sniperScopePlashkas.Count < _plashkasDistances.Count
            ? _sniperScopePlashkas.Count
            : _plashkasDistances.Count;

        _aimOffset = _aimInitialPos.y - _sniperScopePlashkas[0].Point.anchoredPosition.y;
        _aimCenter.anchoredPosition = new Vector2(_aimCenter.anchoredPosition.x, _aimCenter.anchoredPosition.y + _aimOffset.Value);
        
        for (int i = 0; i < _sniperScopePlashkas.Count; i++)
        {
            bool plashkaActive = i < plashkasCount;
            _sniperScopePlashkas[i].Point.gameObject.SetActive(plashkaActive);
            if (plashkaActive)
            {
                Vector2 size = Vector2.Scale(_aimCenter.rect.size, transform.lossyScale);
                var rect = new Rect((Vector2)_aimCenter.position - (size * _aimCenter.pivot), size);
        
                var shootingRay = _camera.ScreenPointToRay(rect.center);

                var flightTime = (_plashkasDistances[i] / _targetGun.BulletSpeed);
                var distanceOffset = (flightTime*flightTime * 9.8f) / 2;

                _sniperScopePlashkas[i].Point.position = _camera.WorldToScreenPoint(shootingRay.origin - Vector3.up * distanceOffset + shootingRay.direction * _plashkasDistances[i]);
                _sniperScopePlashkas[i].DistanceText.text = $"{_plashkasDistances[i]} m";
            }
        }
    }
}

[Serializable]
public class GunSettings
{
    public int aimForce;
    public float bulletSpeed;
    public List<float> plashkasDistances = new List<float>();
}
