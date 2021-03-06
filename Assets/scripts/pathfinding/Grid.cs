using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	[SerializeField]
	private bool showPath = false;

	// Need for checksphere
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	private int nodeRadius = 5;
	Node[,] grid;
	private Node startNode;
	private Node targetNode;

	[SerializeField]
	private GameObject large_skeleton;

	// Objects to instantiate

	[SerializeField]
	public GameObject tile_light;
	private Color redLight = Color.red;
	private Color greenLight = Color.green;
	private Color blueLight = Color.blue;
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
	private GameObject secretTile;

	// Materials
	[SerializeField]
	public Material obstacle_tile_mat;
	public Material finalTile_mat;
	public Material common_tile_mat;
	public Material pickup_tile_mat;

	[SerializeField]
	private Material lavaTileMat;

	private GameObject parent_tiles;
	private GameObject parent_lights;
	private GameObject parent_enemies;
	private GameObject parent_pickups;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	//Randomization
	public int counter;
	public int tile_count;

	// player values
	private int savedHealth;

	// default chances of spawning specific tile
	// 25% obstacles
	private int obstacleMin = 0;
	private int obstacleMax = 70;

	// 2% health
	private int healthMin = 72;
	private int healthMax = 75;

	// 10% ammo
	private int ammoMin = 77;
	private int ammoMax = 80;

	// 63% common
	private int commonMin = 80;
	private int commonMax = 100;

	// Max tile pickups = 5% of total tiles
	private int pickupsMax = 5;
	private int pickupsCurrent = 0;

	[SerializeField]
	private GameObject[] border;

	public List<Node> path;
	public void CreateGrid(int gridWorldSizeX, int gridWorldSizeY, int difficulty) 
	{

		gridWorldSize.x = gridWorldSizeX;
		gridWorldSize.y = gridWorldSizeY;

		parent_tiles = new GameObject();
		parent_tiles.name = "Tiles";

		parent_lights = new GameObject();
		parent_lights.name = "Lights";

		parent_enemies = new GameObject();
		parent_enemies.name = "Enemies";

		parent_pickups = new GameObject();
		parent_pickups.name = "Pickups";


		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);


		pickupsMax += difficulty;

		// Player affected values
		savedHealth = PlayerPrefs.GetInt("health");
		int previousAmmo = PlayerPrefs.GetInt("currentWeaponAmmo");
		Debug.Log("Previous ammo: " + previousAmmo);

		// Player needs health
		if (savedHealth <= 3) 
		{
			healthMax += 5;
			ammoMin += 5;
			ammoMax += 5;
			commonMin += 5;
		}

		// Player needs ammo
		if (previousAmmo <= 10) 
		{
			ammoMax += 5;
			commonMin += 5;
		}


		// Setup complete, build the grid

		BuildGrid();

		// Check that there is a path from the start tile tile to the final tile
		FindPath();

		// If there is no path, make one by finding a path that 
		// ignores obstacle tiles and then exchanges the obstacle tiles on the path for common tiles
		if(path==null)
        {
			Debug.Log("path is null");
			MakePath();
        }
	}

	private void BuildGrid() 
	{
		int finalTileIndex;
		int startTileIndex;
		int secretTileIndex;
		Vector3 worldBottomLeft;

		tile_count = (int)(gridWorldSize.x * gridWorldSize.y) / 100;
		finalTileIndex = (int)Random.Range (tile_count * 0.8f, tile_count);
		startTileIndex = (int)Random.Range (1, tile_count * 0.1f);
		secretTileIndex = (int)Random.Range (startTileIndex+1,finalTileIndex);

		counter = 0;
        grid = new Node[gridSizeX, gridSizeY];
		worldBottomLeft = new Vector3(0.0f,0.0f,0.0f) - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

		SetupBorders();

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {

				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

				if (counter == finalTileIndex) {
					CreateFinalTile(worldPoint, x, y);
				} else if (counter == startTileIndex) {
					CreateStartTile(worldPoint, x, y);
				}else if(counter == secretTileIndex){
					CreateSecretTile(worldPoint, x, y);
				}
				else {
					int random_tile = Random.Range(0, 101);

					// Remainder common tile
					if (obstacleMin <= random_tile && random_tile < obstacleMax) {
						CreateObstacleTile(worldPoint, x, y);
						// Health
					} else if (healthMin <= random_tile && random_tile < healthMax && pickupsCurrent < pickupsMax) {
						CreateHealthTile(worldPoint, x, y);
						/// Ammo
					} else if (ammoMin <= random_tile && random_tile < ammoMax && pickupsCurrent < pickupsMax) {
						CreateAmmoTile(worldPoint, x, y);
						// Enemy
					} else{// if (commonMin <= random_tile && random_tile <= commonMax || pickupsCurrent == pickupsMax) {
						CreateCommonTile(worldPoint, x, y);
					}
				}
				counter++;
			}
		}


		// Assign parents
		
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject enemy in enemies)
		{
			enemy.transform.parent = parent_enemies.transform;
		}
	}


	//limits the player area
	private void SetupBorders()
    {
		// Bottom
		border[0].transform.localScale = new Vector3(border[0].transform.localScale.x, border[0].transform.localScale.y, gridWorldSize.y);
		border[0].transform.position = new Vector3((gridSizeX * 5), border[0].transform.position.y, border[0].transform.position.z);

		// Right
		border[1].transform.localScale = new Vector3(border[1].transform.localScale.x, border[1].transform.localScale.y, gridWorldSize.x);
		border[1].transform.position = new Vector3(border[1].transform.position.x, border[1].transform.position.y, gridSizeY * 5);

		//Left
		border[2].transform.localScale = new Vector3(border[2].transform.localScale.x, border[2].transform.localScale.y, gridWorldSize.x);
		border[2].transform.position = new Vector3(border[2].transform.position.x, border[2].transform.position.y, gridSizeY * -5);

		// Top

		border[3].transform.localScale = new Vector3(border[3].transform.localScale.x, border[3].transform.localScale.y, gridWorldSize.y);
		border[3].transform.position = new Vector3((gridSizeX * -5), border[3].transform.position.y, border[3].transform.position.z);
		
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
		//currentTile.tag = "Final_tile";
		currentTile.transform.parent = parent_tiles.transform;
		grid[x, y] = new Node(walkable, worldPoint, x, y, currentTile);
		targetNode = grid[x, y];

		Debug.Log("created final tile");
		return currentTile;
	}

	private GameObject curentStartTile;

	public void Reset()
    {
		Destroy(parent_tiles);
		Destroy(parent_lights);
		Destroy(parent_enemies);
		Destroy(parent_pickups);

	}

	private void CreateLight(Color lightColor, Vector3 position)
    {
		GameObject newLight = Instantiate(tile_light, new Vector3(position.x, position.y + 7.81f, position.z), tile_light.transform.localRotation);
		
		newLight.GetComponent<Light>().color = lightColor;
		newLight.transform.parent = parent_lights.transform;
	}

	private GameObject CreateStartTile(Vector3 worldPoint, int x, int y)
    {
		bool walkable = true;
		GameObject currentTile = Instantiate(tile, worldPoint, Quaternion.identity);
		currentTile.name = "Start Tile";
		//currentTile.tag = "Start_tile";

		curentStartTile = currentTile;
		CreateLight(greenLight, worldPoint);
		currentTile.transform.parent = parent_tiles.transform;
		grid[x, y] = new Node(walkable, worldPoint, x, y, currentTile);

		startNode = grid[x, y];
		//player.transform.position = new Vector3(worldPoint.x,player.transform.position.y,worldPoint.z);

		return currentTile;
	}

	public void SetPlayerStartPoint(GameObject mainPlayer)
    {
		mainPlayer.transform.position = new Vector3(curentStartTile.transform.position.x, curentStartTile.transform.position.y+1, curentStartTile.transform.position.z);
    }
	private GameObject CreateSecretTile(Vector3 worldPoint, int x, int y)
	{
		bool walkable = true;

		GameObject currentTile = Instantiate(secretTile, worldPoint, Quaternion.identity);
		currentTile.name = "Secret Tile";
		currentTile.GetComponent<Renderer>().material = common_tile_mat;
		GameObject new_enemy;
		int enemy_test = Random.Range(0, 6);
		if (enemy_test == 0)
		{
			new_enemy = Instantiate(enemy_prefab[0], new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
		}
		else if (enemy_test == 1)
		{
			new_enemy = Instantiate(enemy_prefab[1], new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
		}
		else if (enemy_test == 2)
		{
			enemy_test = Random.Range(0, 100);
			if (enemy_test > 50)
			{
				new_enemy = Instantiate(large_skeleton, new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
			}
			else
			{
				new_enemy = Instantiate(enemy_prefab[3], new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
			}
		}
		else if (enemy_test == 3)
		{
			new_enemy = Instantiate(enemy_prefab[2], new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
		}
		else if (enemy_test == 4)
		{
			new_enemy = Instantiate(enemy_prefab[3], new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
		}
		else if (enemy_test == 5)
		{
			new_enemy = Instantiate(enemy_prefab[4], new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
		}




		GameObject newLight = Instantiate(tile_light, new Vector3(worldPoint.x, worldPoint.y + 7.81f, worldPoint.z), tile_light.transform.localRotation);
		newLight.GetComponent<Light>().color = redLight;

		newLight.transform.parent = parent_lights.transform;
		currentTile.transform.parent = parent_tiles.transform;
		grid[x, y] = new Node(walkable, worldPoint, x, y, currentTile);

		return currentTile;
	}
	private GameObject CreateObstacleTile(Vector3 worldPoint, int x, int y)
    {
		bool walkable = false;
		GameObject currentTile = Instantiate(tile, worldPoint, Quaternion.identity);
		currentTile.name = "Obstacle Tile";
		//currentTile.tag = "Obstacle_tile";
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
		//currentTile.tag = "Common_tile";
		currentTile.GetComponent<Renderer>().material = common_tile_mat;
		GameObject new_enemy;
		int enemy_test = Random.Range(0, 6);
		if (enemy_test == 0)
		{
			 new_enemy = Instantiate(enemy_prefab[0], new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
		}
		else if (enemy_test == 1)
		{
			 new_enemy = Instantiate(enemy_prefab[1], new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
		}
		else if (enemy_test == 2)
		{
			enemy_test = Random.Range(0, 100);
			if (enemy_test > 50)
			{
				 new_enemy = Instantiate(large_skeleton, new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
            }
            else
            {
				 new_enemy = Instantiate(enemy_prefab[3], new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
			}
		}
		else if (enemy_test == 3)
		{
			 new_enemy = Instantiate(enemy_prefab[2], new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
		}
		else if (enemy_test == 4)
		{
			 new_enemy = Instantiate(enemy_prefab[3], new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
		}
		else if (enemy_test == 5)
		{
			new_enemy = Instantiate(enemy_prefab[4], new Vector3(worldPoint.x, worldPoint.y + 0.5f, worldPoint.z), Quaternion.identity);
		}




		GameObject newLight = Instantiate(tile_light, new Vector3(worldPoint.x, worldPoint.y + 7.81f, worldPoint.z), tile_light.transform.localRotation);
		newLight.GetComponent<Light>().color = redLight;

		newLight.transform.parent = parent_lights.transform;
		currentTile.transform.parent = parent_tiles.transform;
		grid[x, y] = new Node(walkable, worldPoint, x, y, currentTile);

		return currentTile;

	}

	private GameObject CreateAmmoTile(Vector3 worldPoint, int x, int y)
    {
		bool walkable = true;
		GameObject currentTile = Instantiate(tile, worldPoint, Quaternion.identity);
		currentTile.name = "Ammo Tile";
		currentTile.GetComponent<Renderer>().material = pickup_tile_mat;
		currentTile.transform.parent = parent_tiles.transform;

		GameObject newPickup = Instantiate(pickup, new Vector3(worldPoint.x, worldPoint.y + 2, worldPoint.z), Quaternion.identity);
		newPickup.transform.parent = parent_pickups.transform;
		pickupsCurrent++;

		GameObject newLight = Instantiate(tile_light, new Vector3(worldPoint.x, worldPoint.y + 7.81f, worldPoint.z), tile_light.transform.localRotation);
		newLight.transform.parent = parent_lights.transform;
		newLight.GetComponent<Light>().color = blueLight;

		grid[x, y] = new Node(walkable, worldPoint, x, y, currentTile);

		return currentTile;

	}

	private GameObject CreateHealthTile(Vector3 worldPoint, int x, int y)
    {
		bool walkable = true;

		GameObject currentTile = Instantiate(tile, worldPoint, Quaternion.identity);
		currentTile.name = "Health Tile";
	//	currentTile.tag = "Health_tile";
		currentTile.GetComponent<Renderer>().material = pickup_tile_mat;
		currentTile.transform.parent = parent_tiles.transform;

		GameObject newPickup = Instantiate(health_pickup, new Vector3(worldPoint.x, worldPoint.y + 2, worldPoint.z), Quaternion.identity);
		newPickup.transform.parent = parent_pickups.transform;
		pickupsCurrent++;

		GameObject newLight = Instantiate(tile_light, new Vector3(worldPoint.x, worldPoint.y + 7.81f, worldPoint.z), tile_light.transform.localRotation);
		newLight.GetComponent<Light>().color = blueLight;
		newLight.transform.parent = parent_lights.transform;

		grid[x, y] = new Node(walkable, worldPoint, x, y, currentTile);

		return currentTile;
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition) 
	{
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		return grid[x,y];
	}

	public void FindPath()
	{
		path = null;
		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while (openSet.Count > 0)
		{
			Node node = openSet[0];
			for (int i = 1; i < openSet.Count; i++)
			{
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
				{
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				}
			}

			openSet.Remove(node);
			closedSet.Add(node);

			if (node == targetNode)
			{
				RetracePath(startNode, targetNode,false);
				return;
			}

			foreach (Node neighbour in GetNeighbours(node))
			{
				if (!neighbour.walkable || closedSet.Contains(neighbour))
				{
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
	}

	private void RetracePath(Node startNode, Node targetNode,bool bNewPath)
	{
		List<Node> newPath = new List<Node>();
		Node currentNode = targetNode;

		while (currentNode != startNode)
		{
			if(bNewPath&& currentNode.walkable==false)
            {

				bool updatedNode = false;
				currentNode.walkable = true;

				// Get tile's world position
				Vector3 worldPosition = currentNode.tile.transform.position;


				for (int x = 0; x< gridSizeX;x++)
                {
					for(int y=0;y<gridSizeY;y++)
                    {
						if (currentNode == grid[x, y])
                        {
							Debug.Log("Destroying " + currentNode.tile.name);
							Destroy(currentNode.tile);

							currentNode.tile = CreateCommonTile(worldPosition,x,y);
							updatedNode = true;
						}
					}
                }

				if (updatedNode == false)
                {
					Debug.LogWarning("failed to updated grid");
                }
			}

			
			newPath.Add(currentNode);
			currentNode = currentNode.parent;
		}
		newPath.Reverse();

		path = newPath;
	}

	private void MakePath()
    {

		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while (openSet.Count > 0)
		{
			Node node = openSet[0];
			for (int i = 1; i < openSet.Count; i++)
			{
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
				{
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				}
			}

			openSet.Remove(node);
			closedSet.Add(node);

			if (node == targetNode)
			{
				RetracePath(startNode, targetNode,true);
				return;
			}

			foreach (Node neighbour in GetNeighbours(node))
			{
				if (closedSet.Contains(neighbour))
				{
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
	}
	private int GetDistance(Node nodeA, Node nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14 * dstY + 10 * (dstX - dstY);
		return 14 * dstX + 10 * (dstY - dstX);
	}

	public void ShowPath()
    {
		Debug.Log("showing path");
		foreach (Node currentNode in path)
		{
			currentNode.tile.GetComponent<Renderer>().material = finalTile_mat;

		}
	}
}