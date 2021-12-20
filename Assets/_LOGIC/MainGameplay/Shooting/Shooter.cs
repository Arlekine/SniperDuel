using System;
using DG.Tweening;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Action<Bullet> onShoot;

    public Transform TargetPosition;
    public Transform ShoulderPosition => _shoulderPosition;
    public Health Health;
    public Gun Gun => _gun;
    
    [Space]
    [SerializeField] protected Gun _gun;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected GameObject _shooterModel;
    [SerializeField] protected Transform _shoulderPosition;
    [SerializeField] protected Transform _aimPosition;
    
    [HideInInspector] public CanvasGroup aimGraphic; 
    [HideInInspector] public RectTransform aimCenter; 
    
    protected float _distance;
    protected ShootingCondition _currentShootingCondition;
    
    protected Vector3 _cameraAimingPoint;
    protected Vector3 _cameraAimingRotation;
    protected Vector3 _currentMoveVector;
    protected Vector3 _targetMoveVector;

    protected float _windOffset;
    protected float _distanceOffset;
    
    protected Camera _camera;
    protected Transform _currentTarget;

    protected Sequence _aimSequemce;
    protected Sequence _initialCameraSequence;
    protected bool _isMainAimingPart;
    
    public void SetCamera(Camera camera)
    {
        _camera = camera;
    }

    public void SetTarget(Transform target, ShootingCondition shootingCondition)
    {
        _currentTarget = target;
        var currentPos = transform.position;
        currentPos.y = target.transform.position.y;

        transform.forward = (target.transform.position - currentPos).normalized;
        
        _distance = (target.transform.position - currentPos).magnitude;
        _currentShootingCondition = shootingCondition;
        
        var flightTime = (_distance / _gun.BulletSpeed);
        
        _windOffset = flightTime * _currentShootingCondition.WindForce * _currentShootingCondition.WindDirection / 15;
        _distanceOffset = (flightTime*flightTime * 9.8f) / 2;
        
        print(_distanceOffset);
        
        _cameraAimingPoint = _currentTarget.position;
        _cameraAimingRotation = Quaternion.LookRotation(target.transform.position - _cameraAimingPoint).eulerAngles;
    }

    public virtual void SetReadyToAim()
    {
        _initialCameraSequence?.Kill();
        _initialCameraSequence = DOTween.Sequence();
        _initialCameraSequence.Append(_camera.transform.DOMove(_shoulderPosition.position, 1f));
        _initialCameraSequence.Join(_camera.transform.DORotate(_shoulderPosition.eulerAngles, 1f));
        _animator.SetTrigger("Aim");
    }

    public virtual void SetInactive()
    {
        _animator.SetTrigger("Idle");
    }

    public void StartAiming()
    {
        _isMainAimingPart = false;
        _initialCameraSequence?.Kill();
        
        _aimSequemce?.Kill();
        _aimSequemce = DOTween.Sequence();
        _aimSequemce.Append(_camera.transform.DOMove(_aimPosition.position, 1f).SetEase(Ease.InQuad));
        _aimSequemce.Join(_camera.transform.DORotate(Quaternion.LookRotation(_currentTarget.position - _aimPosition.position).eulerAngles, 1f).SetEase(Ease.InQuad));
        
        _aimSequemce.AppendCallback(() =>
        {
            aimGraphic.alpha = 1f;
            _shooterModel.SetActive(false);
            _camera.nearClipPlane = 1f;
        });
        
        _aimSequemce.Append(_camera.DOFieldOfView(60f / _gun.AimForce, 0.3f).SetEase(Ease.InQuad));
        
        _aimSequemce.AppendCallback(() =>
        {
            _isMainAimingPart = true;
            GunShootReady();
        });
    }

    protected virtual void GunShootReady()
    { }
}