using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HourglassAnimation : MonoBehaviour
{
    [SerializeField] private float _amplitude;
    [SerializeField] private float _circleTime;

    private Sequence _sequence;
    
    private void OnEnable()
    {
        transform.eulerAngles = Vector3.forward * _amplitude;
        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DORotate(-Vector3.forward * _amplitude, _circleTime).SetEase(Ease.InOutCubic));
        _sequence.Append(transform.DORotate(Vector3.forward * _amplitude, _circleTime).SetEase(Ease.InOutCubic));
        _sequence.SetLoops(-1);
    }

    private void OnDisable()
    {
        _sequence?.Kill();
    }
}
