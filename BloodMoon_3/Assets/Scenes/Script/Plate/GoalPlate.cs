using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPlate : MonoBehaviour
{
    AudioSource theAudio;
    NoteManager theNote;
    Result theResult;

    void Start()
    {
        theAudio = GetComponent<AudioSource>();
        theNote = FindObjectOfType<NoteManager>();
        theResult = FindObjectOfType<Result>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            theAudio.Play(); // 효과음 재생
            PlayerController.s_canPressKey = false; // 이동 막기
            theNote.RemoveNote(); // 노트 생성 중지
            theResult.ShowResult();
        }
    }
}