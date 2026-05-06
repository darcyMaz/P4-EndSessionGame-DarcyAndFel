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
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        playerStats = GetComponent<PlayerStats>();
        playerStats.OnSpeedBoostIncrement += ChangeColor;
        color0 = sr.color;
    }

    private void OnDisable()
    {
        playerStats.OnSpeedBoostIncrement -= ChangeColor;
    }



    // Update is called once per frame
    void Update()
    {

    }
    /*   private void OnTriggerEnter2D(Collider2D other)
       {
           if (other.CompareTag("SpeedBoost"))

           {
               sr.color = color1;
               Debug.Log("test");

           }

       } */

    private void ChangeColor(int SpeedBoostStack)
    {
        if (SpeedBoostStack == 1)
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
        Debug.Log("changed color");
    }
}
