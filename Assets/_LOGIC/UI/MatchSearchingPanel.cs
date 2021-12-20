using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MatchSearchingPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _searchingText;
    [SerializeField] private GameObject _foundText;
    [SerializeField] private GameObject _hourglass;
    [SerializeField] private GameObject _matchStartText;
    [SerializeField] private TextMeshProUGUI _matchStartCountText;
    
    public Coroutine Search()
    {
        _canvasGroup.alpha = 1f;
        return StartCoroutine(OpponentSearchRoutine());
    }

    public void FadeOut()
    {
        _canvasGroup.DOFade(0f, 0.5f);
    }

    private IEnumerator OpponentSearchRoutine()
    {
        _searchingText.SetActive(true);
        _foundText.SetActive(false);

        _hourglass.SetActive(true);
        _matchStartText.SetActive(false);
        
        yield return new WaitForSeconds(Random.Range(3f, 5f));
        
        _searchingText.SetActive(false);
        _foundText.SetActive(true);

        _hourglass.SetActive(false);
        _matchStartText.SetActive(true);

        for (int i = 3; i >= 1; i--)
        {
            _matchStartCountText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        
        _matchStartCountText.text = "0";
    }
}