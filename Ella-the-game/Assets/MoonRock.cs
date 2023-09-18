using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonRock : MonoBehaviour
{
    Vector3 playerPos;
    public GameObject rock, TrailVFX, explosionVFX;
    public float rotationSpeed, movementSpeed;
    private Rigidbody2D RB;

    private int healthEffect = -5;
    private int difficulty;

    void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        playerPos = new Vector3(playerPos.x+3f, playerPos.y-5, playerPos.z);

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

        RB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rock.transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.Self);
        transform.position = Vector3.MoveTowards(transform.position, playerPos, 
            movementSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.transform.tag == "Player")
        {
            Debug.Log("kkkkdkdkdkdkdk");
            //1st line = changing player score &&& 2nd line adding current object to a layer that is ignored by player collision
            collision.gameObject.transform.GetComponent<PlayerController>().changeHealth(healthEffect);
            //gameObject.layer = LayerMask.NameToLayer("ignoredObstacles");
            foreach(Transform elem in GetComponentsInChildren<Transform>())
                elem.gameObject.layer = LayerMask.NameToLayer("ignoredObstacles");
            rock.GetComponent<MeshRenderer>().enabled = false;
            TrailVFX.SetActive(false);
            explosionVFX.SetActive(true);
            Destroy(gameObject, (float)2);
        }
    }
}
