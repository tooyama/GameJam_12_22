using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour 
{
    [SerializeField]
    GameObject[] enemyObjects;
    [SerializeField]
    float spawnTime = 1.0f;

    bool isActive = false;
    bool isUpdate = false;

    public bool IsUpdate
    {
        get
        {
            return isUpdate;
        }
        set
        {
            spawnTime = spawnTime *= 0.8f;
            isUpdate = value;
        }
    }

    public void SetSpawn()
    {
        isActive = true;
        StartCoroutine(WaitForSpawn());
    }

    public void End()
    {
        isActive = false;
    }

    IEnumerator WaitForSpawn()
    {
        int enemyId = Random.Range(0, enemyObjects.Length);
        Vector3 devision = new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));

        GameObject enemy = Instantiate(enemyObjects[enemyId],transform.position+devision,transform.rotation) as GameObject;

        if(isUpdate)
        {
            enemy.GetComponent<EnemyManager>().AddSpeed();
        }

        yield return new WaitForSeconds(spawnTime);

        if(isActive)
        {
            StartCoroutine(WaitForSpawn());   
        }
    }
}
