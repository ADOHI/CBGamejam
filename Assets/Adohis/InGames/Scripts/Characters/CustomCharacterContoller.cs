using CBGamejam.Ingame.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CBGamejam.Ingame.Characters
{
    public class CustomCharacterContoller : MonoBehaviour
    {
        private Rigidbody rigidbody;
        private ConfigurableJoint joint;
        private Vector3 characterForward => transform.forward;
        [Header("MoveSetting")]
        public float walkSpeed = 5f;
        public float runSpeed = 10f;
        public ForceMode forceMode;
        [Header("GrabSetting")]
        private bool isGrabbing;
        private Grabbable focusedGrabbable;
        private Grabbable interactableGrabbable;
        public float grabRadius;
        public float grabPowerThreshold;
        public LayerMask grabbableLayerMask;
        [Header("Input")]
        public KeyCode upKey = KeyCode.W;
        public KeyCode downKey = KeyCode.S;
        public KeyCode leftKey = KeyCode.A;
        public KeyCode rightKey = KeyCode.D;
        public KeyCode grabKey = KeyCode.G;
        public KeyCode interactionKey = KeyCode.H;
        public KeyCode runKey = KeyCode.LeftShift;
        
        private void Awake()
        {
            if (!TryGetComponent(out rigidbody))
            {
                rigidbody = gameObject.AddComponent<Rigidbody>();
            }

            if (!TryGetComponent(out joint))
            {
                joint = gameObject.AddComponent<ConfigurableJoint>();
            }
        }

        private void Update()
        {
            ScanAround();
            GrabOrLay();
        }

        private void FixedUpdate()
        {
            Move();
            //Lay();
        }

        private void Move()
        {
            var directionVector = Vector3.zero;
            
            if (Input.GetKey(upKey))
            {
                directionVector += Vector3.forward;
            }
            if (Input.GetKey(downKey))
            {
                directionVector += Vector3.back;
            }
            if (Input.GetKey(leftKey))
            {
                directionVector += Vector3.left;
            }
            if (Input.GetKey(rightKey))
            {
                directionVector += Vector3.right;
            }

            var moveSpeed = (Input.GetKey(runKey)) ? runSpeed : walkSpeed;

            rigidbody.AddForce(directionVector.normalized * moveSpeed * Time.fixedDeltaTime, forceMode);

            if (directionVector != Vector3.zero)
            {
                transform.forward = directionVector;
            }
            //controller.Move(directionVector.normalized * moveSpeed * Time.deltaTime);
        }

        private void GrabOrLay()
        {
            if (Input.GetKeyDown(grabKey))
            {
                Debug.Log("Input Grab Key");

                if (!isGrabbing)
                {
                    if (focusedGrabbable == null)
                    {
                        return;
                    }

                    //Grab

                    Debug.Log("Grab " + focusedGrabbable.gameObject.name);

                    interactableGrabbable = focusedGrabbable;
                    joint.connectedBody = interactableGrabbable.GetComponent<Rigidbody>();
                    joint.xMotion = ConfigurableJointMotion.Locked;
                    joint.yMotion = ConfigurableJointMotion.Locked;
                    joint.zMotion = ConfigurableJointMotion.Locked;
                    interactableGrabbable.OnStartGrab();
                    isGrabbing = true;
                }
                else
                {
                    if (interactableGrabbable == null)
                    {
                        return;
                    }

                    //Lay

                    Debug.Log("Lay " + interactableGrabbable.gameObject.name);

                    joint.connectedBody = null;
                    joint.xMotion = ConfigurableJointMotion.Free;
                    joint.yMotion = ConfigurableJointMotion.Free;
                    joint.zMotion = ConfigurableJointMotion.Free;
                    interactableGrabbable.OnExitGrab();
                    interactableGrabbable = null;
                    isGrabbing = false;
                }

            }
        }        

        private void ScanAround()
        {
            if (isGrabbing)
            {
                if (focusedGrabbable != null)
                {
                    focusedGrabbable.OnExitInteraction();
                    focusedGrabbable = null;
                }
                return;
            }

            var colliders = Physics.OverlapSphere(transform.position, grabRadius, grabbableLayerMask);
            if (!(colliders.Length > 0))
            {
                if (focusedGrabbable != null)
                {
                    focusedGrabbable.OnExitInteraction();
                    focusedGrabbable = null;
                }
                return;
            }
            else
            {
                var maxScore = -1f;
                GameObject maxScoredObject = null;
                for (int i = 0; i < colliders.Length; i++)
                {
                    var direction = colliders[i].transform.position - transform.position;
                    var distance = Vector3.Distance(transform.position, colliders[i].transform.position);
                    var score = CalculateGrabScore(characterForward, direction, distance);
                    if (score > maxScore)
                    {
                        maxScore = score;
                        maxScoredObject = colliders[i].gameObject;
                    }
                }

                var scannedGrabbable = maxScoredObject.GetComponent<Grabbable>();

                if (focusedGrabbable == null)
                {
                    scannedGrabbable.OnEnterInteraction();
                    focusedGrabbable = scannedGrabbable;
                }
                else if (!focusedGrabbable.Equals(scannedGrabbable))
                {
                    focusedGrabbable.OnExitInteraction();
                    scannedGrabbable.OnEnterInteraction();
                    focusedGrabbable = scannedGrabbable;
                }
            }
        }


        private float CalculateGrabScore(Vector3 forward, Vector3 direction, float distance, float minScore = 0.2f, float maxScore = 1f)
        {
            var angle = Vector3.Angle(forward, direction);
            var angleScore = Mathf.Lerp(minScore, maxScore, (180f - angle) / 180f);
            var finalScore = angleScore / distance;
            return finalScore;
        }
    }

}
