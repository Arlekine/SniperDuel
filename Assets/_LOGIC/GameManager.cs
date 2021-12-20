using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Logic")] 
    [SerializeField] private MatchController _matchController;
    [SerializeField] private List<Arena> _arenas = new List<Arena>();
    
    [Header("UI Menues")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _titleScreen;
    [SerializeField] private GameObject _arenasScreen;
    [SerializeField] private ArenasListPanel _arenasList;
    [SerializeField] private Button _goButton;
    [SerializeField] private MatchSearchingPanel _opponentSearchMenu;
    [SerializeField] private GameObject _matchMenu;
    
    public void Play()
    {
        _titleScreen.SetActive(false);
        _arenasScreen.SetActive(true);
        _arenasList.onNewCardSelected += UpdateGoButton;
        _arenasList.SetFirst();
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
        _arenasScreen.SetActive(false);
        
        _arenasList.onNewCardSelected -= UpdateGoButton;
    }

    private void UpdateGoButton(bool areanaUnlocked)
    {
        _goButton.interactable = areanaUnlocked;
    }
    
    private IEnumerator MatchStartRoutine()
    {
        yield return _opponentSearchMenu.Search();

        _matchMenu.SetActive(true);
        _opponentSearchMenu.FadeOut();
        _matchController.StartNewMatch(_arenas[_arenasList.CurrentIndex]);
    }
}