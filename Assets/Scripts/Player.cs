using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Hp;
    public float Damage;
    public float Speed;
    public float AtackSpeed;
    public float AttackRange = 2;

    private float lastAttackTime = 0;
    private bool isDead = false;
    public Animator AnimatorController;

    public bool hasEnemiesNearby = false;
    public float SuperDamage;

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (Hp <= 0)
        {
            Die();
            return;
        }

        var enemies = SceneManager.Instance.Enemies;
        for (int i = 0; i < enemies.Count; i++)
        {
            var enemie = enemies[i];
            if (enemie == null)
            {
                hasEnemiesNearby = false;
                break;
            }

            var distance = Vector3.Distance(transform.position, enemie.transform.position);

            if (distance <= AttackRange)
            {
                hasEnemiesNearby = true;
                break;
            }
            hasEnemiesNearby = false;
        }
    }

    //Attack a single enemy when it's reachable
    public void Attack()
    {
        var enemies = SceneManager.Instance.Enemies;
        Enemie closestEnemie = null;

        if (Time.time - lastAttackTime > AtackSpeed)
        {
            lastAttackTime = Time.time;
            AnimatorController.SetTrigger("Attack");

            for (int i = 0; i < enemies.Count; i++)
            {
                var enemie = enemies[i];
                if (enemie == null)
                {
                    continue;
                }

                if (closestEnemie == null)
                {
                    closestEnemie = enemie;
                    continue;
                }

                var distance = Vector3.Distance(transform.position, enemie.transform.position);
                var closestDistance = Vector3.Distance(transform.position, closestEnemie.transform.position);

                if (distance < closestDistance)
                {
                    closestEnemie = enemie;
                }

            }

            if (closestEnemie != null)
            {
                var distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
                if (distance <= AttackRange)
                {
                    transform.transform.rotation = Quaternion.LookRotation(closestEnemie.transform.position - transform.position);

                    closestEnemie.Hp -= Damage;                    
                }
            }
        }

    }

    //Attack multiple enemies that are reachable at once
    public void SuperAttack()
    {

        var enemies = SceneManager.Instance.Enemies;

        List<Enemie> closestEnemies = new List<Enemie>();
        if (Time.time - lastAttackTime > AtackSpeed)
        {

            lastAttackTime = Time.time;
            AnimatorController.SetTrigger("SuperAttack");

            for (int i = 0; i < enemies.Count; i++)
            {
                var enemie = enemies[i];
                if (enemie == null)
                {
                    break;
                }

                var distance = Vector3.Distance(transform.position, enemie.transform.position);

                if (distance <= AttackRange)
                {
                    closestEnemies.Add(enemie);
                }
            }

            for (int i = 0; i < closestEnemies.Count; i++)
            {
                closestEnemies[i].Hp -= SuperDamage;
            }
        }
    }

    private void Die()
    {
        isDead = true;
        AnimatorController.SetTrigger("Die");

        SceneManager.Instance.GameOver();
    }

    public void AddHP(int hp)
    {
        Hp += hp;
    }
}
