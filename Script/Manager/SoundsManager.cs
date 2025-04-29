using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static AudioSource audioSrc;
    public static AudioClip pickCoin;
    public static AudioClip throwCoin;
    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        //使用Resources加载资源，也可以使用ab包框架，注意优缺点
        pickCoin = Resources.Load<AudioClip>("Music/PickCoin"); 
        throwCoin = Resources.Load<AudioClip>("Music/ThrowCoin");
        //audioSrc.PlayOneShot(pickCoin);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlayPickCoinClip()
    {
        audioSrc.PlayOneShot(pickCoin);
    }

    public static void PlayThrowCoinClip()
    {
        audioSrc.PlayOneShot(throwCoin);
    }
}
