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

    private Camera _camera;
    private Gun _targetGun;
    private List<float> _plashkasDistances;
    
    public void SetCamera(Camera camera)
    {
        _camera = camera;
    }

    public void SetGunSettings(Gun gun, List<float> plashkasDistances)
    {
        _targetGun = gun;
        _plashkasDistances = plashkasDistances;
    }

    private void Update()
    {
        var plashkasCount = _sniperScopePlashkas.Count < _plashkasDistances.Count
            ? _sniperScopePlashkas.Count
            : _plashkasDistances.Count;

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

                _sniperScopePlashkas[i].Point.position = _camera.WorldToScreenPoint(shootingRay.origin - Vector3.up * distanceOffset + shootingRay.direction * 100f);
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
