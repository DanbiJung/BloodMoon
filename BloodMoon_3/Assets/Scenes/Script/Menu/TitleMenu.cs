using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TitleMenu : MonoBehaviour
{
    [SerializeField] GameObject goStageUI = null;

    public void BtnPlay()  // 버튼 이벤트 등록
    {
        goStageUI.SetActive(true);  // 스테이지 메뉴 활성화
        this.gameObject.SetActive(false); // 타이틀 메뉴 끄고 
    }
}