using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressionCountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private ProgressionSystem _progressionSystem;
    
    private void Start()
    {
        UpdateValue(_progressionSystem.CurrentPoints);
        _progressionSystem.onProgressionChanged += UpdateValue;
    }
    
    private void UpdateValue(int value)
    {
        _valueText.text = value.ToString();
    }
}