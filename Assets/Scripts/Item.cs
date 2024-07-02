using UnityEngine;

public class Item : MonoBehaviour {
    public bool isPickedUp;
    private Vector3 originalPosition;
    private Collider itemCollider;   

    void Start() {
        isPickedUp = false;
        originalPosition = transform.position;
        itemCollider = GetComponent<Collider>();
    }

    public Vector3 GetDimensions() {
        if (!itemCollider) {
            return itemCollider.bounds.size;
        }

        return Vector3.one;
    }
}
