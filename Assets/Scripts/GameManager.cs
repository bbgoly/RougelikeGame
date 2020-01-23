using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Entity Prefabs")]
    public GameObject playerPrefab;
    public List<GameObject> enemyPrefabs;
    [Header("Dungeon Prefabs")]
    public List<GameObject> floorTiles;
    public GameObject wallPrefab, cornerWallPrefab;
    public TextMeshProUGUI centerText;

    private int totalNumRooms;
    private int currentRoom = 1;
    private Dictionary<Vector2, int[]> roomPositions = new Dictionary<Vector2, int[]>();

    private void Start()
    {
        int width = Random.Range(8, 32), height = Random.Range(8, 32);
        Vector2 startPos = new Vector2(Random.Range(0, 16), Random.Range(0, 16));
        roomPositions.Add(startPos, new int[2] { width, height });
        totalNumRooms = Random.Range(2, 5);
        for (int i = 0; i < totalNumRooms; i++)
        {
            width = Random.Range(8, 32);
            height = Random.Range(8, 32);
            roomPositions.Add(roomPositions.Keys.ElementAt(i) + new Vector2(width * 4, 0), new int[2] { width, height });
        }
        StartCoroutine(GenerateDungeon());
    }

    private void Update()
    {
        Player player = FindObjectOfType<Player>();
        if (player && FindObjectsOfType<Enemy>().Length == 0)
        {
            currentRoom++;
            TimeManager.rewindObjects.Clear();
            int[] roomDimensions = roomPositions.Values.ElementAt(currentRoom - 1);
            player.transform.position = roomPositions.Keys.ElementAt(currentRoom - 1) + new Vector2(roomDimensions[0], roomDimensions[1]) / 2;
            SpawnEnemies();
        }
    }

    private IEnumerator GenerateDungeon()
    {
        int count = 0;
        int numRooms = 1;
        foreach (KeyValuePair<Vector2, int[]> roomPositionPair in roomPositions)
        {
            int width = roomPositionPair.Value[0], height = roomPositionPair.Value[1];
            for (int x = 0; x <= width; x++)
            {
                centerText.text = $"<size=128><color=#8b0000>Loading room {numRooms}/{totalNumRooms + 1}{string.Concat(Enumerable.Repeat('.', count))}</color></size>";
                for (int y = 0; y <= height; y++)
                {
                    Vector2 tilePosition = roomPositionPair.Key + new Vector2(x, y);
                    if (x % width == 0 && y != height)
                    {
                        Instantiate(x == 0 && y == 0 || x == width && y == 0 ? cornerWallPrefab : wallPrefab, tilePosition, Quaternion.Euler(0, 0, x == 0 ? 0 : x == width && y == 0 ? 90 : 180));
                    }
                    else if (y % height == 0)
                    {
                        Instantiate(x == width && y == height || x == 0 && y == height ? cornerWallPrefab : wallPrefab, tilePosition, Quaternion.Euler(0, 0, x != width ? (y == height ? -90 : 90) : 180));
                    }
                    else
                    {
                        Instantiate(floorTiles[Random.Range(0, floorTiles.Count)], tilePosition, Quaternion.identity);
                    }
                    Camera.main.transform.position = new Vector3(tilePosition.x, tilePosition.y, -10);
                    //Camera.main.fieldOfView = 500;
                    yield return new WaitForFixedUpdate();
                }
                count += count == 3 ? -count : 1;
            }
            numRooms++;
        }
        centerText.text = $"<size=128><color=#00ff00>Done loading {numRooms}/{numRooms} rooms!</color></size>";

        int[] startDimensions = roomPositions.Values.ElementAt(0);
        Vector2 startPosition = roomPositions.Keys.ElementAt(0);
        GameObject player = Instantiate(playerPrefab, startPosition + new Vector2(startDimensions[0], startDimensions[1]) / 2, Quaternion.identity);
        CameraController.targetRB2D = player.GetComponent<Rigidbody2D>();
        TimeManager.rewindObjects.Add(player, new Stack<ObjectInfo>());
        Bullet.player = player.GetComponent<Player>();
        centerText.text = "";
        SpawnEnemies();
    }
    private void SpawnEnemies()
    {
        for (int i = 0; i < currentRoom * Random.Range(3, 5); i++)
        {
            int[] roomDimensions = roomPositions.Values.ElementAt(currentRoom - 1);
            GameObject enemy = Instantiate(enemyPrefabs[Mathf.Clamp(Random.Range(0, enemyPrefabs.Count), 0, currentRoom)], roomPositions.Keys.ElementAt(currentRoom - 1) + new Vector2(roomDimensions[0] + Random.Range(-2, 2), roomDimensions[1] + Random.Range(-2, 2)) / 2, Quaternion.identity);
            TimeManager.rewindObjects.Add(enemy, new Stack<ObjectInfo>());
        }
    }
}
