using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjsActivator : MonoBehaviour
{
    public GameObject[] objects;
    void Start()
    {
        objects[Random.Range(0, objects.Length)].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
