using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using Unity.VisualScripting;
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

    void FixedUpdate()
    {
       Move(); 
    }

    void Move()
    {
        Vector2 forceVector = new Vector2(0,0);
        if (Input.GetKey(KeyCode.W))
        {
            Jump();
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            forceVector.x -= speed;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            forceVector.x += speed;
        }
        rb.AddForce(forceVector, ForceMode2D.Force);
    }

    void Jump()
    {
        // Cast a ray straight down.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1, LayerMask.GetMask("Floor"));

        // If it missing below check left and right
        if (hit.collider != null)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
        else
        {
          RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 1, LayerMask.GetMask("Floor"));
          if (hitLeft.collider != null)
          {
             rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);      
          }
          else
          { 
              RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 1, LayerMask.GetMask("Floor"));
              if (hitRight.collider != null)
              {
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);      
              }   
          }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.GetMask("Floor"))
        {
            
        }
    }

    void LeftRightMovement()
    {
        float xInput = Input.GetAxis("X");

        float xForce = xInput * speed * Time.deltaTime;

        Vector2 force = new Vector2(xForce, 0);
        rb.AddForce(force);
    }
}
