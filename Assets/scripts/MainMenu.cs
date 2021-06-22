using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	[SerializeField]
	private Text playerName;

	void Start () {

		if (PlayerPrefs.GetString ("name") != null) {
			Debug.Log ("Saved name: " + PlayerPrefs.GetString ("name"));
			playerName.text = PlayerPrefs.GetString ("name").ToString();
		}
	}
	
	public void Play(){

		SceneManager.SetActiveScene (SceneManager.GetSceneByBuildIndex(0));
	}

	public void LoadByIndex(int sceneIndex)
	{
		Debug.Log (playerName.text);
		PlayerPrefs.SetString ("name", playerName.text);
		SceneManager.LoadScene (sceneIndex);
	}

	void Update()
	{
		//Press the space key to change the Text message
		if (Input.GetKey(KeyCode.Space))
		{
			playerName.text = "My text has now changed.";
		}
	}
}
