using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float baseMoveSpeed,dashSpeed,dashDuration,jumpForce,gravity;
    public CharacterController controller;
    private float smoothRotationVelocity,dashTime;
    private bool isDashing;    
    private Vector3 moveDirection, velocity; 
    private ItemPickup itemPickup;
    private List<GameObject> heldItems;

    void Start() {
        baseMoveSpeed = 5f;
        dashSpeed = 20f;
        dashDuration = 0.2f;
        jumpForce = 7f;
        gravity = Physics.gravity.y;
        isDashing = false;
        heldItems = new List<GameObject>();
        controller = GetComponent<CharacterController>();
        itemPickup = GetComponent<ItemPickup>();
    }

    void Update() {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;

        if (direction.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothRotationVelocity, 0.1f);
            int heldItemCount = itemPickup.heldItems.Count;
            float currentMoveSpeed = baseMoveSpeed - (heldItemCount * (baseMoveSpeed / 4));
            
            transform.rotation = Quaternion.Euler(0f, angle, 0f);      
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && heldItemCount < 3) {
                StartCoroutine(Dash(heldItemCount));
            }        

            if (isDashing) {
                currentMoveSpeed = dashSpeed - (heldItemCount * (baseMoveSpeed / 4));
            }

            controller.Move(moveDirection * currentMoveSpeed * Time.deltaTime);
        }

        if (controller.isGrounded) {
            velocity.y = -2f; 

            if (Input.GetButtonDown("Jump")) {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
        } else {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    IEnumerator Dash(int heldItemCount) {
        isDashing = true;
        dashTime = dashDuration - heldItemCount * 0.05f;

        while (dashTime > 0) {
            dashTime -= Time.deltaTime;
            CheckCollisionDuringDash();
            yield return null;
        }

        isDashing = false;
    }
    void CheckCollisionDuringDash() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, moveDirection, out hit, 1.5f)) {
            if (hit.collider.CompareTag("Obstacle")) {
                itemPickup.DropAllItem();
            }
        }
    }
}
