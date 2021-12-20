using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Enemy : Shooter
{
    [Space] 
    [HideInInspector] public GameObject enemyShootingUI;

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

        var time = 0f;

        var targets = GetRandomTargetPoints(5);
        
        _currentMoveVector = _cameraAimingPoint + (_currentTarget.position - _aimPosition.position);
        _targetMoveVector = targets[0];
        
        yield return new WaitForSeconds(1.3f);

        set = true;
        
        for (int i = 0; i < 5; i++)
        {
            print(targets[i]);
            _targetMoveVector = targets[i];
            
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

            if (i != amount - 1)
            {
                perfectTarget.x += Random.Range(-0.75f, 0.75f);
                perfectTarget.y += Random.Range(-1.5f, 2f);
            }
            else
            {
                perfectTarget.x += Random.Range(-0.3f, 0.3f);
                perfectTarget.y += Random.Range(-0.5f, 0.5f);
            }

            print(perfectTarget);
            _randomAimingPositions.Add(perfectTarget);
        }

        return _randomAimingPositions;
    }

    private void Update()
    {
        if (set)
        {
            _currentMoveVector = Vector3.Lerp(_currentMoveVector, _targetMoveVector, Time.deltaTime * 0.75f);
            _camera.transform.forward = (_currentMoveVector - _aimPosition.position).normalized;
        }
        
        _animator.transform.localPosition = Vector3.zero;
        _animator.transform.localEulerAngles = Vector3.zero;
        //_animator.transform.localEulerAngles = Vector3.zero;
        Vector2 size = Vector2.Scale(aimCenter.rect.size, transform.lossyScale);
        var rect = new Rect((Vector2)aimCenter.position - (size * aimCenter.pivot), size);
        
        var shootingRay = _camera.ScreenPointToRay(rect.center);

        var flightTime = (100f / _gun.BulletSpeed);
        var distanceOffset = (flightTime*flightTime * 9.8f) / 2;
    }

    private void Shoot()
    {
        print("shoot");
        StopAllCoroutines();

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
            shootPoint = initalOrigin + shootingRay.direction.normalized * (initalOrigin- _currentTarget.transform.position).magnitude;
            shootTarget = null;
        }


        print(shootPoint);

        _shooterModel.SetActive(true);
        var bullet = _gun.Shoot(shootPoint, shootTarget, _camera);
        onShoot?.Invoke(bullet);
    }
}
