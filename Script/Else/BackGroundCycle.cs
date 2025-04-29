using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BackGroundCycle : MonoBehaviour
{
    public GameObject mainCamera;
    public float mapWidth; 
    public int mapNums;

    private float totalWidth;
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mapWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        totalWidth = mapWidth * mapNums;
    
    
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPos = transform.position;
        if (mainCamera.transform.position.x > transform.position.x + totalWidth / 2)
        {
            currentPos.x += totalWidth;
            transform.position = currentPos;
        }else if (mainCamera.transform.position.x < transform.position.x - totalWidth / 2)
        {
            currentPos.x -= totalWidth;
            transform.position = currentPos;
        }
    }
}
