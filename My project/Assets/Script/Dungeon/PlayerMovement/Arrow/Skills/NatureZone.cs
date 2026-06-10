using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NatureZone : MonoBehaviour
{
    private float duration;
    private float damage;
    private float slowAmount = 0.5f;
    private List<EnemyController> affectedEnemies = new List<EnemyController>();

    public void Setup(float dur, float dmg) 
    {
        duration = dur;
        damage = dmg;
        StartCoroutine(DestroyRoutine());
        StartCoroutine(DotDamageRoutine());
    }

    private IEnumerator DotDamageRoutine()
    {
        while (true)
        {
            foreach (var enemy in affectedEnemies)
            {
                if (enemy != null) enemy.TakeDamage(damage, "Nature");
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}