using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    private MonsterData myData;
    private float currentHp;
    private float currentMoveSpeed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    public void Setup(MonsterData data)
    {
        myData = data;
        
        if (spriteRenderer != null) spriteRenderer.sprite = data.monsterSprite;
        currentHp = data.maxHp;
        currentMoveSpeed = data.moveSpeed;
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * currentMoveSpeed * Time.deltaTime;
        }
    }

    public void ApplySlow(float slowAmount)
    {
        currentMoveSpeed *= slowAmount;
    }

    public void RemoveSlow(float slowAmount)
    {
        currentMoveSpeed /= slowAmount;
    }

    public void TakeDamage(float amount, string weaponElement)
    {
        float finalDamage = CalculateElementalDamage(amount, weaponElement);
        currentHp -= finalDamage;

        Debug.Log($"{myData.monsterName}이 {weaponElement} 속성 공격을 받아 {finalDamage}의 데미지를 입음! (남은체력: {currentHp})");

        if (currentHp <= 0)
        {
            DieAndDropCoin();
        }
    }

    private float CalculateElementalDamage(float baseDamage, string weaponElement)
    {
        if (myData == null) return baseDamage;

        switch (myData.monsterElement)
        {
            case MonsterData.ElementType.Nature:
                if (weaponElement == "Fire") return baseDamage * 2f;
                break;
            case MonsterData.ElementType.Fire:
                if (weaponElement == "Water") return baseDamage * 2f;
                break;
            case MonsterData.ElementType.Water:
                if (weaponElement == "Nature") return baseDamage * 2f;
                break;
        }
        return baseDamage;
    }

    private void DieAndDropCoin()
    {
        if (myData != null)
        {
            if (myData.deathSound != null) AudioSource.PlayClipAtPoint(myData.deathSound, transform.position);
            if (myData.coinPrefab != null) Instantiate(myData.coinPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}