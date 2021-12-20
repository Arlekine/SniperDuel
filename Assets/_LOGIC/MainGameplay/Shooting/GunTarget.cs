using System;
using UnityEngine;

public class GunTarget : MonoBehaviour
{
    public Action<int> onDamageRecieved;
    
    [SerializeField] private int _targetDamage;

    public void Hit()
    {
        print($"{gameObject.name} damaged");
        onDamageRecieved?.Invoke(_targetDamage);
    }
}