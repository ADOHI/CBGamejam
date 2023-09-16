using CBGamejam.Ingame.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBGamejam.Ingame.Objects
{
    [RequireComponent(typeof(Interactable))]
    public class Steerable : MonoBehaviour
    {
        private Vector3 rotateDirection;
        private CustomCharacterContoller[] characterContollers;
        public float steeringSpeed = 5f;
        public GameObject steeringMesh;
        //public bool isSteering;

        private void Awake()
        {
            characterContollers = new CustomCharacterContoller[2];
        }

        // Update is called once per frame
        void Update()
        {
            Steer();
        }

        private void Steer()
        {
            if (characterContollers[0] != null)
            {
                var xAngle = 0f;
                var yAngle = 0f;
                if (Input.GetKey(characterContollers[0].upKey))
                {
                    xAngle += steeringSpeed;
                }
                if (Input.GetKey(characterContollers[0].downKey))
                {
                    xAngle -= steeringSpeed;
                }
                if (Input.GetKey(characterContollers[0].leftKey))
                {
                    yAngle += steeringSpeed;
                }
                if (Input.GetKey(characterContollers[0].rightKey))
                {
                    yAngle -= steeringSpeed;
                }

                steeringMesh.transform.Rotate(0f, yAngle * Time.deltaTime, 0f, Space.World);
                steeringMesh.transform.Rotate(xAngle * Time.deltaTime, 0f, 0f, Space.Self);
            }
            if (characterContollers[1] != null)
            {
                var xAngle = 0f;
                var yAngle = 0f;
                if (Input.GetKey(characterContollers[1].upKey))
                {
                    xAngle += steeringSpeed;
                }
                if (Input.GetKey(characterContollers[1].downKey))
                {
                    xAngle -= steeringSpeed;
                }
                if (Input.GetKey(characterContollers[1].leftKey))
                {
                    yAngle += steeringSpeed;
                }
                if (Input.GetKey(characterContollers[1].rightKey))
                {
                    yAngle -= steeringSpeed;
                }

                steeringMesh.transform.Rotate(0f, yAngle * Time.deltaTime, 0f, Space.World);
                steeringMesh.transform.Rotate(xAngle * Time.deltaTime, 0f, 0f, Space.Self);
            }
        }

        public void OnStartSteer(CustomCharacterContoller controller)
        {
            characterContollers[controller.playerIndex] = controller;
        }

        public void OnEndSteer(CustomCharacterContoller controller)
        {
            characterContollers[controller.playerIndex] = null;
        }
    }

}
