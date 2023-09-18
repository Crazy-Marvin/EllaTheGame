using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prizes : MonoBehaviour {

    [SerializeField]
    private int scoreEffect;
    private AudioSource prizeAudio;
    // Use this for initialization
    void Start () {
        prizeAudio = GetComponent<AudioSource>();
        Invoke("DestroyIfNotUsed", 30f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform.tag == "Player"){
            //1st line = changing player score &&& 2nd line adding current object to a layer that is ignored by player collision
            collision.gameObject.transform.GetComponent<PlayerController>().changeScore(scoreEffect);
            gameObject.layer = LayerMask.NameToLayer("ignoredObstacles");

            //Checking for particles in children than playing it
            if (gameObject.GetComponentInChildren<ParticleSystem>() != null)
            {
                gameObject.GetComponentInChildren<ParticleSystem>().Play();
            }

            GetComponent<SpriteRenderer>().enabled = false;
            prizeAudio.Play();
            Destroy(this.transform.parent.gameObject, (float)1);
        }
    }
    private void DestroyIfNotUsed()
    {
        bool isSomeThingVisible = false;
        foreach(SpriteRenderer sp in transform.parent.GetComponentsInChildren<SpriteRenderer>())
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
