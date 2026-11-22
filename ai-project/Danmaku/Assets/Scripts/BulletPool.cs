using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    public enum BulletType { Player, Enemy }

    [Header("Prefabs & Pool Sizes")]
    public GameObject playerBulletPrefab;
    public int playerPoolSize = 60;

    public GameObject enemyBulletPrefab;
    public int enemyPoolSize = 400;

    private List<GameObject> playerPool = new List<GameObject>();
    private List<GameObject> enemyPool = new List<GameObject>();

    void Awake()
    {
        Instance = this;
        InitializePool(playerBulletPrefab, playerPoolSize, playerPool);
        InitializePool(enemyBulletPrefab, enemyPoolSize, enemyPool);
    }

    private void InitializePool(GameObject prefab, int count, List<GameObject> pool)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetBullet(BulletType type)
    {
        List<GameObject> targetPool = (type == BulletType.Player) ? playerPool : enemyPool;
        foreach (GameObject obj in targetPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        return null; // Expand pool if needed
    }
}