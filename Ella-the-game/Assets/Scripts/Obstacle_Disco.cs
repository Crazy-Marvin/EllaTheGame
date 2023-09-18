using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Disco : MonoBehaviour
{
    public float destructionDelay = 2;
    public float rotationSpeed = 25;
    public float lifeTime=15;
    public int healthEffect = 20;
    private int difficulty;
    public GameObject colliderObject;
    void Start()
    {
        Destroy(this.gameObject, 15);
        difficulty = PlayerPrefs.GetInt("difficulty");
        if (difficulty == 0)
        {
            healthEffect = -5;
        }
        else if (difficulty == 1)
        {
            healthEffect = -20;
        }
        else if (difficulty == 2)
        {
            healthEffect = -100;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.transform.tag == "Player")
        {
            //1st line = changing player score &&& 2nd line adding current object to a layer that is ignored by player collision
            collision.gameObject.transform.GetComponent<PlayerController>().changeHealth(healthEffect);
            colliderObject.layer = LayerMask.NameToLayer("ignoredObstacles");
            Destroy(gameObject, destructionDelay);
        }
    }
}
