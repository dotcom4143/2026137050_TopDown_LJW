using System.Collections.Generic;
using UnityEngine;

public class InfiniteMapController : MonoBehaviour
{
    [System.Serializable]
    public class ChunkData
    {
        public Vector2Int chunkCoord;
        public GameObject spawnedChunkObject;
        
        public ChunkData(Vector2Int coord)
        {
            chunkCoord = coord;
        }
    }

    [Header("필수 연결 컴포넌트")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject chunkPrefab;

    [Header("맵 생성 규칙 설정")]
    [SerializeField] private int chunkSize = 14;
    [SerializeField] private int viewDistance = 3; 

    private Grid gridSystem;
    private Vector2Int currentChunkCoord;
    private Dictionary<Vector2Int, ChunkData> allChunks = new Dictionary<Vector2Int, ChunkData>();
    private Dictionary<Vector2Int, GameObject> activeChunkObjects = new Dictionary<Vector2Int, GameObject>();
    
    private Queue<GameObject> chunkPool = new Queue<GameObject>();

    private void Start()
    {
        if (playerTransform == null || chunkPrefab == null)
        {
            return;
        }

        gridSystem = GetComponentInParent<Grid>();
        if (gridSystem == null)
        {
            gridSystem = FindFirstObjectByType<Grid>();
        }

        UpdateCurrentChunk();
        UpdateVisibleChunks();
    }

    private void Update()
    {
        Vector2Int newChunkCoord = GetChunkCoordFromPosition(playerTransform.position);
        if (newChunkCoord != currentChunkCoord)
        {
            currentChunkCoord = newChunkCoord;
            UpdateVisibleChunks();
        }
    }

    private Vector2Int GetChunkCoordFromPosition(Vector3 position)
    {
        if (gridSystem != null)
        {
            Vector3Int cellPos = gridSystem.WorldToCell(position);
            int x = Mathf.FloorToInt((float)cellPos.x / chunkSize);
            int y = Mathf.FloorToInt((float)cellPos.y / chunkSize);
            return new Vector2Int(x, y);
        }
        else
        {
            int x = Mathf.FloorToInt(position.x / chunkSize);
            int y = Mathf.FloorToInt(position.y / chunkSize);
            return new Vector2Int(x, y);
        }
    }

    private void UpdateCurrentChunk()
    {
        if (playerTransform != null)
        {
            currentChunkCoord = GetChunkCoordFromPosition(playerTransform.position);
        }
    }

    private void UpdateVisibleChunks()
    {
        HashSet<Vector2Int> currentFrameChunks = new HashSet<Vector2Int>();

        for (int xOffset = -viewDistance; xOffset <= viewDistance; xOffset++)
        {
            for (int yOffset = -viewDistance; yOffset <= viewDistance; yOffset++)
            {
                Vector2Int targetChunkCoord = new Vector2Int(currentChunkCoord.x + xOffset, currentChunkCoord.y + yOffset);
                currentFrameChunks.Add(targetChunkCoord);

                if (!allChunks.ContainsKey(targetChunkCoord))
                {
                    allChunks.Add(targetChunkCoord, new ChunkData(targetChunkCoord));
                }

                if (!activeChunkObjects.ContainsKey(targetChunkCoord))
                {
                    GameObject chunkObj = GetChunkFromPool();
                    
                    if (gridSystem != null)
                    {
                        Vector3Int targetCellPos = new Vector3Int(targetChunkCoord.x * chunkSize, targetChunkCoord.y * chunkSize, 0);
                        chunkObj.transform.position = gridSystem.CellToWorld(targetCellPos);
                    }
                    else
                    {
                        chunkObj.transform.position = new Vector3(targetChunkCoord.x * chunkSize, targetChunkCoord.y * chunkSize, 0);
                    }

                    chunkObj.transform.rotation = Quaternion.identity;
                    chunkObj.transform.localScale = Vector3.one;
                    chunkObj.SetActive(true);

                    activeChunkObjects.Add(targetChunkCoord, chunkObj);
                    allChunks[targetChunkCoord].spawnedChunkObject = chunkObj;
                }
            }
        }

        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (var kvp in activeChunkObjects)
        {
            if (!currentFrameChunks.Contains(kvp.Key))
            {
                GameObject chunkObj = kvp.Value;
                chunkObj.SetActive(false);
                chunkPool.Enqueue(chunkObj);

                chunksToRemove.Add(kvp.Key);
                if (allChunks.ContainsKey(kvp.Key))
                {
                    allChunks[kvp.Key].spawnedChunkObject = null;
                }
            }
        }

        foreach (Vector2Int coord in chunksToRemove)
        {
            activeChunkObjects.Remove(coord);
        }
    }

    private GameObject GetChunkFromPool()
    {
        if (chunkPool.Count > 0)
        {
            return chunkPool.Dequeue();
        }
        else
        {
            GameObject newChunk = Instantiate(chunkPrefab);
            if (gridSystem != null)
            {
                newChunk.transform.SetParent(gridSystem.transform, true);
            }
            else
            {
                newChunk.transform.SetParent(this.transform, true);
            }
            return newChunk;
        }
    }
}