using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : Spawner<Enemy>
{
    [SerializeField] private Enemy[] _prefabs;
    [SerializeField] private int _capacityPool;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform[] _targetPoints;
    [SerializeField] private float _timeWaitSpawn;
    [SerializeField] private ScoreCounter _scoreCounter;

    private int _counterEnemyForSpawn;
    private WaitForSeconds _waitForSeconds;
    Dictionary<Transform, Enemy> _occupationTargetPoints = new Dictionary<Transform, Enemy>();

    public void Reset()
    {
        if (_occupationTargetPoints.Count(targetPoint => targetPoint.Value == null) == _occupationTargetPoints.Count)
        {
            StartCoroutine(RunSpawn(_occupationTargetPoints.Count));
            return;
        }

        foreach (Enemy enemy in _occupationTargetPoints.Values.ToArray())
            enemy?.Reset();
    }

    protected override void HandleAwake()
    {
        foreach (Transform targetPoint in _targetPoints)
            _occupationTargetPoints[targetPoint] = null;

        _waitForSeconds = new WaitForSeconds(_timeWaitSpawn);
    }

    protected override Enemy CreateSpawnObject()
    {
        int indexRandom = UnityEngine.Random.Range(0, _prefabs.Length);
        Enemy spawnObject = Instantiate(_prefabs[indexRandom], transform);
        spawnObject.gameObject.SetActive(false);

        return spawnObject;
    }

    protected override void GetSpawnObject(Enemy enemy)
    {
        enemy.Resetted += OnResetEnemy;
        enemy.Damageable.Died += OnDied;
        enemy.transform.position = _startPoint.transform.position;
        Transform targetPoint = GetFreeTargetPoint();
        _occupationTargetPoints[targetPoint] = enemy;
        enemy.gameObject.SetActive(true);
        StartMoveTargetSpawnPoint(enemy, targetPoint);
    }

    private void OnDied(IDamageable damageable)
    {
        damageable.Died -= OnDied;
        _scoreCounter.Add();
    }

    protected override int InitilizeCapacity()
    {
        return _capacityPool;
    }

    protected override void RefundSpawnObject(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private IEnumerator RunSpawn(int countEnemies)
    {
        _counterEnemyForSpawn += countEnemies;

        for (int i = 0; i < countEnemies; i++)
        {
            Spawn();
            --_counterEnemyForSpawn;
            yield return _waitForSeconds;
        }
    }

    private Transform GetFreeTargetPoint()
    {
        KeyValuePair<Transform, Enemy>[] freeTargetPoints = _occupationTargetPoints.Where(occupationTargetPoint => occupationTargetPoint.Value == null).ToArray();

        if (freeTargetPoints.Length == 0)
        {
            throw new InvalidOperationException($"Нет свободных целевых точек для старта движения при спавне {nameof(Enemy)}!");
        }

        int indexRandom = UnityEngine.Random.Range(0, freeTargetPoints.Length);
        Transform freeTargetPoint = freeTargetPoints[indexRandom].Key;
        return freeTargetPoint;
    }

    private void StartMoveTargetSpawnPoint(Enemy enemy, Transform freeTargetPoint)
    {
        if (enemy.TryGetComponent(out BezierMover bezierMover) == false)
        {
            throw new InvalidOperationException($"{nameof(enemy)} не содержит компонент BezierMover, необходимый для продолжения работы!");
        }

        Vector2 targetPostition = freeTargetPoint.localPosition;
        float offsetXCurvePointPosition = -3f;
        Vector2 curvePointPosition = new Vector2(targetPostition.x + offsetXCurvePointPosition, (targetPostition.y - _startPoint.localPosition.y) / 2 + _startPoint.localPosition.y);
        bezierMover.StartLocalMove(_startPoint.transform.localPosition, curvePointPosition, targetPostition);
    }

    private void OnResetEnemy(Enemy enemy)
    {
        enemy.Resetted -= OnResetEnemy;
        Pool.Refund(enemy);
        FreeTargetPoint(enemy);
    }

    private void FreeTargetPoint(Enemy enemy)
    {
        KeyValuePair<Transform, Enemy> findOccupationTargetPoint = _occupationTargetPoints.FirstOrDefault(occupationTargetPoint => occupationTargetPoint.Value == enemy);

        if (findOccupationTargetPoint.Value == null)
            throw new InvalidOperationException($"Ошибка при попытке освободить место для спавна. Данный {nameof(Enemy)} не принадлежит текущему спавну.");

        _occupationTargetPoints[findOccupationTargetPoint.Key] = null;

        int countFreeTargetPoints = _occupationTargetPoints.Count(occupationTargetPoint => occupationTargetPoint.Value == null) - _counterEnemyForSpawn;

        if (countFreeTargetPoints == _occupationTargetPoints.Count)
        {
            StartCoroutine(RunSpawn(countFreeTargetPoints));
        }
    }
}