using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Action onDeath;
    
    [HideInInspector] public Slider healthUI;
    
    [SerializeField] private int _maxHealth;
    [SerializeField] private PhysicalUnit _physicalUnit;
    [SerializeField] private Animator _animator;

    [SerializeField] private int _currentHealth;
    private List<GunTarget> _gunTargets = new List<GunTarget>();

    private void Start()
    {
        _currentHealth = _maxHealth;
        _gunTargets = GetComponentsInChildren<GunTarget>().ToList();
        
        foreach (var guntarget in _gunTargets)
        {
            guntarget.onDamageRecieved += ReceiveDamage;
        }
    }

    private void ReceiveDamage(int damage)
    {
        _currentHealth -= damage;
        healthUI.value = (float)_currentHealth / (float)_maxHealth;

        if (_currentHealth <= 0)
        {
            _animator.enabled = false;
            _physicalUnit.SetLastHitDirection(-transform.forward);
            _physicalUnit.ActivateRagdoll(true);
            _physicalUnit.FinalImpulse();
            onDeath?.Invoke();
        }
        else
        {
            _animator.SetTrigger("Hit");
        }
    }
}