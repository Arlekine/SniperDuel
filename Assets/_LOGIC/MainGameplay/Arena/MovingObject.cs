using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] private Transform _firstPoint;
    [SerializeField] private Transform _secondPoint;
    [SerializeField] private float _speed;
    [SerializeField] private bool _isCicled;

    private float _lerpTimeMod;
    private float _currentMovePos;

    bool _stop;

    private void Awake()
    {
        var initialDistance = (transform.position - _firstPoint.position).magnitude;
        var distance = (_firstPoint.position - _secondPoint.position).magnitude;
        _lerpTimeMod = 1 / (distance / _speed);

        _currentMovePos = initialDistance / distance;
    }

    private void Update()
    {
        if (_currentMovePos < 0 && _isCicled)
        {
            _lerpTimeMod = -_lerpTimeMod;
        }
        
        if (_currentMovePos >= 1)
        {
            if (_isCicled)
                _lerpTimeMod = -_lerpTimeMod;
            else
                _currentMovePos = 0;
        }


        transform.position = Vector3.LerpUnclamped(_firstPoint.position, _secondPoint.position, _currentMovePos);
        _currentMovePos += Time.deltaTime * _lerpTimeMod;


        if (transform.position == _secondPoint.position && !_stop)
            StartCoroutine(Delay());
    }


    IEnumerator Delay()
    {
        _stop = true;

       yield return new WaitForSeconds(2);

        transform.position = _firstPoint.position;

        yield return new WaitForSeconds(0.5f);

        _stop = false;
    }
}