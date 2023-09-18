using UnityEngine;
using System.Collections;

public class ItemsDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            Destroy(collision.gameObject);
        }
    }
}
