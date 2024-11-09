using System;
using System.Collections;
using System.Collections.Generic;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.Analytics;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private GameObject youWin;
    [SerializeField] private GameObject youLose;
    [SerializeField] private float spawnEnemyInterval; //Use this to implement spawn enemy interval
    
    private bool gameIsOver;
    public bool GameIsOver => gameIsOver;

    private void Start()
    {
        gameIsOver = false;
        youWin.SetActive(false);
        youLose.SetActive(false);
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
