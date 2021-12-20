using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Arena : MonoBehaviour
{
    [Serializable]
    public class ArenaPositions
    {
        public Transform PlayerPos;
        public Transform EnemyPos;
    }
    
    public List<ArenaPositions> arenaPositions = new List<ArenaPositions>();
    public ShootingCondition arenaShootingCondition;

    public ArenaPositions GetRandomPosition()
    {
        return arenaPositions[Random.Range(0, arenaPositions.Count)];
    }
}