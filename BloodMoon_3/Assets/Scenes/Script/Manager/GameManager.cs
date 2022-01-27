using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] goGameUI = null; // 게임이 시작되어야 비로소 활성화 되는 모든 오브젝트들 이 배열에 할당해둔 상태
    [SerializeField] GameObject goTitleUI = null;

    public static GameManager instance;
    public bool isStartGame = false;

    ComboManager theCombo;
    ScoreManager theScore;
    TimingManager theTiming;
    StatusManager theStatus;
    PlayerController thePlayer;
    StageManager theStage;
    NoteManager theNote;
    [SerializeField] CenterFlame theMusic = null;

    private void Start()
    {
        instance = this; // 싱글톤
        theNote = FindObjectOfType<NoteManager>();
        theCombo = FindObjectOfType<ComboManager>();
        theScore = FindObjectOfType<ScoreManager>();
        theTiming = FindObjectOfType<TimingManager>();
        theStatus = FindObjectOfType<StatusManager>();
        thePlayer = FindObjectOfType<PlayerController>();
        theStage = FindObjectOfType<StageManager>();
    }

    public void GameStart(int p_songNum, int p_bpm)
    {
        for (int i = 0; i < goGameUI.Length; i++)
        {
            goGameUI[i].SetActive(true);
        }
        theMusic.bgmName = "BGM" + p_songNum;
        theNote.bpm = p_bpm;
        theStage.RemoveStage();
        theStage.SettingsStage(p_songNum);
        theCombo.ResetCombo();
        theScore.Initialized();
        theTiming.Initialized();
        thePlayer.Initialized();
        theStatus.Initialized();

        AudioManager.instance.StopBGM();

        isStartGame = true;
    }

    public void MainMenu()
    {
        for (int i = 0; i < goGameUI.Length; i++)
        {
            goGameUI[i].SetActive(false);
        }
        goTitleUI.SetActive(true);
    }
}