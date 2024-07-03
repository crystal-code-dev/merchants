using UnityEngine;

public class HorseIA : MonoBehaviour {
    // Public variables
    public float speed = 25f, maxDistance = 100f;

    // Private variables
    private Vector3 target;
    private Animator animator;

    // Built-in methods
    private void Start() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        animator.SetInteger("animation", 2);

        Vector3 moveDirection = (target - transform.position).normalized;

        if (moveDirection != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
