using System.Collections;
using DG.Tweening;
using Lean.Touch;
using TMPro;
using UnityEngine;

public class Player : Shooter
{
    [Header("Player")]
    [SerializeField] private float _fingerScreenMoveMultiplayer;
    [SerializeField] private float _controlLerp;
    [SerializeField] private float _breathForce = 0.0025f;
    [SerializeField] private float _breathHalfCicleTime = 1.5f;
    [SerializeField] private float _moveArea = 1f;
    [SerializeField] private Transform _aimingMoveCenter;
    
    private Vector3 _breath;
    private LeanFinger _leanFinger;
    private bool _readyToShoot;

    public override void SetReadyToAim()
    {
        base.SetReadyToAim();
        
        LeanTouch.OnFingerDown += StartAim;
        LeanTouch.OnFingerUp += EndAim;
    }

    public override void SetInactive()
    {
        base.SetInactive();
    }

    protected override void GunShootReady()
    {
        base.GunShootReady();
        
        _readyToShoot = true;
        StartCoroutine(BreathRoutine());
        _currentMoveVector = _currentTarget.position;
        _targetMoveVector = _currentTarget.position;
    }

    private void StartAim(LeanFinger finger)
    {
        if (_leanFinger == null)
        {
            _leanFinger = finger;
            
            StartAiming();
        }
    }

    private void EndAim(LeanFinger finger)
    {
        if (finger == _leanFinger)
        {
            if (_isMainAimingPart)
            {
                
                _aimSequemce?.Kill();
                Shoot();
                _leanFinger = null;
                aimGraphic.DOFade(0f, 0.05f);

                LeanTouch.OnFingerDown -= StartAim;
                LeanTouch.OnFingerUp -= EndAim;
            }
            else
            {
                _aimSequemce?.Kill();

                _leanFinger = null;
                
                _initialCameraSequence?.Kill();
                _initialCameraSequence = DOTween.Sequence();
                _initialCameraSequence.Append(_camera.transform.DOMove(_shoulderPosition.position, 2f));
                _initialCameraSequence.Join(_camera.transform.DORotate(_shoulderPosition.eulerAngles, 2f));
                
                aimGraphic.DOFade(0f, 0.05f);
            }
        }
    }

    private void Shoot()
    {
        print("shoot");
        StopAllCoroutines();
        _readyToShoot = false;

        Vector2 size = Vector2.Scale(aimCenter.rect.size, transform.lossyScale);
        var rect = new Rect((Vector2)aimCenter.position - (size * aimCenter.pivot), size);
        
        var shootingRay = _camera.ScreenPointToRay(rect.center);
        var initalOrigin = shootingRay.origin;
        
        shootingRay.origin = shootingRay.origin + shootingRay.direction.normalized * 5f + transform.right * _windOffset - Vector3.up * _distanceOffset;
                
        Vector3 shootPoint;
        GunTarget shootTarget;
        RaycastHit hit;

        if (Physics.Raycast(shootingRay, out hit))
        {
            var gunTarget = hit.collider.GetComponent<GunTarget>();
            if (gunTarget)
            {
                shootPoint = hit.point;
                shootTarget = gunTarget;
            }
            else
            {
                if ((_camera.transform.position - hit.point).magnitude > (transform.position - _currentTarget.transform.position).magnitude)
                    shootPoint = initalOrigin + shootingRay.direction.normalized * (initalOrigin - _currentTarget.transform.position).magnitude;
                else
                    shootPoint = hit.point;
                shootTarget = null;
            }
        }
        else
        {
            shootPoint = initalOrigin + shootingRay.direction.normalized * (initalOrigin - _currentTarget.transform.position).magnitude;
            shootTarget = null;
        }


        print(shootPoint);
        
        _shooterModel.SetActive(true);
        var bullet = _gun.Shoot(shootPoint, shootTarget, _camera);
        onShoot?.Invoke(bullet);
    }

    private void Update()
    {
        _animator.transform.localPosition = Vector3.zero;
        _animator.transform.localEulerAngles = Vector3.zero;
        Vector2 size = Vector2.Scale(aimCenter.rect.size, transform.lossyScale);
        var rect = new Rect((Vector2)aimCenter.position - (size * aimCenter.pivot), size);
        
        var shootingRay = _camera.ScreenPointToRay(rect.center);

        var flightTime = (100f / _gun.BulletSpeed);
        var distanceOffset = (flightTime*flightTime * 9.8f) / 2;

        
        Debug.DrawRay(shootingRay.origin, shootingRay.direction * 10000f, Color.green);
        
        shootingRay.origin = shootingRay.origin + shootingRay.direction.normalized * 5f + Vector3.right * _windOffset - Vector3.up * _distanceOffset;
        
        Debug.DrawRay(shootingRay.origin, shootingRay.direction * 10000f, Color.red);
        
        if (_readyToShoot && _leanFinger != null)
        {
            
            var fingerDelta = _leanFinger.ScreenDelta;

            //TODO: make resolution independent
            fingerDelta /= _fingerScreenMoveMultiplayer;

            _targetMoveVector += (-_currentTarget.right * fingerDelta.x);
            _targetMoveVector.y += fingerDelta.y;
            _targetMoveVector += _breath * _breathForce;

            _currentMoveVector = Vector3.Lerp(_currentMoveVector, _targetMoveVector, Time.deltaTime * _controlLerp);
            //_targetMoveVector = (_targetMoveVector - _cameraCenterPoint).normalized * Vector3.ClampMagnitude(_targetMoveVector, 1f).magnitude;

            var aimToCurrent = _currentMoveVector - _currentTarget.position;
            aimToCurrent = Vector3.ClampMagnitude(aimToCurrent, _moveArea);
            _currentMoveVector = _currentTarget.position + aimToCurrent;
            
            _camera.transform.forward = (_currentMoveVector - (_camera.transform.position));
        }
    }

    IEnumerator BreathRoutine()
    {
        int sign = 1;
        while (true)
        {
            _breath += sign * (Vector3.up * Time.deltaTime * _breathHalfCicleTime + Vector3.right * Time.deltaTime * Random.Range(-0.05f * _breathHalfCicleTime, 0.05f * _breathHalfCicleTime));

            if (sign == 1 && _breath.y >= 1)
                sign = -1;
            else if (sign == -1 && _breath.y <= -1)
                sign = 1;

            yield return null;
        }
    }
}