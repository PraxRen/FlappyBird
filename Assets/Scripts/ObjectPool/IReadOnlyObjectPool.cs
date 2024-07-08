using System;
using UnityEngine;

public interface IReadOnlyObjectPool<T> where T : MonoBehaviour
{
    public event Action<T> Geted;
    public event Action<T> Refunded;
}
