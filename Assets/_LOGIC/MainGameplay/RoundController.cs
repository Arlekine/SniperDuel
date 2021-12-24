using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    public Action<bool> onRoundEnd;
    
    [SerializeField] private Camera _camera;
    [SerializeField] private SniperScopeDistance _sniperScope;
    [SerializeField] private CoolText _coolText;

    [Header("UI")] 
    [SerializeField] private TextMeshProUGUI _aimForceText;
    [SerializeField] private TextMeshProUGUI _distanceText;
    [SerializeField] private TextMeshProUGUI _windText;
    [SerializeField] private GameObject _windDirectionRight;
    [SerializeField] private GameObject _windDirectionLeft;
    
    private Shooter _activeShooter;
    private Shooter _activeTarget;
    private List<float> _sniperScopeDistance;
    

    private bool _gameEnd;
    
    public void StartRound(Shooter firstShooter, Shooter secondShooter, ShootingCondition arenaCondition)
    {
        _gameEnd = false;
        _activeShooter = firstShooter;
        _activeTarget = secondShooter;

        _sniperScopeDistance = arenaCondition.sniperScopeDistance;
        
        _activeShooter.SetCamera(_camera);
        _activeShooter.SetTarget(_activeTarget.TargetPosition, arenaCondition);

        _sniperScope.enabled = true;
        _sniperScope.SetCamera(_camera);
        _sniperScope.SetGunSettings(_activeShooter.Gun, arenaCondition.sniperScopeDistance);
        
        _activeTarget.SetCamera(_camera);
        _activeTarget.SetTarget(_activeShooter.TargetPosition, arenaCondition);

        _activeShooter.SetReadyToAim();
        _activeTarget.SetInactive();

        _activeShooter.onShoot += Shoot;
        _activeTarget.onShoot += Shoot;

        Camera.main.transform.position = _activeShooter.shootingPos.position + _activeShooter.shootingPos.rotation * _activeShooter.ShoulderPosition.localPosition;
        Camera.main.transform.rotation = _activeShooter.ShoulderPosition.rotation;
        
        
        _activeShooter.Health.onDeath += () =>
        {
            _gameEnd = true;
            _sniperScope.enabled = false;
            onRoundEnd?.Invoke(false);
        };
        _activeTarget.Health.onDeath += () =>
        {
            _gameEnd = true;
            _sniperScope.enabled = false;
            onRoundEnd?.Invoke(true);
        };

        var distance = (_activeShooter.transform.position - _activeTarget.transform.position).magnitude;

        _aimForceText.text = $"x{_activeShooter.Gun.AimForce}";
        _distanceText.text = $"{Math.Round(distance, MidpointRounding.ToEven)} m";
        _windText.text = $"{arenaCondition.WindForce} m/s";
        _windDirectionRight.SetActive(arenaCondition.WindDirection == 1);
        _windDirectionLeft.SetActive(arenaCondition.WindDirection == -1);
    }

    private void Shoot(Bullet bullet)
    {
        bullet.onFreeCamera += FreeCamera;
        bullet.onTargetHit += ChangeActiveShooter;
    }

    private Sequence _freeSeq;
    
    private void FreeCamera(bool hitSomething, GameObject hitted)
    {
        _camera.transform.parent = null;
        
        _freeSeq?.Kill();
        _freeSeq = DOTween.Sequence();

        if (hitSomething)
        {
            _freeSeq.AppendCallback(() =>
            {
                if (hitted.GetComponent<GunTarget>() == null)
                {
                    var mO = hitted.GetComponent<MovingObject>();

                    if (mO == null && hitted.transform.parent != null)
                        mO = hitted.transform.parent.GetComponent<MovingObject>();

                    if (mO != null)
                        mO.enabled = false;
                    
                    
                    _coolText.Show("MISS!");
                }
            });
            _freeSeq.AppendInterval(2f);
            _freeSeq.AppendCallback(() =>
            {
                if (hitted.GetComponent<GunTarget>() == null)
                {
                    var mO = hitted.GetComponent<MovingObject>();

                    if (mO == null && hitted.transform.parent != null)
                        mO = hitted.transform.parent.GetComponent<MovingObject>();

                    if (mO != null)
                        mO.enabled = true;
                }
            });
        }
        else
        {
            _camera.transform.DOMove(_activeTarget.TargetPosition.position + (_activeShooter.TargetPosition.position - _activeTarget.TargetPosition.position).normalized * 10f, 0.5f);
            _camera.transform.DORotate(Quaternion.LookRotation((_activeTarget.TargetPosition.position - _activeShooter.TargetPosition.position)).eulerAngles, 0.5f);
        }
    }

    private void ChangeActiveShooter(GunTarget targetHitted, Vector3 hitPos)
    {
        if (_activeShooter is Player && targetHitted != null)
        {
            if (targetHitted.TargetDamage == 100)
            {
                _coolText.Show("HEADSHOT!");
            }
            else if (targetHitted.TargetDamage < 50)
            {
                _coolText.Show("COOL!");
            }
            else
            {
                _coolText.Show("AWESOME!");
            }
        }
        
        if (!_gameEnd)
            StartCoroutine(SwitchShooters());
    }

    private IEnumerator SwitchShooters()
    {
        yield return new WaitForSeconds(2f);
        
        var buffer = _activeShooter;
        _activeShooter = _activeTarget;
        _activeTarget = buffer;
        
        _activeTarget.SetInactive();
        _activeShooter.SetReadyToAim();

        _aimForceText.text = $"x{_activeShooter.Gun.AimForce}";
        _sniperScope.SetGunSettings(_activeShooter.Gun,  _sniperScopeDistance);
    }
}
