using CBGamejam.Ingame.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Crafts"))
        {
            IngameManager.Instance.isCraftsInLighting = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Crafts"))
        {
            IngameManager.Instance.isCraftsInLighting = false;
        }
    }
}
