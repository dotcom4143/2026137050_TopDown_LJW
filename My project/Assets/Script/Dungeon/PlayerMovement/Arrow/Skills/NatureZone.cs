using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NatureZone : MonoBehaviour
{
    private float duration;
    private float damage;
    private List<EnemyController> affectedEnemies = new List<EnemyController>();

    public void Setup(float dur, float dmg) 
    {
        duration = dur;
        damage = dmg;
        StartCoroutine(DotDamageRoutine());
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null && !affectedEnemies.Contains(enemy))
            {
                affectedEnemies.Add(enemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null && affectedEnemies.Contains(enemy))
            {
                affectedEnemies.Remove(enemy);
            }
        }
    }

    private IEnumerator DotDamageRoutine()
    {
        while (true)
        {
            for (int i = affectedEnemies.Count - 1; i >= 0; i--)
            {
                if (affectedEnemies[i] == null)
                {
                    affectedEnemies.RemoveAt(i);
                    continue;
                }
                affectedEnemies[i].TakeDamage(damage, "Nature");
            }
            yield return new WaitForSeconds(1.0f);
        }
    }
}