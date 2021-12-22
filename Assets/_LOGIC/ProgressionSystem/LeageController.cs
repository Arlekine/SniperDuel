using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeageController : MonoBehaviour
{
    public int CurrentLeage
    {
        get => _currentLeage;
        set
        {
            _currentLeage = value;
            PlayerPrefs.SetInt("CurrentLeage", _currentLeage);
        }
    }
    public int CurrentLeagePosition
    {
        get => _currentLeagePosition;
        set
        {
            var newLeagePos = value;
            if (newLeagePos == 0)
            {
                if (CurrentLeage >= leages.Count)
                    CurrentLeage = 0;
                else
                    CurrentLeage = CurrentLeage + 1;

                newLeagePos = leages[CurrentLeage].enemyToSpawn.Count;
            }
            
            _currentLeagePosition = newLeagePos;
            PlayerPrefs.SetInt("CurrentLeagePositions", _currentLeagePosition);
        }
    }
    
    public List<Leage> leages = new List<Leage>();
    [SerializeField] private LeageUI _leageUi;

    private int _currentLeage;
    private int _currentLeagePosition;
    
    private void Awake()
    {
        _currentLeage = PlayerPrefs.GetInt("CurrentLeage", 0);
        _currentLeagePosition = PlayerPrefs.GetInt("CurrentLeagePositions", leages[0].enemyToSpawn.Count);
        for(var i = 0; i < leages.Count; i++)
        {
            leages[i].Init(i);
        }
    }

    public Leage GetCurrentLeage()
    {
        return leages[CurrentLeage];
    }

    public Arena GetCurrentArena()
    {
        return leages[_currentLeage].leageArena;
    }

    public Enemy GetCurrentEnemy()
    {
        return leages[_currentLeage].enemyToSpawn[_currentLeagePosition - 1];
    }
}
