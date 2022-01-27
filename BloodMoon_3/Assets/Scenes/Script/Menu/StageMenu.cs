using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Song
{
    public string name;
    public string composer;
    public int bpm;
    public Sprite sprite;
}

public class StageMenu : MonoBehaviour
{
    [SerializeField] Song[] songList = null;

    [SerializeField] Text txtSongName = null;
    [SerializeField] Text txtSongComposer = null;
    [SerializeField] Image imgDisk = null;


    [SerializeField] GameObject TitleMenuUI = null;


    int currentSong = 0;

    public void BtnNext()
    {
        AudioManager.instance.PlaySFX("Touch");

        if (++currentSong > songList.Length - 1)
            currentSong = 0;
    }

    public void BtnPrior()
    {
        AudioManager.instance.PlaySFX("Touch");

        if (--currentSong < 0)
            currentSong = songList.Length - 1;
    }

    void SettingSong()
    {
        txtSongName.text = songList[currentSong].name;
        txtSongComposer.text = songList[currentSong].composer;
        imgDisk.sprite = songList[currentSong].sprite;

        AudioManager.instance.PlayBGM("BGM" + currentSong);
    }


    public void BtnBack()  // 버튼 이벤트 등록
    {
       

        TitleMenuUI.SetActive(true); // 타이틀 메뉴 활성화
        this.gameObject.SetActive(false); // 스테이지 비활
    }

    public void BtnPlay()  // 버튼 이벤트 등록
    {
        int t_bpm = songList[currentSong].bpm;

        GameManager.instance.GameStart(currentSong, t_bpm); 
        this.gameObject.SetActive(false); // 스테이지 비활
    }
}