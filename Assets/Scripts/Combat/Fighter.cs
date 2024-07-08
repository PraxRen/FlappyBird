using UnityEngine;

public class Fighter : MonoBehaviour, IDamageDealer
{
    [SerializeField] private SpawnerProjectile _spawnerProjectile;
    [SerializeField] private float _damage;

    public float Damage => _damage;

    public void Attack()
    {
        Projectile projectile = _spawnerProjectile.Spawn();

        if (projectile == null)
            return;

        projectile.Collided += OnCollided;
    }

    private void OnCollided(Projectile projectile, Transform target)
    {
        projectile.Collided -= OnCollided;

        if (target.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(this);
        }
    }
}
