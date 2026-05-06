using System.Collections.Generic;
using UnityEngine;

public class PlayerStacking : MonoBehaviour
{
   // int stackCount = 0;
   // SpriteRenderer spriteRenderer;

    private void Start()
    {
        
    }


    // add to stack, color change
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            Debug.Log($"Collected speed boost");
            //  other.GetComponent<SpriteRenderer>();
            PlayerStats  playerStats = other.GetComponent<PlayerStats>();
            int currentSpeedBoost = playerStats.ChangeSpeedBoost(1);
            Debug.Log(currentSpeedBoost);

        }
        // add maximum stack capacity logic
    }

  
}
