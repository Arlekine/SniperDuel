using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CoolText : MonoBehaviour
{
    [SerializeField] private float _showTime;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private CanvasGroup _textCG;
    [SerializeField] private Vector2 _centerPos;
    [SerializeField] private Vector2 _upPos;

    private Sequence _currentSeq;
    
    public void Show(string text)
    {
        _text.transform.localScale = Vector3.zero;
        _textCG.alpha = 0;
        _text.GetComponent<RectTransform>().anchoredPosition = _centerPos;
        _text.text = text;
        
        _currentSeq?.Kill();
        _currentSeq = DOTween.Sequence();

        _currentSeq.Append(_text.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
        _currentSeq.Join(_textCG.DOFade(1f, 0.3f));
        _currentSeq.AppendInterval(_showTime);
        _currentSeq.Append(_text.transform.DOScale(1.2f, 0.3f));
        _currentSeq.Join(_textCG.DOFade(0f, 0.4f));
        _currentSeq.Join(_text.GetComponent<RectTransform>().DOAnchorPos(_upPos, 0.3f));
    }
}
