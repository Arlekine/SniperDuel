using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShootingCondition
{
    public float WindForce;
    public int WindDirection;

    [Space] 
    [Range(300f, 1000f)]public float playerBulletSpeed;
    [Range(2, 32)]public int playerAimForce;
    [Range(300f, 1000f)] public float enemyBulletSpeed;
    [Range(2, 32)] public int enemyAimForce;
    
    [Space]
    public List<float> sniperScopeDistance;
}