using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControl : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 beforePos;
    private Quaternion beforeRotation;
    
    private Vector3 currentPos;
    private Quaternion currentRotation;

    public float thresholdLen = 0.5f;
    public float thresholdRotation = 10f;

    public ParticleSystem particle;
    void Start()
    {
        beforePos = gameObject.transform.position;
        beforeRotation = gameObject.transform.rotation;
        currentPos = beforePos;
        currentRotation = beforeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        currentPos = gameObject.transform.position;
        currentRotation = gameObject.transform.rotation;

        if (isMoving())
        {
            particle.Play();
        }
        else
        {
            particle.Stop();
        }

        beforePos = currentPos;
        beforeRotation = currentRotation;
    }

    bool isMoving()
    {
        float angleChange = Vector3.Magnitude(currentRotation.eulerAngles - beforeRotation.eulerAngles);
        float posChange = Vector3.Magnitude(currentPos - beforePos);

        return (angleChange > thresholdRotation || posChange > thresholdLen);
    }
}
