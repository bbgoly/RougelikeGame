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
    public GameObject topCorner, bottomCorner, verticalWall, horizontalWall;


    private int currentDifficulty = 0;
    private float currentTime = 0;
    private List<Vector2> floorGrid = new List<Vector2>();
    private List<Vector2> wallGrid = new List<Vector2>();

    private void Start()
    {
        int width = Random.Range(1, 4), height = Random.Range(1, 4);
        FillArea(width, height);
        GenerateSides(width, height);
    }

    private void Update()
    {

    }

    private void GenerateSides(int width, int height)
    {
        foreach (Vector2 floorPos in floorGrid)
        {
            GameObject wallTile;
            if (floorPos.x == 0)
            {
                wallTile = Instantiate(verticalWall, floorPos, Quaternion.identity);
            }
        }
    }

    private void FillArea(int width, int height)
    {
        for (float x = 0; x <= width; x += 0.15f)
        {
            for (float y = 0; y <= height; y += 0.15f)
            {
                Vector2 tilePostion = new Vector2(x - 1, y);
                GameObject floorTile = Instantiate(floorTiles[Random.Range(0, floorTiles.Count)], tilePostion, Quaternion.identity);
                floorGrid.Add(tilePostion);
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
