using UnityEngine;

public class BirdTracker : MonoBehaviour
{
    [SerializeField] private Bird _bird;
    
    private float _xOffset;

    private void Start()
    {
        _xOffset = transform.position.x - _bird.transform.position.x;
    }

    private void Update()
    {
        transform.position = new Vector2(_bird.transform.position.x + _xOffset, transform.position.y);
    }
}
