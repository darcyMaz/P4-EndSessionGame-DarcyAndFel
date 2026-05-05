using Unity.VisualScripting;
using UnityEngine;

public class PlayerColor : MonoBehaviour
{
    SpriteRenderer sr;
    Color newColor;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
       //newColor = sr.color;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpeedBoost"))

        {
            sr.color = new Color(140, 21, 118);
            Debug.Log("test");
            
        }

    }
}
