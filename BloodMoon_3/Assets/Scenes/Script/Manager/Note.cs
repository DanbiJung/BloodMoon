using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    public float noteSpeed = 400;
    private Image noteImage;

    void OnEnable()
    {
        if (noteImage == null)
            noteImage = GetComponent<Image>();

        noteImage.enabled = true;
    }

    public void HideNote()
    {
        noteImage.enabled = false;
    }
    
    

    void Update()
    {
        transform.localPosition += Vector3.right * noteSpeed * Time.deltaTime;
    }

    public bool GetNoteFlag()
    {
        return noteImage.enabled;
    }
   
}