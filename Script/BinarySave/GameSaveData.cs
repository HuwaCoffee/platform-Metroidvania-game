using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Vector3Serializable
{
    public float x;
    public float y;
    public float z;

    public Vector3Serializable(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public Vector3 ToVector3() => new Vector3(x, y, z);
}

[System.Serializable]
public class GameSaveData
{
    // 玩家状态
    public int coins;
    public Vector3Serializable playerPosition;
    public bool[] abilities = new bool[4]; // [0]二段跳 [1]滑翔 [2]冲刺 [3]魔法
    public int healthMax;
    public int damage;

    public string sceneName;
    public bool[] questProgress;
    // 世界状态
    public  Dictionary<int,DoorSaveState> doors = new Dictionary<int,DoorSaveState>();
    public  Dictionary<int,SwitchSaveState> switches = new Dictionary<int,SwitchSaveState>();
    public  Dictionary<int,ChestSaveState> chests = new Dictionary<int,ChestSaveState>();
    

    
}

[System.Serializable]
public class DoorSaveState
{
    public int doorID;
    
    public bool isTrigger;
}

[System.Serializable]
public class SwitchSaveState
{
    public int switchID;
    public bool isTriggered;
}

[System.Serializable]
public class ChestSaveState
{
    public int chestID;
    public bool isOpened;
}