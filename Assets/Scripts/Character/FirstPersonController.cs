using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MovementController
    {
        [SerializeField]
        private MouseLook m_MouseLook;
        [SerializeField]
        private bool m_UseFovKick;
        [SerializeField]
        private FOVKick m_FovKick = new FOVKick();
        [SerializeField]
        private bool m_UseHeadBob;
        [SerializeField]
        private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField]
        private LerpControlledBob m_JumpBob = new LerpControlledBob();

        private Camera m_Camera;
        private Vector3 m_OriginalCameraPosition;

        protected override void Start()
        {
            base.Start();

            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, stepInterval);
            m_MouseLook.Init(transform, m_Camera.transform);
        }

        protected override void Update()
        {
            RotateView();

            // the jump state needs to read here to make sure it is not missed
            if (!isJumping)
            {
                isJumping = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            base.Update();
        }

        protected override void OnGrounded()
        {
            StartCoroutine(m_JumpBob.DoBobCycle());
            base.OnGrounded();
        }

        protected override void FixedUpdate()
        {
            GetInput();

            base.FixedUpdate();

            UpdateCameraPosition();
            m_MouseLook.UpdateCursorLock();
        }

        private void UpdateCameraPosition()
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (characterController.velocity.magnitude > 0 && characterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(characterController.velocity.magnitude +
                                      (m_Speed * (isWalking ? 1f : runstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput()
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = isWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            isWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            m_Speed = isWalking ? walkSpeed : runSpeed;

            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (isWalking != waswalking && m_UseFovKick && characterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!isWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }

        private void RotateView()
        {
            m_MouseLook.LookRotation(transform, m_Camera.transform);
        }
    }
}
