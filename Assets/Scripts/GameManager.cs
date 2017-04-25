using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
public class GameManager : MonoBehaviour {

	private static GameManager _instance;
	public static GameManager Instance{
		get { return _instance; }
	}

	[Header("Important Variables")]
	public int currentLevel;
	public int score;
	public float currentCrumbs = 0;
	public float maxCrumbsToObtain = 100;
	public int crumbsAmountToSpawn = 10;
	public float countdown = 100f;
	public float respawnTime = 5f;
	public bool timeOver;
	public bool isDead;
	public bool invulnerable;
	public int timerAmountToSpawn = 10;
	[Space]
	public float gameToStartTimer = 6f;

	[Header("Bonuses")]
	public float timeAddBonus;
	public float moveSpeedBonus;
	[Space]
	public List<GameObject> spawns = new List<GameObject>();
	[SerializeField]
	GameObject waveTextGO;
	[SerializeField]
	GameObject starterTextGO;
	[SerializeField]
	GameObject crumbPrefab;
	[SerializeField]
	GameObject timerPrefab;

	public GameObject humanPrefab,spiderPrefab;
	Text waveText, starterText;
	Stopwatch timer;

	float startTimer;
	bool gameHasStarted;

	int[] waveDifficulty = { 3, 6, 9, 14, 17, 20, 25, 28, 32, 35, 39, 41 };
	int currentDifficulty = 0;

	public GameObject timerAddGO;
	public GameObject ThreeDObject;

	public AudioClip bonusAdd;
	public AudioClip nextWave, cd, go;
	AudioSource source;
	float time = 6;
	void Awake(){
		source = GetComponent<AudioSource> ();
		_instance = this;
		waveText = waveTextGO.GetComponent<Text> ();
		waveText.text = "";
		starterText = starterTextGO.GetComponent<Text> ();
		starterText.text = "";
		startTimer = gameToStartTimer;
		invulnerable = true;

		StartCoroutine( GameStartDecrementer ());
	}
	public void PlayBonusClip(){
		source.PlayOneShot (bonusAdd);
	}
	public void vAnimate(){
		StartCoroutine (Animate ());
	}
	IEnumerator Animate(){
		timerAddGO.SetActive (true);
		yield return new WaitForSeconds (1f);
		timerAddGO.SetActive (false);
	}
	bool caller;
	IEnumerator PlayClip(){
		if (!caller) {
			if (time <= 1) {
				yield return new WaitForSeconds (1);
				source.PlayOneShot (go);
				StartCoroutine (PlayClip ());
			}
			if (time > 1) {
				yield return new WaitForSeconds (1);
				source.PlayOneShot (cd);
				StartCoroutine (PlayClip ());
			}

		}
	}

	void Update()
	{
		if (currentLevel >= waveDifficulty [currentDifficulty] && currentLevel < waveDifficulty [currentDifficulty + 1]) {
			if (currentLevel == waveDifficulty[0])
				GameObject.Instantiate(spiderPrefab, GameObject.Find("Environment").transform);
			if (currentLevel == waveDifficulty [1])
				GameObject.Instantiate (humanPrefab, GameObject.Find ("Environment").transform);
			if (currentLevel == waveDifficulty [2])
				GameObject.Instantiate (spiderPrefab, GameObject.Find ("Environment").transform);
			if (currentLevel == waveDifficulty [3])
				GameObject.Instantiate (humanPrefab, GameObject.Find ("Environment").transform);
			if (currentLevel == waveDifficulty [4])
				GameObject.Instantiate (spiderPrefab, GameObject.Find ("Environment").transform);
			if (currentLevel == waveDifficulty [5])
				GameObject.Instantiate (humanPrefab, GameObject.Find ("Environment").transform);
			if (currentLevel == waveDifficulty [6])
				GameObject.Instantiate (spiderPrefab, GameObject.Find ("Environment").transform);
			if (currentLevel == waveDifficulty [7])
				GameObject.Instantiate (humanPrefab, GameObject.Find ("Environment").transform);
			if (currentLevel == waveDifficulty [8])
				GameObject.Instantiate (spiderPrefab, GameObject.Find ("Environment").transform);
			if (currentLevel == waveDifficulty [9])
				GameObject.Instantiate (humanPrefab, GameObject.Find ("Environment").transform);
			if (currentLevel == waveDifficulty [10])
				GameObject.Instantiate (spiderPrefab, GameObject.Find ("Environment").transform);
			if (currentLevel == waveDifficulty [11])
				GameObject.Instantiate (humanPrefab, GameObject.Find ("Environment").transform);
			currentDifficulty += 1;
		}
		if (!gameHasStarted) {
			StopCoroutine (PlayClip ());
			startTimer -= Time.deltaTime;
		}
		if (startTimer <= 1)
			caller = true;
		if (startTimer <= 0)
			gameHasStarted = true;
		if (gameHasStarted) {
			invulnerable = false;
			countdown -= Time.deltaTime;
		}
		if (countdown <= 0) {
			countdown = 0;
			timeOver = true;
		}
		if (currentCrumbs >= maxCrumbsToObtain)
			StartCoroutine (StartNextLevel ());
			
	}

	IEnumerator StartNextLevel(){
		source.PlayOneShot (nextWave);
		currentLevel += 1;
		currentCrumbs = 0;
		maxCrumbsToObtain += 5;
		invulnerable = true;
		for (int i = 0; i < spawns.Count; i++)
			Destroy (spawns [i].gameObject);
		spawns.Clear ();
		StartCoroutine (WaveDecrementer ());
		yield return new WaitForSeconds (4);

		invulnerable = false;
		SpawnCrumbs ();
	}
	IEnumerator WaveDecrementer(){
		timer = new Stopwatch ();
		timer.Start ();
		while ((float)timer.Elapsed.TotalSeconds < GameManager.Instance.respawnTime) {
			float time = 4f - (float)timer.Elapsed.TotalSeconds;
			if(time >= 1)
				waveText.text = "Next wave will start in " + (int)time + "...";
			else 
				waveText.text = "";
			yield return null;
		}
		waveText.text = "";
		yield break;
	}
	IEnumerator GameStartDecrementer(){
		timer = new Stopwatch ();
		timer.Start ();
		StartCoroutine (PlayClip ());
		while ((float)timer.Elapsed.TotalSeconds < GameManager.Instance.respawnTime) {
			time = (gameToStartTimer -1) - (float)timer.Elapsed.TotalSeconds;
			if(time >= 1)
				starterText.text = "Game will start in " + (int)time + "...";
			else 
				starterText.text = "Ready";
			yield return null;
		}
		StopCoroutine (PlayClip ());
		starterText.text = "Go";

		yield return new WaitForSeconds (1);
		starterText.text = "";
		SpawnTimers ();
		SpawnCrumbs ();
		Destroy (ThreeDObject);
		yield break;
	}
	void SpawnTimers(){
		for (int i = 0; i < timerAmountToSpawn; i++) {
			GameObject.Instantiate (timerPrefab, new Vector3 (Random.Range (-50f, 50f), timerPrefab.transform.position.y, Random.Range (-50, 50)), Quaternion.identity, gameObject.transform.FindChild ("Crumbs"));
		}
	}
	float totalScoreCrumbs = 0;
	List<CrumbScript> crumbScript = new List<CrumbScript>();
	void SpawnCrumbs(){
		totalScoreCrumbs = 0;
		for (int i = 0; i < crumbsAmountToSpawn; i++) {
			GameObject go = GameObject.Instantiate (crumbPrefab, new Vector3 (Random.Range (-50f, 50f), 1.53f, Random.Range (-50, 50)), Quaternion.identity, gameObject.transform.FindChild ("Crumbs"));
			spawns.Add (go);
			crumbScript.Add(go.GetComponent<CrumbScript>());
		}
		StartCoroutine (CalculateCrumbScore ());
	}
	IEnumerator CalculateCrumbScore(){
		yield return new WaitForSeconds (0.2f);
		RecalculateCrumbScores ();
		yield return new WaitForSeconds (0.1f);
		while(totalScoreCrumbs < (maxCrumbsToObtain + 50)){
			GameObject go = GameObject.Instantiate (crumbPrefab, new Vector3 (Random.Range (-50f, 50f), 1.53f, Random.Range (-50, 50)), Quaternion.identity, gameObject.transform.FindChild ("Crumbs"));
			spawns.Add (go);
			crumbScript.Add (go.GetComponent<CrumbScript> ());
			RecalculateCrumbScores ();
			yield return null;
		}
		yield break;
	}
	void RecalculateCrumbScores(){
		totalScoreCrumbs = 0;
		for (int i = 0; i < crumbScript.Count; i++) {
			totalScoreCrumbs += crumbScript [i].scoreToAdd;
		}
	}
	public void AddScore(float amount)
	{
		currentCrumbs += amount;
		score += (int)amount;
	}
	public void AddCountdownTimer(){
		countdown += timeAddBonus;
	}

}
