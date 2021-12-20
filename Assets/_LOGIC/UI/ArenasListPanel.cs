using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ArenasListPanel : MonoBehaviour
{
    public Action<bool> onNewCardSelected;
    
    public int CurrentIndex => _currentIndex;

    [SerializeField] private ProgressionSystem _progressionSystem;
    [SerializeField] private List<ArenaCard> _arenaCards = new List<ArenaCard>();
    [SerializeField] private RectTransform _arenasParent;
    [SerializeField] private Button _moveLeftButton;
    [SerializeField] private Button _moveRightButton;

    private int _currentIndex;
    
    public void SetFirst()
    {
        _currentIndex = 0;
        _arenasParent.anchoredPosition = Vector2.zero;
        _moveLeftButton.interactable = _currentIndex != 0;
        _moveRightButton.interactable = _currentIndex != _arenaCards.Count - 1;

        foreach (var arenaCard in _arenaCards)
        {
            arenaCard.SetUnlocked(_progressionSystem.CurrentPoints);
        }
    }
    
    public void MoveRight()
    {
        _currentIndex = Mathf.Clamp(_currentIndex + 1, 0, _arenaCards.Count - 1);
        _moveLeftButton.interactable = _currentIndex != 0;
        _moveRightButton.interactable = _currentIndex != _arenaCards.Count - 1;
        SetArena(_currentIndex);
    }

    public void MoveLeft()
    {
        _currentIndex = Mathf.Clamp(_currentIndex - 1, 0, _arenaCards.Count - 1);
        _moveLeftButton.interactable = _currentIndex != 0;
        _moveRightButton.interactable = _currentIndex != _arenaCards.Count - 1;
        SetArena(_currentIndex);
    }

    private void SetArena(int index)
    {
        _arenasParent.DOAnchorPosX(index * -1080f, 0.3f);
        onNewCardSelected?.Invoke(_arenaCards[index].IsUnlocked);
    }
}