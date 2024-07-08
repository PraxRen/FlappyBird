using System;
using UnityEngine;

[RequireComponent(typeof(BirdMover), typeof(ScoreCounter), typeof(BirdCollisionHandler))]
[RequireComponent(typeof(Fighter), typeof(Health))]
public class Bird : MonoBehaviour
{
    [SerializeField] private KeyCode _butttonAttack;
    [SerializeField] private KeyCode _butttonJump;

    private BirdMover _birdMover;
    private ScoreCounter _scoreCounter;
    private BirdCollisionHandler _handler;
    private Fighter _fighter;
    private Health _health;

    public event Action GameOver;

    private void Awake()
    {
        _scoreCounter = GetComponent<ScoreCounter>();
        _handler = GetComponent<BirdCollisionHandler>();
        _birdMover = GetComponent<BirdMover>();
        _fighter = GetComponent<Fighter>();
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        _handler.CollisionDetected += ProcessCollision;
        _health.Died += OnDied;
    }

    private void OnDisable()
    {
        _handler.CollisionDetected -= ProcessCollision;
    }

    private void Update()
    {
        if (Input.GetKeyDown(_butttonAttack))
            _fighter.Attack();

        if (Input.GetKeyDown(_butttonJump))
            _birdMover.Jump();
    }

    private void ProcessCollision(IInteractable interactable)
    {
        if (interactable is Border)
        {
            GameOver?.Invoke();
        }
    }

    private void OnDied(IDamageable damageable)
    {
        GameOver?.Invoke();
    }

    public void Reset()
    {
        _scoreCounter.Reset();
        _birdMover.Reset();
        _health.Reset();
    }
}