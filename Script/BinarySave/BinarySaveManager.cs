using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class BinarySaveManager
{

    private static readonly string SavePath =
        Path.Combine(Application.persistentDataPath, "save.bin");

    private static readonly string ReloadPath =
        Path.Combine(Application.persistentDataPath, "initial.bin");
    private static readonly byte[] EncryptionKey = new byte[16] {
    0x12, 0x34, 0x56, 0x78,
    0x90, 0xAB, 0xCD, 0xEF,
    0x01, 0x23, 0x45, 0x67,
    0x89, 0xAB, 0xCD, 0xEF
};

    public static bool ReloadGame()
    {
        if (!File.Exists(ReloadPath)) return false;

        try
        {
            byte[] encryptedData = File.ReadAllBytes(ReloadPath);
            GameSaveData data = Deserialize(DecryptData(encryptedData)); //从二进制文件，解析读取为GameSaveData
            ApplyGameData(data);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"加载失败: {e.Message}");
            return false;
        }

        
    }
    public static void SaveGame() //保存游戏
    {
        var saveData = CollectGameData();  //收集信息
        byte[] encryptedData = EncryptData(Serialize(saveData)); //将GameSaveData解析为二进制
        Debug.Log("存档文件路径："+SavePath);
        File.WriteAllBytes(SavePath, encryptedData);
    }

    public static void InitGame() //如果两个文件都没有，则初始化initial.bin
    {
         var reloadData = CollectGameData();  //收集信息
        byte[] encryptedData = EncryptData(Serialize(reloadData)); //将GameSaveData解析为二进制
        Debug.Log("初始化存档文件路径："+ReloadPath);
        File.WriteAllBytes(ReloadPath, encryptedData);
    }

    public static bool LoadGame() //将存档信息加载至游戏（重新打开exe时使用）
    {
        if (!File.Exists(SavePath)) return false;

        try
        {
            byte[] encryptedData = File.ReadAllBytes(SavePath);
            GameSaveData data = Deserialize(DecryptData(encryptedData)); //从二进制文件，解析读取为GameSaveData
            ApplyGameData(data);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"加载失败: {e.Message}");
            return false;
        }
    }
    static void ApplyGameData(GameSaveData data)
    {
        // 玩家状态
        // CoinUI.CurrentCoinQuantity = data.coins; //玩家金币数
        //PlayerController.Instance.transform.position = data.playerPosition.ToVector3(); //玩家位置
        GameController.checkPointSceneName = data.sceneName;
        GameController.canDoubleJump = data.abilities[0];
        GameController.canGlide = data.abilities[1];
        GameController.canDash = data.abilities[2];
        GameController.canMagic = data.abilities[3];
        GameController.checkPointSceneName = data.sceneName;
        //HealthBar.HealthMax = data.healthMax;
        //PlayerAttack.Instance.damage = data.damage;
        GameController.chests = data.chests; //GameController中的字典是实时更新的，但是只有在保存时才会持久化
        GameController.doors = data.doors;
        GameController.switches = data.switches;
        GameController.damage = data.damage;
        GameController.healthMax = data.healthMax;
        GameController.CoinNum = data.coins;
        GameController.sceneName = data.sceneName;
        Debug.Log("应用金币数：" + GameController.CoinNum);

        // 任务进度
        GameProgressManager.Instance.completedQuests = data.questProgress;
        
         //此处加载场景名为sceneName的场景

        //SceneManager.LoadScene("Start"); // 加载游戏场景
    
    var entry = GameObject.Find("EntryPoint")?.transform;
    SceneTransitionManager.Instance.TransitionTo(
    data.sceneName,
    "Sign",Vector2.down,0,0);   //这里后续可能要改
}
    private static GameSaveData CollectGameData()
    {
        Debug.Log("保存：角色位置：" + PlayerController.Instance.transform.position);
        Debug.Log("保存：场景名：" + SceneManager.GetActiveScene().name);
        var data = new GameSaveData {
        coins          = GameController.CoinNum,
       
        playerPosition = new Vector3Serializable(PlayerController.Instance.transform.position),
        abilities      = new bool[] {
            GameController.canDoubleJump,
            GameController.canGlide,
            GameController.canDash,
            GameController.canMagic
        },
        healthMax  = GameController.healthMax,
        damage     = GameController.damage,
        sceneName  = SceneManager.GetActiveScene().name,
        questProgress = GameProgressManager.Instance.completedQuests
        // 注意这里暂时不要写 doors、switches、chests
    };
        Debug.Log("存储金币数：" + GameController.CoinNum);
    // 完成实例初始化后，再给这些静态列表赋给实例字段：
    data.doors    = GameController.doors;
    data.switches = GameController.switches;
    data.chests   = GameController.chests;

    return data;
    }


    private static byte[] Serialize(object data)
    {
        using (var stream = new MemoryStream())
        {
            new BinaryFormatter().Serialize(stream, data);
            return stream.ToArray();
        }
    }

    private static GameSaveData Deserialize(byte[] data)
    {
        using (var stream = new MemoryStream(data))
        {
            return (GameSaveData)new BinaryFormatter().Deserialize(stream);
        }
    }

    private static byte[] EncryptData(byte[] data)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = EncryptionKey;   // 设置 128 位密钥
            aes.Mode = CipherMode.CBC; // 采用 CBC 链式分组加密模式

            // ← 在此刻，内部会为 aes.IV 生成一个随机的 16 字节值
             
            using (var encryptor = aes.CreateEncryptor())
            using (var resultStream = new MemoryStream())
            {
                // 写入IV
                resultStream.Write(aes.IV, 0, aes.IV.Length);

                // 加密数据
                using (var cryptoStream = new CryptoStream(
                    resultStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                }
                return resultStream.ToArray();
            }
        }
    }

    private static byte[] DecryptData(byte[] encryptedData)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = EncryptionKey;
            aes.Mode = CipherMode.CBC;

            // 读取IV
            byte[] iv = new byte[aes.BlockSize / 8];
            Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);
            aes.IV = iv;

            // 解密数据
            using (var decryptor = aes.CreateDecryptor())
            using (var encryptedStream = new MemoryStream(encryptedData, iv.Length, 
                encryptedData.Length - iv.Length))
            using (var cryptoStream = new CryptoStream(
                encryptedStream, decryptor, CryptoStreamMode.Read))
            using (var resultStream = new MemoryStream())
            {
                cryptoStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}