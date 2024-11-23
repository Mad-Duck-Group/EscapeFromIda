using System;
using System.Collections;
using System.Collections.Generic;
using Searching;
using TMPro;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private GameObject youWin;
    [SerializeField] private GameObject youLose;
    [SerializeField] private float spawnEnemyInterval;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text healthText;
    //Use this to implement spawn enemy interval
    private float enemyTimer;
    private bool gameIsOver;
    public bool GameIsOver => gameIsOver;

    private void Start()
    {
        enemyTimer = spawnEnemyInterval;
        gameIsOver = false;
        youWin.SetActive(false);
        youLose.SetActive(false);
    }

    private void Update()
    {
        CountdownSpawn();
        UpdateTimer();
    }
    
    public void UpdateHealth(int health)
    {
        healthText.SetText("HP: " + health);
    }
    
    private void UpdateTimer()
    {
        if (gameIsOver) return;
        float myTime = Time.realtimeSinceStartup;
        timerText.SetText(myTime.ToString("Time : "+"00:00"));
    }

    private void CountdownSpawn()
    {
        if (gameIsOver) return;
        if (spawnEnemyInterval <= 0) return;
        enemyTimer -= Time.deltaTime;
        if (enemyTimer > 0) return;
        var randomEmptyTile = OOPMapGenerator.Instance.GetRandomEmpty();
        if (randomEmptyTile.x == -1 || randomEmptyTile.y == -1) return;
        OOPMapGenerator.Instance.PlaceEnemy(randomEmptyTile.x, randomEmptyTile.y);
        enemyTimer = spawnEnemyInterval;
    }

    public void Lose()
    {
        gameIsOver = true;
        youLose.SetActive(true);
    }

    public void Win()
    {
        gameIsOver = true;
        youWin.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
