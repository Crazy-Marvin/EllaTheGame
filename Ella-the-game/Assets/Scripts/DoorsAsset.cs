using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsAsset : MonoBehaviour
{
    public GameObject[] doors;
    void Start()
    {
        doors[Random.Range(0, doors.Length)].SetActive(true);
        Destroy(gameObject, 7.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
