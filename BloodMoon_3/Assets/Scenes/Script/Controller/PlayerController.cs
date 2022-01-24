using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static bool s_canPressKey = true;  // 이 값이 false면 큐브를 움직일 수 없도록(이동 회전 반동 시키는 StartAction 호출 불가)

    // 이동
    [SerializeField] float moveSpeed = 3;   // 1초에 3만큼 움직이도록
    Vector3 dir = new Vector3();
    public Vector3 destPos = new Vector3();

    // 회전
    [SerializeField] float spinSpeed = 270f;  // 270도 
    Vector3 rotDir = new Vector3();
    Quaternion destRot = new Quaternion();

    bool canMove = true; // 쿠브 이동중에는 노트 판정이 불가능 하도록 (코루틴 중복 실행 차단)
    bool isFalling = false; // 추락 중인지

    // 수직 반동
    [SerializeField] float recoilPosY = 0.25f;
    [SerializeField] float recoilSpeed = 1.5f;

    [SerializeField] Transform fakeCube = null;
    [SerializeField] Transform realCube = null;


    TimingManager theTimingManager;
    CameraController theCam;
    Rigidbody myRigid;
    StatusManager theStatus;

    Vector3 originPos = new Vector3(); // 추락하고 나면 원래 위치로 부활시켜야 하므로 기억하기 위해

    void Start()
    {
        theTimingManager = FindObjectOfType<TimingManager>();
        theCam = FindObjectOfType<CameraController>();
        myRigid = GetComponentInChildren<Rigidbody>(); // 이 스크립트가 붙는 빈 오브젝트인 Player의 자식 큐브에 Rigidbody가 있다. 큐브인 자식에게서 Rigidbody 가져옴.
        originPos = transform.position; // 부활 포인트이자 시작 위치. 부활 포인트가 여러개면 그때 그때마다 새롭게 세팅하고 저장해서 기억시켜놓으면 될 것 같다.
        theStatus = FindObjectOfType<StatusManager>();
    }

    void Update()
    {
        if (GameManager.instance.isStartGame)
        {
            CheckFalling(); // 혹시 추락중은 아닌지 체크

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
            {
                if (canMove && s_canPressKey && !isFalling)
                {
                    Calc();

                    // 판정 체크 : Perfect, Cool, Good, Bad 일 때만 True
                    if (theTimingManager.CheckTiming())
                    {
                        StartAction();
                    }
                }
            }
        }
    }

    void Calc()
    {
        // 큐브의 x축 -> 위 아래   큐브의 z축 -> 오른쪽, 왼쪽
        dir.Set(Input.GetAxisRaw("Vertical"), 0, Input.GetAxisRaw("Horizontal"));

        // 이동 목표값 게산
        destPos = transform.position + new Vector3(-dir.x, 0, dir.z);

        // 회전 목표값 계산
        rotDir = new Vector3(-dir.z, 0f, -dir.x);
        fakeCube.RotateAround(transform.position, rotDir, spinSpeed); //공전
        destRot = fakeCube.rotation;
    }

    void StartAction() // Perfect, Cool, Good, Bad 일 때만 한번 시랳ㅇ됨
    {
        StartCoroutine(MoveCo());
        StartCoroutine(SpinCo());
        StartCoroutine(RecoilCo());
        StartCoroutine(theCam.ZoomCam());
    }

    IEnumerator MoveCo()
    {
        canMove = false;
        while (Vector3.SqrMagnitude(transform.position - destPos) >= 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = destPos;
        canMove = true;
    }

    IEnumerator SpinCo()
    {
        while (Quaternion.Angle(realCube.rotation, destRot) > 0.5f)
        {
            realCube.rotation = Quaternion.RotateTowards(realCube.rotation, destRot, spinSpeed * Time.deltaTime);
            yield return null;
        }

        realCube.rotation = destRot;
    }

    IEnumerator RecoilCo()
    {
        // 올라감
        while (realCube.position.y < recoilPosY)
        {
            realCube.position += new Vector3(0, recoilSpeed * Time.deltaTime, 0);
            yield return null;
        }

        // 내려감
        while (realCube.position.y > 0)
        {
            realCube.position -= new Vector3(0, recoilSpeed * Time.deltaTime, 0);
            yield return null;
        }

        realCube.localPosition = new Vector3(0, 0, 0);
    }

    void CheckFalling()
    {
        if (!isFalling && canMove) // 완전히 멈추고 체크 + 떨어지는 중이 아닐 때만 체크하도록
        {
            // '!' 주목! 플레이어에서 아래로 Raycast를 쐈을 때 아무 것도 없다면 추락 중인것이다. 
            // 추락 중이 아니라면 스테이지의 Plate 와 충돌되야 정상이다.
            if (!Physics.Raycast(transform.position, Vector3.down, 1.1f))
            {
                Falling();
            }
        }
    }

    // 추락 중이라면 물리적으로 추락하도록 자식 큐브의 중력 활성화, 물리 활성화
    void Falling()
    {
        isFalling = true;
        myRigid.useGravity = true;
        myRigid.isKinematic = false;
    }

    // 이제 추락이 끝나서(밑에 깔린 Deadzone Collider에 충돌) 부활해야 한다면 
    // 자식 큐브의 중력 다시 비활성화, 물리 비활성화
    // 원래 위치로 리셋
    // ✨떨어진건 Rigidbody가 붙어있는 자식 큐브이고 부모인 빈 오브젝트는 원래 위치 그대로에 있다. 따라서 자식 큐브도 다시 부모 위치에 붙여주어야 하므로 localPosition을 원점으로 리셋하는 것이다.
    public void ResetFalling()
    {
        theStatus.DecreaseHp(1);

        if (!theStatus.Isdead())
        {
            isFalling = false;
            myRigid.useGravity = false;
            myRigid.isKinematic = true;

            transform.position = originPos;
            realCube.localPosition = Vector3.zero;
        }
    }

}