using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Difficulty Properties")]
    public float difficultyMultiplier = 1.1f;
    [Header("Entity Prefabs")]
    public GameObject playerPrefab;
    public GameObject bigDemonPrefab, smallDemonPrefab, bigZombiePrefab, smallZombiePrefab;
    [Header("Dungeon Prefabs")]
    public List<GameObject> floorTiles;
    public GameObject wallPrefab;


    private int currentDifficulty = 0;
    private float currentTime = 0;
    private List<Vector2> floorGrid = new List<Vector2>();
    private List<Vector2> wallGrid = new List<Vector2>();

    private void Start()
    {
        GenerateDungeon(Random.Range(2, 4), Random.Range(2, 4));
    }

    private void Update()
    {
    }

    private void GenerateDungeon(int width, int height)
    {
        for (float x = 0; x <= width; x += 0.15f)
        {
            for (float y = 0; y <= height; y += 0.15f)
            {
                Vector2 tilePosition = new Vector2(x - 1, y);
				if (x == 0)
				{
					GameObject wallTile = Instantiate(wallPrefab, tilePosition, Quaternion.Euler(0, 0, x >= width ? 90 : 0));
					wallGrid.Add(tilePosition);
				}
                else
                {
                    GameObject floorTile = Instantiate(floorTiles[Random.Range(0, floorTiles.Count)], tilePosition, Quaternion.identity);
                    floorGrid.Add(tilePosition);
                }
            }
        }
    }

    private void ClearDungeon()
    {
        floorGrid.Clear();
    }

    private Enemy[] EnemiesWithinScene()
    {
        return FindObjectsOfType<Enemy>();
    }
}
