using UnityEngine;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    public enum WeaponElement { Normal, Fire, Water, Nature, Light, Darkness }

    [Header("설정 및 참조")]
    public WeaponElement currentWeapon = WeaponElement.Normal;
    [SerializeField] private Transform shotPoint;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float bowDistance = 1.2f;
    [SerializeField] private float attackDamage = 5f;

    [Header("데이터 및 프리팹")]
    private Dictionary<WeaponElement, GameObject> arrowPrefabs = new Dictionary<WeaponElement, GameObject>();
    private Dictionary<WeaponElement, GameObject> skillPrefabs = new Dictionary<WeaponElement, GameObject>();
    private Dictionary<WeaponElement, SkillData> skillDataMap = new Dictionary<WeaponElement, SkillData>();

    [SerializeField] private SkillData normalData, fireData, waterData, natureData;
    [SerializeField] private GameObject normalArrow, fireArrow, waterArrow, natureArrow;
    [SerializeField] private GameObject normalSkill, fireZone, waterWave, natureZone;

    private Dictionary<string, float> lastUsedTimes = new Dictionary<string, float>();

    void Awake()
    {
        InitializeMaps();
    }

    private void InitializeMaps()
    {
        arrowPrefabs = new Dictionary<WeaponElement, GameObject> { { WeaponElement.Normal, normalArrow }, { WeaponElement.Fire, fireArrow }, { WeaponElement.Water, waterArrow }, { WeaponElement.Nature, natureArrow } };
        skillPrefabs = new Dictionary<WeaponElement, GameObject> { { WeaponElement.Normal, normalSkill }, { WeaponElement.Fire, fireZone }, { WeaponElement.Water, waterWave }, { WeaponElement.Nature, natureZone } };
        skillDataMap = new Dictionary<WeaponElement, SkillData> { { WeaponElement.Normal, normalData }, { WeaponElement.Fire, fireData }, { WeaponElement.Water, waterData }, { WeaponElement.Nature, natureData } };
    }

    void Update()
    {
        RotateAndPositionBow();
        if (Input.GetMouseButtonDown(0)) ExecuteM1Attack();
        if (Input.GetMouseButtonDown(1)) ExecuteM2Skill();
    }

    private void ExecuteM1Attack()
    {
        if (!arrowPrefabs.ContainsKey(currentWeapon)) return;
        GameObject arrow = Instantiate(arrowPrefabs[currentWeapon], shotPoint.position, shotPoint.rotation);
        arrow.GetComponent<BaseProjectile>()?.Setup(attackDamage, currentWeapon.ToString());
    }

    private void ExecuteM2Skill()
    {
        if (!skillDataMap.ContainsKey(currentWeapon) || !CanUseSkill(skillDataMap[currentWeapon])) return;

        SkillData data = skillDataMap[currentWeapon];
        GameObject prefab = skillPrefabs[currentWeapon];
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        GameObject skillObj = Instantiate(prefab, (currentWeapon == WeaponElement.Water) ? transform.position : mousePos, Quaternion.identity);
        
        ApplySkillData(skillObj, data, mousePos);

        lastUsedTimes[data.skillName] = Time.time;
    }

    private void ApplySkillData(GameObject obj, SkillData data, Vector3 targetPos)
    {
        if (obj.TryGetComponent<WaterWave>(out var water)) water.Setup((targetPos - transform.position).normalized, data.damage);
        else if (obj.TryGetComponent<FireZone>(out var fire)) fire.Setup(data.damage, data.duration, 0.5f, "Fire");
        else if (obj.TryGetComponent<NatureZone>(out var nature)) nature.Setup(data.duration, data.damage);
        else if (obj.TryGetComponent<NormalSkill>(out var normal)) normal.Setup(data.damage);
    }

    private bool CanUseSkill(SkillData data) => data != null && (!lastUsedTimes.ContainsKey(data.skillName) || Time.time >= lastUsedTimes[data.skillName] + data.cooldown);

    private void RotateAndPositionBow()
    {
        Vector3 dir = (mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        dir.z = 0;
        shotPoint.position = transform.position + dir * bowDistance;
        shotPoint.rotation = Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, Vector3.forward);
    }
}