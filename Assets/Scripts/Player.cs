using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Highlighters
{

    public class Player : MonoBehaviour
    {
        public float baseSpeed = 3f, baseDashSpeed = 10f, baseSprintSpeed = 6f, baseGravityScale = 1f, baseDashDuration = 0.2f, baseJumpForce = 6f, interactRadius = 2f, viewAngle = 180f;
        public bool cannotMove = false;
        public CharacterController controller;
        public Transform groundCheck;
        public LayerMask whatIsGround;

        private float smoothRotationVelocity, gravity, normalSpeed, heldItemCount, pressTime, currentSpeed;
        private bool isDashing, isGrounded, lockAutoInteractableSelection = false;
        private Vector3 moveDirection, velocity;
        private ItemPickup itemPickup;
        private List<GameObject> interactableObjects;
        private GameObject currentInteractableObject;
        private Animator animator;
        private CharacterController characterController;
        private const float groundCheckRadius = 0.1f, obstacleCheckDistance = 1.5f;
        private Transform originalParent;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            gravity = Physics.gravity.y;
            currentSpeed = 0;
            normalSpeed = baseSpeed;
            itemPickup = GetComponent<ItemPickup>();
            pressTime = 0f;
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            StartCoroutine(ChangeIdleVariation());
        }

        private void Update()
        {
            heldItemCount = (int)itemPickup?.heldItems.Count;
            animator.SetInteger("Held Items", (int)heldItemCount);
            HandleInput();
            HandleMovement();
            GetInteractableObjects();
            HandleInteractionHighlight();
            ApplyGravity();

        }

        private void FixedUpdate()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, whatIsGround);
            if (isGrounded) animator.SetBool("Jumping", false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("coli enter");
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                originalParent = transform.parent;
                transform.SetParent(collision.transform);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            Debug.Log("coli exit");
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                transform.SetParent(originalParent);
            }
        }

        private void HandleMovement()
        {
            Vector3 direction = GetInputDirection();
            animator.SetFloat("Speed", 0);
            if (cannotMove) return;

            if (direction.magnitude >= 0.1f)
            {
                animator.SetFloat("Speed", 0.5f);
                lockAutoInteractableSelection = false;
                RotatePlayer(direction);
                MovePlayer(direction);
                return;
            }
        }

        private Vector3 GetInputDirection()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            return new Vector3(moveX, 0f, moveZ).normalized;
        }

        private void RotatePlayer(Vector3 direction)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothRotationVelocity, 0.1f);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        private void MovePlayer(Vector3 direction)
        {
            moveDirection = Quaternion.Euler(0f, Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg, 0f) * Vector3.forward;
            float currentSpeed = isDashing ? baseDashSpeed : normalSpeed;
            controller.Move(currentSpeed * Time.deltaTime * moveDirection);
        }

        private void HandleInput()
        {
            if (Input.GetButtonDown("Run") && isGrounded && !isDashing) normalSpeed = baseSprintSpeed;

            if (Input.GetButtonUp("Run") && isGrounded && !isDashing) normalSpeed = baseSpeed - (heldItemCount * (baseSpeed / 4));

            if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && isGrounded && heldItemCount == 0) StartCoroutine(Dash());

            if (Input.GetButtonDown("Jump") && !isDashing && isGrounded) Jump();

            if (Input.GetKeyUp(KeyCode.Z)) InteractPositive();

            if (Input.GetKeyDown(KeyCode.X)) pressTime = Time.time;

            if (Input.GetKeyUp(KeyCode.X)) InteractNegative();

            if (Input.GetKeyUp(KeyCode.Tab)) SelectCurrentInteractableObject();
        }

        private void SelectCurrentInteractableObject()
        {
            lockAutoInteractableSelection = true;
            if (interactableObjects.Count == 0) return;

            int currentIndex = interactableObjects.IndexOf(currentInteractableObject);
            if (currentIndex == -1 || currentIndex == interactableObjects.Count - 1)
            {
                currentInteractableObject = interactableObjects[0];
                return;
            }

            currentInteractableObject = interactableObjects[currentIndex + 1];
        }


        private class ColliderScore
        {
            public Collider Collider { get; set; }
            public float Score { get; set; }
        }

        private void HandleInteractionHighlight()
        {
            HighlighterTrigger[] highlighterTriggers = FindObjectsOfType<HighlighterTrigger>();
            foreach (HighlighterTrigger anyHighlighterTrigger in highlighterTriggers)
            {
                anyHighlighterTrigger.ChangeTriggeringState(false);
            }

            HighlighterTrigger highlighterTrigger = GetComponentInChildren<HighlighterTrigger>();
            Highlighter highlighter = GetComponentInChildren<Highlighter>();

            if (!highlighterTrigger || !highlighter) return;

            if (!currentInteractableObject)
            {
                highlighterTrigger.ChangeTriggeringState(false);
                return;
            }

            highlighter.Renderers.Clear();

            Renderer currentRenderer = currentInteractableObject.GetComponent<Renderer>();
            if (!currentRenderer) return;
            HighlighterRenderer highlighterRenderer = new(currentRenderer, 1);
            highlighter.Renderers.Add(highlighterRenderer);
            highlighterTrigger.ChangeTriggeringState(true);

            highlighter.HighlighterValidate();
        }

        private void GetInteractableObjects()
        {
            if (lockAutoInteractableSelection) return;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRadius);
            List<ColliderScore> validColliderScores = new();

            Vector3 playerForward = transform.forward;

            foreach (var hitCollider in hitColliders)
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                Vector3 directionToCollider = (hitCollider.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(playerForward, directionToCollider);
                if (angle > viewAngle * 0.5f) continue;
                float score = distance * 0.1f + angle * 0.3f;
                if (
                hitCollider.GetComponent<Storage>() ||
                hitCollider.GetComponent<Transformer>() ||
                hitCollider.GetComponent<Item>() ||
                hitCollider.GetComponent<Resource>()
                ) validColliderScores.Add(new ColliderScore { Collider = hitCollider, Score = score });
            }

            validColliderScores = validColliderScores.OrderBy(cs => cs.Score).ToList();
            interactableObjects = validColliderScores.Select(cs => cs.Collider.gameObject).ToList();
            if (interactableObjects.Count == 0) return;
            currentInteractableObject = interactableObjects[0];
        }


        private void InteractPositive()
        {
            if (!currentInteractableObject) return;

            Item heldItem = null;
            Item item = currentInteractableObject.GetComponent<Item>();
            Resource resource = currentInteractableObject.GetComponent<Resource>();

            if (itemPickup.heldItems.Count > 0)
            {
                heldItem = itemPickup.heldItems[0].GetComponent<Item>();
            }

            if (item && !item.isPickedUp)
            {
                itemPickup.PickUpItem(item);
                return;
            }

            if (resource)
            {
                StartCoroutine(PerformInteractionWithDelay("Mining", () => resource.GenerateResourceItem(heldItem), heldItem, resource.gameObject));
                return;
            }
        }

        private IEnumerator PerformInteractionWithDelay(String animation, Func<IEnumerator> action, Item ItemTool, GameObject target)
        {
            cannotMove = true;
            RotateTowards(target.transform.position);
            itemPickup.AttachItemToolToHand(ItemTool);
            animator.SetBool(animation, true);
            yield return StartCoroutine(action());
            animator.SetBool(animation, false);
            itemPickup.AttachItemToHoldPoint(ItemTool, new Vector3(0, 0, 0));
            cannotMove = false;
        }

        private void InteractNegative()
        {
            Item heldItem = null;
            if (itemPickup.heldItems.Count > 0)
            {
                heldItem = itemPickup.heldItems[0].GetComponent<Item>();
            }
            if (Time.time - pressTime > 1f)
            {
                itemPickup.ThrowItem();
                return;
            }

            foreach (GameObject interactableObject in interactableObjects)
            {
                Storage storage = interactableObject.GetComponent<Storage>();
                Transformer transformer = interactableObject.GetComponent<Transformer>();
                if (storage && heldItem)
                {
                    StartCoroutine(PerformInteractionWithDelay("Storage", () => storage.StoreItem(heldItem), heldItem, storage.gameObject));
                    itemPickup.heldItems.RemoveAt(itemPickup.heldItems.Count - 1);
                    return;
                }
                if (transformer && heldItem)
                {
                    StartCoroutine(transformer.TransformItem(heldItem));
                    itemPickup.heldItems.RemoveAt(itemPickup.heldItems.Count - 1);
                    return;
                }
            }

            itemPickup.DropItem();
        }

        private void ApplyGravity()
        {
            if (!isGrounded)
            {
                animator.SetBool("Jumping", true);
                velocity.y += gravity * baseGravityScale * Time.deltaTime;
            }

            controller.Move(velocity * Time.deltaTime);
        }

        private void Jump()
        {
            if (heldItemCount < 3)
            {
                velocity.y = baseJumpForce - (heldItemCount * 1.5f);

            }
        }

        private IEnumerator Dash()
        {
            isDashing = true;
            normalSpeed = baseDashSpeed;

            float dashTime = baseDashDuration - heldItemCount * 0.05f;

            while (dashTime > 0)
            {
                animator.SetFloat("Speed", 1f);
                dashTime -= Time.deltaTime;
                CheckCollisionDuringDash();
                yield return null;
            }

            isDashing = false;
            normalSpeed = baseSpeed - (heldItemCount * (baseSpeed / 4));
        }

        private void CheckCollisionDuringDash()
        {
            if (Physics.Raycast(transform.position, moveDirection, out RaycastHit hit, obstacleCheckDistance))
            {
                if (hit.collider.CompareTag("Obstacle"))
                {
                    GetComponent<ItemPickup>().DropAllItems();
                }
            }
        }

        private IEnumerator ChangeIdleVariation()
        {
            while (true)
            {
                int randomIdle = UnityEngine.Random.Range(0, 2);
                animator.SetInteger("IdleVariation", randomIdle);
                yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 5f)); // Change every 5 to 10 seconds
            }
        }

        private void RotateTowards(Vector3 target)
        {
            Vector3 direction = (target - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 0.2f);
        }

    }
}