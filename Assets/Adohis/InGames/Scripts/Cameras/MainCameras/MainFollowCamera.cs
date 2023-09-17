using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace CBGamejam.Ingame.Cameras
{
    public class MainFollowCamera : MonoBehaviour
    {
        private Vector3 targetPosition;
        private float interporatedDistance;

        public GameObjectReference firstPlayerCharater;
        public GameObjectReference secondPlayerCharater;

        [Header("FollowSetting")]
        public float minDistance;
        public float distanceCoefficient = 1f;
        public float xAngle;
        public float lerpValue = 5f;

        void Start()
        {
            interporatedDistance = minDistance;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            Follow();
        }

        private void Follow()
        {

            if (firstPlayerCharater.Value == null && secondPlayerCharater.Value == null)
            {
                return;
            }

            if (firstPlayerCharater.Value != null && secondPlayerCharater.Value != null)
            {
                //´õºí
                targetPosition = (firstPlayerCharater.Value.transform.position + secondPlayerCharater.Value.transform.position) * 0.5f;
                var distanceBetween = Vector3.Distance(firstPlayerCharater.Value.transform.position, secondPlayerCharater.Value.transform.position);
                var toDistance = minDistance + distanceBetween * distanceCoefficient;
                interporatedDistance = Mathf.Lerp(interporatedDistance, toDistance, lerpValue * Time.deltaTime);
            }
            else if (firstPlayerCharater.Value != null)
            {
                targetPosition = firstPlayerCharater.Value.transform.position;
            }
            else
            {
                targetPosition = secondPlayerCharater.Value.transform.position;
            }

            var directionVector = Quaternion.Euler(-xAngle, 0f, 0f) * Vector3.up;

            transform.position = Vector3.Lerp(transform.position, targetPosition + directionVector * interporatedDistance, lerpValue * Time.deltaTime);
            transform.forward = -directionVector;
            /*            {
                            if (secondPlayerCharater.Value != null)
                            {
                                targetPosition = (firstPlayerCharater.Value.transform.position + secondPlayerCharater.Value.transform.position) * 0.5f;
                            }
                            else
                            {

                            }
                        }*/
        }
    }

}
