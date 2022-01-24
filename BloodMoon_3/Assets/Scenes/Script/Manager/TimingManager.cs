using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingManager : MonoBehaviour
{
    public List<GameObject> boxNoteList = new List<GameObject>();

    int[] judgementRecord = new int[5];

    [SerializeField] Transform center = null; // 판정 범위의 중심
    [SerializeField] RectTransform[] timingRect = null; // 다양한 판정 범위
    Vector2[] timingBoxs = null; // 판정 범위 최소값 x, 최대값 y

    EffectManager theEffect;
    ComboManager theCombo;
    ScoreManager theScoreManager;
    StageManager theStage;
    PlayerController thePlayer;
    StatusManager theStatus;


    void Start()
    {
        theEffect = FindObjectOfType<EffectManager>();
        theCombo = FindObjectOfType<ComboManager>();
        theScoreManager = FindObjectOfType<ScoreManager>();
        theStage = FindObjectOfType<StageManager>();
        thePlayer = FindObjectOfType<PlayerController>();
        theStatus = FindObjectOfType<StatusManager>();

        timingBoxs = new Vector2[timingRect.Length];

        for (int i = 0; i < timingRect.Length; i++)
        {
            timingBoxs[i].Set(center.localPosition.x - timingRect[i].rect.width / 2,
                              center.localPosition.x + timingRect[i].rect.width / 2);
        }
    }


    public bool CheckTiming()
    {
        for (int i = 0; i < boxNoteList.Count; i++)
        {
            float t_notePosX = boxNoteList[i].transform.localPosition.x;

            // 판정 순서 : Perfect -> Cool -> Good -> Bad
            for (int j = 0; j < timingBoxs.Length; j++)
            {
                if (timingBoxs[j].x <= t_notePosX && t_notePosX <= timingBoxs[j].y)
                {
                    // 노트 제거
                    boxNoteList[i].GetComponent<Note>().HideNote();
                    

                    //  Perfect, Cool, Good 이펙트 연출 + 콤보 증가
                    if (CheckCanNextPlate())
                    {
                        theScoreManager.IncreaseScore(j);  // 점수 증가
                        theStage.ShowNextPlate(); // 다음 플레이트 호출
                        theEffect.JudgementEffect(j); //판정 연출
                        judgementRecord[j]++;  // 판정 기록
                        theStatus.CheckShield(); // 쉴드 체크
                    }
                    else
                    {
                        // Normal 이미지 뜨게끔. 점수 반영도 X
                        theEffect.JudgementEffect(5);
                    }

                    return true;
                }
            }
        }

        // 스페이스바 누르긴 했는데 아예 판정이 안된 Miss
        theCombo.ResetCombo(); // 콤보 초기화
        theEffect.JudgementEffect(timingBoxs.Length);
        MissRecord();
        return false;

        bool CheckCanNextPlate()
        {
            if (Physics.Raycast(thePlayer.destPos, Vector3.down, out RaycastHit t_hitInfo, 1.1f))
            {
                if (t_hitInfo.transform.CompareTag("BasicPlate"))
                {
                    BasicPlate t_plate = t_hitInfo.transform.GetComponent<BasicPlate>();
                    if (t_plate.flag)
                    {
                        t_plate.flag = false;
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public int[] GetJudgementRecord()
    {
        return judgementRecord;
    }

    public void MissRecord()
    {
        judgementRecord[4]++;  // Miss의 판정 기록
        theStatus.ResetShieldCombo();
    }


}


