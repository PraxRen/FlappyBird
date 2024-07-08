using System;
using UnityEngine;
using System.Collections;

public class BezierMover : MonoBehaviour
{
    [SerializeField] private float _speed;

    private float _t;
    private Coroutine _jobUpdateLocalPosition;

    public event Action Finish;

    private void OnDisable()
    {
        CancelUpdateLocalPosition();
    }

    public void StartLocalMove(Vector2 positionOne, Vector2 positionTwo, Vector2 positionThree)
    {
        CancelUpdateLocalPosition();
        _jobUpdateLocalPosition = StartCoroutine(UpdateLocalPosition(positionOne, positionTwo, positionThree));
    }

    private IEnumerator UpdateLocalPosition(Vector2 positionOne, Vector2 positionTwo, Vector2 positionThree)
    {
        while (_t < 1)
        {
            _t += _speed * Time.deltaTime;
            transform.localPosition = GetBezierPoint(positionOne, positionTwo, positionThree, _t);
            yield return null;
        }

        transform.localPosition = positionThree;
        _jobUpdateLocalPosition = null;
        Finish?.Invoke();
    }

    private void CancelUpdateLocalPosition()
    {
        if (_jobUpdateLocalPosition != null)
        {
            StopCoroutine(_jobUpdateLocalPosition);
            _jobUpdateLocalPosition = null;
        }

        _t = 0;
    }

    private Vector3 GetBezierPoint(Vector2 positionOne, Vector2 positionTwo, Vector2 positionThree, float t)
    {
        t = Mathf.Clamp01(t);
        Vector2 p1 = Vector2.Lerp(positionOne, positionTwo, t);
        Vector3 p2 = Vector2.Lerp(positionTwo, positionThree, t);
        Vector2 result = Vector2.Lerp(p1, p2, t);
        return result;
    }
}