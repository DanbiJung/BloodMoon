using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{
    [SerializeField] float blinkSpeed = 0.1f; // 깜빡이는 속도
    [SerializeField] int blinkCount = 10;  // 깜빡거릴 횟수 (키고 끄고 합해서 2회)
    int currentBlinkCount = 0; // 이게 blinkCount 에 다다르면 그만 깜빡거릴 것.
    bool isBlink = false;  // 지금 깜빡이는 중인지. 깜빡이고 있다면 무적상태여야.

    bool isDead = false;  // 죽은 상태인지

    int maxHp = 3;
    int currentHp = 3;

    int maxShield = 3;
    int currentShield = 0;  // 쉴드는 hp 와 다르게 0 에서 시작함. 게이지 다 찰때마다 쉴드 하나씩 증가

    [SerializeField] Image[] hpImage = null;
    [SerializeField] Image[] shieldImage = null;

    [SerializeField] int shieldIncreaseCombo = 5; // 5번 연속 콤보때마다 쉴드 게이지 꽉차고 쉴드 하나씩 증가하게 할 것.
    int currentShieldCombo = 0;  // 이게 5에 다다르면 쉴드 하나 증가.
    [SerializeField] Image shieldGauge = null;

    Result theResult;
    NoteManager theNote;
    [SerializeField] MeshRenderer playerMesh = null; // 깜빡거리는 효과를 주기 위해 플레이어 큐브의 MeshRenderer를 비활/활성화 반복할 것.

    private void Start()
    {
        theResult = FindObjectOfType<Result>();
        theNote = FindObjectOfType<NoteManager>();
    }

    // 쉴드 게이지 업뎃
    // 5콤보 때마다 쉴드 1개씩 증가
    public void CheckShield()
    {
        currentShieldCombo++;

        if (currentShieldCombo >= shieldIncreaseCombo)
        {
            currentShieldCombo = 0;
            IncreaseShield();
        }

        shieldGauge.fillAmount = (float)currentShieldCombo / shieldIncreaseCombo;
    }

    // 콤보 놓쳣을 때 다시 리셋해야 함. 게이지도 다시 빵으로.
    public void ResetShieldCombo()
    {
        currentShieldCombo = 0;
        shieldGauge.fillAmount = (float)currentShieldCombo / shieldIncreaseCombo;
    }

    // 쉴드 1개 증가. 대신 3개는 넘으면 안됨
    public void IncreaseShield()
    {
        currentShield++;

        if (currentShield >= maxShield)
            currentShield = maxShield;

        SettingShieldImage();
    }

    // 낭떠러지 추락할 때 쉴드 하나씩 깎아야 함
    public void DecreaseShield(int p_num)
    {
        currentShield -= p_num;

        if (currentShield <= 0)
            currentShield = 0;

        SettingShieldImage();
    }

    // 쉴드 이미지 활성화/비활성화 (currentShield 개수만 활성화)
    void SettingShieldImage()
    {
        for (int i = 0; i < shieldImage.Length; i++)
        {
            if (i < currentShield)
                shieldImage[i].gameObject.SetActive(true);
            else
                shieldImage[i].gameObject.SetActive(false);
        }
    }

    // HP 1 증가
    public void IncreaseHp(int p_num)
    {
        currentHp += p_num;
        if (currentHp >= maxHp)
            currentHp = maxHp;

        SettingHPImage();
    }

    // HP 1 깎기 (단, 깜빡일 때 말고! 깜빡일 떈 무적 효과여야. 그리고 쉴드 있을 땐 HP 대신 쉴드를 깎아야 함.)
    public void DecreaseHp(int p_num)
    {
        if (!isBlink)
        {
            if (currentShield > 0)
                DecreaseShield(p_num);
            else
            {
                currentHp -= p_num;

                if (currentHp <= 0)
                {
                    isDead = true;
                    theResult.ShowResult();
                    theNote.RemoveNote();
                }
                else
                {
                    StartCoroutine(BlinkCo());
                }
                SettingHPImage();
            }
        }
    }

    void SettingHPImage()
    {
        for (int i = 0; i < hpImage.Length; i++)
        {
            if (i < currentHp)
                hpImage[i].gameObject.SetActive(true);
            else
                hpImage[i].gameObject.SetActive(false);
        }
    }

    public bool Isdead()
    {
        return isDead;
    }

    // 깜빡이기 👉 플레이어 큐브의 Mesh 껐다 키기
    IEnumerator BlinkCo()
    {
        isBlink = true;
        while (currentBlinkCount <= blinkCount) // 이 짓을 10번 함
        {
            playerMesh.enabled = !playerMesh.enabled; // 끄거나 혹은 키거나 
            yield return new WaitForSeconds(blinkSpeed); // 1.5초마다
            currentBlinkCount++;
        }

        playerMesh.enabled = true; // 제대로 다시 켜주고
        currentBlinkCount = 0;
        isBlink = false;
    }
}
