using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace inonego
{

[Serializable]
public class Pool
{
    /// <summary>
    /// 계층 상에서 부모가 되는 트랜스폼
    /// </summary>
    public Transform Parent { get; private set; }

    /// <summary>
    /// 풀에 생성할 게임 오브젝트의 프리팹
    /// </summary>
    public GameObject Prefab;

    /// <summary>
    /// 풀에 남아있는 게임 오브젝트 목록
    /// </summary>
    private Queue<GameObject> left = new Queue<GameObject>();
    public IReadOnlyCollection<GameObject> Left => left;
    
    /// <summary>
    /// 스폰된 게임 오브젝트 목록
    /// </summary>  
    private List<GameObject> spawned = new List<GameObject>();
    public IReadOnlyList<GameObject> Spawned => spawned;
    
    /// <summary>
    /// 초기 생성 개수
    /// </summary>
    public int InitalCount = 0;
    
    /// <summary>
    /// 풀에 남아있는 게임 오브젝트 개수
    /// </summary>
    public int LeftCount    => left.Count;
    
    /// <summary>
    /// 현재 스폰된 게임 오브젝트 개수
    /// </summary>
    public int SpawnedCount => spawned.Count;
    
    /// <summary>
    /// 풀에 있는 게임 오브젝트 총 개수
    /// </summary>
    public int TotalCount   => LeftCount + SpawnedCount;

    /// <summary>
    /// 풀을 초기화합니다.
    /// </summary>
    /// <param name="parent">부모 트랜스폼</param>
    public void Init(Transform parent)
    {
        Parent = parent;
        
        // 초기 개수만큼 게임 오브젝트를 추가합니다.
        for (int i = 0; i < InitalCount; i++)
        {
            left.Enqueue(InstantiateGO());
        }
    }

    #region 부모 트랜스폼
    
    private void AttachToParent(GameObject GO)
    {
        GO.transform.SetParent(Parent);
        
        // 위치 및 회전 설정
        GO.transform.localPosition = Vector3.zero;
        GO.transform.localRotation = Quaternion.identity;
    }

    private void DetachFromParent(GameObject GO, Vector3 position, Quaternion rotation)
    {
        GO.transform.SetParent(null);
        
        // 위치 및 회전 설정
        GO.transform.position = position;
        GO.transform.rotation = rotation;
    }

    #endregion

    /// <summary>
    /// 게임 오브젝트를 생성합니다.
    /// </summary>
    /// <returns>생성된 게임 오브젝트</returns>
    private GameObject InstantiateGO()
    {
        GameObject GO = GameObject.Instantiate(Prefab);
        
        // 게임 오브젝트 상태 설정
        GO.SetActive(false);
        AttachToParent(GO);

        return GO;
    }

    /// <summary>
    /// 게임 오브젝트를 스폰합니다.
    /// </summary>
    /// <returns>스폰된 게임 오브젝트</returns>
    public GameObject Spawn()
    {
        return Spawn(Vector3.zero, Quaternion.identity);
    }

    /// <summary>
    /// 게임 오브젝트를 스폰합니다.
    /// </summary>
    /// <param name="position">위치</param>
    /// <param name="rotation">회전</param>
    /// <returns>스폰된 게임 오브젝트</returns>
    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        // 풀 목록에서 제거
        if (!left.TryDequeue(out GameObject GO))
        {
            // 부족하면 게임 오브젝트 생성
            GO = InstantiateGO();
        }

        // 게임 오브젝트 상태 설정
        DetachFromParent(GO, position, rotation);
        GO.SetActive(true);
        
        // 스폰 목록에 추가
        spawned.Add(GO);
        PoolUtil.Register(this, GO);
        
        return GO;
    }

    /// <summary>
    /// 게임 오브젝트를 디스폰합니다.
    /// </summary>
    /// <param name="GO">디스폰할 게임 오브젝트</param>
    internal void Despawn(GameObject GO)
    {
        if (spawned.Contains(GO))
        {
            // 스폰 목록에서 제거
            spawned.Remove(GO);
            
            // 게임 오브젝트 상태 설정
            GO.SetActive(false);
            AttachToParent(GO);

            // 풀 목록에 추가
            left.Enqueue(GO);
            PoolUtil.Remove(GO);
        }
        #if UNITY_EDITOR
            else
            { 
                Debug.LogError($"{GO.name}(은)는 {Prefab.name}의 풀에 존재하지 않습니다.");
            }
        #endif
    }
    
    private List<GameObject> GOToBeDespawned = new List<GameObject>();
        
    /// <summary>
    /// 모든 스폰된 게임 오브젝트를 디스폰합니다.
    /// </summary>
    public void DespawnAll()
    {
        GOToBeDespawned.AddRange(spawned);

        for (int i = 0; i < GOToBeDespawned.Count; i++)
        {
            Despawn(GOToBeDespawned[i]);
        }
        
        GOToBeDespawned.Clear();
    }
}

public class PoolPack : MonoBehaviour
{
    [SerializeField] private List<Pool> poolList = new List<Pool>();

    public IReadOnlyList<Pool> PoolList => poolList;

    private void Awake()
    {
        foreach (var pool in poolList)
        {
            pool.Init(transform);
        }
    }

    private void OnDestroy()
    {
        DespawnAll();
    }

    public void DespawnAll()
    {
        foreach (var pool in poolList)
        {
            pool.DespawnAll();
        }
    }
}

public static class PoolUtil
{
    private static Dictionary<GameObject, Pool> GOPool = new Dictionary<GameObject, Pool>();

    internal static void Register(Pool pool, GameObject GO)
    {
        GOPool[GO] = pool;
    }

    internal static void Remove(GameObject GO)
    {
        GOPool.Remove(GO);
    }
    
    public static void Despawn(GameObject GO)
    {
        if (GOPool.ContainsKey(GO))
        {
            GOPool[GO].Despawn(GO);
        }
        else
        {
            GameObject.Destroy(GO);
        }
    }
}

public class BasicHP : HP
{
    protected override void Destroy() => PoolUtil.Despawn(gameObject);
}

}