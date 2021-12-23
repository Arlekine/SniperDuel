using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchController : MonoBehaviour
{
    public Action<int, int> onMatchFinished;

    [SerializeField] private ProgressionSystem _progressionSystem;
    [SerializeField] private Transform _arenaSpawnPoint;
    [SerializeField] private Transform _roundStartCameraPos;
    [SerializeField] private RoundController _roundController;
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private int _winsToEndMatch;
    
    [Header("Leagues")]
    [SerializeField] private LeageController _leageController;
    [SerializeField] private LeageUI _leageUi;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _onScreenScorePlayer;
    [SerializeField] private TextMeshProUGUI _onScreenScoreEnemy;
    [SerializeField] private GameObject _roundEndPanel;
    [SerializeField] private TextMeshProUGUI _roundEndPanelScoreText;
    [SerializeField] private GameObject _matchEndPanel;
    [SerializeField] private TextMeshProUGUI _matchEndPanelScoreText;
    [SerializeField] private TextMeshProUGUI _matchEndPanelWinText;
    [SerializeField] private CanvasGroup _aimGraphic;
    [SerializeField] private RectTransform _aimCenter;
    [SerializeField] private GameObject _enemyShootingUI;
    [SerializeField] private Slider _playerHealthSlider;
    [SerializeField] private Slider _enemyHealthSlider;

    private Arena _arenaPrefab;
    private Arena _currentArena;
    
    private Player _activePlayer;
    private Enemy _activeEnemy;

    private bool _isPlayerTurnFirst;

    private int _playerWins;
    private int _enemyWins;

    private void Start()
    {
        _roundController.onRoundEnd += OnRoundEnd;
    }

    public void StartNewMatch(Arena arena)
    {
        _arenaPrefab = arena;
        
        _isPlayerTurnFirst = true;
        
        _playerWins = 0;
        _enemyWins = 0;
        
        _onScreenScorePlayer.text = $" - {0}";
        _onScreenScoreEnemy.text = $" - {0}";

        _roundEndPanel.SetActive(false);
        _matchEndPanel.SetActive(false);
        
        StartNewRound();
    }

    private void StartNewRound()
    {
        _currentArena = Instantiate(_arenaPrefab, _arenaSpawnPoint);
        _currentArena.transform.localPosition = Vector3.zero;

        var positions = _currentArena.GetPosition(_playerWins + _enemyWins);

        _activePlayer = Instantiate(_playerPrefab, positions.PlayerPos.position, Quaternion.identity);
        _activeEnemy = Instantiate(_leageController.GetCurrentEnemy(), positions.EnemyPos.position, Quaternion.identity);
        
        InitActiveShooters(positions);
        
        var firstShooter = _isPlayerTurnFirst ? (Shooter)_activePlayer : (Shooter)_activeEnemy;
        var secondShooter = _isPlayerTurnFirst ? (Shooter)_activeEnemy : (Shooter)_activePlayer;

        _roundController.StartRound(firstShooter, secondShooter, _currentArena.arenaShootingCondition);
    }

    private void OnRoundEnd(bool firstShooterWin)
    {
        if ((firstShooterWin && _isPlayerTurnFirst) || (!firstShooterWin && !_isPlayerTurnFirst))
            _playerWins++;
        else
            _enemyWins++;
        
        _onScreenScorePlayer.text = $" - {_playerWins}";
        _onScreenScoreEnemy.text = $" - {_enemyWins}";
        
        if (_playerWins == _winsToEndMatch || _enemyWins == _winsToEndMatch)
        {
            EndMatch();
        }
        else
         StartCoroutine(StartNextRoundRoutine());
    }

    private void EndMatch()
    {
        StartCoroutine(MatchEndRoutine());
    }

    private void ClearCurrentContent()
    {
        Destroy(_currentArena.gameObject);
        Destroy(_activePlayer.gameObject);
        Destroy(_activeEnemy.gameObject);

        _playerHealthSlider.value = 1f;
        _enemyHealthSlider.value = 1f;
    }

    private void InitActiveShooters(Arena.ArenaPositions positions)
    {
        _activePlayer.Gun.AimForce = _currentArena.arenaShootingCondition.playerAimForce;
        _activePlayer.Gun.BulletSpeed = _currentArena.arenaShootingCondition.playerBulletSpeed;

        _activePlayer.shootingPos = positions.PlayerPos;
        _activePlayer.hidingPos = positions.PlayerHidingPos;
        _activeEnemy.shootingPos = positions.EnemyPos;
        _activeEnemy.hidingPos = positions.EnemyHidingPos;
            
        _activeEnemy.Gun.AimForce = _currentArena.arenaShootingCondition.enemyAimForce;
        _activeEnemy.Gun.BulletSpeed = _currentArena.arenaShootingCondition.enemyBulletSpeed;
        
        _activePlayer.aimCenter = _aimCenter;
        _activePlayer.aimGraphic = _aimGraphic;
        _activePlayer.Health.healthUI = _playerHealthSlider;
        
        _activeEnemy.aimCenter = _aimCenter;
        _activeEnemy.aimGraphic = _aimGraphic;
        _activeEnemy.Health.healthUI = _enemyHealthSlider;
        _activeEnemy.enemyShootingUI = _enemyShootingUI;
    }

    private IEnumerator StartNextRoundRoutine()
    {
        yield return new WaitForSeconds(3f);
        
        _roundEndPanel.gameObject.SetActive(true);
        _roundEndPanelScoreText.text = $"{_playerWins} - {_enemyWins}";
        
        yield return new WaitForSeconds(2f);
        
        ClearCurrentContent();

        _isPlayerTurnFirst = !_isPlayerTurnFirst;
        StartNewRound();
        
        _roundEndPanel.gameObject.SetActive(false);
    }
    
    private IEnumerator MatchEndRoutine()
    {
        yield return new WaitForSeconds(3f);
        
        _matchEndPanel.SetActive(true);
        _matchEndPanelWinText.text = _playerWins > _enemyWins ? "You win!" : "You loose!";
        _matchEndPanelScoreText.text = $"{_playerWins} - {_enemyWins}";
        
        ClearCurrentContent();
        
        _leageUi.SetPlayersList(_leageController.GetCurrentLeage()._enemiedNickNames, _leageController.GetCurrentLeage().icon);
        yield return null;
        _leageUi.Open(_playerWins > _enemyWins);
    }
    
}