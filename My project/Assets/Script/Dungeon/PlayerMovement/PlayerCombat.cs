using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public enum WeaponElement { Normal, Fire, Water, Nature, Light, Darkness }
    
    [Header("현재 장착 무기")]
    public WeaponElement currentWeapon = WeaponElement.Normal;

    [Header("공격 설정")]
    [SerializeField] private GameObject arrowPrefab; 
    [SerializeField] private Transform shotPoint;    
    [SerializeField] private float bowDistance = 1.2f; 
    [SerializeField] private float attackDamage = 5f; 

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleDirectSwapInput();
        HandleCycleSwapInput();
        RotateAndPositionBow();

        if (Input.GetMouseButtonDown(0))
        {
            AttackM1();
        }

        if (Input.GetMouseButtonDown(1))
        {
            SkillM2();
        }
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
        Debug.Log($"무기 변경: {currentWeapon}");
    }

    private void RotateAndPositionBow()
    {
        if (shotPoint == null) return;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetDirection = (mousePosition - transform.position).normalized;
        targetDirection.z = 0; 

        shotPoint.position = transform.position + targetDirection * bowDistance;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        shotPoint.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void AttackM1()
    {
        if (arrowPrefab == null || shotPoint == null) return;
        
        GameObject arrow = Instantiate(arrowPrefab, shotPoint.position, shotPoint.rotation);
        ArrowInstance arrowScript = arrow.GetComponent<ArrowInstance>();
        if (arrowScript != null)
        {
            arrowScript.SetupArrow(attackDamage, currentWeapon.ToString());
        }
        Debug.Log($"{currentWeapon} 속성 평타 발사");
    }

    private void SkillM2()
    {
        Debug.Log($"{currentWeapon} 속성 스킬 발사");
    }
}