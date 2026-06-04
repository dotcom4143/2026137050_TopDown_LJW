using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public enum WeaponElement { Normal, Fire, Water, Nature, Light, Darkness }

    [Header("현재 장착 무기")]
    public WeaponElement currentWeapon = WeaponElement.Normal;

    [Header("속성별 프리팹 등록")]
    [SerializeField] private GameObject normalArrowPrefab;
    [SerializeField] private GameObject fireArrowPrefab;
    [SerializeField] private GameObject waterArrowPrefab;
    [SerializeField] private GameObject natureArrowPrefab;
    [SerializeField] private GameObject lightArrowPrefab;
    [SerializeField] private GameObject darknessArrowPrefab;

    [Header("스킬 프리팹 등록")]
    [SerializeField] private GameObject fireZonePrefab;
    [SerializeField] private GameObject natureZonePrefab;
    [SerializeField] private GameObject waterWavePrefab;

    [Header("공격/이동 설정")]
    [SerializeField] private Transform shotPoint;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float bowDistance = 1.2f;
    [SerializeField] private float attackDamage = 5f;
    [SerializeField] private float blinkDistance = 5f;
    [SerializeField] private LayerMask obstacleLayer;

    private float natureAttackTimer;
    private float natureAttackCooldown = 1.5f;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        HandleInput();
        if (shotPoint == null || mainCamera == null) return;
        RotateAndPositionBow();
        if (natureAttackTimer > 0) natureAttackTimer -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0)) ExecuteM1Attack();
        if (Input.GetMouseButtonDown(1)) ExecuteM2Skill();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentWeapon = WeaponElement.Normal;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentWeapon = WeaponElement.Fire;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentWeapon = WeaponElement.Water;
        if (Input.GetKeyDown(KeyCode.Alpha4)) currentWeapon = WeaponElement.Nature;
        if (Input.GetKeyDown(KeyCode.Alpha5)) currentWeapon = WeaponElement.Light;
        if (Input.GetKeyDown(KeyCode.Alpha6)) currentWeapon = WeaponElement.Darkness;
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
        GameObject prefab = GetArrowPrefab();
        if (prefab == null) return;
        if (currentWeapon == WeaponElement.Nature && natureAttackTimer > 0) return;

        GameObject arrow = Instantiate(prefab, shotPoint.position, shotPoint.rotation);
        arrow.GetComponent<BaseProjectile>()?.Setup(attackDamage, currentWeapon.ToString());

        if (currentWeapon == WeaponElement.Nature) natureAttackTimer = natureAttackCooldown;
    }

    private void ExecuteM2Skill()
    {
        switch (currentWeapon)
        {
            case WeaponElement.Normal: ExecuteNormalSkill(); break;
            case WeaponElement.Fire: ExecuteFireSkill(); break;
            case WeaponElement.Water: ExecuteWaterSkill(); break;
            case WeaponElement.Nature: ExecuteNatureSkill(); break;
            case WeaponElement.Light: ExecuteLightSkill(); break;
        }
    }

    private GameObject GetArrowPrefab() => currentWeapon switch
    {
        WeaponElement.Normal => normalArrowPrefab,
        WeaponElement.Fire => fireArrowPrefab,
        WeaponElement.Water => waterArrowPrefab,
        WeaponElement.Nature => natureArrowPrefab,
        WeaponElement.Light => lightArrowPrefab,
        _ => darknessArrowPrefab
    };

    private void ExecuteNormalSkill() {}
    private void ExecuteFireSkill()
    {
        GameObject zone = Instantiate(fireZonePrefab, mainCamera.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
        zone.GetComponent<FireZone>()?.Setup(attackDamage * 0.5f, 4f, 0.5f, "Fire");
    }
    private void ExecuteWaterSkill()
    {
        Vector3 dir = (mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        dir.z = 0;
        Instantiate(waterWavePrefab, transform.position, Quaternion.identity).GetComponent<WaterWave>()?.Setup(dir);
    }
    private void ExecuteNatureSkill()
    {
        GameObject zone = Instantiate(natureZonePrefab, mainCamera.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
        zone.GetComponent<NatureZone>()?.Setup(4f);
    }
    private void ExecuteLightSkill() {}
}