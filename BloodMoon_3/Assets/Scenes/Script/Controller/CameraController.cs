using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform thePlayer = null;
    [SerializeField] float followSpeed = 15;

    Vector3 playerDistance = new Vector3();

    float hitDistance = 0;
    [SerializeField] float zoomDistance = -1.25f; // 뒤로 -1.25 거리만큼 줌아웃

    void Start()
    {
        playerDistance = transform.position - thePlayer.position;
    }

    void Update()
    {
        Vector3 t_destPos = thePlayer.position + playerDistance + (transform.forward * hitDistance);
        transform.position = Vector3.Lerp(transform.position, t_destPos, followSpeed * Time.deltaTime);
    }

    public IEnumerator ZoomCam()
    {
        hitDistance = zoomDistance;
        yield return new WaitForSeconds(0.15f);
        hitDistance = 0;
    }
}

