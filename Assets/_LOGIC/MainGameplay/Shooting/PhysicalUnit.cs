using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class PhysicalUnit : MonoBehaviour
{
    [Space]
    [SerializeField] private Rigidbody[] _rigidbodies;
    [SerializeField] private Collider[] _colliders;
    [SerializeField] private CharacterJoint[] _joints;
    
    private Vector3 _lastHitDirection;
    
    private void Start()
    {
        ActivateRagdoll(false);
    }
    
    public void ActivateRagdoll(bool isActive)
    {
        for (int i = 0; i < _rigidbodies.Length; i++)
        {
            _rigidbodies[i].isKinematic = !isActive;
        }
    }

    public void SetLastHitDirection(Vector3 impulseDirection)
    {
        _lastHitDirection = impulseDirection.normalized;
    }

    public void FinalImpulse()
    {
        if (_lastHitDirection != Vector3.zero)
        {
            _rigidbodies[0].AddForce(_lastHitDirection * 20f + Vector3.up, ForceMode.Impulse);
        }

    }
    
    public void RemovePhysics()
    {
        for (int i = 0; i < _joints.Length; i++)
        {
            Destroy(_joints[i]);
        }
        
        for (int i = 0; i < _rigidbodies.Length; i++)
        {
            Destroy(_rigidbodies[i]);
        }
        
        for (int i = 0; i < _colliders.Length; i++)
        {
            Destroy(_colliders[i]);
        }
    }
}
