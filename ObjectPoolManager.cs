using System.Collections.Generic;
using UnityEngine;
using SolarFalcon;

public class ObjectPoolManager : MonoBehaviour
{
    public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();
    public int PoolStartCount = 10;
    public List<GameObject> Prefabs = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < Prefabs.Count; i++)
        {
            for (int j = 0; j < PoolStartCount; j++)
            {
                GameObject o = SpawnAObject(Prefabs[i], Vector3.zero, Quaternion.identity);
            }
        }
    }

    public static GameObject SpawnAObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupObject == objectToSpawn);

        // IF Pool doesn't exist, create it
        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookupObject = objectToSpawn};
            ObjectPools.Add(pool);
        }

        // Check if there are any inactive objects in the pool
        GameObject spawnableObj = null;

        foreach (GameObject o in pool.InactiveObjects)
        {
            if (!o.activeInHierarchy)
            {
                spawnableObj = o;
                break;
            }
        }

        // IF there are none, create one
        if (spawnableObj == null)
        {
            spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
            pool.InactiveObjects.Add(spawnableObj);
        }

        // If there is an inactive object, reactivate it
        else
        {
            spawnableObj.transform.position = spawnPosition;
            spawnableObj.transform.rotation = spawnRotation;
            pool.InactiveObjects.Add(spawnableObj);
            spawnableObj?.SetActive(true);
        }

        return spawnableObj;
    }

    public static GameObject SpawnAObjectToParent(GameObject objectToSpawn, Transform spawn) // Parents to transform
    {
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupObject == objectToSpawn);

        // IF Pool doesn't exist, create it
        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookupObject = objectToSpawn};
            ObjectPools.Add(pool);
        }

        // Check if there are any inactive objects in the pool
        GameObject spawnableObj = null;

        foreach (GameObject o in pool.InactiveObjects)
        {
            if (!o.activeInHierarchy)
            {
                spawnableObj = o;
                break;
            }
        }

        // IF there are none, create one
        if (spawnableObj == null)
        {
            spawnableObj = Instantiate(objectToSpawn, spawn.position, spawn.rotation); 
            spawnableObj.transform.SetParent(spawn.transform);
            pool.InactiveObjects.Add(spawnableObj);
        }

        // If there is an inactive object, reactivate it
        else
        {
            spawnableObj.transform.position = spawn.position;
            spawnableObj.transform.rotation = spawn.rotation;
            spawnableObj.transform.SetParent(spawn.transform);
            pool.InactiveObjects.Add(spawnableObj);
            spawnableObj?.SetActive(true);
        }

        return spawnableObj;
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupObject == obj);

        if (pool == null)
        {
            Debug.LogWarning("Trying to release an object that isn't pooled " + obj.name);
        }

        else
        {
            obj.SetActive(false);
            //pool.InactiveObjects.Add(obj);
        }
    }
}

[System.Serializable]
public class PooledObjectInfo
{
    public GameObject LookupObject;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}