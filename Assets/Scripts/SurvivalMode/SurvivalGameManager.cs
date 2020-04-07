﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class SurvivalGameManager : MonoBehaviour
{
    public GameObject room;
    public float gameRadius,gameTime,scaleDownStartTime,roomScaleDownChance=0.2f,maxRoomScale,minRoomScale;
    private int score,coinGained;
    SurvivalGameUI gameUI;
    public bool gameStopped,isGameStarted = false,gameEnded,roomClosing,willRoomScale = false,waweEnded;
    public int waveIndex = 0;
    [SerializeField] GameObject walls,enemys,checkpoints;

    void Start()
    {
        gameUI = FindObjectOfType<SurvivalGameUI>();
        gameTime = 0;
        SetRoom();
    }
    
    void Update()
    {
        if(!gameStopped && isGameStarted && !gameEnded)
        {
            gameTime += Time.deltaTime;
            if(roomClosing == false && gameTime > scaleDownStartTime && willRoomScale == true)
            {
                roomClosing = true;
                FindObjectOfType<WallScaler>().CallScaler();
            }
        }
    }
    public void SetRoom()
    {
        gameRadius = Random.Range(minRoomScale,maxRoomScale);
        room.transform.localScale = new Vector3(gameRadius/3,gameRadius/3,1);
        willRoomScale = ChooseWillRoomScale();
        roomClosing = false;
        gameStopped = false;
        isGameStarted = false;
        enemys.SetActive(true);
        walls.SetActive(true);
        FindObjectOfType<CreateRandomWalls>().CreateWalls();
        FindObjectOfType<DeadlyFieldController>().ResetField();
    }
    public void GetScore()
    {
        score += 100;
        gameUI.UpdateScoreText(score);
    }
    public void GetEnemyScore()
    {
        score += 60;
        gameUI.UpdateScoreText(score);
    }
    public void LoseScore()
    {
        score -= 100;
        gameUI.UpdateScoreText(score);
    }
    //It works when you touh or click for game to start
    public void StartGame()
    {
        waveIndex++;
        waweEnded = false;
        gameEnded = false;
        isGameStarted = true;
        gameStopped = false;
        Player_Shoot ps = FindObjectOfType<Player_Shoot>();
        ps.enabled = true;
        ps.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None; 
        //Oluştur
        FindObjectOfType<CheckPointManager>().CreateCheckPoints();
        FindObjectOfType<SurvivalEnemyManager>().ProduceEnemys();
    }
    public void StopGame()
    {
        gameStopped = true;
        //düşmanları sakla
        enemys.SetActive(false);
        walls.SetActive(false);
        Player_Shoot ps = FindObjectOfType<Player_Shoot>();
        ps.enabled = false;
        ps.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }
    public void ResumeGame()
    {
        gameStopped = false;
        //düşmanları açığa çıkar
        enemys.SetActive(true);
        walls.SetActive(true);
        Player_Shoot ps = FindObjectOfType<Player_Shoot>();
        ps.enabled = true;
        ps.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None; 
    }
    public void CleanGame()
    {
        waweEnded = true;
        gameTime = 0;
        gameStopped = true;
        FindObjectOfType<Player_Shoot>().transform.position = Vector2.zero;
        FindObjectOfType<Player_Shoot>().enabled = false;
        foreach (Transform child in enemys.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in checkpoints.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in walls.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void CalculateScore()
    {
        //Son skoru hesapla
        CalculateTimeScore();
        SaveScore();
        gameUI.UpdateScoreText(score);
    }
    void CalculateCoin(int coin)
    {
        coinGained += coin;
        SaveCoin();
    }
    void CalculateCoin()
    {
        coinGained += score / 10;
        SaveCoin();
    }
    void SaveCoin()
    {
        SaveAndLoadGameData.instance.savedData.coin +=  coinGained;
        SaveAndLoadGameData.instance.Save();
    }
    public int GetCoinGained() { return coinGained; }
    void SaveScore()
    {
        SaveAndLoadGameData.instance.savedData.totalScore += this.score;
        if(this.score > SaveAndLoadGameData.instance.savedData.score)
        {
            Debug.Log("New High score");
            SaveAndLoadGameData.instance.savedData.score = this.score;
        }
        SaveAndLoadGameData.instance.Save();
    }
    private void CalculateTimeScore()
    {
        if( gameTime <= 25 )
        {
            score += 500;
        }
        else if( gameTime > 25 && gameTime <= 50 )
        {
            score += 300;
        }
        else if( gameTime > 50 && gameTime <=75 )
        {
            score += 100;
        }
    }
    public void EndGame()
    {
        gameEnded = true;
        CleanGame();
        //Score u kaydet ve her şeyi sıfırla
        CalculateScore();
        CalculateCoin();
        gameUI.EndGameUI();
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void ReturnHome()
    {
        SceneManager.LoadScene(0);
    }
    bool ChooseWillRoomScale()
    {
        float val = Random.Range(0f,1f);
        if(val <= roomScaleDownChance)
        {
            return true;
        }
        else 
        {
            return false; 
        }
    }
    
}
