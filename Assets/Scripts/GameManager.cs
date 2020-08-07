﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum PlayerType
{
    CAT, HUMAN
}



public class GameManager : MonoBehaviour
{
    PlayerType winner;
    public float gameTime;
    public Text timeText;
    public Image leftBarImage, rightBarImage;
    public Player cat, human;
    public Color humanSkilledBarColor, humanNormalBarColor;

    float timer;
    float toiletValue;
    const float TOILET_PAPER_RATIO = 0.5f;//衛生紙量
    const float MIN_TOILET_VALUE = 0.0f;//衛生紙最小值
    public const float MAX_TOILET_VALUE = 100.0f;//衛生紙最大值
    public const float ATTACK_RATIO = 3.0f;//攻擊比例
    bool isGameOver;
    
    // Start is called before the first frame update
    void Start()
    {
        toiletValue = TOILET_PAPER_RATIO * MAX_TOILET_VALUE;
        isGameOver = false;
        cat.attackEvent += OnPlayerAttack;
        human.attackEvent += OnPlayerAttack;
        cat.skillEvent += OnPlayerSkill;
        human.skillEvent += OnPlayerSkill;
        cat.unskilledEvent += OnPlayerUnSkilled;
        human.unskilledEvent += OnPlayerUnSkilled;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            float fillValue = toiletValue / MAX_TOILET_VALUE;
            cat.SetAddSkillValue(1.0f - fillValue);
            human.SetAddSkillValue(fillValue);
            RefreshTimer();
            UpdateUI();
        }
        else
        {
            human.UpdateToiletPaper(toiletValue);
            cat.UpdateToiletPaper(toiletValue);
            //TODO 顯示贏家
            Debug.Log("winner->" + winner.ToString());
        }
    }

    void RefreshTimer()
    {
        gameTime -= Time.deltaTime; 
        if(gameTime <= 0.0f)
        {
            gameTime = 0.0f;
            SetGameOver();
            winner = toiletValue >= MAX_TOILET_VALUE / 2 ? PlayerType.CAT : PlayerType.HUMAN;
        }
    }

    void SetGameOver()
    {
        isGameOver = true;
        cat.SetGameOver();
        human.SetGameOver();
    }


    void UpdateUI()
    {
        timeText.text = Mathf.Ceil(gameTime).ToString();
        float fillValue = toiletValue / MAX_TOILET_VALUE;
        leftBarImage.fillAmount = DOVirtual.EasedValue(leftBarImage.fillAmount, fillValue, 0.5f, Ease.InOutBack);
        rightBarImage.fillAmount = DOVirtual.EasedValue(rightBarImage.fillAmount, 1.0f - fillValue, 0.5f, Ease.InOutBack);//感覺沒效
        human.UpdateToiletPaper(toiletValue);
        cat.UpdateToiletPaper(toiletValue);

        if (human.GetIsSkilled())
        {
            rightBarImage.color = Color.Lerp(Color.white, humanSkilledBarColor, Mathf.PingPong(Time.time, 1));
        }
        else
        {
            rightBarImage.color = humanNormalBarColor;
        }
    }

    //event
    void OnPlayerAttack(object sender, AttackEventArgs param)
    {
        float minus = param.playerType == PlayerType.CAT ? 1 : -1;
        toiletValue += minus * param.attackValue * ATTACK_RATIO;
        if (toiletValue >= MAX_TOILET_VALUE)
        {
            toiletValue = MAX_TOILET_VALUE;
            winner = PlayerType.CAT;
            SetGameOver();
          
        }
        else if (toiletValue <= MIN_TOILET_VALUE)
        {
            toiletValue = MIN_TOILET_VALUE;
            winner = PlayerType.HUMAN;
            SetGameOver();
        }
    }

    void OnPlayerSkill(object sender, SkillEventArgs param)
    {
        if(param.playerType == PlayerType.CAT)
        {
            human.SetIsSkilled(true);
        }
        else
        {
            cat.SetIsSkilled(true);
        }
    }

    void OnPlayerUnSkilled(object sender, SkillEventArgs param)
    {
        if (param.playerType == PlayerType.CAT)
        {
            human.SetUseSkill(false);
        }
        else
        {
            cat.SetUseSkill(false);
        }
    }
}
