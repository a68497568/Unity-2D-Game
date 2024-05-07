using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogHurt : MonoBehaviour
{
    public const int maxHealth = 100;
    public int Health = maxHealth;
    public GameObject hp_bar;
    public Animator Anim;
    public int DeadCode = 0;

    public void Start()
    {
        Anim = GetComponent<Animator>();
    }

    public void Update()
    {
        float _precent = ((float)Health / (float)maxHealth);
        hp_bar.transform.localScale = new Vector3(_precent, hp_bar.transform.localScale.y, hp_bar.transform.localScale.z);
    }

    public void TakeDamage(int damage)
    {
        if (DeadCode == 0)
        {
            Health -= damage;
        }

        Anim.SetTrigger("Hurt");

        if (Health <= 0)
        {
            DeadCode = 1;
            GetComponent<Enemy_Frog>().DeadState(DeadCode);
            Health = 0;
            Anim.SetTrigger("Death");
        }
    }

}