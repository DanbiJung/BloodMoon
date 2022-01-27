using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] GameObject[] stageArray = null;
    Transform[] stagePlates;
    GameObject currentStage;

    [SerializeField] float offsetY = -5;    // 플레이트 활성화시 밑에서 올라오게 할거라..
    [SerializeField] float plateSpeed = 10; // 플레이트 활성화시 올라오는 속도

    int stepCount = 0;
    int totalPlateCount = 0; // stepCount가 stagePlates.Length를 넘으면 안된다.(배열 인덱스 범위 초과 에러 막기 위해)




    public void RemoveStage()
    {
        if (currentStage != null)
            Destroy(currentStage);
    }

    public void SettingsStage(int p_songNum)
    {
        stepCount = 0;

        currentStage = Instantiate(stageArray[p_songNum], Vector3.zero, Quaternion.identity);
        stagePlates = currentStage.GetComponent<Stage>().plates;
        totalPlateCount = stagePlates.Length;

        // 플레이트들 밑에서 올라오면서 활성화 되도록 offset 만큼 밑에 위치하도록 세팅 (현재 비활성화 상태)
        for (int i = 0; i < totalPlateCount; i++)
        {
            stagePlates[i].position = new Vector3(stagePlates[i].position.x,
                                                  stagePlates[i].position.y + offsetY,
                                                  stagePlates[i].position.z);
        }
    }

    public void ShowNextPlate()
    {
        if (stepCount < totalPlateCount)
            StartCoroutine(MovePlateCo(stepCount++));
    }

    // 플레이트를 활성화 하고 코루틴으로 천천히 부드럽게 위로 올라 오게 함
    IEnumerator MovePlateCo(int p_num)
    {
        stagePlates[p_num].gameObject.SetActive(true);
        Vector3 t_destPos = new Vector3(stagePlates[p_num].position.x,
                                        stagePlates[p_num].position.y - offsetY,
                                        stagePlates[p_num].position.z);
        while (Vector3.SqrMagnitude(stagePlates[p_num].position - t_destPos) >= 0.001f)
        {
            stagePlates[p_num].position = Vector3.Lerp(stagePlates[p_num].position, t_destPos, plateSpeed * Time.deltaTime);
            yield return null;
        }
        stagePlates[p_num].position = t_destPos;
    }
}