using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossHurt : MonoBehaviour
{
    public const int maxHealth = 500;
    public int Health = maxHealth;
    public GameObject hp_bar;
    public Animator Anim;
    public int DeadCode = 0;
    public bool isHurt;
    public GameObject player;

    public void Start()
    {
        Anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update()
    {
        float _precent = ((float)Health / (float)maxHealth);
        hp_bar.transform.localScale = new Vector3(_precent, hp_bar.transform.localScale.y, hp_bar.transform.localScale.z);
    }

    public void TakeDamage(int damage)
    {                 
            if (!isHurt && DeadCode == 0)
            {
                Health -= damage;
                Anim.SetTrigger("Hurt");
                isHurt = true;
                Invoke("HurtClose", 2f);
            }

        if (Health <= 0)
        {
            DeadCode = 1;
            Health = 0;         
            GetComponent<BossAI>().DeadState(DeadCode);           
            Invoke("DeathOpen", 1f);
            player.GetComponent<PlayerController_BossBT>().BossDead(DeadCode);
        }
    }

    public void HurtClose()
    {
        isHurt = false;
    }

    public void DeathOpen()
    {     
            Anim.SetTrigger("Death");            
    }

}