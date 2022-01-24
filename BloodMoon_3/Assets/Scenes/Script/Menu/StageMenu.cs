using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageMenu : MonoBehaviour
{
    [SerializeField] GameObject TitleMenuUI = null;

    public void BtnBack()  // 버튼 이벤트 등록
    {
        TitleMenuUI.SetActive(true); // 타이틀 메뉴 활성화
        this.gameObject.SetActive(false); // 스테이지 비활
    }

    public void BtnPlay()  // 버튼 이벤트 등록
    {
        GameManager.instance.GameStart(); // 게임 매니저를 통해 게임 스타트 (밑에서 작성)
        this.gameObject.SetActive(false); // 스테이지 비활
    }
}