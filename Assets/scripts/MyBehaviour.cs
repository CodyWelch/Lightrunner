// SDK v4
using UnityEngine;
using DeltaDNA;

public class MyBehaviour : MonoBehaviour {


	void Start()
	{
		// Enter additional configuration here


		// Launch the SDK
		DDNA.Instance.StartSDK(
			"38858906661194912401889469715179",
			"https://collect12996lghtr.deltadna.net/collect/api",
			"https://engage12996lghtr.deltadna.net"
		);

		GameEvent missionStartedEvent = new GameEvent ("missionStarted")
			.AddParam ("missionName", "Mission 01")
			.AddParam ("missionID", "M002")
			.AddParam ("isTutorial", false);

		DDNA.Instance.RecordEvent (missionStartedEvent);
	}

	public void MissionCompleted(float minutes,int experience){
		GameEvent missionCompleted = new GameEvent ("missionCompleted")
			.AddParam ("missionName", "Level 01")
			.AddParam ("missionDifficulty", "EASY")
			.AddParam ("missionID", (minutes * 60) + 60)
			.AddParam ("userXP", experience)
			.AddParam("missionID","001")
			.AddParam ("isTutorial", false);

		// Record the missionStarted event event with some parameters
		DDNA.Instance.RecordEvent (missionCompleted);
	}
}