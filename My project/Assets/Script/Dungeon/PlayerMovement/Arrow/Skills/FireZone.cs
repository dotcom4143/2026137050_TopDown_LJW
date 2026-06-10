using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireZone : MonoBehaviour
{
    private float damage;
    private float duration;
    private float interval;
    private string element;
    private List<EnemyController> enemiesInZone = new List<EnemyController>();

    public void Setup(float dmg, float dur, float inter, string ele)
    {
        this.damage = dmg;
        this.duration = dur;
        this.interval = inter;
        this.element = ele;
        
        StartCoroutine(DotDamageRoutine());
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemy = collision.GetComponent<EnemyController>();
        if (enemy != null && !enemiesInZone.Contains(enemy))
        {
            enemiesInZone.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        EnemyController enemy = collision.GetComponent<EnemyController>();
        if (enemy != null && enemiesInZone.Contains(enemy))
        {
            enemiesInZone.Remove(enemy);
        }
    }

    private IEnumerator DotDamageRoutine()
    {
        float timer = 0f;
        while (timer < duration)
        {
            for (int i = enemiesInZone.Count - 1; i >= 0; i--)
            {
                if (enemiesInZone[i] == null)
                {
                    enemiesInZone.RemoveAt(i);
                    continue;
                }
                enemiesInZone[i].TakeDamage(damage, element);
            }
            timer += interval;
            yield return new WaitForSeconds(interval);
        }
    }
}