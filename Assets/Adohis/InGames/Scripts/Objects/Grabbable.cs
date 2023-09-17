using CBGamejam.Ingame.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace CBGamejam.Ingame.Objects
{
    [RequireComponent(typeof(Interactable))]
    public class Grabbable : MonoBehaviour
    {

        [HideInInspector] public CustomCharacterContoller currentInteractingCharacter;
        private float initialPlayerMass;
        private Collider collider;
        [HideInInspector] public Rigidbody rigidbody;

        public bool isForward;
        public bool isHolding;
        public bool isPulling;

        private void Awake()
        {
            if (!TryGetComponent(out rigidbody))
            {
                rigidbody = gameObject.AddComponent<Rigidbody>();
            }

            if (!TryGetComponent(out collider))
            {
                collider = gameObject.AddComponent<BoxCollider>();
            }
        }

        private void Update()
        {
            if (isHolding)
            {
                transform.position = currentInteractingCharacter.holdPositionTransform.position;
                if (isForward)
                {
                    transform.forward = currentInteractingCharacter.transform.forward;
                }
                else
                {
                    transform.up = currentInteractingCharacter.transform.forward;
                }
            }
        }

        public void OnStartPull(CustomCharacterContoller controller)
        {
            currentInteractingCharacter = controller;
            isPulling = true;
        }

        public void OnEndPull()
        {
            currentInteractingCharacter = null;
            isPulling = false;
        }

        public void OnEndGrab()
        {
            if (isHolding)
            {
                OnEndHold();
            }
            if (isPulling)
            {
                OnEndPull();
            }
        }

        public void OnStartHold(CustomCharacterContoller controller)
        {
            isHolding = true;
            rigidbody.isKinematic = true;
            collider.enabled = false;
            currentInteractingCharacter = controller;
            initialPlayerMass = currentInteractingCharacter.rigidbody.mass;
            currentInteractingCharacter.rigidbody.mass = initialPlayerMass + rigidbody.mass;
        }

        public void OnEndHold()
        {
            isHolding = false;
            rigidbody.isKinematic = false;
            collider.enabled = true;
            currentInteractingCharacter.rigidbody.mass = initialPlayerMass;
            currentInteractingCharacter = null;
        }
    }

}
