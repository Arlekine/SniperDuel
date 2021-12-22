using DG.Tweening;
using TMPro;
using UnityEngine;

public class LeageUIPlayer : MonoBehaviour
{
    public RectTransform RectTransform;
    
    [SerializeField] private TextMeshProUGUI _playerName;

    private Sequence _goSequence;
    
    public void SetName(string name)
    {
        _playerName.text = name;
    }

    [EditorButton]
    public Tween GoUp(Vector2 newPos)
    {
        _goSequence?.Kill();
        _goSequence = DOTween.Sequence();

        _goSequence.Append(RectTransform.DOAnchorPos(newPos, 0.5f).SetEase(Ease.InOutExpo));

        var scaleSeq = DOTween.Sequence();
        scaleSeq.Join(RectTransform.DOScale(1.2f, 0.25f).SetEase(Ease.InExpo));
        scaleSeq.Append(RectTransform.DOScale(1f, 0.25f).SetEase(Ease.OutExpo));

        _goSequence.Join(scaleSeq);

        return _goSequence;
    }

    [EditorButton]
    public Tween GoDown(Vector2 newPos)
    {
        _goSequence?.Kill();
        _goSequence = DOTween.Sequence();

        _goSequence.Append(RectTransform.DOAnchorPos(newPos, 0.5f));

        return _goSequence;
    }
}