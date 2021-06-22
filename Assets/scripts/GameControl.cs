using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {

	public static GameControl control;

	public float health;
	public float experience;

	public float seconds, minutes;

	void Update(){
		minutes = (int)(Time.timeSinceLevelLoad / 60f);
		seconds = (int)(Time.timeSinceLevelLoad % 60f);
	}


	void Awake() {
		if (control == null) {
			DontDestroyOnLoad (gameObject);
			control = this;
		}else if (control != this){
			Destroy (gameObject);
		}
	}

	void OnGUI(){
		GUI.Label (new Rect (10, 10, 100, 30), "Health: " + health);
		GUI.Label (new Rect (10, 40, 100, 30), "Experience: " + experience);
		GUI.Label (new Rect (10, 70, 100, 30), "Time: " + minutes.ToString("0000") + ":" + seconds.ToString("00"));
	}
}
