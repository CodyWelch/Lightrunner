using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	[SerializeField]
	private Text playerName;
	[SerializeField]
	private Text[] playerScores;

	[SerializeField]
	private GameObject MainMenuUI;
	[SerializeField]
	private GameObject HighScoresUI;
	[SerializeField]
	private GameObject ControlsUI;

	void Start () {

		if (PlayerPrefs.GetString("times0") == null)
		{
			Debug.Log("init setup");

			string[] stringArray = new string[20];
			for(int i=0;i<20;i++)
			{
				stringArray[i] = "";
				PlayerPrefs.SetString("times" + i, stringArray[i]);
			}
        }
        else
        {
			int level = 0;
			for (int i = 0; i < 20; i++)
			{
				Debug.Log(PlayerPrefs.GetString("times" + i));
				level++;
				int totalSeconds = PlayerPrefs.GetInt("times" + i);
				int minutes = totalSeconds / 60;
				int seconds = totalSeconds % 60;
				string str = "Level " + level + " : " + minutes.ToString("0000") + ":" + seconds.ToString("00");
				playerScores[i].text = str;
			}
		}
	}
	public void LoadByIndex(int sceneIndex)
	{
//		Debug.Log(playerName.text);
	//	PlayerPrefs.SetString("name", playerName.text);
		SceneManager.LoadScene(sceneIndex);
	}

	public void ShowMainMenu()
    {
		MainMenuUI.SetActive(true);
		HighScoresUI.SetActive(false);
		ControlsUI.SetActive(false);
	}
	public void ShowHighScores()
	{
		MainMenuUI.SetActive(false);
		HighScoresUI.SetActive(true);
	}
	public void ShowControls()
	{
		MainMenuUI.SetActive(false);
		ControlsUI.SetActive(true);
	}
}
