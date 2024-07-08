using UnityEngine;

public class SpawnerProjectile : Spawner<Projectile>
{
    [SerializeField] private Projectile _prefab;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private int _capacity;
    [SerializeField] private Direction _directionShot;

    protected override Projectile CreateSpawnObject()
    {
        Projectile projectile = Instantiate(_prefab, transform);
        projectile.gameObject.SetActive(false);

        return projectile;
    }

    protected override void GetSpawnObject(Projectile projectile)
    {
        projectile.Collided += OnCollided;
        projectile.transform.rotation = _startPoint.rotation;
        projectile.transform.position = _startPoint.position;
        projectile.transform.parent = null;
        projectile.gameObject.SetActive(true);
        projectile.Shot(_directionShot);
    }

    protected override int InitilizeCapacity()
    {
        return _capacity;
    }

    protected override void RefundSpawnObject(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        projectile.transform.parent = transform;
    }

    private void OnCollided(Projectile projectile, Transform target)
    {
        projectile.Collided -= OnCollided;
        Pool.Refund(projectile);
    }
}
