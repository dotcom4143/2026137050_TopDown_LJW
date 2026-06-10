using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillUI : MonoBehaviour
{
    [Header("UI 구성 요소")]
    public Image iconImage;
    public Image cooldownOverlay;

    void Awake()
    {
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0f;
        }
    }

    public void StartCooldown(float totalCooldown)
    {
        if (cooldownOverlay != null)
        {
            StopAllCoroutines();
            StartCoroutine(CooldownRoutine(totalCooldown));
        }
    }

    private IEnumerator CooldownRoutine(float totalCooldown)
    {
        float timer = totalCooldown;
        cooldownOverlay.fillAmount = 1f;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            cooldownOverlay.fillAmount = timer / totalCooldown;
            yield return null; 
        }

        cooldownOverlay.fillAmount = 0f;
    }
}