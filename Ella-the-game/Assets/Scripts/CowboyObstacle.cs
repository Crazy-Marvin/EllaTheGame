using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowboyObstacle : MonoBehaviour
{
    private Rigidbody2D RB;
    RaycastHit2D hit;
    public float minDistanceToSlide=5;
    public Transform rayCastPos;
    public LayerMask layerMask;

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        hit = Physics2D.Raycast(rayCastPos.position, -Vector2.right, layerMask);
        if (hit.collider != null)
        {
            // Calculate the distance from the surface and the "error" relative
            // to the floating height.
            float distance = Mathf.Abs(hit.point.x - transform.position.x);
            Debug.Log(hit.transform.name);


           // RB.AddForce(Vector3.up * force);
        }
    }
}
