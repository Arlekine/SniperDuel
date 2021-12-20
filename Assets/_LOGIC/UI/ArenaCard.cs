using System.Collections;
using TMPro;
using UnityEngine;

public class ArenaCard : MonoBehaviour
{
    public bool IsUnlocked { get; private set; }

    [SerializeField] private int _progressionToUnlock;
    [SerializeField] private TextMeshProUGUI _toUnlockText;
    [SerializeField] private GameObject _toUnlockBlock;

    public void SetUnlocked(int globalValue)
    {
        _toUnlockText.text = _progressionToUnlock.ToString();
        IsUnlocked = globalValue >= _progressionToUnlock;
        _toUnlockBlock.SetActive(!IsUnlocked);
    }
}