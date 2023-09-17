using CBGamejam.Ingame.Characters.FXs;
using CBGamejam.Ingame.Manager;
using CBGamejam.Ingame.Objects;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace CBGamejam.Ingame.Characters
{
    public class CustomCharacterContoller : MonoBehaviour
    {
        [HideInInspector] public Rigidbody rigidbody;
        [HideInInspector] public bool isDancing;
        private ConfigurableJoint joint;
        private Vector3 characterForwardDirection => transform.forward;
        private Vector3 characterViewingDirection;

        public int playerIndex;
        [Header("MoveSetting")]
        private bool isMoveAvailable = true;
        public float walkSpeed = 5f;
        public float runSpeed = 10f;
        public float lerpSpeed = 10f;
        public ForceMode forceMode;
        public GameObject characterModel;
        [Header("GrabSetting")]
        private bool isGrabbing;
        private bool isHolding;
        private bool isPulling;
        private bool isLaying;
        private float currentLayingDuration;
        private Interactable focusedInteractable;
        private Grabbable interactingGrabbable;
        public float grabRadius;
        public float grabPowerThreshold;
        public LayerMask grabbableLayerMask;
        public float minLayingDuration;
        public float maxLayingDuration;
        public float minThrowingPower;
        public float maxThrowingPower;
        private float layingProgress;
        public Image layingProgressImage;
        public Transform holdPositionTransform;
        [Header("SteeringSetting")]
        private bool isSteering;
        private Steerable interactingSteerable;
        [Header("Breakable")]
        private Grabbable grabbable;
        private float breakPoint;
        public float breakPowerSpeed;
        public float breakThrowingProgress = 0.1f;
        [Header("Animations")]
        public Animator animator;
        
        [Header("FXs")]
        public LetterEffectSystem letterEffectSystem;
        public float letterEffectInterval = 10f;
        public float letterEffectRandomness = 0.3f;
        public ParticleSystem footStep;
        public Vector3 balloonOffset;
        public GameObject steeringImage;
        public GameObject hardImage;
        [Header("SFXs")]
        public AudioClip throwSound;
        public AudioClip footStepSfx;
        public float footStepInterval = 0.5f;
        public AudioClip grabSfx;
        [Header("Respawn")]
        public Transform respawnPoint;

        [Header("Input")]
        public KeyCode upKey = KeyCode.W;
        public KeyCode downKey = KeyCode.S;
        public KeyCode leftKey = KeyCode.A;
        public KeyCode rightKey = KeyCode.D;
        public KeyCode grabKey = KeyCode.G;
        public KeyCode interactionKey = KeyCode.H;
        public KeyCode runKey = KeyCode.LeftShift;
        public KeyCode danceKey = KeyCode.Space;
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

            if (!TryGetComponent(out grabbable))
            {
                grabbable = gameObject.AddComponent<Grabbable>();
            }
        }

        private void Start()
        {
            ShowLetterFxAsnyc().AttachExternalCancellation(this.destroyCancellationToken).Forget();
            layingProgressImage.color = (playerIndex == 0) ? IngameManager.Instance.firstPlayerColor : IngameManager.Instance.secondPlayerColor;

            PlayFootstepSfx(footStepInterval).AttachExternalCancellation(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private void Update()
        {
            RotateCharacterModel();
            ScanAround();
            GrabOrLay();
            Interact();

            footStep.transform.position = transform.position + Vector3.up;
            layingProgressImage.transform.position = Camera.main.WorldToScreenPoint(transform.position);
            layingProgressImage.fillAmount = layingProgress;
            var emission = footStep.emission;
            if (isMoveAvailable)
            {
                emission.rateOverTime = rigidbody.velocity.magnitude;
            }
            else
            {
                emission.rateOverTime = 0f;
            }

            steeringImage.transform.position = Camera.main.WorldToScreenPoint(transform.position + balloonOffset);
            hardImage.transform.position = Camera.main.WorldToScreenPoint(transform.position + balloonOffset);
            steeringImage.SetActive(isSteering);
            hardImage.SetActive(isPulling);

        }

        private void FixedUpdate()
        {
            if (isMoveAvailable)
            {
                Move();
                animator.SetBool("isMove", true);
            }
            else
            {
                animator.SetBool("isMove", false);
            }
            animator.SetFloat("moveSpeed", rigidbody.velocity.magnitude);

            animator.SetBool("isCarrying", isHolding || isPulling);
            //Lay();
        }
        public void ResetPosition()
        {
            transform.position = respawnPoint.transform.position;
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


            if (directionVector != Vector3.zero)
            {
                //directionVector = Vector3.Lerp(transform.forward, directionVector, Time.fixedDeltaTime * 5f);

                //transform.forward = directionVector;

                if (grabbable.isPulling)
                {
                    Debug.Log("Test");

                    if (breakPoint >= 1f)
                    {
                        //Break;
                        grabbable.currentInteractingCharacter.Lay(breakThrowingProgress);
                        breakPoint = 0f;
                    }
                    else
                    {
                        var pullerDirection = grabbable.currentInteractingCharacter.characterViewingDirection;
                        var angle = Vector3.Angle(directionVector, pullerDirection);
                        var score = (180f - angle) / 180f;
                        breakPoint += breakPowerSpeed * score * Time.fixedDeltaTime;
                        Debug.Log((directionVector, pullerDirection, angle, score, breakPoint));
                    }
                }
                else
                {
                    //Debug.Log(Vector3.SignedAngle(directionVector, transform.forward, Vector3.up));
                    var moveAngle = Vector3.SignedAngle(directionVector, transform.forward, Vector3.up);
                    animator.SetBool("isMoveLeft", moveAngle > 0.1f);
                    animator.SetBool("isMoveRight", moveAngle < -0.1f);


                    transform.rotation =
                        Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(directionVector), Time.deltaTime * lerpSpeed);
                    rigidbody.AddForce(transform.forward * moveSpeed * Time.fixedDeltaTime, forceMode);

                    if (isGrabbing)
                    {
                        if (isHolding)
                        {
                            characterViewingDirection = directionVector;
                        }
                        else if (isGrabbing)
                        {
                            characterViewingDirection = (interactingGrabbable.transform.position - transform.position).normalized;
                        }
                    }
                    else
                    {
                        characterViewingDirection = directionVector;
                    }
                }

                if (Input.GetKey(KeyCode.Space))
                {
                    animator.SetBool("isDance", false);
                    isDancing = false;
                }
            }
            else
            {
                animator.SetBool("isMoveLeft", false);
                animator.SetBool("isMoveRight", false);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    animator.SetInteger("danceIndex", Random.Range(0, 4));
                }

                if (Input.GetKey(KeyCode.Space))
                {
                    animator.SetBool("isDance", true);

                    isDancing = true;
                }
                else
                {
                    animator.SetBool("isDance", false);
                    isDancing = false;
                }
            }

            //controller.Move(directionVector.normalized * moveSpeed * Time.deltaTime);
        }

        private async UniTask PlayFootstepSfx(float interval)
        {
            if (rigidbody.velocity.magnitude > 0.1f)
            {
                SoundManager.PlayFx(footStepSfx, volume: 5f);
            }
            await UniTask.Delay((int)(interval * 1000f));
            PlayFootstepSfx(interval).AttachExternalCancellation(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private void RotateCharacterModel()
        {
            if (characterViewingDirection != Vector3.zero)
            {
                characterModel.transform.rotation =
                    Quaternion.Lerp(characterModel.transform.rotation, Quaternion.LookRotation(characterViewingDirection), Time.deltaTime * lerpSpeed);
            }
        }

        private void GrabOrLay()
        {
            if (Input.GetKeyDown(grabKey))
            {
                Debug.Log("Input Grab Key");

                if (!isGrabbing && !isSteering)
                {
                    if (focusedInteractable == null)
                    {
                        return;
                    }

                    if (!focusedInteractable.TryGetComponent(out Grabbable focusedGrabbable))
                    {
                        return;
                    }
                    //interactableGrabbable = focusedGrabbable;
                    //Grab
                    var grabbedRigidbody = focusedGrabbable.rigidbody;

                    SoundManager.PlayFx(grabSfx, volume: 3f);

                    if (grabbedRigidbody.mass > grabPowerThreshold)
                    {
                        //Pull
                        Pull(focusedGrabbable);
                    }
                    else
                    {
                        //Hold
                        Hold(focusedGrabbable);
                    }
                }
                else if (isGrabbing)
                {
                    if (interactingGrabbable == null)
                    {
                        Debug.LogError("interactingGrabbable is null");
                        return;
                    }

                    //Lay

                    StartLay();
                }

            }
            else if (Input.GetKeyUp(grabKey))
            {
                if (isLaying)
                {
                    Lay(layingProgress);
                }
            }

            if (isLaying)
            {
                currentLayingDuration = currentLayingDuration + Time.deltaTime;
                layingProgress = Mathf.Clamp01((currentLayingDuration - minLayingDuration) / maxLayingDuration);

            }
        }        

        private void Pull(Grabbable focusedGrabbable)
        {
            Debug.Log("Pull " + focusedInteractable.gameObject.name);

            characterViewingDirection = (focusedGrabbable.transform.position - transform.position).normalized;

            interactingGrabbable = focusedGrabbable;
            joint.connectedBody = focusedGrabbable.rigidbody;
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
            interactingGrabbable.OnStartPull(this);
            isGrabbing = true;
            isPulling = true;
        }

        private void Hold(Grabbable focusedGrabbable)
        {
            Debug.Log("Hold " + focusedInteractable.gameObject.name);

            characterViewingDirection = (focusedGrabbable.transform.position - transform.position).normalized;
            interactingGrabbable = focusedGrabbable;            
            interactingGrabbable.OnStartHold(this);
            isGrabbing = true;
            isHolding = true;
        }

        private void StartLay()
        {
            isLaying = true;
        }

        private void Lay(float progress)
        {
            joint.connectedBody = null;
            joint.xMotion = ConfigurableJointMotion.Free;
            joint.yMotion = ConfigurableJointMotion.Free;
            joint.zMotion = ConfigurableJointMotion.Free;
            interactingGrabbable.OnEndGrab();

            if (progress > 0f)
            {
                SoundManager.PlayFx(throwSound);
            }
            //Throw
            var throwingPower = Mathf.Lerp(minThrowingPower, maxThrowingPower, progress);
            var throwDirection = (characterViewingDirection + Vector3.up).normalized;
            interactingGrabbable.rigidbody.AddForce(throwDirection * throwingPower, ForceMode.Impulse);
            //interactableGrabbable
            layingProgress = 0f;
            currentLayingDuration = 0f;
            interactingGrabbable = null;
            isGrabbing = false;
            isLaying = false;
            isPulling = false;
            isHolding = false;
        }

        private void Interact()
        {
            if (Input.GetKeyDown(interactionKey))
            {
                Debug.Log("Input Interaction Key");

                if (!isGrabbing && !isSteering)
                {
                    if (focusedInteractable == null)
                    {
                        return;
                    }

                    if (!focusedInteractable.TryGetComponent(out Steerable focusedSteerable))
                    {
                        return;
                    }
                    
                    StartSteering(focusedSteerable);

                }
                else if (isSteering)
                {
                    if (interactingSteerable == null)
                    {
                        Debug.LogError("interactingSteerable is null");
                        return;
                    }

                    EndSteering();
                    //Lay
                }
            }

            if (isSteering)
            {
                var distance = Vector3.Distance(transform.position, interactingSteerable.transform.position);
                if (distance > grabRadius)
                {
                    EndSteering();
                }
            }
        }

        private void StartSteering(Steerable steerable)
        {
            isMoveAvailable = false;
            isSteering = true;
            interactingSteerable = steerable;
            steerable.OnStartSteer(this);
        }

        private void EndSteering()
        {
            isMoveAvailable = true;
            isSteering = false;
            interactingSteerable.OnEndSteer(this);
            interactingSteerable = null;
        }

        private void ScanAround()
        {
            if (isGrabbing || isSteering)
            {
                if (focusedInteractable != null)
                {
                    focusedInteractable.OnExitFocus(playerIndex);
                    focusedInteractable = null;
                }
                return;
            }

            var colliders = Physics.OverlapSphere(transform.position, grabRadius, grabbableLayerMask);
            colliders = colliders.Where(c => !c.gameObject.Equals(gameObject)).ToArray();
            if (!(colliders.Length > 0))
            {
                if (focusedInteractable != null)
                {
                    focusedInteractable.OnExitFocus(playerIndex);
                    focusedInteractable = null;
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
                    var score = CalculateGrabScore(characterForwardDirection, direction, distance);
                    if (score > maxScore)
                    {
                        maxScore = score;
                        maxScoredObject = colliders[i].gameObject;
                    }
                }

                var scannedGrabbable = maxScoredObject.GetComponent<Interactable>();

                if (scannedGrabbable == null)
                {
                    return;
                }

                if (focusedInteractable == null)
                {
                    scannedGrabbable.OnEnterFocus(playerIndex);
                    focusedInteractable = scannedGrabbable;
                }
                else if (!focusedInteractable.Equals(scannedGrabbable))
                {
                    focusedInteractable.OnExitFocus(playerIndex);
                    scannedGrabbable.OnEnterFocus(playerIndex);
                    focusedInteractable = scannedGrabbable;
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

        private int GetStateIndex()
        {
            if (grabbable.isPulling)
            {
                return 3;
            }
            else if (isPulling)
            {
                if (interactingGrabbable.TryGetComponent(out CustomCharacterContoller _))
                {
                    return 5;
                }
                else
                {
                    return 2;
                }
            }
            else if (isHolding)
            {
                return 1;
            }
            else if (isMoveAvailable && rigidbody.velocity.magnitude > 3f)
            {
                return 0;
            }
            else
            {
                return 4;
            }
        }


        private async UniTask ShowLetterFxAsnyc()
        {
            var nextInterval = letterEffectInterval * Random.Range(1f - letterEffectRandomness, 1f + letterEffectRandomness);

            await UniTask.Delay((int)(1000 * nextInterval)).AttachExternalCancellation(this.destroyCancellationToken);

            var index = GetStateIndex();

            letterEffectSystem.ShowTextFX(index);

            ShowLetterFxAsnyc().AttachExternalCancellation(this.destroyCancellationToken).Forget();
        }
    }

}
