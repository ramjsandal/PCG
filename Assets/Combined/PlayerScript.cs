using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private float jumpForce = 10;
    private Rigidbody2D rb;
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
       Move(); 
    }

    void Move()
    {
        Vector2 forceVector = new Vector2(0,0);
        if (Input.GetKey(KeyCode.W))
        {
            forceVector.y += jumpForce;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            forceVector.x -= speed;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            forceVector.x += speed;
        }
        rb.AddForce(forceVector, ForceMode2D.Impulse);
    }
    
}
