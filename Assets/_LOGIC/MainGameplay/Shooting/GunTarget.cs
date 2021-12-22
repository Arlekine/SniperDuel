using System;
using UnityEngine;

public class GunTarget : MonoBehaviour
{
    public Action<int> onDamageRecieved;

    public int TargetDamage => _targetDamage;
    
    [SerializeField] private int _targetDamage;

    public int Hit()
    {
        print($"{gameObject.name} damaged");
        onDamageRecieved?.Invoke(_targetDamage);
        return _targetDamage;
    }
}