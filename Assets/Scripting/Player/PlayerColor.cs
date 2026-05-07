using Unity.VisualScripting;
using UnityEngine;

public class PlayerColor : MonoBehaviour
{
    SpriteRenderer sr;
    Color newColor;
    PlayerStats playerStats;
    [SerializeField] Color color0;
    [SerializeField] Color color1;
    [SerializeField] Color color2;
    [SerializeField] Color color3;

    private bool HasSR = false;
    private bool HasStats = false;

    void Start()
    {
        if (!TryGetComponent(out sr)) Debug.Log("PlayerColor component could not find the sprite renderer component.");
        else HasSR = true;

        if (!TryGetComponent(out playerStats)) Debug.Log("PlayerColor component could not find the PlayerStats component.");
        else HasStats = true;

        //sr = GetComponent<SpriteRenderer>();
        //playerStats = GetComponent<PlayerStats>();
        
        if (HasStats) playerStats.OnSpeedBoostChange += ChangeColor;
        if (HasSR) color0 = sr.color;
    }

    private void OnDisable()
    {
        if (HasStats) playerStats.OnSpeedBoostChange -= ChangeColor;
    }

    private void ChangeColor(int SpeedBoostStack)
    {
        if (!HasSR) return;

        if (SpeedBoostStack == 0)
        {
            sr.color = color0;
        }
        else if (SpeedBoostStack == 1)
        {
            sr.color = color1;
        }
        else if (SpeedBoostStack == 2)
        {
            sr.color = color2;
        }
        else if (SpeedBoostStack == 3)
        {
            sr.color = color3;
        }
    }
}
