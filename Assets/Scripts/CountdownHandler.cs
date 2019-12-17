using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;
using System.Text;

public class CountdownHandler : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float difficultyMultiplier = 1.1f;
    public int currentDifficulty = 0;

    private float currentTime = 0;
    private float difficultyChangeTime = 180;

    private List<string> difficulties = new List<string>() {
        "Baby steps",
        "You haven't seen anything yet",
        "This will hurt",
        "Insane",
        "Don't get too cocky",
        "Impossible",
        "...",
        "........",
        "........................",
        "YOU'RE ASKING FOR IT.....",
        "I SEE YOU HAVE CHOSEN DEATH",
        "<EnemySpecies> ON STERIODS",
        "NIGHTMARE NIGHTMARE NIGHTMARE NIGHTMARE NIGHTMARE NIGHTMARE",
        "abcdefghijklmnopqrstuvwxyz"
    };

    private void Start()
    {
        textMesh.text = $"Difficulty: Very Easy\n0:00";
    }

    void LateUpdate()
    {
        StringBuilder sb = new StringBuilder();

        currentTime += Time.deltaTime;
        if (currentTime >= difficultyChangeTime && currentDifficulty < difficulties.Count - 1)
        {
            difficultyChangeTime = Mathf.Round(180 + difficultyChangeTime * 1.2f);
            currentDifficulty++;
        }
        else if (currentDifficulty >= difficulties.Count - 1)
        {
            for (int i = 0; i < 20; i++)
            {
                sb.Append((char)UnityEngine.Random.Range('a', 'z' + 1));
            }
        }
        //currentDifficulty = Mathf.FloorToInt(currentTime / difficultyChangeTime % 60) > difficulties.Count - 1 ? difficulties.Count - 1 : Mathf.FloorToInt(currentTime / difficultyChangeTime % 60);
        //Debug.Log($"difficultychangeTime: {difficultyChangeTime}, currentDifficulty: {currentDifficulty}");//180 * Mathf.Pow(1.25f, f/60)); //Mathf.Round(difficultyChangeTime) = startTime * Mathf.Pow(growthTime, Mathf.Round(difficultyChangeTime) / 60)
        textMesh.text = $"Difficulty: {(currentDifficulty == difficulties.Count - 1 ? string.Join("", "abcdefghijklmnopqrstuvwxyz".OrderBy(x => UnityEngine.Random.Range(0, 26))) : difficulties[currentDifficulty])}\n{TimeSpan.FromSeconds(currentTime).ToString(TimeSpan.FromSeconds(currentTime).TotalHours < 1 ? @"m\:ss" : @"h\:mm\:ss")}";
    }
}