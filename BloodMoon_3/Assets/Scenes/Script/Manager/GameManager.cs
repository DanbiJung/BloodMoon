using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] goGameUI = null; // 게임이 시작되어야 비로소 활성화 되는 모든 오브젝트들 이 배열에 할당해둔 상태
    [SerializeField] GameObject goTitleUI = null;

    public static GameManager instance;
    public bool isStartGame = false;

    private void Start()
    {
        instance = this; // 싱글톤
    }

    public void GameStart()
    {
        for (int i = 0; i < goGameUI.Length; i++)
        {
            goGameUI[i].SetActive(true);
        }
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