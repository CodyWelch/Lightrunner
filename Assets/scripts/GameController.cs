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
	public GameObject playerPrefab;

	[SerializeField]
	private GameObject mainPlayer;
	string playerName;
	protected MyBehaviour events;

	vp_SimpleHUD getHealthMultiplier;
	private int playerHP;
	private int runonce = 0;

	[SerializeField]
	private int difficulty;

	private int Xsize;
	private int Ysize;

	[SerializeField]
	private int rowSize;
	[SerializeField]
	private int columnSize;

	[SerializeField]
	private Grid grid;

	private bool bGridInitialized = false;

	private bool bSwap = true;
	public static GameController control;


	void Awake()
	{
		if (control == null)
		{
			DontDestroyOnLoad(gameObject);
			control = this;
		}
		else if (control != this)
		{
			Destroy(gameObject);
		}
	}
	void Start() {

		columnSize = 10;
		rowSize = 5;

		/*if (SceneManager.GetActiveScene ().buildIndex == 1) {
			Debug.Log ("Level 1, reset params");
			PlayerPrefs.SetInt ("experience", 0);
			PlayerPrefs.SetInt ("health", 0);
		} else {
			Debug.Log ("Loading level: " + SceneManager.GetActiveScene ().buildIndex);
		}*/

	}

	void Update()
	{

		if(bGridInitialized == false)
        {
			bGridInitialized = true;
			Debug.Log("Building a grid, difficulty: " + difficulty);





			// Reset UFPS player
			GameObject[] deletePlayer = GameObject.FindGameObjectsWithTag("Player");
			for(int i=0;i<deletePlayer.Length;i++)
            {
				//Destroy(deletePlayer[i]);
            }

			if (deletePlayer.Length > 1)
			{
             /*   if (bSwap)
                {
					GameObject.Destroy(deletePlayer[1]);
				}
				else
                {
					GameObject.Destroy(deletePlayer[0]);
				}
				bSwap = !bSwap;*/
			}

			//mainPlayer = Instantiate(playerPrefab);
			//mainPlayer.name = "NEW PLAYER";

			//mainPlayer = GameObject.FindGameObjectWithTag("Player");
			//GameObject temp = GameObject.FindGameObjectWithTag("GridGO");

			//grid = temp.GetComponent<Grid>();
			Xsize = rowSize * 10;
			Ysize = columnSize * 10;
			grid.CreateGrid(Xsize, Ysize);
			Debug.Log("Finished Grid");

			playerName = PlayerPrefs.GetString("name").ToString();
			getHealthMultiplier = mainPlayer.GetComponent<vp_SimpleHUD>();
			m_Player = mainPlayer.GetComponent<vp_FPPlayerEventHandler>();

			grid.SetPlayerStartPoint(mainPlayer);

		}
		Debug.Log("helllllooooo");
		//grid.FindPath();

		if (mainPlayer == null) 
		{

		}
		/*
		if (SceneManager.GetActiveScene ().buildIndex == 1 && runonce == 0 ) 
		{
			Debug.Log ("Level 1, reset params");
			//PlayerPrefs.SetInt ("experience", 0);
			//PlayerPrefs.SetInt ("health", 0);
			runonce = 1;
		}*/


		playerHP = (int)(m_Player.Health.Get() * getHealthMultiplier.HealthMultiplier);
		minutes = (int)(Time.timeSinceLevelLoad / 60f);
		seconds = (int)(Time.timeSinceLevelLoad % 60f);
		experience = PlayerPrefs.GetInt ("experience");

		if (Input.GetKeyDown ("z")) {
			//events.MissionCompleted (minutes,experience);
			//Debug.Log("hello");
			LevelFinished ();
		}
	}

	public void LevelFinished()
	{
		//Save ();
		//UpdateAnalytics ();
		//Debug.Break ();

		//int active_scene = SceneManager.GetActiveScene().buildIndex;
		/*Debug.Log ("Scene: " + active_scene);
		if (active_scene == 6) 
		{
			SceneManager.LoadScene (1);
		} 
		else
		{*/
			//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
			difficulty++;
			rowSize += 2;
			columnSize += 2;
			bGridInitialized = false;
		//		vp_Timer.CancelAll();
		/*vp_Timer.DestroyAll();
		GameObject[] deletePlayer = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < deletePlayer.Length; i++)
		{
			Destroy(deletePlayer[i]);
		}*/
		grid.Reset();

		
		//GameObject.Destroy(GameObject.FindGameObjectWithTag("Destroy"));
		/*if (bSwap)
        {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
		}else
        {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
		}
		bSwap = !bSwap;
		*/
		//string thisScene = SceneManager.GetActiveScene().name;
		//SceneManager.UnloadSceneAsync(thisScene);
//		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		//SceneManager.LoadScene(thisScene);
		//}

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
		//GUI.Label (new Rect (10, 10, 100, 30), "Time: " + minutes.ToString("0000") + ":" + seconds.ToString("00"));
		//GUI.Label (new Rect (10, 10, 100, 30), playerName);
		//GUI.Label (new Rect (10, 40, 100, 30), "Health: " + playerHP);
		//GUI.Label (new Rect (10, 70, 100, 30), "Experience: " + PlayerPrefs.GetInt("experience"));
	//	GUI.Label (new Rect (10, 100, 100, 30), "Time: " + minutes.ToString("0000") + ":" + seconds.ToString("00"));
		GUI.Label(new Rect(10, 10, 100, 30), "Health: " + health);
		GUI.Label(new Rect(10, 40, 100, 30), "Experience: " + experience);
		GUI.Label(new Rect(10, 70, 100, 30), "Time: " + minutes.ToString("0000") + ":" + seconds.ToString("00"));
	}

	// Event handlers (events added through PlayerEventHandlers
	void OnMessage_EnemyDied()
	{
		Debug.Log ("Enemy has died.");

		experience++;
		PlayerPrefs.SetInt ("experience", experience);
	}

}