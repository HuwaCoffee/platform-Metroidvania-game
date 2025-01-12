using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrans : MonoBehaviour
{
    public GameObject img1;
    public GameObject img2;
    public float time;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("°´H");
            anim.SetBool("ChangeToWhite", true);
            Invoke("ChangeImag", time);
        }
        
    }

    //
    void ChangeImag()
    {
        img1.SetActive(false);
        img2.SetActive(true);
    }
}
