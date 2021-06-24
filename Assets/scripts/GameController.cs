using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Reference the Unity Analytics namespace
using UnityEngine.Analytics;

public class GameController : MonoBehaviour {

	private float seconds;
	private float minutes;
	protected vp_FPPlayerEventHandler m_Player = null;
	private float health;
	private int experience;
	public GameObject Player;
	string playerName;
	protected MyBehaviour events;

	vp_SimpleHUD getHealthMultiplier;
	private int playerHP;
	private int runonce = 0;

	[SerializeField]
	private int difficulty;

	[SerializeField]
	private int Xsize;
	[SerializeField]
	private int Ysize;

	private Grid grid;

	void Start() {
		Xsize = 50;
		Ysize = 100;
		/*if (SceneManager.GetActiveScene ().buildIndex == 1) {
			Debug.Log ("Level 1, reset params");
			PlayerPrefs.SetInt ("experience", 0);
			PlayerPrefs.SetInt ("health", 0);
		} else {
			Debug.Log ("Loading level: " + SceneManager.GetActiveScene ().buildIndex);
		}*/

	}

	private bool bGridInitialized = false;
	void Update()
	{

		if(bGridInitialized == false)
        {
			GameObject temp = GameObject.FindGameObjectWithTag("GridGO");
			grid = temp.GetComponent<Grid>();
			grid.CreateGrid(Xsize, Ysize);
			playerName = PlayerPrefs.GetString("name").ToString();
			getHealthMultiplier = Player.GetComponent<vp_SimpleHUD>();
			m_Player = Player.GetComponent<vp_FPPlayerEventHandler>();
			bGridInitialized = true;

		}
		//grid.FindPath();

		if (Player == null) 
		{
			Player = GameObject.FindGameObjectWithTag ("Player");
		}
		if (SceneManager.GetActiveScene ().buildIndex == 1 && runonce == 0 ) 
		{
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
			//events.MissionCompleted (minutes,experience);
			LevelFinished ();
		}
	}

	public void LevelFinished()
	{
		Save ();
		//UpdateAnalytics ();
		//Debug.Break ();

		int active_scene = SceneManager.GetActiveScene().buildIndex;
		Debug.Log ("Scene: " + active_scene);
		if (active_scene == 6) 
		{
			SceneManager.LoadScene (1);
		} 
		else
		{
			//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
			difficulty++;
			Xsize += 20;
			Ysize += 20;
			bGridInitialized = false;
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

	}

	private void Save()
	{
		//Debug.Log ("Updating Save Time: " + Time.fixedTime);
		PlayerPrefs.SetFloat("minutes", minutes);
		PlayerPrefs.SetFloat("seconds", seconds);
		PlayerPrefs.SetFloat ("experience", experience);
		PlayerPrefs.SetInt("health", playerHP);
	}
		

	private void UpdateAnalytics()
	{
		Debug.Log ("Updating analytics");
		Analytics.CustomEvent("beatLevel", new Dictionary<string, object>
			{
				{ "seconds", (minutes*60)+seconds},
				{ "health", playerHP},
				{ "name", playerName},
				{ "experience", experience}
			});
	}
		
	void OnGUI()
	{
		GUI.Label (new Rect (10, 10, 100, 30), "Time: " + minutes.ToString("0000") + ":" + seconds.ToString("00"));
		//GUI.Label (new Rect (10, 10, 100, 30), playerName);
		//GUI.Label (new Rect (10, 40, 100, 30), "Health: " + playerHP);
		//GUI.Label (new Rect (10, 70, 100, 30), "Experience: " + PlayerPrefs.GetInt("experience"));
	//	GUI.Label (new Rect (10, 100, 100, 30), "Time: " + minutes.ToString("0000") + ":" + seconds.ToString("00"));
	}

	protected virtual void OnEnable()
	{

		if (m_Player != null)

			m_Player.Register(this);

	}


	protected virtual void OnDisable()
	{

		if (m_Player != null)

			m_Player.Unregister(this);

	}

	// Event handlers (events added through PlayerEventHandlers
	void OnMessage_EnemyDied()
	{
		Debug.Log ("Enemy has died.");

		experience++;
		PlayerPrefs.SetInt ("experience", experience);
	}


}
