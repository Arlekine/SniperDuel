using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Shooter
{
    [Space] 
    [HideInInspector] public GameObject enemyShootingUI;
    [HideInInspector] public RectTransform enemyAimCenter;
    [SerializeField] private LayerMask _shootingLayerMask;

    [Space] 
    [SerializeField] private float _minX = -0.3f;
    [SerializeField] private float _maxX = 0.3f;
    [SerializeField] private float _minY = -0.5f;
    [SerializeField] private float _maxY = 0.5f;
    
    private bool set;
    
    public override void SetReadyToAim()
    {
        base.SetReadyToAim();
        
        StartCoroutine(ShootingRoutine());
        enemyShootingUI.SetActive(true);
    }

    public override void SetInactive()
    {
        base.SetInactive();
        enemyShootingUI.SetActive(false);
    }

    private IEnumerator ShootingRoutine()
    {
        yield return new WaitForSeconds(3f);

        StartAiming();
        _animator.SetTrigger("Aim");
        
        var time = 0f;

        var targets = GetRandomTargetPoints(5);
        
        _currentMoveVector = _cameraAimingPoint + (_currentTarget.position - _aimPosition.position);
        _targetMoveVector = targets[0];
        
        yield return new WaitForSeconds(1.3f);

        set = true;
        
        for (int i = 0; i < 5; i++)
        {
            Vector2 size = Vector2.Scale(aimCenter.rect.size, transform.lossyScale);
            var rect = new Rect((Vector2)aimCenter.position - (size * aimCenter.pivot), size);
            
            var shootingRay = _camera.ScreenPointToRay(rect.center);
            var shootingRay2 = _camera.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f));

            float diff = (shootingRay.origin + shootingRay.direction.normalized * _distance).y - (shootingRay2.origin + shootingRay2.direction.normalized * _distance).y;
            
            _targetMoveVector = targets[i];
            
            _targetMoveVector.y -= diff;
            
            yield return new WaitForSeconds( i != 4 ? 1f : 2f);
        }

        aimGraphic.DOFade(0f, 0.05f);
        set = false;
        Shoot();
    }

    private List<Vector3> GetRandomTargetPoints(int amount)
    {
        List<Vector3> _randomAimingPositions = new List<Vector3>();

        for (int i = 0; i < amount; i++)
        {
            var perfectTarget = _currentTarget.position + Vector3.up * _distanceOffset - transform.right * _windOffset;
            
            print("-------");
            print("Target pos - " + _currentTarget.position);
            print("Paerfect point - " + perfectTarget);

            if (i != amount - 1)
            {
                perfectTarget.x += Random.Range(-0.4f, 0.4f);
                perfectTarget.y += Random.Range(-0.5f, 1f);
            }
            else
            {
                perfectTarget.x += Random.Range(_minX, _maxX);
                perfectTarget.y += Random.Range(_minY, _maxY);
            }
            
            
            print("Paerfect point random - " + perfectTarget);
            print("-------");
            _randomAimingPositions.Add(perfectTarget);
        }

        return _randomAimingPositions;
    }

    private void Update()
    {
        if (set)
        {
            _currentMoveVector = Vector3.Lerp(_currentMoveVector, _targetMoveVector, Time.deltaTime * 0.75f);
            _camera.transform.forward = (_currentMoveVector - _camera.transform.position).normalized;
        }
        
        _animator.transform.localPosition = Vector3.zero;
        _animator.transform.localEulerAngles = Vector3.zero;
    }

    private void Shoot()
    {
        print("shoot");
        StopAllCoroutines();

        _camera.nearClipPlane = 0.01f;
        Vector2 size = Vector2.Scale(aimCenter.rect.size, transform.lossyScale);
        var rect = new Rect((Vector2)aimCenter.position - (size * aimCenter.pivot), size);
        
        var shootingRay = _camera.ScreenPointToRay(rect.center);
        
        shootingRay.origin = shootingRay.origin + transform.right * _windOffset - Vector3.up * _distanceOffset;
        var initalOrigin = shootingRay.origin;
        shootingRay.origin = shootingRay.origin + shootingRay.direction.normalized * 5f;
        
        Vector3 shootPoint;
        GunTarget shootTarget;
        RaycastHit hit;

        if (Physics.Raycast(shootingRay, out hit, 3000f, _shootingLayerMask))
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
                    shootPoint = initalOrigin + shootingRay.direction.normalized * ((initalOrigin - _currentTarget.transform.position).magnitude + 3f);
                else
                    shootPoint = hit.point;
                shootTarget = null;
            }
        }
        else
        {
            shootPoint = initalOrigin + shootingRay.direction.normalized * ((initalOrigin - _currentTarget.transform.position).magnitude + 3f);
            shootTarget = null;
        }


        print(shootPoint);

        _shooterModel.SetActive(true);
        var bullet = _gun.Shoot(shootPoint, shootTarget, _camera);
        onShoot?.Invoke(bullet);
    }

    private void LateUpdate()
    {
        _animator.transform.localPosition = Vector3.zero;
        _animator.transform.localEulerAngles = Vector3.zero;
    }
}
