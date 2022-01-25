using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Text txtScore = null;

    [SerializeField] int increaseScore = 10;
    int currentSrore = 0;

    [SerializeField] float[] weight = null;
    [SerializeField] int comboBonusScore = 10;

    Animator myAnim;
    string animScoreUp = "ScoreUp";

    ComboManager theCombo;

    private void Start()
    {
        theCombo = FindObjectOfType<ComboManager>();
        myAnim = GetComponent<Animator>();
        currentSrore = 0;
        txtScore.text = "0";
    }

    public void Initialized()
    {
        currentSrore = 0;
        txtScore.text = "0";
    }

    public void IncreaseScore(int p_JudgementState)
    {
        // 콤보 증가
        theCombo.IncreaseCombo();

        // 콤보 보너스 점수 계산
        int t_currentCombo = theCombo.GetCurrentCombo();
        int t_bonusComboScore = (t_currentCombo / 10) * comboBonusScore; // ex) 10~19 콤보는 추가점수 10, 30~39 콤보는 추가점수 30.

        // 판정 가중치 계산
        int t_increaseScore = increaseScore + t_bonusComboScore;        // 추가할 점수 (기본 증가점수 + 콤보 추가점수)
        t_increaseScore = (int)(t_increaseScore * weight[p_JudgementState]); // 판정에 따른 가중치 적용. 최종적으로 추가할 점수 완성

        // 점수 반영
        currentSrore += t_increaseScore;
        txtScore.text = string.Format("{0:#,##0}", currentSrore);

        // 애니메이션 실행
        myAnim.SetTrigger(animScoreUp);
    }

    public int GetCurrentScore()
    {
        return currentSrore;
    }
}