using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour
{
    
    
    public float shakeDuration = 1.0f; // 흔들림 지속 시간
    public float shakeAmount = 15.0f;  // 흔들림 크기
    public float currentShakeDuration = 0.0f; // 현재 흔들림 지속 시간
    
    Quaternion originRotation;
    public Vector3 originPos;
    // Start is called before the first frame update
    void Start()
    {
        originPos = gameObject.transform.position;
        originRotation = gameObject.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        shake();
    }

    void shake()
    {
        if (currentShakeDuration > 0)
        {
            // 랜덤한 회전을 생성하여 흔들림 효과 적용
            Quaternion randomRotation = Random.rotation;
            transform.rotation = originRotation * randomRotation;
            // 랜덤한 포지션을 생성하여 흔들림 효과 적용
            Vector3 randomPosition = Random.insideUnitSphere;
            transform.position =  originPos + randomPosition;
            // 흔들림 시간 감소
            currentShakeDuration -= Time.deltaTime;
        }
        else
        {
            // 흔들림이 끝났을 때 원래 회전 값으로 돌아감
            transform.rotation = originRotation;
            transform.position = originPos;
        }
    }

  

    public void setShakeDuration(float f)
    {
        currentShakeDuration = f;
    }
}
