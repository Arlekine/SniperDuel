using System;
using UnityEngine;

public class ProgressionSystem : MonoBehaviour
{
    private const string ProgressionPlayerPrefs = "ProgressionPoints";
    
    public Action<int> onProgressionChanged;
    
    public int CurrentPoints => PlayerPrefs.GetInt(ProgressionPlayerPrefs);
    public int WinReward => _winReward;
    public int LooseReward => _looseReward;

    [SerializeField] private int _winReward;
    [SerializeField] private int _looseReward;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey(ProgressionPlayerPrefs))
            PlayerPrefs.SetInt(ProgressionPlayerPrefs, 0);
    }

    public void AddPoints(int points)
    {
        var currentPoints = CurrentPoints;
        
        currentPoints += points;
        currentPoints = Mathf.Max(0, currentPoints);
        
        PlayerPrefs.SetInt(ProgressionPlayerPrefs, currentPoints);
        
        onProgressionChanged?.Invoke(currentPoints);
    }
}