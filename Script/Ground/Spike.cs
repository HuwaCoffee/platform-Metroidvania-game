using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public int damage;
    private PlayerHealth ph;
    // Start is called before the first frame update
    void Start()
    {
        ph = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player")&& collision.GetType().ToString()== "UnityEngine.CapsuleCollider2D")
        {
            ph.DamagePlayer(damage);
        }
    }
}
