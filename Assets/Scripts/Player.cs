using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    // Public variables
    public float baseSpeed = 3f, baseDashSpeed = 10f, baseSprintSpeed = 6f, baseGravityScale = 1f, baseDashDuration = 0.5f, baseJumpForce = 6f, interactRadius = 2f;
    public bool cannotMove = false;

    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask whatIsGround;

    // Private variables
    private float smoothRotationVelocity, gravity, normalSpeed, heldItemCount, pressTime;
    private bool isDashing, isGrounded;
    private Vector3 moveDirection, velocity;
    private ItemPickup itemPickup;
    private const float groundCheckRadius = 0.1f, obstacleCheckDistance = 1.5f;

    // Built-in methods
    private void Start() {
        controller = GetComponent<CharacterController>();
        gravity = Physics.gravity.y;
        normalSpeed = baseSpeed;
        itemPickup = GetComponent<ItemPickup>();
        pressTime = 0f;
    }

    private void Update() {        
        heldItemCount = (int)itemPickup?.heldItems.Count;
        HandleInput();
        HandleMovement();
        ApplyGravity();
    }

    private void FixedUpdate() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    // Private custom methods
    private void HandleMovement() {
        if (cannotMove) return;
        Vector3 direction = GetInputDirection();

        if (direction.magnitude >= 0.1f) {
            RotatePlayer(direction);
            MovePlayer(direction);
        }
    }

    private Vector3 GetInputDirection() {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        return new Vector3(moveX, 0f, moveZ).normalized;
    }

    private void RotatePlayer(Vector3 direction) {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothRotationVelocity, 0.1f);
        
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private void MovePlayer(Vector3 direction) {
        moveDirection = Quaternion.Euler(0f, Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg, 0f) * Vector3.forward;

        float currentSpeed = isDashing ? baseDashSpeed : normalSpeed;

        controller.Move(currentSpeed * Time.deltaTime * moveDirection);
    }

    private void HandleInput() {
        if (Input.GetButtonDown("Run") && isGrounded && !isDashing) {
            normalSpeed = baseSprintSpeed;
        } 
        
        if (Input.GetButtonUp("Run") && isGrounded && !isDashing) {
            normalSpeed = baseSpeed - (heldItemCount * (baseSpeed / 4));
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && isGrounded) {
            StartCoroutine(Dash());
        }

        if (Input.GetButtonDown("Jump") && !isDashing && isGrounded) {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            InteractPositive();
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            pressTime = Time.time;
        }

          if (Input.GetKeyUp(KeyCode.X)) {
            InteractNegative();
        }
    }

    private Collider? GetNearestInteractableObject() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRadius);
        Collider nearestCollider = null;
        float bestScore = float.MaxValue;
        Vector3 playerForward = transform.forward; // Direção para a qual o player está olhando

        foreach (var hitCollider in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
            Vector3 directionToCollider = (hitCollider.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(playerForward, directionToCollider);

            float score = distance * 0.1f + angle * 0.3f;

            if (score < bestScore)
            {
                Item item = hitCollider.GetComponent<Item>();
                if (item && !item.isPickedUp)
                {
                    nearestCollider = hitCollider;
                    bestScore = score;
                }
                else
                {
                    Resource resource = hitCollider.GetComponent<Resource>();
                    if (resource)
                    {
                        nearestCollider = hitCollider;
                        bestScore = score;
                    }
                }
            }
        }
        return nearestCollider;
    }

    private void InteractPositive() {
       
        Collider nearestCollider = GetNearestInteractableObject();
        // Interact with the nearest collider
        if (nearestCollider)
        {
            if (nearestCollider.CompareTag("PickupItem"))
            {
                Item item = nearestCollider.GetComponent<Item>();
                if (item && !item.isPickedUp)
                {
                    itemPickup.PickUpItem(item);
                }
            }
            else if (nearestCollider.CompareTag("Resource"))
            {
                Resource resource = nearestCollider.GetComponent<Resource>();
                if (resource) {
                    Item heldItem = null;
                    if (itemPickup.heldItems.Count > 0) {
                        heldItem = itemPickup.heldItems[0].GetComponent<Item>();
                    }
                    StartCoroutine(PerformInteractionWithDelay(() => resource.GenerateResourceItem(heldItem)));
                }
            }
        }
    }

    private IEnumerator PerformInteractionWithDelay(Func<IEnumerator> action) {
        cannotMove = true;
        yield return StartCoroutine(action());
        cannotMove = false;
    }

    private void InteractNegative() {
        if (Time.time - pressTime > 1f) {
            Debug.Log("throw");
            itemPickup.ThrowItem();
        } else {
            itemPickup.DropItem();
        }   

    }

    private void ApplyGravity() {
        if (!isGrounded) {
            velocity.y += gravity * baseGravityScale * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private void Jump() {
        if (heldItemCount < 3) {
            velocity.y = baseJumpForce - (heldItemCount * 1.5f);
        }
    }

    private IEnumerator Dash() {
        if (heldItemCount >= 3) {
            GetComponent<ItemPickup>().DropAllItems();
        }

        isDashing = true;
        normalSpeed = baseDashSpeed;

        float dashTime = baseDashDuration - heldItemCount * 0.05f;
        
        while (dashTime > 0) {
            dashTime -= Time.deltaTime;
            CheckCollisionDuringDash();
            yield return null;
        }

        isDashing = false;
        normalSpeed = baseSpeed - (heldItemCount * (baseSpeed / 4));
    }

    private void CheckCollisionDuringDash() {
        if (Physics.Raycast(transform.position, moveDirection, out RaycastHit hit, obstacleCheckDistance)) {
            if (hit.collider.CompareTag("Obstacle")) {
                GetComponent<ItemPickup>().DropAllItems();
            }
        }
    }
}
