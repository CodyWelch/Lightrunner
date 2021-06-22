using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DeltaDNA;

// Reference the Unity Analytics namespace
using UnityEngine.Analytics;

public class GameController : MonoBehaviour {

	public Text time_score;
	[SerializeField]
	float seconds;
	[SerializeField]
	float minutes;
	protected vp_FPPlayerEventHandler m_Player = null;
	public static GameController control;
	[SerializeField]
	float health;
	[SerializeField]
	int experience;
	public GameObject Player;
	string pname;
	protected MyBehaviour events;

	vp_SimpleHUD getHealthMultiplier;
	int playerHP;

	void Start() {
		DDNA.Instance.StartSDK(
			"38858906661194912401889469715179",
			"https://collect12996lghtr.deltadna.net/collect/api",
			"https://engage12996lghtr.deltadna.net"
		);

		//UpdateDeltaDNAStartLevel ();
		pname = PlayerPrefs.GetString("name").ToString();
		getHealthMultiplier = Player.GetComponent<vp_SimpleHUD> ();
		m_Player = Player.GetComponent<vp_FPPlayerEventHandler>();
		events = this.GetComponent<MyBehaviour> ();

		if (control == null) {
			DontDestroyOnLoad (gameObject);
			control = this;
		}else if (control != this){
			Destroy (gameObject);
		}

		/*if (SceneManager.GetActiveScene ().buildIndex == 1) {
			Debug.Log ("Level 1, reset params");
			PlayerPrefs.SetInt ("experience", 0);
			PlayerPrefs.SetInt ("health", 0);
		} else {
			Debug.Log ("Loading level: " + SceneManager.GetActiveScene ().buildIndex);
		}*/

	}
	int runonce = 0;

	void Update(){
		if (Player == null) {
			Player = GameObject.FindGameObjectWithTag ("Player");
		}
		if (SceneManager.GetActiveScene ().buildIndex == 1 && runonce ==0) {
			Debug.Log ("Level 1, reset params");
			//PlayerPrefs.SetInt ("experience", 0);
			//PlayerPrefs.SetInt ("health", 0);
			runonce = 1;
		}


		playerHP = (int)(m_Player.Health.Get() * getHealthMultiplier.HealthMultiplier);
		minutes = (int)(Time.timeSinceLevelLoad / 60f);
		seconds = (int)(Time.timeSinceLevelLoad % 60f);
		experience = PlayerPrefs.GetInt ("experience");

		if (Input.GetKeyDown ("p")) {
			events.MissionCompleted (minutes,experience);
			LevelFinished ();
		}
	}

	public void LevelFinished(){
		save ();
		//UpdateAnalytics ();
		UpdateDeltaDNAEndLevel ();
		//Debug.Break ();

		int active_scene = SceneManager.GetActiveScene().buildIndex;
		Debug.Log ("Scene: " + active_scene);
		if (active_scene == 6) {
			SceneManager.LoadScene (1);
		} else {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
		}

	}

	void save(){
		//Debug.Log ("Updating Save Time: " + Time.fixedTime);
		PlayerPrefs.SetFloat("minutes", minutes);
		PlayerPrefs.SetFloat("seconds", seconds);
		PlayerPrefs.SetFloat ("experience", experience);
		PlayerPrefs.SetInt("health", playerHP);
	}
		
	void UpdateDeltaDNAStartLevel (){
		// Build some event parameters
		GameEvent missionStartedEvent = new GameEvent ("missionStarted")
			.AddParam ("missionName", "Level 01")
			.AddParam ("missionID", "001")
			.AddParam ("isTutorial", false);

		// Record the missionStarted event event with some parameters
		DDNA.Instance.RecordEvent(missionStartedEvent);
	}

	void UpdateDeltaDNAEndLevel (){
		// Build some event parameters
		GameEvent missionCompleted= new GameEvent("missionCompleted")
			.AddParam("missionName", "Level 01")
			.AddParam("missionDifficulty", "EASY")
			.AddParam("missionID",(minutes*60)+60)
			.AddParam("userXP",experience)
			.AddParam("missionID","001")
			.AddParam("isTutorial", false);

		// Record the missionStarted event event with some parameters
		DDNA.Instance.RecordEvent(missionCompleted);
	}

	void UpdateAnalytics(){
		Debug.Log ("Updating analytics");
		Analytics.CustomEvent("beatLevel", new Dictionary<string, object>
			{
				{ "seconds", (minutes*60)+seconds},
				{ "health", playerHP},
				{ "name", pname},
				{ "experience", experience}
			});
	}
		
	void OnGUI(){
		GUI.Label (new Rect (10, 10, 100, 30), "Time: " + minutes.ToString("0000") + ":" + seconds.ToString("00"));
		//GUI.Label (new Rect (10, 10, 100, 30), pname);
		//GUI.Label (new Rect (10, 40, 100, 30), "Health: " + playerHP);
		//GUI.Label (new Rect (10, 70, 100, 30), "Experience: " + PlayerPrefs.GetInt("experience"));
	//	GUI.Label (new Rect (10, 100, 100, 30), "Time: " + minutes.ToString("0000") + ":" + seconds.ToString("00"));
	}

	protected virtual void OnEnable(){

		if (m_Player != null)

			m_Player.Register(this);

	}


	protected virtual void OnDisable(){

		if (m_Player != null)

			m_Player.Unregister(this);

	}

	// Event handlers (events added through PlayerEventHandlers
	void OnMessage_EnemyDied(){
		Debug.Log ("Enemy has died Dead");

		experience++;
		PlayerPrefs.SetInt ("experience", experience);
	}


}
