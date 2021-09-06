using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snowmanObstacle : MonoBehaviour {

    private Rigidbody2D RB;
    [SerializeField]
    private float movemmentSpeed;

    private int healthEffect = -5;
    private int difficulty;
    public bool rotating;
    public float rotationSpeed=5;
    public GameObject colliderObj;
    // Use this for initialization
    void Start () {
        //Difficulty Settup
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
	
	void FixedUpdate () {
        RB.velocity = new Vector2(movemmentSpeed, RB.velocity.y);
        if (rotating)
        {
            RB.MoveRotation(RB.rotation + rotationSpeed);
            //RB.rotation += rotationSpeed;
            //Debug.Log(RB.rotation);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.transform.tag == "Player")
        {
            //1st line = changing player score &&& 2nd line adding current object to a layer that is ignored by player collision
            collision.gameObject.transform.GetComponent<PlayerController>().changeHealth(healthEffect);
            gameObject.layer = LayerMask.NameToLayer("ignoredObstacles");
            if(colliderObj != null)
                colliderObj.layer = LayerMask.NameToLayer("ignoredObstacles");
            /*for (int i=0;i<transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("ignoredObstacles");
            }*/
            Destroy(gameObject, (float)2);
        }
    }
}
