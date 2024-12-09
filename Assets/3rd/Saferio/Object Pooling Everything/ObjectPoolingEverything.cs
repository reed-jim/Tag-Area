using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SaferioPrefabIdentifier
{
    Explosion
}

public class ObjectPoolingEverything : MonoBehaviour
{
    private static ObjectPoolingEverything instance;

    private int _poolSize;

    [SerializeField] private PrefabWithIdentifier[] prefabsWithIdentifier;
    private Dictionary<string, GameObject[]> _poolGroup;

    private Dictionary<string, int> _currentPoolGroupItemIndex;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        _poolSize = 3;

        Spawn();

        DontDestroyOnLoad(gameObject);
    }

    private void Spawn()
    {
        _poolGroup = new Dictionary<string, GameObject[]>();
        _currentPoolGroupItemIndex = new Dictionary<string, int>();

        foreach (var prefabWithIdentifier in prefabsWithIdentifier)
        {
            GameObject[] poolGroupItems = new GameObject[_poolSize];

            for (int j = 0; j < _poolSize; j++)
            {
                GameObject spawnItem = Instantiate(prefabWithIdentifier.Prefab, transform);

                spawnItem.name = $"{prefabWithIdentifier.Prefab.name} {j}";

                spawnItem.SetActive(false);

                poolGroupItems[j] = spawnItem;
            }

            _poolGroup.Add(prefabWithIdentifier.Identifier, poolGroupItems);
            _currentPoolGroupItemIndex.Add(prefabWithIdentifier.Identifier, 0);
        }
    }

    public static GameObject GetFromPool(string identifier)
    {
        foreach (var group in instance._poolGroup)
        {
            if (identifier == group.Key)
            {
                int currentPoolGroupItemIndex = instance._currentPoolGroupItemIndex[identifier];

                instance._currentPoolGroupItemIndex[identifier]++;

                if (instance._currentPoolGroupItemIndex[identifier] >= instance._poolSize)
                {
                    instance._currentPoolGroupItemIndex[identifier] = 0;
                }

                return group.Value[currentPoolGroupItemIndex];
            }
        }

        return null;
    }

    public static GameObject GetFromPool(GameObject referencePrefab)
    {
        // foreach (var group in instance._poolGroup)
        // {
        //     if (identifier == group.Key)
        //     {
        //         return group.Value[0];
        //     }
        // }

        return null;
    }

    public static void SetPoolSize(int size)
    {
        instance._poolSize = size;
    }

    public static void RegisterPoolItem(string groupIdentifer, GameObject prefab)
    {

    }
}

[Serializable]
public struct PrefabWithIdentifier
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private string identifier;

    public GameObject Prefab => prefab;
    public string Identifier => identifier;
}
