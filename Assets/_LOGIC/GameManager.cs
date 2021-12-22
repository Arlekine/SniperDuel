using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Logic")] 
    [SerializeField] private MatchController _matchController;
    [SerializeField] private LeageController _leageController;
    [SerializeField] private LeageUI _leageUI;
    [SerializeField] private List<Arena> _arenas = new List<Arena>();
    
    [Header("UI Menues")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _titleScreen;
    [SerializeField] private MatchSearchingPanel _opponentSearchMenu;
    [SerializeField] private GameObject _matchMenu;

    private void Start()
    {
        _leageUI.onLeageRoutineEnd += MatchFinished;
    }

    public void StartMatch()
    {
        _mainMenu.SetActive(false);
        _opponentSearchMenu.gameObject.SetActive(true);
        StartCoroutine(MatchStartRoutine());
    }

    public void MatchFinished()
    {
        _matchMenu.SetActive(false);
        
        _mainMenu.SetActive(true);
        
        _titleScreen.SetActive(true);
    }
    
    private IEnumerator MatchStartRoutine()
    {
        yield return _opponentSearchMenu.Search();

        _matchMenu.SetActive(true);
        _opponentSearchMenu.FadeOut();
        _matchController.StartNewMatch(_leageController.GetCurrentArena());
    }
}