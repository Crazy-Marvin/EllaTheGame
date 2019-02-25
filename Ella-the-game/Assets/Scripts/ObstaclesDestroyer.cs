using UnityEngine;
using System.Collections;

public class ObstaclesDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Obstacle")
        {
            if(collision.transform.parent!=null && collision.transform.parent.tag == "Obstacle")
            {
                Destroy(collision.transform.parent.gameObject);
            }
            else
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
