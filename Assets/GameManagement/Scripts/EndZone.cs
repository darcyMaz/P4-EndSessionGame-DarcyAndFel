using UnityEngine;

public class EndZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject.tag == "Player")
            collision.gameObject.transform.position = new Vector2(-10, -7.5f);
    }
}
