using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _force;

    public event Action Shotted;
    public event Action<Projectile, Transform> Collided;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Collided?.Invoke(this, collider.transform);
    }

    public void Shot(Direction directionForce)
    {
        Vector3 direction = directionForce == Direction.Right ? transform.right : -transform.right;
        _rigidbody.AddForce(direction * _force, ForceMode2D.Impulse);
        Shotted?.Invoke();
    }
}
