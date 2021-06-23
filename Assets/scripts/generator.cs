using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Generator : MonoBehaviour {

	public GameObject node;
	private GameObject currentNode;
	public GameObject Player;
	public GameObject enemy_prefab;
	public bool nolight;
	public GameObject tile_light;
	public GameObject finalTile;

	[SerializeField]
	private Material Final_Tile_Mat;

	[SerializeField]
	public GameObject tile_parent;

	[SerializeField]
	public GameObject enemy_parent;

	public Material final_tile_material;

	[SerializeField]
	private Vector2 gridWorldSize;
	
	public int xpos_max = 10;
	public int zpos_max = 20;
	
	private Grid grid;

	public int timer = 0;

	void Start(){
		grid = GetComponent<Grid> ();
		//grid.gridWorldSize.x = 10;
		//grid.gridWorldSize.y = 20;
		//gridSizeX = 10;
		//gridSizeY = 20;
		//gridWorldSize.x = 10;
		//gridWorldSize.y = 20;
		GenerateTileGrid(node);
		Debug.Log ("Last Save time: " + PlayerPrefs.GetFloat("timescore"));
	}
		
	
	void GenerateTileGrid(GameObject Node){

		int counter = 0;

		// set index for final tile and start tile
		int final = Random.Range (0, 101);
		int start = Random.Range (0, 101);

		if (start == final) {
			start += 1;
			if(start>100)
            {
				start = 1;
            }
		}

		for (int xpos = 0; xpos < xpos_max; xpos++) {
			for (int zpos = 0; zpos < zpos_max; zpos++) {

				counter++;
				currentNode = Instantiate(node, new Vector3(xpos*10, 0, zpos*10), Quaternion.identity);
				int test = Random.Range (0, 101);
				if (counter == final)
				{
					Debug.Log("hellllo");
					node = finalTile;
					currentNode.name = "Final_Tile";
					//currentNode.GetComponent<Renderer>().material = Final_Tile_Mat;
					//Instantiate(Final_tile, new Vector3(currentNode.transform.position.x, currentNode.transform.position.y, currentNode.transform.position.z), tile_light.transform.localRotation);
					//Color gold = new Color (255, 215, 0);
				}
				else if (counter == start)
				{
					currentNode.name = "Start_Tile";
					Instantiate(tile_light, new Vector3(currentNode.transform.position.x, currentNode.transform.position.y, currentNode.transform.position.z), tile_light.transform.localRotation);
					//Renderer rend = currentNode.GetComponent<Renderer> ();
					//Color gold = new Color (255, 215, 0);
					Player.transform.position = new Vector3(xpos * 10, 0.5f, zpos * 10);
					//Instantiate(Player, new Vector3(xpos*5, 0, zpos*5), Quaternion.identity);
					//rend.material.color = gold;
				}
				else
				{
					if (test < 10)
					{
						Renderer rend = currentNode.GetComponent<Renderer>();
						rend.material.color = Color.red;
						currentNode.layer = 20;
						//walkable = false;
						currentNode.transform.localScale += new Vector3(0, 10, 0);
					}
					else if (10 <= test && test < 40)
					{
						//walkable = true;
						Renderer rend = currentNode.GetComponent<Renderer>();
						rend.material.color = Color.blue;
						if (nolight == true)
						{
							Instantiate(tile_light, new Vector3(currentNode.transform.position.x, currentNode.transform.position.y + 10, currentNode.transform.position.z), tile_light.transform.localRotation);
						}
						//currentNode.AddComponent<NavMeshSurface> ();
						/*LocalNavMeshBuilder navmeshpart = currentNode.GetComponent<LocalNavMeshBuilder> ();
					navmeshpart.m_Size.x = currentNode.transform.localScale.x; 
					navmeshpart.m_Size.z = currentNode.transform.localScale.z;  						
					*/
						// enemy
					}
					else if (40 <= test && test <= 100 && counter != final && counter != start)
					{
						//walkable = false;
						generate_enemy_tile(currentNode);
					}

				}
				currentNode.transform.parent = tile_parent.transform;
				//grid [xpos, zpos] = new Node (walkable, currentNode.transform.position);
				run_pathfinding ();
			}
		}			
	}

	void run_pathfinding(){
		//this.gameObject.GetComponent<Pathfinding> ().FindPath (start_pos, end_pos);
	}

	void generate_enemy_tile(GameObject enemy){

		// Colour tile
		Renderer rend = enemy.GetComponent<Renderer> ();
		rend.material.color = Color.green;

		// Spawn light
		if (nolight == true) {
			Instantiate (tile_light, new Vector3(currentNode.transform.position.x,currentNode.transform.position.y+7.81f,currentNode.transform.position.z), tile_light.transform.localRotation);
		}

		// 33% chance to spawn enemy
		int test = Random.Range (0, 3);
		if (test == 0) {
			GameObject new_enemy = Instantiate(enemy_prefab, new Vector3(enemy.transform.position.x, enemy.transform.position.y + 1.5f, enemy.transform.position.z), Quaternion.identity);
			new_enemy.transform.parent = enemy_parent.transform;
		}
	}
}