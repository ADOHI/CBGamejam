using CBGamejam.Ingame.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public AudioClip waterSfx;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            var controller = collision.transform.GetComponentInChildren<CustomCharacterContoller>();
            SoundManager.PlayFx(waterSfx, volume: 3f);
            controller.ResetPosition();
            
        }
    }
}
