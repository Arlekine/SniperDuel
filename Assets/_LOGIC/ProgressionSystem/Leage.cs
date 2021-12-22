using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DATA/Leage", fileName = "Leage")]
public class Leage : ScriptableObject
{
    public Sprite icon;
    public Arena leageArena;
    public List<Enemy> enemyToSpawn = new List<Enemy>();
    
    [NonSerialized] public List<string> _enemiedNickNames = new List<string>();

    public void Init(int leageIndex)
    {
        for (int i = 0; i < enemyToSpawn.Count; i++)
        {
            if (!PlayerPrefs.HasKey("LeageEnemyNickname_" + leageIndex + "_" + i))
            {
                string name = Names.GetRandomName();
                _enemiedNickNames.Add(name);
                PlayerPrefs.SetString("LeageEnemyNickname_" + leageIndex + "_" + i, name);
            }
            else
            {
                string name = PlayerPrefs.GetString("LeageEnemyNickname_" + leageIndex + "_" + i);;
                _enemiedNickNames.Add(name);
            }
        }
    }
}
