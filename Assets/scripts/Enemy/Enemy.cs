using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshSourceTag))]
public class Enemy : MonoBehaviour {

	NavMeshAgent pathfinder;
	public Transform target;
	[SerializeField]
	private bool playerDetected;
	private NavMeshAgent nav;
	private float dist=0;

	void Start () {

		playerDetected = false;
		pathfinder = GetComponent<NavMeshAgent> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		StartCoroutine (UpdatePath ());

		//nav = GetComponent<NavMeshAgent> ();
	}

	/*void OnTriggerEnter(Collider other){
		if (other.gameObject.name == "Player") {
			Debug.Log ("Player detected");
			//playerDetected = true;
		}	
	}*/

	IEnumerator UpdatePath(){
		/*NavMeshHit closestHit;
		if( NavMesh.SamplePosition(  this.transform.position, out closestHit, 10.0f, NavMesh.AllAreas ) ){
			//this.transform.position = closestHit.position;
			//go.AddComponent<NavMeshAgent>();
			//TODO
		}
*/
		float refreshRate = 0.25f;

		// need pause (to build navmesh?)
		yield return new WaitForSeconds (1);


		if (target == null) {
			target = GameObject.FindGameObjectWithTag ("Player").transform;
		}
		//pathfinder.enabled = true;
		
		/*NavMeshHit hit;
		if (NavMesh.SamplePosition(target.position, out hit, 1.0f, NavMesh.AllAreas)) {
			transform.position = hit.position;
		}*/

		dist = 1000f;
		while (target != null) {


				dist = Vector3.Distance(transform.position, target.transform.position);
				Debug.Log("dist is " + dist);
				if (dist < 6) {
					//playerDetected = true;
					pathfinder.SetDestination(target.position);
				}

			yield return new WaitForSeconds (refreshRate);
		}
	}
}
