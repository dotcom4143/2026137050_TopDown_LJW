using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterData", menuName = "ScriptableObject/MonsterData")]
public class MonsterData : ScriptableObject
{
    public string monsterName;

    public enum ElementType { Normal, Water, Fire, Nature }

    public ElementType monsterElement;
    
    public float maxHp;
    public float moveSpeed;

    public Sprite monsterSprite;

    [Header("사운드 & 드롭 아이템")]
    public AudioClip deathSound; 
    public GameObject coinPrefab;
}