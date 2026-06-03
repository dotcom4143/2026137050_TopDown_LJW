using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public enum WeaponElement { Normal, Fire, Water, Nature, Light, Darkness }
    
    [Header("현재 장착 무기")]
    public WeaponElement currentWeapon = WeaponElement.Normal;

    [Header("속성별 평타 프리팹 등록")]
    [SerializeField] private GameObject normalArrowPrefab;
    [SerializeField] private GameObject fireArrowPrefab;
    [SerializeField] private GameObject waterArrowPrefab;
    [SerializeField] private GameObject natureArrowPrefab;
    [SerializeField] private GameObject lightArrowPrefab;
    [SerializeField] private GameObject darknessArrowPrefab;

    [Header("속성별 스킬 프리팹 등록")]
    [SerializeField] private GameObject fireZonePrefab;
    [SerializeField] private GameObject natureZonePrefab; // 이 줄이 있어야 슬롯이 생겨!

    [Header("공격 설정")]
    [SerializeField] private Transform shotPoint;    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float bowDistance = 1.2f; 
    [SerializeField] private float attackDamage = 5f; 

    [Header("빛 스킬 설정")]
    [SerializeField] private float blinkDistance = 5f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("불 스킬 설정")]
    [SerializeField] private float fireZoneDuration = 4f;
    [SerializeField] private float fireZoneInterval = 0.5f;

    [Header("풀 스킬 설정")]
    [SerializeField] private float natureZoneDuration = 4f;

    private float natureAttackTimer;
    private float natureAttackCooldown = 1.5f; 

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        HandleDirectSwapInput();
        HandleCycleSwapInput();
        
        if (shotPoint == null || mainCamera == null) return;

        RotateAndPositionBow();

        if (natureAttackTimer > 0) natureAttackTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0)) ExecuteM1Attack();
        if (Input.GetMouseButtonDown(1)) ExecuteM2Skill();
    }

    private void HandleDirectSwapInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(WeaponElement.Normal);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(WeaponElement.Fire);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchWeapon(WeaponElement.Water);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchWeapon(WeaponElement.Nature);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SwitchWeapon(WeaponElement.Light);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SwitchWeapon(WeaponElement.Darkness);
    }

    private void HandleCycleSwapInput()
    {
        int currentIndex = (int)currentWeapon;
        int totalWeapons = System.Enum.GetValues(typeof(WeaponElement)).Length;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentIndex--;
            if (currentIndex < 0) currentIndex = totalWeapons - 1;
            SwitchWeapon((WeaponElement)currentIndex);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            currentIndex++;
            if (currentIndex >= totalWeapons) currentIndex = 0;
            SwitchWeapon((WeaponElement)currentIndex);
        }
    }

    private void SwitchWeapon(WeaponElement newWeapon)
    {
        currentWeapon = newWeapon;
    }

    private void RotateAndPositionBow()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetDirection = (mousePosition - transform.position).normalized;
        targetDirection.z = 0; 

        shotPoint.position = transform.position + targetDirection * bowDistance;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        shotPoint.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void ExecuteM1Attack()
    {
        switch (currentWeapon)
        {
            case WeaponElement.Normal: SpawnProjectile(normalArrowPrefab, "Normal"); break;
            case WeaponElement.Fire: SpawnProjectile(fireArrowPrefab, "Fire"); break;
            case WeaponElement.Water: SpawnProjectile(waterArrowPrefab, "Water"); break;
            case WeaponElement.Nature:
                if (natureAttackTimer <= 0f)
                {
                    SpawnProjectile(natureArrowPrefab, "Nature");
                    natureAttackTimer = natureAttackCooldown;
                }
                break;
            case WeaponElement.Light: SpawnProjectile(lightArrowPrefab, "Light"); break;
            case WeaponElement.Darkness: SpawnProjectile(darknessArrowPrefab, "Darkness"); break;
        }
    }

    private void SpawnProjectile(GameObject prefab, string element)
    {
        if (prefab == null) return;
        GameObject arrow = Instantiate(prefab, shotPoint.position, shotPoint.rotation);
        BaseProjectile projectileScript = arrow.GetComponent<BaseProjectile>();
        if (projectileScript != null) projectileScript.Setup(attackDamage, element);
    }

    private void ExecuteM2Skill()
    {
        switch (currentWeapon)
        {
            case WeaponElement.Normal: ExecuteNormalSkill(); break;
            case WeaponElement.Fire: ExecuteFireSkill(); break;
            case WeaponElement.Water: Debug.Log("물 스킬 발사"); break;
            case WeaponElement.Nature: ExecuteNatureSkill(); break;
            case WeaponElement.Light: ExecuteLightSkill(); break;
            case WeaponElement.Darkness: Debug.Log("어둠 스킬 발사"); break;
        }
    }

    private void ExecuteNormalSkill()
    {
        if (normalArrowPrefab == null) return;
        float[] angles = { -30f, -15f, 0f, 15f, 30f };
        foreach (float angleOffset in angles)
        {
            Quaternion rotationOffset = shotPoint.rotation * Quaternion.Euler(0, 0, angleOffset);
            GameObject arrow = Instantiate(normalArrowPrefab, shotPoint.position, rotationOffset);
            BaseProjectile projectileScript = arrow.GetComponent<BaseProjectile>();
            if (projectileScript != null) projectileScript.Setup(attackDamage, "Normal");
        }
    }

    private void ExecuteFireSkill()
    {
        if (fireZonePrefab == null) return;
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        GameObject zone = Instantiate(fireZonePrefab, mousePosition, Quaternion.identity);
        FireZone fireZoneScript = zone.GetComponent<FireZone>();
        if (fireZoneScript != null) fireZoneScript.Setup(attackDamage * 0.5f, fireZoneDuration, fireZoneInterval, "Fire");
    }

    private void ExecuteNatureSkill()
    {
        if (natureZonePrefab == null) return;
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        GameObject zone = Instantiate(natureZonePrefab, mousePosition, Quaternion.identity);
        NatureZone natureZoneScript = zone.GetComponent<NatureZone>();
        if (natureZoneScript != null) natureZoneScript.Setup(natureZoneDuration);
    }

    private void ExecuteLightSkill()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector3 blinkDirection = (mousePosition - transform.position).normalized;
        float targetDistance = Vector3.Distance(transform.position, mousePosition);
        if (targetDistance > blinkDistance) targetDistance = blinkDistance;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, blinkDirection, targetDistance, obstacleLayer);
        Vector3 finalBlinkPosition = (hit.collider != null) ? (Vector3)hit.point - (blinkDirection * 0.5f) : transform.position + (blinkDirection * targetDistance);
        
        transform.position = finalBlinkPosition;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero; 
    }
}