using UnityEngine;
using System.Collections;

public class ScrollingTrigger : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "SpriteBG")
        {
            //GetComponentInParent<ScrollingBackground>().UpdateBackgrounds();
        }
    }
}
