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
        public Transform PlayerHidingPos;
        public Transform EnemyPos;
        public Transform EnemyHidingPos;
    }
    
    public List<ArenaPositions> arenaPositions = new List<ArenaPositions>();
    public ShootingCondition arenaShootingCondition;

    public ArenaPositions GetPosition(int index)
    {
        if (index >= arenaPositions.Count)
            index -= arenaPositions.Count;
        return arenaPositions[index];
    }
}