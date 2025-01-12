using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//无限背景
public class BackGroundCycle : MonoBehaviour
{
    public GameObject mainCamera;
    public float mapWidth; //地图宽度
    public int mapNums;//地图重复次数

    private float totalWidth;//总地图宽度
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
