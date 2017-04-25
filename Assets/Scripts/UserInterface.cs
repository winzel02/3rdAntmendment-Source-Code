using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserInterface : MonoBehaviour {

	Text countDText,respawnText, levelText;

	Image crumbPrcImg, crumbPrcFillImg;
	RectTransform toPanel;
	int minutes, seconds;
	Stopwatch timer;

	public static UserInterface Instance;
	Text highScore, highWave;
	AudioSource source;
	string[] deadMessages = { "You just died","Sad", "Take a moment and rest", "Rest In Peace", "Give up already", "Unworthy servant!!!" };
	void Awake()
	{
		Instance = this;
		countDText = transform.FindChild ("CountdownText").GetComponent<Text> ();
		respawnText = transform.FindChild ("RespawnText").GetComponent<Text> ();
		toPanel = transform.FindChild("TimeOverPanel").GetComponent<RectTransform>();

		crumbPrcImg = transform.FindChild ("CrumbImg").GetComponent<Image> ();
		crumbPrcFillImg = crumbPrcImg.transform.FindChild ("Fill").GetComponent<Image> ();
		levelText = crumbPrcImg.transform.FindChild ("LevelText").GetComponent<Text> ();

		highScore = toPanel.FindChild("BestScore").FindChild ("ScoreText").GetComponent<Text> ();
		highWave = toPanel.FindChild("BestScore").FindChild ("WaveText").GetComponent<Text> ();
		highScore.text = PlayerPrefs.GetInt ("HighScore", 0).ToString();
		highWave.text = PlayerPrefs.GetInt ("HighWave", 1).ToString();

		source = GetComponent<AudioSource> ();
		respawnText.text = "";
	}

	void Update(){

		minutes = Mathf.FloorToInt (GameManager.Instance.countdown / 60f);
		seconds = Mathf.FloorToInt (GameManager.Instance.countdown - minutes * 60f);
		countDText.text = string.Format ("{0:0}:{1:00}", minutes, seconds);
		levelText.text = "Wave " + GameManager.Instance.currentLevel;
		crumbPrcFillImg.fillAmount = ((float)GameManager.Instance.currentCrumbs / (float)GameManager.Instance.maxCrumbsToObtain);
		if (GameManager.Instance.timeOver)
			TimeOver ();
		
	}

	public IEnumerator DecrementRespawnTimer(){
		timer = new Stopwatch ();
		timer.Start ();
		int rNumber = Random.Range (0, deadMessages.Length);
		while ((float)timer.Elapsed.TotalSeconds < GameManager.Instance.respawnTime) {
			float time = GameManager.Instance.respawnTime - (float)timer.Elapsed.TotalSeconds;
			if (time > (GameManager.Instance.respawnTime - 1))
				for (int i = 0; i < deadMessages.Length; i++) {
					if(i == rNumber)
						respawnText.text = deadMessages[i];
				}
			if(time > 1 && time < (GameManager.Instance.respawnTime - 1))
				respawnText.text = "Respawning in " + (int)time + "...";
			if(time <= 1)
				respawnText.text = "";
			yield return null;
		}
		respawnText.text = "";
		yield break;
	}
	bool oneTimeCaller;
	void TimeOver(){
		if (!oneTimeCaller) {
			source.Play();
			oneTimeCaller = true;
		}
		countDText.gameObject.SetActive (false);
		crumbPrcImg.gameObject.SetActive (false);
		respawnText.gameObject.SetActive (false);
		levelText.gameObject.SetActive (false);
		Text scoreText, waveText;
		scoreText = toPanel.FindChild("CurScore").FindChild ("ScoreText").GetComponent<Text> ();
		waveText = toPanel.FindChild("CurScore").FindChild ("WaveText").GetComponent<Text> ();

		scoreText.text = ""+GameManager.Instance.score;
		waveText.text = ""+GameManager.Instance.currentLevel;
		toPanel.anchoredPosition = new Vector2 (Mathf.Lerp (toPanel.anchoredPosition.x, 0f, 2 * Time.deltaTime), Mathf.Lerp (toPanel.anchoredPosition.y, 0f, 2 * Time.deltaTime));
		CalculateScore ();
	}
	void CalculateScore(){
		
		if (GameManager.Instance.score > PlayerPrefs.GetInt ("HighScore", 0)) {
			PlayerPrefs.SetInt ("HighScore", GameManager.Instance.score);
			PlayerPrefs.SetInt ("HighWave", GameManager.Instance.currentLevel);
			highWave.text = ""+ GameManager.Instance.currentLevel;
			highScore.text = ""+ GameManager.Instance.score;
		}
	}
	public void PlayAgain(){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}
}
