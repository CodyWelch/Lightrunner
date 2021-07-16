using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	// Player & Player Stats
	[SerializeField]
	private GameObject mainPlayer;

	private bool bPlayerDead;
	private float seconds;
	private float minutes;
	private vp_PlayerDamageHandler m_Stats;
	private vp_PlayerEventHandler m_Events;

	[SerializeField]
	private GameObject FinalUI;
	[SerializeField]
	private GameObject PauseUI;
	[SerializeField]
	private GameObject ControlsUI;

	// Grid config
	private int difficulty;
	private int Xsize;
	private int Ysize;
	[SerializeField]
	private int rowSize;
	[SerializeField]
	private int columnSize;
	[SerializeField]
	private Grid grid;
	private bool bGridInitialized;

	private bool gameOver;

	private float timer;
	#region Singleton

	public static GameController instance;
	void Awake()
	{
		if(instance != null)
        {
			Destroy(this);
			return;
        }
		instance = this;
	}

    #endregion

    void Start() 
	{
		timer = 0;
		gameOver = false;
		bPlayerDead = false;
		bGridInitialized = false;
		difficulty = 1;
		columnSize = 10;
		rowSize = 5;

		PlayerPrefs.SetInt ("health", 0);

		m_Stats = mainPlayer.GetComponent<vp_PlayerDamageHandler>();
		m_Events = mainPlayer.GetComponent<vp_PlayerEventHandler>();
	}

	void Update()
	{

		if(!bGridInitialized)
        {
			InitGrid();
		}
		UpdateTime();

		if (Input.GetKeyDown ("z")) 
		{
			LevelFinished ();
		}

		if(bPlayerDead)
        {
			if (Input.GetKeyDown("r"))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			//SceneManager.LoadScene(0);
		}

		if (Input.GetKeyDown("m"))
		{
			grid.ShowPath();
		}

		// SFX
		if (m_Stats.CurrentHealth <= 3)
		{
			AudioManager.instance.Play("Damaged");
		}
	/*	else if (m_Stats.CurrentHealth <= 8)
		{
			AudioManager.instance.Play("Hit");
		}*/
	}
	private void UpdateTime()
    {
		timer += Time.deltaTime;
		minutes = (int)(timer / 60f);
		seconds = (int)(timer % 60f);
	}
	private void InitGrid()
    {
		timer = 0;
		bGridInitialized = true;
		Debug.Log("Building grid, difficulty: " + difficulty);

		Xsize = rowSize * 10;
		Ysize = columnSize * 10;
		grid.CreateGrid(Xsize, Ysize, difficulty);
		Debug.Log("Finished Grid");

		grid.SetPlayerStartPoint(mainPlayer);

	}

	public void LevelFinished()
	{
		Save();
		difficulty++;
		rowSize += 2;
		columnSize += 2;
		bGridInitialized = false;
		grid.Reset();

		if(difficulty>20)
        {
			GameOver();
		}
	}

	public void ReturnToMainMenu()
	{
		SceneManager.LoadScene(0);
	}

	public void ShowPauseMenu()
    {
		if (PauseUI.activeSelf)
		{
			PauseUI.SetActive(false);
			vp_Utility.LockCursor = true;
		}
		else
        {
			vp_Utility.LockCursor = false;
			PauseUI.SetActive(true);
		}
		mainPlayer.GetComponent<vp_FPInput>().MouseCursorForced = !mainPlayer.GetComponent<vp_FPInput>().MouseCursorForced;

	}

	public void ShowControlsUI()
    {
		PauseUI.SetActive(false);
		ControlsUI.SetActive(true);
	}

	public void ReturnToPauseMenuUI()
    {
		PauseUI.SetActive(true);
		ControlsUI.SetActive(false);
	}

	private void GameOver()
    {

    }

	private void Save()
	{
		PlayerPrefs.SetInt("health", (int)m_Stats.CurrentHealth);
		int totalSeconds = (int)minutes * 60 + (int)seconds;
		int saveValue = difficulty - 1;
		int oldScore = PlayerPrefs.GetInt("times" + saveValue);
		int currentWeaponAmmo = m_Events.CurrentWeaponAmmoCount.Get();

		PlayerPrefs.SetInt("currentWeaponAmmo", (int)currentWeaponAmmo);

		if (totalSeconds<oldScore)
        {
			PlayerPrefs.SetInt("times" + saveValue, totalSeconds);
		}
	}
		
	private void OnGUI()
	{
		int level = difficulty;
		GUI.Label(new Rect(10, 10, 100, 30), "Level: " + level);
//		GUI.Label(new Rect(10, 40, 100, 30), "Experience: " + Experience);
		GUI.Label(new Rect(10, 70, 100, 30), "Time: " + minutes.ToString("0000") + ":" + seconds.ToString("00"));
		if(bPlayerDead)
        {
			GUI.Label(new Rect(500, 300, 170, 30), "You ran to level " + level);
			GUI.Label(new Rect(500, 330, 170, 30), "Press R to run again " + difficulty);
			GUI.Label(new Rect(500, 360, 170, 30), "Press Escape to run away " + difficulty);
		}
	}

	public void EnemyDied(GameObject deadEnemy)
	{
		Debug.Log(deadEnemy.name + " has died.");
	}

	public void PlayerDead()
    {
		bPlayerDead = true;
	}

	public void SecretTileFound()
    {

    }
}