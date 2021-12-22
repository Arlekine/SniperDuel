using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Action<GunTarget, Vector3> onTargetHit;
    public Action<bool, GameObject> onFreeCamera;
    
    [HideInInspector]
    public float Speed;
    
    [SerializeField] private ParticleSystem _blood;
    [SerializeField] private ParticleSystem _sparks;
    [SerializeField] private Collider _collider;
    [SerializeField] private Transform _cameraPos;
    [SerializeField] private Transform _cameraPos2;
    [SerializeField] private float _cameraStopDistance;
    [SerializeField] private float _bulletDestractionRadius;
    [SerializeField] private float _destractionForce;

    private Camera _camera;
    private Vector3 _targetPos;
    private GunTarget _target;
    private Vector3 _cameraStopTarget;
    private float _currentSpeed;
    private Sequence _sequence;
    private Sequence _slowMotionSequence;
    private Sequence _oneMoreSequence;
    
    public void Shoot(Vector3 targetPos, GunTarget target, Camera camera, ParticleSystem shootEffect)
    {
        enabled = false;
        transform.forward = (targetPos - transform.position).normalized;
        _targetPos = targetPos;
        _camera = camera;
        _target = target;

        _cameraStopTarget = (transform.position - _targetPos).normalized * _cameraStopDistance;

        _sequence = DOTween.Sequence();
        _sequence.AppendCallback(() =>
        {
            _collider.enabled = false;
            _camera.transform.parent = _cameraPos;
            _camera.nearClipPlane = 0.1f;
        });
        
        _sequence.Append(_camera.DOFieldOfView(60f, 0f).SetEase(Ease.InExpo));
        _sequence.Join(_camera.transform.DOLocalMove(Vector3.zero, 0f));
        _sequence.Join(_camera.transform.DOLocalRotate(Vector3.zero, 0f));
        _sequence.AppendCallback(() =>
        {
            shootEffect.Play();
            _slowMotionSequence = DOTween.Sequence();
            
            _slowMotionSequence.AppendCallback(() =>
            {
                enabled = true;
                _currentSpeed = 0.5f;
                Time.timeScale = 0.1f;
                _slowMotionSequence.timeScale = 10f; 
            });
                    
            _slowMotionSequence.AppendInterval(1f);
            _slowMotionSequence.AppendCallback(() =>
            {
                if (_camera.transform.parent != null)
                {
                    _oneMoreSequence = DOTween.Sequence();
                    _oneMoreSequence.AppendCallback(() => { _camera.transform.parent = _cameraPos2; });
                    _oneMoreSequence.Append(_camera.transform.DOLocalMove(Vector3.zero, 0.5f));
                    _oneMoreSequence.Join(_camera.transform.DOLocalRotate(Vector3.zero, 0.5f));
                }
            });

            _slowMotionSequence.AppendInterval(1f);
            _slowMotionSequence.AppendCallback(() =>
                {
                    _collider.enabled = true;
                });
            
            _slowMotionSequence.Append(DOTween.To(() => 0.1f, x =>
                {
                    _currentSpeed = x * Speed;
                    Time.timeScale = x;
                    _slowMotionSequence.timeScale = 1 / x;
                }, 100f / Speed, 0.5f));
        });
    }

    private void Update()
    {
        transform.position = transform.position + transform.forward * _currentSpeed * Time.deltaTime;

        RaycastHit hit;
        bool hitSomething = (Physics.Raycast(transform.position, transform.forward, out hit, _currentSpeed * Time.deltaTime));

        if (_camera.transform.parent != null && (transform.position - _targetPos).magnitude < _cameraStopDistance)
        {
            _oneMoreSequence?.Kill();
            _camera.transform.parent = null;
            onFreeCamera?.Invoke(false, null);
        }

        if (hitSomething || (transform.position - _targetPos).magnitude <= _currentSpeed * Time.deltaTime)
        {
            if ((transform.position - _targetPos).magnitude <= _currentSpeed * Time.deltaTime)
                hitSomething = false;
            
            if (hitSomething)
                _target = hit.collider.GetComponent<GunTarget>();
            
            if (_camera.transform.parent != null)
            {
                _oneMoreSequence?.Kill();
                
                _camera.transform.parent = null;
                
                if(hitSomething)
                    onFreeCamera?.Invoke(true, hit.collider.gameObject);
                else
                    onFreeCamera?.Invoke(false, null);
            }

            if (_target != null)
            {
                var damage = _target.Hit();
                var bl = Instantiate(_blood, _targetPos, Quaternion.identity);

                bl.transform.forward = transform.position - _targetPos;
        
                _sequence?.Kill();
                _slowMotionSequence?.Kill();
                _oneMoreSequence?.Kill();

                onTargetHit?.Invoke(_target, _targetPos);
                
                Time.timeScale = 1f;
                Destroy(gameObject);
            }
            else
            {
                _sequence?.Kill();
                _slowMotionSequence?.Kill();
                _oneMoreSequence?.Kill();

                if (hitSomething)
                {
                    _targetPos = hit.point;
                    
                    var bl = Instantiate(_sparks, _targetPos, Quaternion.identity);
                    bl.transform.forward = transform.position - _targetPos;
                }

                var colliders = Physics.OverlapSphere(_targetPos, _bulletDestractionRadius);
                var affectedBodies = new List<Rigidbody>();
            
                foreach (var col in colliders)
                {
                    var part = col.GetComponent<DestractablePart>();
                    if (part != null)
                    {
                        part.Destroy();
                        affectedBodies.Add(part.Rigidbody);
                    }
                }

                foreach (var body in affectedBodies)
                {
                    body.AddExplosionForce(_destractionForce, transform.position, _bulletDestractionRadius);
                }
                
                Time.timeScale = 1f;
                onTargetHit?.Invoke(null, _targetPos);
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        _target = other.collider.GetComponent<GunTarget>();
        
        if (_camera.transform.parent != null)
        {
            _oneMoreSequence?.Kill();
            _camera.transform.parent = null;
            onFreeCamera?.Invoke(true, other.collider.gameObject);
        }
        
        if (_target != null)
        {
            _sequence?.Kill();
            _slowMotionSequence?.Kill();
            _oneMoreSequence?.Kill();
            _target.Hit();
            var bl = Instantiate(_blood, _targetPos, Quaternion.identity);

            bl.transform.forward = transform.position - _targetPos;
            onTargetHit?.Invoke(_target, other.contacts[0].point);
                
            Time.timeScale = 1f;
            Destroy(gameObject);
        }
        else
        {
            _sequence?.Kill();
            _slowMotionSequence?.Kill();
            _oneMoreSequence?.Kill();
            Time.timeScale = 1f;
            onTargetHit?.Invoke(null, other.contacts[0].point);

            var colliders = Physics.OverlapSphere(transform.position, _bulletDestractionRadius);
            var affectedBodies = new List<Rigidbody>();
            
            foreach (var col in colliders)
            {
                var part = GetComponent<DestractablePart>();
                if (part != null)
                    part.Destroy();
            }

            foreach (var body in affectedBodies)
            {
                body.AddExplosionForce(_destractionForce, transform.position, _bulletDestractionRadius);
            }
            
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * _currentSpeed);
    }
}