using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Touch;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float AimForce = 5f;
    public float BulletSpeed;
    
    [SerializeField] private Bullet _bullet;
    [SerializeField] private Transform _bulletParent;
    [SerializeField] private ParticleSystem _shootEffect;

    public Bullet Shoot(Vector3 shootPoint, GunTarget target, Camera _camera)
    {
        var newBullet = Instantiate(_bullet, _bulletParent.position, Quaternion.identity);
        
        newBullet.Speed = BulletSpeed;
        newBullet.Shoot(shootPoint, target, _camera, _shootEffect);
        
        return newBullet;
    }
}
