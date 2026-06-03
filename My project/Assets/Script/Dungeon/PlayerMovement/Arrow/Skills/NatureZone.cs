using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NatureZone : MonoBehaviour
{
    private float duration;
    private float slowAmount = 0.5f;
    private List<EnemyController> affectedEnemies = new List<EnemyController>();

    public void Setup(float dur)
    {
        duration = dur;
        Debug.Log($"[로그] 풀 장판 생성: {duration}초 후 삭제 시작");
        StartCoroutine(DestroyRoutine());
    }

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(duration);
        Debug.Log("[로그] 시간이 되어 장판을 삭제합니다.");
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemy = collision.GetComponent<EnemyController>();
        if (enemy != null && !affectedEnemies.Contains(enemy))
        {
            affectedEnemies.Add(enemy);
            enemy.ApplySlow(slowAmount);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        EnemyController enemy = collision.GetComponent<EnemyController>();
        if (enemy != null && affectedEnemies.Contains(enemy))
        {
            enemy.RemoveSlow(slowAmount);
            affectedEnemies.Remove(enemy);
        }
    }

    private void OnDestroy()
    {
        foreach (var enemy in affectedEnemies)
        {
            if (enemy != null)
            {
                enemy.RemoveSlow(slowAmount);
            }
        }
    }
}