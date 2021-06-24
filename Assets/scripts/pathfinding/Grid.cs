﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	[SerializeField]
	private bool showPath = false;
	// Settings
	public bool nolight = true;

	// Need for checksphere
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	private int nodeRadius = 5;
	Node[,] grid;

	[SerializeField]
	private GameObject large_skeleton;

	// Objects to instantiate

	[SerializeField]
	public GameObject tile_light;
	[SerializeField]
	public GameObject[] enemy_prefab;
	[SerializeField]
	public GameObject pickup;
	[SerializeField]
	public GameObject health_pickup;

	// Tile Prefabs
	[SerializeField]
	public GameObject tile;
	[SerializeField]
	public GameObject finalTile;
	[SerializeField]
	public GameObject obstacleTile;
	[SerializeField]
	public GameObject commonTile;
	[SerializeField]
	public GameObject pickupTile;

	[SerializeField]
	public Material obstacle_tile_mat;
	public Material finalTile_mat;
	public Material common_tile_mat;
	public Material pickup_tile_mat;

	[SerializeField]
	public GameObject parent_tiles;
	public GameObject parent_lights;
	public GameObject parent_enemies;

	Transform tile_seeker;
	Transform tile_target;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	//Randomization
	public int final;
	public int start;
	public int counter;
	public int tile_count;

	int saved_health;

	public int obstacle_min = 0;
	public int obstacle_max = 10;

	public int health_min = 10;
	public int health_max = 12;

	public int ammo_min = 12;
	public int ammo_max = 20;

	public int common_min = 20;
	public int common_max = 100;

	int max_pickups = 5;
	int current_pickups = 0;
	[SerializeField]
	private GameObject player_object;

	[SerializeField]
	private GameObject[] border;

	void Awake() 
	{
		tile_seeker = this.gameObject.GetComponent<Pathfinding> ().seeker;
		tile_target = this.gameObject.GetComponent<Pathfinding> ().target;

		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);

		saved_health = PlayerPrefs.GetInt("health");

		Debug.Log("Health in grid: " + PlayerPrefs.GetInt("health"));
		Debug.Log("Exp in grid: " + PlayerPrefs.GetInt("experience"));

		if (saved_health <= 30) 
		{
			health_max += 5;
			ammo_min += 5;
			ammo_max += 5;
			common_min += 5;
		}
		CreateGrid();
	}


	void CreateGrid() 
	{
		tile_count = (int)(gridWorldSize.x * gridWorldSize.y) / 100;
		int final;
		int start;
		final = (int)Random.Range (tile_count*0.8f, tile_count);
		start = (int)Random.Range (1, 5);
		counter = 0;
		grid = new Node[gridSizeX,gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;


		CreateBorders();


		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {

				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

				GameObject currentTile;


				if (counter == final) {
					currentTile = CreateFinalTile(worldPoint,x,y);
				} else if (counter == start) {
					currentTile = CreateStartTile(worldPoint, x, y);
				}
				else {
					int random_tile = Random.Range (0, 101);
					common_max = tile_count;
					if (obstacle_min <= random_tile && random_tile < obstacle_max) {
						currentTile = CreateObstacleTile(worldPoint, x, y);
						// Health
					} else if (health_min <= random_tile && random_tile< health_max && current_pickups < max_pickups) { 
						currentTile = CreateHealthTile(worldPoint, x, y);
						/// Ammo
					} else if (ammo_min <= random_tile && random_tile< ammo_max || current_pickups < max_pickups) { 
						currentTile = CreateAmmoTile(worldPoint, x, y);
						// Enemy
					} else if (common_min <= random_tile && random_tile <= common_max || current_pickups == max_pickups) {
						currentTile = CreateCommonTile(worldPoint, x, y);
					}
				}
				counter++;
			}
		}
	}


	//limits to player area
	private void CreateBorders()
    {
		// Bottom
		border[0].transform.localScale = new Vector3(border[0].transform.localScale.x, border[0].transform.localScale.y, gridWorldSize.y);
		border[0].transform.position = new Vector3((gridSizeX * 5), border[0].transform.position.y, border[0].transform.position.x);
		// Right
		border[1].transform.localScale = new Vector3(border[1].transform.localScale.x, border[1].transform.localScale.y, border[1].transform.localScale.z * gridSizeY * 5);
		border[1].transform.position = new Vector3(border[1].transform.position.x, border[1].transform.position.y, gridWorldSize.x);

		//Left
		border[2].transform.localScale = new Vector3(border[2].transform.localScale.x, border[2].transform.localScale.y, border[2].transform.localScale.z * gridSizeY * 5);
		border[2].transform.position = new Vector3(border[2].transform.position.x, border[2].transform.position.y, gridWorldSize.x * -1);

		// Top
		border[3].transform.localScale = new Vector3(border[3].transform.localScale.x, border[3].transform.localScale.y, gridWorldSize.y);
		border[3].transform.position = new Vector3((gridSizeX * 5 * -1), border[3].transform.position.y, border[3].transform.position.x);
	}
	public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		// Get adjacent and diagonal neighbours
		/*for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}*/

		// Get adjacent neighbours
		for (int x = 0; x <= 0; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}
		for (int x = -1; x <= 1; x++) {
			for (int y = 0; y <= 0; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}
	

	private GameObject CreateFinalTile(Vector3 worldPoint, int x, int y)
    {
		bool walkable = true;

		GameObject currentTile = Instantiate(finalTile, worldPoint, Quaternion.identity);
		currentTile.name = "Final Tile";
		currentTile.tag = "finalTile";
		tile_target.position = worldPoint;
		this.gameObject.GetComponent<Pathfinding>().target.position = worldPoint;
		currentTile.transform.parent = parent_tiles.transform;
		grid[x, y] = new Node(walkable, worldPoint, x, y, currentTile);

		return currentTile;
	}

	private GameObject CreateStartTile(Vector3 worldPoint, int x, int y)
    {
		bool walkable = true;

		GameObject currentTile = Instantiate(tile, worldPoint, Quaternion.identity);
		currentTile.name = "Start Tile";
		currentTile.tag = "Start_tile";
		tile_seeker.position = worldPoint;
		if (nolight)
		{
			Instantiate(tile_light, new Vector3(worldPoint.x, worldPoint.y + 7.81f, worldPoint.z), tile_light.transform.localRotation);
		}
		currentTile.transform.parent = parent_tiles.transform;
		grid[x, y] = new Node(walkable, worldPoint, x, y, currentTile);

		return currentTile;
	}

	private GameObject CreateObstacleTile(Vector3 worldPoint, int x, int y)
    {
		bool walkable = true;

		GameObject currentTile = Instantiate(tile, worldPoint, Quaternion.identity);
		currentTile.name = "Obstacle Tile";
		currentTile.tag = "Obstacle_tile";
		currentTile.GetComponent<Renderer>().material = obstacle_tile_mat;
		currentTile.transform.localScale += new Vector3(0, 10, 0);
		currentTile.transform.parent = parent_tiles.transform;
		grid[x, y] = new Node(walkable, worldPoint, x, y, currentTile);

		return currentTile;
    }

	private GameObject CreateCommonTile(Vector3 worldPoint, int x, int y)
    {
		bool walkable = true;

		GameObject currentTile = Instantiate(tile, worldPoint, Quaternion.identity);
		currentTile.name = "Common Tile";
		currentTile.tag = "Common_tile";
		currentTile.GetComponent<Renderer>().material = common_tile_mat;

		int enemy_test = Random.Range(0, 3);
		if (enemy_test == 0)
		{
			//Debug.Log (worldPoint.y);
			GameObject new_enemy = Instantiate(enemy_prefab[0], new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
			new_enemy.transform.parent = parent_enemies.transform;
		}
		else if (enemy_test == 1)
		{
			//Debug.Log (worldPoint.y);
			GameObject new_enemy = Instantiate(enemy_prefab[1], new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
			new_enemy.transform.parent = parent_enemies.transform;
		}
		else if (enemy_test == 2)
		{
			enemy_test = Random.Range(0, 100);
			if (enemy_test > 50)
			{
				GameObject new_enemy = Instantiate(large_skeleton, new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
				new_enemy.transform.parent = parent_enemies.transform;
			}

		}
		if (nolight)
		{
			GameObject currentLight = Instantiate(tile_light, new Vector3(worldPoint.x, worldPoint.y + 7.81f, worldPoint.z), tile_light.transform.localRotation);
			currentLight.transform.parent = currentTile.transform;
		}
		currentTile.transform.parent = parent_tiles.transform;
		grid[x, y] = new Node(walkable, worldPoint, x, y, currentTile);

		return currentTile;

	}

	private GameObject CreateAmmoTile(Vector3 worldPoint, int x, int y)
    {
		bool walkable = true;
		GameObject currentTile = Instantiate(tile, worldPoint, Quaternion.identity);
		currentTile.name = "Ammo Tile";
		currentTile.tag = "Ammo_tile";
		currentTile.GetComponent<Renderer>().material = pickup_tile_mat;
		Instantiate(pickup, new Vector3(worldPoint.x, worldPoint.y + 2, worldPoint.z), Quaternion.identity);
		if (nolight)
		{
			Instantiate(tile_light, new Vector3(worldPoint.x, worldPoint.y + 7.81f, worldPoint.z), tile_light.transform.localRotation);
		}
		current_pickups++;
		currentTile.transform.parent = parent_tiles.transform;
		grid[x, y] = new Node(walkable, worldPoint, x, y, currentTile);

		return currentTile;

	}

	private GameObject CreateHealthTile(Vector3 worldPoint, int x, int y)
    {
		bool walkable = true;
		GameObject currentTile = Instantiate(tile, worldPoint, Quaternion.identity);

		currentTile.name = "Health Tile";
		currentTile.tag = "Health_tile";
		currentTile.GetComponent<Renderer>().material = pickup_tile_mat;
		Instantiate(health_pickup, new Vector3(worldPoint.x, worldPoint.y + 2, worldPoint.z), Quaternion.identity);
		if (nolight)
		{
			Instantiate(tile_light, new Vector3(worldPoint.x, worldPoint.y + 7.81f, worldPoint.z), tile_light.transform.localRotation);
		}
		current_pickups++;
		currentTile.transform.parent = parent_tiles.transform;
		grid[x, y] = new Node(walkable, worldPoint, x, y, currentTile);

		return currentTile;
	}


	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		return grid[x,y];
	}

	public bool doOnce = false;
	public List<Node> path;
	public Material onPath;
	public Material offPath;

	void Update(){
		if (doOnce == false) {
			if (grid != null) {
				foreach (Node n in grid) {
					//Gizmos.color = (n.walkable) ? Color.white : Color.red;
					//Debug.Log(n.tile_mat);
					//n.tile.GetComponent<Renderer>().material = (n.walkable) ? n.tile_mat : obstacle_tile_mat;

					/*if (n.tile.tag == "Common_tile") {
						n.tile.GetComponent<Renderer>().material = common_tile_mat;	
					} else if (n.tile.tag == "finalTile") {
						n.tile.GetComponent<Renderer>().material = finalTile_mat;	
					}else if(n.tile.tag == "Pickup_tile"){
						n.tile.GetComponent<Renderer>().material = pickup_tile_mat;	
					}else if(n.tile.tag == "Obstacle_tile"){
						n.tile.GetComponent<Renderer>().material = obstacle_tile_mat;	
					}else if(n.tile.tag == "Start_tile"){
						//n.tile.GetComponent<Renderer>().material = common_tile_mat;	
					}*/

					GameObject currentTile = n.tile;
					if (path != null) {
						Debug.Log ("path exists");
						if (path.Contains (n)) {
							//currentTile.GetComponent<Renderer> ().material = onPath;
							//Debug.Log (n + "is on path");
							if (showPath) {
								currentTile.GetComponent<Renderer> ().material = finalTile_mat;
							}


							/*if (currentTile.GetComponentInChildren<Light> () != null) {
								
								Light test = currentTile.GetComponentInChildren<Light> ();
								test.enabled = false;
							}*/
							doOnce = true;
							if (n.worldPosition == tile_target.position) {
								currentTile.GetComponent<Renderer> ().material = finalTile_mat;

							}
						} else {
							//currentTile.GetComponent<Renderer> ().material = common_tile_mat;
						}
					}
				}
			}
		}
	}
}