using UnityEngine;
using System.Collections;

public class PrizesDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Prize")
        {
            Destroy(collision.transform.parent.gameObject);
        }
    }
}
