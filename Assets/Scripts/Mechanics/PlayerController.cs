using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        public float maxSpeed = 7;
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        public Collider2D collider2d;
        public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        public float fireRate = .5f;
        private bool canFire;
        private float fireTime = 0;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public GameObject muzzleFlash;
        public Bullet bulletPrefab;

        public Bounds Bounds => collider2d.bounds;
        private PoolBullets poolBullets;

        public int playerDir = 1;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            poolBullets = PoolBullets.instance;
        }

        protected override void Update()
        {
            if (controlEnabled)
            {

                if (fireTime <= Time.time)
                    canFire = true;

                move.x = Input.GetAxisRaw("Horizontal");
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }

                if (Input.GetKey(KeyCode.J) && canFire)
                {
                    StartCoroutine(Fire());
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        IEnumerator Fire()
        {
            canFire = false;
            fireTime = Time.time + fireRate;
            muzzleFlash.SetActive(true);
            if (poolBullets.canPool)
            {
                Bullet newBullet = Instantiate(bulletPrefab, muzzleFlash.transform.position, Quaternion.identity, poolBullets.transform);
                newBullet.dir = playerDir;
                poolBullets.AddPool();
            }
            else
            {
                Bullet newBullet = poolBullets.GetBullet();
                newBullet.dir = playerDir;
                newBullet.transform.SetPositionAndRotation(muzzleFlash.transform.position, Quaternion.identity);
                newBullet.gameObject.SetActive(true);
            }
            yield return new WaitForEndOfFrame();
            muzzleFlash.SetActive(false);
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpModifier;
                }
            }

            if (move.x > 0.01f)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                playerDir = 1;
            }
            else if (move.x < -0.01f)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
                playerDir = -1;
            }

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}