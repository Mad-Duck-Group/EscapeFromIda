using System;
using System.Collections;
using System.Collections.Generic;
using Searching;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private GameObject youWin;
    [SerializeField] private GameObject youLose;
    [SerializeField] private float spawnEnemyInterval; //Use this to implement spawn enemy interval
    private float i;
    
    
    private bool gameIsOver;
    public bool GameIsOver => gameIsOver;

    private void Start()
    {
        i = spawnEnemyInterval;
        gameIsOver = false;
        youWin.SetActive(false);
        youLose.SetActive(false);
    }

    private void Update()
    {
        CountdownSpawn();
    }

    private void CountdownSpawn()
    {
        int xRandom = Random.Range(0, OOPMapGenerator.Instance.MapData.GetLength(0));
        int yRandom = Random.Range(0, OOPMapGenerator.Instance.MapData.GetLength(1));
        
        if (spawnEnemyInterval > 0)
        {
            i -= Time.deltaTime;

            if (i <= 0)
            {
                OOPMapGenerator.Instance.PlaceEnemy(xRandom, yRandom);
                i = spawnEnemyInterval;
            }

        }        
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
}
