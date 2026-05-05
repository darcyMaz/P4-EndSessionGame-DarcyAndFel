using System.Collections.Generic;
using UnityEngine;

public class PlayerStacking : MonoBehaviour
{
    int stackCount = 0;
   // SpriteRenderer spriteRenderer;

    private void Start()
    {
        
    }


    // add to stack, color change
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            stackCount = ++stackCount;
            Debug.Log($"Collected{stackCount}speed boosts");
            other.GetComponent<SpriteRenderer>();

        }
        // add maximum stack capacity logic
    }

  
}
