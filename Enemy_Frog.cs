using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Frog : Enemy
{

    private Rigidbody2D rb;
    //private Animator Anim;
    private Collider2D Coll;
    public LayerMask Ground;
    public Transform leftpoint, rightpoint;
    public float Speed,JumpForce;
    public float leftx, rightx;
    private bool Faceleft = true;
    public int DeathCode = 0;
    

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        //Anim = GetComponent<Animator>();
        Coll = GetComponent<Collider2D>();

        
        leftx = leftpoint.position.x;
        rightx = rightpoint.position.x;
        Destroy(leftpoint.gameObject);
        Destroy(rightpoint.gameObject);

    }

   
    void Update()
    {
        SwitchAnim();
        if(DeathCode == 1)
        {
            Anim.SetTrigger("Death");
        }
    }

    void Movement()
    {
        if(DeathCode == 0)
        {
            if (Faceleft)
            {
                if (Coll.IsTouchingLayers(Ground))
                {
                    Anim.SetBool("jumping", true);
                    rb.velocity = new Vector2(-Speed, JumpForce);
                }
                if (transform.position.x < leftx)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    Faceleft = false;
                }
            }
            else
            {
                if (Coll.IsTouchingLayers(Ground))
                {
                    Anim.SetBool("jumping", true);
                    rb.velocity = new Vector2(Speed, JumpForce);
                }
                if (transform.position.x > rightx)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    Faceleft = true;
                }
            }
        }      
    }

    void SwitchAnim()
    {
        if (Anim.GetBool("jumping"))
        {
            if(rb.velocity.y < 0.1)
            {
                Anim.SetBool("jumping", false);
                Anim.SetBool("falling", true);
            }
        }
        if (Coll.IsTouchingLayers(Ground) && Anim.GetBool("falling"))
        {
            Anim.SetBool("falling",false);
        }
    }

    public void DeadState(int DTCode)
    {
        DeathCode = DTCode;
    }

}
