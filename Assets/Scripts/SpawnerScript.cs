using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour {


	public GameObject prefab;
	public float minSpawnDelay, maxSpawnDelay;

	float curDelTime;

	void Start(){
		StartCoroutine (SpawnObject ());
	}
	IEnumerator SpawnObject(){
		yield return new WaitForSeconds (Random.Range (minSpawnDelay, maxSpawnDelay));
		GameObject.Instantiate (prefab, new Vector3 (Random.Range (-50, 50), 1.18f, Random.Range (-50, 50)), Quaternion.identity);
		StartCoroutine (SpawnObject ());
	}

}
