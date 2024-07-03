using UnityEngine;

public class Item : MonoBehaviour {
    // Public variables
    public bool isPickedUp = false;

    // Private variables
    private Collider itemCollider;
    private Vector3 defaultSize = Vector3.one;

    // Public methods
    public Vector3 GetDimensions() {
        return itemCollider ? itemCollider.bounds.size : defaultSize;
    }

    // Built-in methods
    private void Start() {
        InitializeItem();
    }

    // Private custom methods
    private void InitializeItem() {
        itemCollider = GetComponent<Collider>();
    }
}
