using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HeroKnight : MonoBehaviour
{

    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    
    public Rigidbody2D rb;
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private Coroutine inputDelayRoutine;
    private float skillInputDelay = 0.5f;
    public bool falling;
    public bool isdefense;
    public bool isHurt;
    public RectTransform HealthBar, Hurt;
    public Collider2D coll;
    public LayerMask Ground;
    public const int maxHealth = 200;
    public int currentHealth = maxHealth;
    public bool isDeath;
    public Animator anim;
    public AudioSource CureAudio, cherryAudio, chestAudio;
    //public AudioSource hurtAudio, DeathAudio;




    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    // Update is called once per frame
    void Update()
    {
        SwitchAnim();
        health();

        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            anim.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            anim.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0 && !isHurt && !isDeath)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }

        else if (inputX < 0 && !isHurt && !isDeath)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }



        // Move
        if (!m_rolling && !isdefense && !isHurt && !isDeath)
        {
            rb.velocity = new Vector2(inputX * m_speed, rb.velocity.y);
        }



        //Set AirSpeed in animator
        anim.SetFloat("AirSpeedY", rb.velocity.y);



        





     



        //Attack
        if (Input.GetKeyDown("j") && m_timeSinceAttack > 0.35f && !isHurt && !isdefense && !isDeath)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
            {
                m_currentAttack = 1;
            }


            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.40f)
            {
                m_currentAttack = 1;
            }

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            anim.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;
        }






        // Block
        else if (Input.GetKeyDown("k") && !isHurt && !isDeath)
        {
            rb.velocity = new Vector2(0, 0);
            isdefense = true;
            anim.SetTrigger("Block");
            anim.SetBool("IdleBlock", true);
        }

        else if (Input.GetKeyUp("k"))
        {
            anim.SetBool("IdleBlock", false);
            isdefense = false;

        }



        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && !isdefense && !isDeath)
        {
            
            anim.SetTrigger("Roll");
            rb.velocity = new Vector2(m_facingDirection * m_rollForce, rb.velocity.y);
        }


        //Jump
        else if (Input.GetKeyDown("space") && m_grounded && !isdefense && coll.IsTouchingLayers(Ground) && !isHurt && !isDeath)
        {
            anim.SetTrigger("Jump");
            m_grounded = false;
            anim.SetBool("Grounded", m_grounded);
            rb.velocity = new Vector2(rb.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            anim.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                anim.SetInteger("AnimState", 0);
        }

    }

    //碰撞觸發器
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "DeadLine")
        {
            //GetComponent<AudioSource>().enabled = false;
            Invoke("Restart", 2f); //在2秒後使用Restart這個函式
            //DeathAudio.Play();
        }

        if (collision.tag == "NextLevel")
        {
            Invoke("Nextscene", 2f);
        }

        if (collision.tag == "Chest")
        {
            Destroy(collision.gameObject);
            currentHealth += 100;
            if (currentHealth > 200)
            {
                currentHealth = 200;
            }
            chestAudio.Play();
        }

        if(collision.tag == "SmallCure")
        {
            Destroy(collision.gameObject);
            currentHealth += 20;
            if(currentHealth > 200)
            {
                currentHealth = 200;
            }
            CureAudio.Play();
        }

        if (collision.tag == "Collection")
        {        
            collision.GetComponent<Animator>().Play("Cherry-Destroy");
            cherryAudio.Play();
        }

    }

    // Animation Events
    // Called in end of roll animation.
    void AE_ResetRoll()
    {
        m_rolling = false;
    }

    public void health()
    {
        HealthBar.sizeDelta = new Vector2(currentHealth, HealthBar.sizeDelta.y);

        //呈現傷害量

        if (Hurt.sizeDelta.x > HealthBar.sizeDelta.x)

        {

            //讓傷害量(紅色血條)逐漸追上當前血量

            Hurt.sizeDelta += new Vector2(-1, 0) * Time.deltaTime * 20;

        }
    }

    //受傷效果
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy-Animal" && !isDeath && !isHurt && !isdefense)   //敵人設計
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                if (transform.position.x < collision.gameObject.transform.position.x)
                {
                    HurtEffect();
                }
                else if (transform.position.x > collision.gameObject.transform.position.x)
                {
                    HurtEffect();
                }
             }
    }


    void Nextscene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    

    void HurtClose()
    {
        isHurt = false;
    }

    void HurtEffect()
    {
        isHurt = true;
        anim.SetTrigger("Hurt");
        rb.velocity = new Vector2(2, rb.velocity.y);
        //hurtAudio.Play();
        if (!isdefense)
        {
            currentHealth = currentHealth - 15;
        }

        if (currentHealth <= 0) //死亡效果
        {
            anim.SetBool("noBlood", m_noBlood);
            anim.SetTrigger("Death");
            Invoke("Restart", 2f);
            isDeath = true;
            rb.velocity = new Vector2(0, 0);
            //DeathAudio.Play();
        }
    }


    void SwitchAnim()
    {
        if (rb.velocity.y < 0.1f && !m_grounded)
        {
            anim.SetBool("falling", true);
        }

        else if (m_grounded)
        {
            anim.SetBool("falling", false);
        }

        if(isHurt) //受傷效果
        {
            anim.SetFloat("running", 0);
            if (Mathf.Abs(rb.velocity.x) < 2.0f)
            {
                Invoke("HurtClose", 0.5f);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        //hurtAudio.Play();

        if (!isdefense)
        {
            currentHealth -= damage;
            anim.SetTrigger("Hurt");
        }
        

        if (currentHealth <= 0)
        {
            anim.SetBool("noBlood", m_noBlood);
            anim.SetTrigger("Death");
            Invoke("Restart", 2f);
            isDeath = true;
            rb.velocity = new Vector2(0, 0);
        }
    }

    public void Restart()
    {
        isDeath = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }




    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }

    public void CherryCount()
    {
        currentHealth += 10;
        if (currentHealth > 200)
        {
            currentHealth = 200;
        }
    }

}
