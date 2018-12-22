using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour 
{
    [SerializeField]
    GameObject[] enemyObjects;
    [SerializeField]
    float spawnTime = 1.0f;


	void Start () 
    {
        StartCoroutine(WaitForSpawn());
	}

    IEnumerator WaitForSpawn()
    {
        yield return new WaitForSeconds(spawnTime);

        int enemyId = Random.Range(0, enemyObjects.Length);
        Vector3 devision = new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));

        Instantiate(enemyObjects[enemyId],transform.position+devision,transform.rotation);

        StartCoroutine(WaitForSpawn());
    }
	
}
