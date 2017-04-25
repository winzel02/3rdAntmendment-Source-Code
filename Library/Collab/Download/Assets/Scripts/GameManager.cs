using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class GameManager : MonoBehaviour {

	private static GameManager _instance;
	public static GameManager Instance{
		get { return _instance; }
	}

	[Header("Important Variables")]
	public int crumbs;

	public float countdown = 100f;
	public float respawnTime = 5f;
	public bool timeOver;
	public bool isDead;

	[Header("Spawn Delays")]
	public float crumbSpawnDelay = 10f;

	[Header("Bonuses")]
	public float timeAddBonus;
	public float moveSpeedBonus;

	[Space]
	[SerializeField] GameObject crumbPrefab;



	void Awake(){
		_instance = this;

	}

	void Update()
	{
		countdown -= Time.deltaTime;
		if (countdown <= 0) {
			countdown = 0;
			timeOver = true;
		}

	}
	public void AddCountdownTimer(){
		countdown += timeAddBonus;
	}

}
