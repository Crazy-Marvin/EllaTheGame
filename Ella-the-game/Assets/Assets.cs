using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assets : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        StartCoroutine(ExecuteAfterTime(10));
    }



    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        InvokeRepeating("DestroyIfInvisible", 0.5f, 5f);
    }

    // Update is called once per frame
    void Update () {
		
	}
    void DestroyIfInvisible()
    {
        if (GetComponent<SpriteRenderer>() != null)
        {
            if (!GetComponent<SpriteRenderer>().isVisible)
                Destroy(this.gameObject, 0.001f);
        }
        else
        {
            foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
            {
                if (!sr.isVisible)
                {
                    Destroy(this.gameObject, 0.001f);
                    break;
                }
            }
        }
        
    }
}
