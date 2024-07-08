using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Fighter _fighter;
    [SerializeField] private BezierMover _bezierMover;
    [SerializeField] private Health _health;
    [SerializeField] private float _cooldownAttack;

    private WaitForSeconds _waitCooldownAttack;
    private Coroutine _jobUpdateAttack;

    public event Action<Enemy> Resetted;

    public IDamageable Damageable => _health;

    private void Awake()
    {
        _waitCooldownAttack = new WaitForSeconds(_cooldownAttack);
    }

    private void OnEnable()
    {
        _bezierMover.Finish += OnFinish;
        _health.Died += OnDied;
    }

    private void OnDisable()
    {
        _bezierMover.Finish -= OnFinish;
        _health.Died -= OnDied;
        CancelUpdateAttack();
    }

    public void Reset()
    {
        _health.Reset();
        Resetted?.Invoke(this);
    }

    private void OnFinish()
    {
        _bezierMover.Finish -= OnFinish;
        CancelUpdateAttack();
        _jobUpdateAttack = StartCoroutine(UpdateAttack());
    }

    private IEnumerator UpdateAttack() 
    {
        while (true)
        {
            _fighter.Attack();
            yield return _waitCooldownAttack;
        }
    }

    private void CancelUpdateAttack()
    {
        if (_jobUpdateAttack != null)
        {
            StopCoroutine(_jobUpdateAttack);
            _jobUpdateAttack = null;
        }
    }

    private void OnDied(IDamageable damageable)
    {
        Reset();
    }
}