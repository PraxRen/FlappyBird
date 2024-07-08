using UnityEngine;

public class ReactionHit : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Animator _animator;

    private void OnEnable()
    {
        _health.TookDamage += OnTookDamage;
    }

    private void OnDisable()
    {
        _health.TookDamage -= OnTookDamage;
    }

    private void OnTookDamage(IDamageDealer damageDealer)
    {
        _animator.SetTrigger(CharacterAnimatorData.Params.Hit);
    }
}
