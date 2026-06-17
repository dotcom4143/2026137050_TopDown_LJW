using UnityEngine;
using System.Collections.Generic;
using System.IO;

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

    [System.Serializable]
    private class UpgradeData
    {
        public int damageLevel = 0;
    }

    void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        InitializeMaps();
    }

    void Start()
    {
        ApplyJsonUpgrades();
    }

    private void InitializeMaps()
    {
        arrowPrefabs = new Dictionary<WeaponElement, GameObject> { { WeaponElement.Normal, normalArrow }, { WeaponElement.Fire, fireArrow }, { WeaponElement.Water, waterArrow }, { WeaponElement.Nature, natureArrow } };
        skillPrefabs = new Dictionary<WeaponElement, GameObject> { { WeaponElement.Normal, normalSkill }, { WeaponElement.Fire, fireZone }, { WeaponElement.Water, waterWave }, { WeaponElement.Nature, natureZone } };
        skillDataMap = new Dictionary<WeaponElement, SkillData> { { WeaponElement.Normal, normalData }, { WeaponElement.Fire, fireData }, { WeaponElement.Water, waterData }, { WeaponElement.Nature, natureData } };
    }

    void Update()
    {
        HandleWeaponSwitch();
        RotateAndPositionBow();
        if (Input.GetMouseButtonDown(0)) ExecuteM1Attack();
        if (Input.GetMouseButtonDown(1)) ExecuteM2Skill();
    }

    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentWeapon = WeaponElement.Normal;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) currentWeapon = WeaponElement.Fire;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) currentWeapon = WeaponElement.Water;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) currentWeapon = WeaponElement.Nature;
    }

    private void ExecuteM1Attack()
    {
        if (!arrowPrefabs.ContainsKey(currentWeapon) || arrowPrefabs[currentWeapon] == null) return;
        if (shotPoint == null) return;

        Vector3 spawnPos = transform.position;
        Vector3 shootDirection = (shotPoint.position - transform.position).normalized;
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Quaternion spawnRot = Quaternion.AngleAxis(angle, Vector3.forward);

        GameObject arrow = Instantiate(arrowPrefabs[currentWeapon], spawnPos, spawnRot);
        arrow.GetComponent<BaseProjectile>()?.Setup(attackDamage, currentWeapon.ToString());
    }

    private void ExecuteM2Skill()
    {
        if (!skillDataMap.ContainsKey(currentWeapon) || !CanUseSkill(skillDataMap[currentWeapon])) return;

        SkillData data = skillDataMap[currentWeapon];
        GameObject prefab = skillPrefabs[currentWeapon];
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 skillDir = (mousePos - transform.position).normalized;
        float skillAngle = Mathf.Atan2(skillDir.y, skillDir.x) * Mathf.Rad2Deg;
        Quaternion skillRotation = Quaternion.AngleAxis(skillAngle, Vector3.forward);

        Vector3 skillSpawnPos = (currentWeapon == WeaponElement.Water || currentWeapon == WeaponElement.Normal) ? transform.position : mousePos;
        GameObject skillObj = Instantiate(prefab, skillSpawnPos, skillRotation);
        
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

    private void ApplyJsonUpgrades()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "SaveData.json");

        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                UpgradeData data = JsonUtility.FromJson<UpgradeData>(json);

                attackDamage += data.damageLevel;

                Debug.Log($"[공격력 강화 적용 완료] 공격력 LV.{data.damageLevel} -> 최종 공격력: {attackDamage}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[강화 로드 실패] JSON 해석 중 오류 발생: {e.Message}");
            }
        }
        else
        {
            Debug.Log("[공격력 강화] 세이브 파일이 없어 기본 능력치로 시작합니다.");
        }
    }
}