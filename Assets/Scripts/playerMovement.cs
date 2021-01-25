using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float jumpHeight;
    public GameObject camera;
    public bool landed = false;
    void Start()
    {
        
    }

    void Update()
    {

        if (Input.GetKey(KeyCode.D))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
        }

        if (Input.GetKey(KeyCode.A))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            landed = false;
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpHeight);
        }

       
        GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity * 0.98f;
    }
    public void moveRight()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
    }

    public void jump()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpHeight);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(landed == false)
        {
            Debug.Log("Hit");
            landed = true;
            camera.GetComponent<DynamicCameraController>().trauma = 0.7f;
        }
       
    }
}