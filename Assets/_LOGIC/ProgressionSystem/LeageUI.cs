using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LeageUI : MonoBehaviour
{
    public Action onLeageRoutineEnd;
    
    [SerializeField] private LeageController _leageController;
    
    [Space]
    [SerializeField] private Image _leageImage;
    
    [SerializeField] private CanvasGroup _mainWindowGC;
    [SerializeField] private RectTransform _mainWindowRect;
    [SerializeField] private VerticalLayoutGroup _playersParent;
    [SerializeField] private LeageUIPlayer _playersUIPrefab;

    [Header("League")] 
    [SerializeField] private CanvasGroup _updateWindowGC;
    [SerializeField] private RectTransform _updateWindowRect;
    [SerializeField] private Image _updateLeageImage;

    private List<LeageUIPlayer> _enemies = new List<LeageUIPlayer>();
    private LeageUIPlayer _player;

    private Sequence _mainWindowSeq;
    private Sequence _updateWindowSeq;
    private Sequence _secondaryWindowSeq;

    public void SetPlayersList(List<string> enemiesNames, Sprite icon)
    {
        _leageImage.sprite = icon;

        _updateWindowGC.alpha = 0f;
        _updateWindowRect.localScale = Vector3.zero;
        
        if (_enemies.Count > 0)
        {
            foreach (var enemy in _enemies)
            {
                Destroy(enemy.gameObject);
            }
            _enemies.Clear();
            
            Destroy(_player.gameObject);
        }

        int playerPosition = _leageController.CurrentLeagePosition;

        _player = Instantiate(_playersUIPrefab, _playersParent.transform);
        _player.SetName("You");
        
        for (int i = 0; i < enemiesNames.Count; i++)
        {
            var newEnemy = Instantiate(_playersUIPrefab, _playersParent.transform);
            newEnemy.SetName(enemiesNames[i]);
            _enemies.Add(newEnemy);
        }
    }

    public void Open(bool isWin)
    {
        Canvas.ForceUpdateCanvases();
        _player.transform.SetSiblingIndex(_leageController.CurrentLeagePosition);
        _mainWindowGC.alpha = 0f;
        _mainWindowRect.localScale = Vector3.zero;
        
        _mainWindowSeq?.Kill();
        _mainWindowSeq = DOTween.Sequence();

        _mainWindowSeq.Append(_mainWindowGC.DOFade(1f, 0.4f));
        _mainWindowSeq.Append(_mainWindowRect.DOScale(1, 0.4f).SetEase(Ease.OutBack));

        _mainWindowSeq.AppendInterval(1f);

        if (isWin)
            _mainWindowSeq.AppendCallback(() => UpdatePlayerPos());
        else
        {
            _mainWindowSeq.AppendInterval(1.5f);
            _mainWindowSeq.AppendCallback(() => { onLeageRoutineEnd?.Invoke(); });
        }
    }

    public Tween UpdatePlayerPos()
    {
        _updateWindowSeq?.Kill();
        _updateWindowSeq = DOTween.Sequence();

        int playerPosition = _leageController.CurrentLeagePosition;
        
        Canvas.ForceUpdateCanvases();
        
        var playerPos = _player.RectTransform.anchoredPosition;
        var enemyPos = _enemies[playerPosition - 1].RectTransform.anchoredPosition;
        
        _updateWindowSeq.AppendCallback(() =>
        {
            _playersParent.enabled = false;
        });
        
        _updateWindowSeq.Append(_player.GoUp(enemyPos));
        _updateWindowSeq.Join(_enemies[playerPosition - 1].GoDown(playerPos));
        _updateWindowSeq.AppendInterval(1.5f);
        _updateWindowSeq.AppendCallback(() =>
        {
            _player.transform.SetSiblingIndex(_player.transform.GetSiblingIndex() - 1);
            _playersParent.enabled = true;

            int leage = _leageController.CurrentLeage;
            _leageController.CurrentLeagePosition = _leageController.CurrentLeagePosition - 1;
            if (leage != _leageController.CurrentLeage)
            {
                ShowLeageUpdate(_leageController.leages[_leageController.CurrentLeage].icon);
            }
            else
            {
                var waitSeq = DOTween.Sequence();
                waitSeq.AppendInterval(1.5f);
                waitSeq.AppendCallback(() =>
                {
                    onLeageRoutineEnd?.Invoke();
                });
            }
        });

        return _updateWindowSeq;
    }

    public void ShowLeageUpdate(Sprite newLeageIcon)
    {
        _updateLeageImage.sprite = newLeageIcon;
        
        _updateWindowGC.alpha = 0f;
        _updateWindowRect.localScale = Vector3.zero;
        
        _secondaryWindowSeq?.Kill();
        _secondaryWindowSeq = DOTween.Sequence();

        _secondaryWindowSeq.Append(_updateWindowGC.DOFade(1f, 0.4f));
        _secondaryWindowSeq.Join(_updateWindowRect.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        _secondaryWindowSeq.AppendInterval(1.5f);
        _secondaryWindowSeq.AppendCallback(() => { 
        
            onLeageRoutineEnd?.Invoke();
        });
    }
}