using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class staticObstacles : MonoBehaviour {

    private int healthEffect=-5;
    private int difficulty;
    // Use this for initialization
    void Start () {
        //Difficulty Settup
        difficulty = PlayerPrefs.GetInt("difficulty");
        if (difficulty==0)
        {
            healthEffect = -5;
        }else if (difficulty == 1)
        {
            healthEffect = -20;
        }
        else if (difficulty == 2)
        {
            healthEffect = -100;
        }
        Invoke("DestroyIfNotUsed", 30f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.transform.tag == "Player")
        {
            //1st line = changing player score &&& 2nd line adding current object to a layer that is ignored by player collision
            collision.gameObject.transform.GetComponent<PlayerController>().changeHealth(healthEffect);
            gameObject.layer = LayerMask.NameToLayer("ignoredObstacles");
            Destroy(this.transform.parent.gameObject, 2);
        }
    }
    private void DestroyIfNotUsed()
    {
        bool isSomeThingVisible = false;
        foreach (SpriteRenderer sp in transform.parent.GetComponentsInChildren<SpriteRenderer>())
        {
            if (sp.isVisible)
            {
                isSomeThingVisible = true;
                break;
            }
        }
        if (!isSomeThingVisible)
        {
            Destroy(this.transform.parent.gameObject, 0.001f);
        }

    }
}
