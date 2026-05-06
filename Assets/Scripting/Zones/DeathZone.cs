using System;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public static event Action OnDeath;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            OnDeath?.Invoke();
        }
    }
}
