using UnityEngine;
using System.Collections;

public class FireArrow : BaseProjectile
{
    protected override void OnHitEnemy(EnemyController enemy)
    {
        enemy.TakeDamage(damage, element);
        
        EnemyDotEffect dot = enemy.gameObject.GetComponent<EnemyDotEffect>();
        if (dot == null)
        {
            dot = enemy.gameObject.AddComponent<EnemyDotEffect>();
        }
        dot.StartDot(damage * 0.2f, 3f, 0.5f, element);

        Destroy(gameObject);
    }
}

public class EnemyDotEffect : MonoBehaviour
{
    private EnemyController enemy;

    private void Awake()
    {
        enemy = GetComponent<EnemyController>();
    }

    public void StartDot(float tickDamage, float duration, float interval, string element)
    {
        StartCoroutine(DotRoutine(tickDamage, duration, interval, element));
    }

    private IEnumerator DotRoutine(float tickDamage, float duration, float interval, string element)
    {
        float timer = 0f;
        while (timer < duration)
        {
            if (enemy == null) yield break;
            enemy.TakeDamage(tickDamage, element);
            timer += interval;
            yield return new WaitForSeconds(interval);
        }
        Destroy(this);
    }
}