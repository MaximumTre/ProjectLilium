using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using CMF;
using UnityEngine.InputSystem;

namespace SolarFalcon
{
    public class PlayerDash : MonoBehaviour
    {
        PlayerMelee PlayerMelee;
        AdvancedWalkerController MyMover;
        CharacterKeyboardInput mKeyboardInput;
        CharacterJoystickInput mJoystickInput;
        public Animator animator;
        public bool usingKeyboard = false;
        public UnityEvent OnDashUsed;
        public float forwardDashPower, horizontalDashPower, backDashPower,
            forwardDashTime = 0.2f, horizontalDashTime = 0.1f, backDashTime = 0.5f;
        float dashTime, cachedMoveSpeed, cachedSlideGravity;
        [SerializeField] bool debug = false, allDashAreForward = false;
        [SerializeField] TurnTowardCameraDirection CameraDirection;
        [SerializeField] TurnTowardControllerVelocity ControllerVelocity;

        PlayerLock PlayerLock;

        int currentAnimatorLayerIndex;

        public float HypermodeMoveSpeed, HypermodeForwardDashPower, HypermodeBackDashPower, HypermodeHorizontalDashPower;
        bool hypermode;

        private void Awake()
        {
            PlayerMelee = GetComponent<PlayerMelee>();
            PlayerLock = GetComponent<PlayerLock>();
            MyMover = GetComponent<AdvancedWalkerController>();
            mKeyboardInput = GetComponent<CharacterKeyboardInput>();
            mJoystickInput = GetComponent<CharacterJoystickInput>();
        }

        private void Start()
        {
            cachedMoveSpeed = MyMover.movementSpeed;
            cachedSlideGravity = MyMover.slideGravity;
        }

        private void OnEnable()
        {
            PlayerMelee.OnPlayerWeaponSwitch += OnPlayerMeleeSwitch;
        }

        private void OnDisable()
        {
            PlayerMelee.OnPlayerWeaponSwitch -= OnPlayerMeleeSwitch;
        }

        private void Update()
        {
            if (usingKeyboard)
            {
                animator.SetFloat("HorizontalMovement", mKeyboardInput.GetHorizontalMovementInput());
                animator.SetFloat("ForwardMovement", mKeyboardInput.GetVerticalMovementInput());
            }

            else
            {
                animator.SetFloat("HorizontalMovement", mJoystickInput.GetHorizontalMovementInput());
                animator.SetFloat("ForwardMovement", mJoystickInput.GetVerticalMovementInput());
            }
        }

        void OnPlayerMeleeSwitch(int animatorLayerIndex)
        {
            currentAnimatorLayerIndex = animatorLayerIndex;   
        }

        public void ForwardDash()
        {
            if(PlayerLock.GetDashLock())
            {
                return;
            }
            animator.Play("Dash Forward", currentAnimatorLayerIndex);
            //animator.SetTrigger("Dash Forward");
            dashTime = forwardDashTime;
            MyMover.movementSpeed = 0;
            MyMover.slideGravity = 0;

            OnDashUsed.Invoke();

            if (hypermode)
            {
                MyMover.AddMomentum(animator.transform.forward * HypermodeHorizontalDashPower);
            }

            else
            {
                MyMover.AddMomentum(animator.transform.forward * horizontalDashPower);
            }

            PlayerLock.PlayerDashLockAttack(true);
            PlayerLock.PlayerDashLockBlock(true);
            PlayerLock.PlayerDashLockCast(true);

            OnDashUsed.Invoke();
            PlayerLock.PlayerDashLockDash(true);
            PlayerLock.PlayerDashLockRotation(true);
            PlayerLock.PlayerDashLockMouseRotation(true);
            StartCoroutine("EndDash");
        }

        public void BackDash()
        {
            if (PlayerLock.GetDashLock())
            {
                return;
            }

            if(!CameraDirection.enabled)
            {
                animator.Play("Dash Forward");
                dashTime = forwardDashTime;
                MyMover.movementSpeed = 0;
                MyMover.slideGravity = 0;

                OnDashUsed.Invoke();

                if (hypermode)
                {
                    MyMover.AddMomentum(-animator.transform.forward * HypermodeHorizontalDashPower);
                }

                else
                {
                    MyMover.AddMomentum(-animator.transform.forward * horizontalDashPower);
                }

                PlayerLock.PlayerDashLockAttack(true);
                PlayerLock.PlayerDashLockBlock(true);
                PlayerLock.PlayerDashLockCast(true);

                OnDashUsed.Invoke();
                PlayerLock.PlayerDashLockDash(true);
                PlayerLock.PlayerDashLockRotation(true);
                PlayerLock.PlayerDashLockMouseRotation(true);
                StartCoroutine("EndDash");
                return;
            }

            animator.Play("Dash Back", currentAnimatorLayerIndex);
            //animator.SetTrigger("Dash Backward");
            dashTime = backDashTime;
            MyMover.movementSpeed = 0;
            MyMover.slideGravity = 0;

            OnDashUsed.Invoke();

            if (hypermode)
            {
                MyMover.AddMomentum(-animator.transform.forward * HypermodeHorizontalDashPower);
            }

            else
            {
                MyMover.AddMomentum(-animator.transform.forward * horizontalDashPower);
            }

            PlayerLock.PlayerDashLockAttack(true);
            PlayerLock.PlayerDashLockBlock(true);
            PlayerLock.PlayerDashLockCast(true);

            OnDashUsed.Invoke();
            PlayerLock.PlayerDashLockDash(true);
            PlayerLock.PlayerDashLockRotation(true);
            PlayerLock.PlayerDashLockMouseRotation(true);
            StartCoroutine("EndDash");
        }

        public void LeftDash()
        {
            if (PlayerLock.GetDashLock())
            {
                return;
            }

            if (!CameraDirection.enabled)
            {
                //animator.Play("Dash Forward");
                dashTime = forwardDashTime;
                MyMover.movementSpeed = 0;
                MyMover.slideGravity = 0;

                OnDashUsed.Invoke();

                if (hypermode)
                {
                    MyMover.AddMomentum(-animator.transform.right * HypermodeHorizontalDashPower);
                }

                else
                {
                    MyMover.AddMomentum(-animator.transform.right * horizontalDashPower);
                }


                PlayerLock.PlayerDashLockAttack(true);
                PlayerLock.PlayerDashLockBlock(true);
                PlayerLock.PlayerDashLockCast(true);

                OnDashUsed.Invoke();
                PlayerLock.PlayerDashLockDash(true);
                PlayerLock.PlayerDashLockRotation(true);
                PlayerLock.PlayerDashLockMouseRotation(true);
                StartCoroutine("EndDash");
                return;
            }

            animator.Play("Dash Left", currentAnimatorLayerIndex);
            //animator.SetTrigger("Dash Left");
            dashTime = horizontalDashTime;
            MyMover.movementSpeed = 0;
            MyMover.slideGravity = 0;

            OnDashUsed.Invoke();

            if (hypermode)
            {
                MyMover.AddMomentum(-animator.transform.right * HypermodeHorizontalDashPower);
            }

            else
            {
                MyMover.AddMomentum(-animator.transform.right * horizontalDashPower);
            }


            PlayerLock.PlayerDashLockAttack(true);
            PlayerLock.PlayerDashLockBlock(true);
            PlayerLock.PlayerDashLockCast(true);

            OnDashUsed.Invoke();
            PlayerLock.PlayerDashLockDash(true);
            PlayerLock.PlayerDashLockRotation(true);
            PlayerLock.PlayerDashLockMouseRotation(true);
            StartCoroutine("EndDash");
        }

        public void RightDash()
        {
            if (PlayerLock.GetDashLock())
            {
                return;
            }

            if (!CameraDirection.enabled)
            {

                animator.Play("Dash Forward");
                dashTime = forwardDashTime;
                MyMover.movementSpeed = 0;
                MyMover.slideGravity = 0;

                OnDashUsed.Invoke();

                if (hypermode)
                {
                    MyMover.AddMomentum(animator.transform.right * HypermodeHorizontalDashPower);
                }

                else
                {
                    MyMover.AddMomentum(animator.transform.right * horizontalDashPower);
                }


                PlayerLock.PlayerDashLockAttack(true);
                PlayerLock.PlayerDashLockBlock(true);
                PlayerLock.PlayerDashLockCast(true);

                OnDashUsed.Invoke();
                PlayerLock.PlayerDashLockDash(true);
                PlayerLock.PlayerDashLockRotation(true);
                PlayerLock.PlayerDashLockMouseRotation(true);
                StartCoroutine("EndDash");
                return;
            }

            animator.Play("Dash Right", currentAnimatorLayerIndex);
            //animator.SetTrigger("Dash Right");
            dashTime = horizontalDashTime;
            MyMover.movementSpeed = 0;
            MyMover.slideGravity = 0;

            OnDashUsed.Invoke();

            if (hypermode)
            {
                MyMover.AddMomentum(animator.transform.right * HypermodeHorizontalDashPower);
            }

            else
            {
                MyMover.AddMomentum(animator.transform.right * horizontalDashPower);
            }

            PlayerLock.PlayerDashLockAttack(true);
            PlayerLock.PlayerDashLockBlock(true);
            PlayerLock.PlayerDashLockCast(true);

            OnDashUsed.Invoke();
            PlayerLock.PlayerDashLockDash(true);
            PlayerLock.PlayerDashLockRotation(true);
            PlayerLock.PlayerDashLockMouseRotation(true);
            StartCoroutine("EndDash");
        }

        public IEnumerator EndDash()
        {
            yield return new WaitForSeconds(dashTime);
            // animator.SetBool("IsDashing", false);
            PlayerLock.PlayerDashLockDash(false);
            MyMover.slideGravity = cachedSlideGravity;

            if(hypermode)
            {
                MyMover.movementSpeed = HypermodeMoveSpeed;
            }

            else
            {
                MyMover.movementSpeed = cachedMoveSpeed;
            }

            PlayerLock.PlayerDashLockAttack(false);
            PlayerLock.PlayerDashLockBlock(false);
            PlayerLock.PlayerDashLockCast(false);
            PlayerLock.PlayerDashLockRotation(false);
            PlayerLock.PlayerDashLockMouseRotation(false);
        }

        public void EndDashAnimation()
        {
            StopCoroutine("EndDash");
            // animator.SetBool("IsDashing", false);
            PlayerLock.PlayerDashLockDash(false);
            MyMover.slideGravity = cachedSlideGravity;

            if (hypermode)
            {
                MyMover.movementSpeed = HypermodeMoveSpeed;
            }

            else
            {
                MyMover.movementSpeed = cachedMoveSpeed;
            }

            PlayerLock.PlayerDashLockAttack(false);
            PlayerLock.PlayerDashLockBlock(false);
            PlayerLock.PlayerDashLockCast(false);
            PlayerLock.PlayerDashLockRotation(false);
            PlayerLock.PlayerDashLockMouseRotation(false);
        }

        public void LockMove(bool setting)
        {
            if (setting)
            {
                MyMover.movementSpeed = 0;
            }

            else
            {
                MyMover.movementSpeed = cachedMoveSpeed;
            }
        }

        public void HalfMovement(bool setting)
        {
            if (!PlayerLock.GetMoveLock())
            {
                if (setting)
                    MyMover.movementSpeed = cachedMoveSpeed * 0.5f;

                else
                    MyMover.movementSpeed = cachedMoveSpeed;
            }

            else
            {
                MyMover.movementSpeed = cachedMoveSpeed;
            }
        }

        public void EnableHypermode(bool setting)
        {
            hypermode = setting;
        }
    }
}
