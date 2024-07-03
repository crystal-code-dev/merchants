using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {
    // Public variables
    public Transform holdPoint;
    public List<GameObject> heldItems = new List<GameObject>();
    public int maxItems = 3;
    public float pickupRadius = 2f;

    // Built-in methods
    private void Start() {
        Debug.Log("Max items definido para: " + maxItems);
    }

    private void Update() {
        HandleInput();
    }

    private void HandleInput() {
        if (Input.GetButtonDown("Fire1")) {
            TryPickUpItem();
        }

        if (Input.GetButtonDown("Fire2")) {
            DropItem();
        }
    }

    // Public methods
    public void DropAllItems() {
        while (heldItems.Count > 0) {
            DropItem();
        }
    }

    // Private custom methods
    private void TryPickUpItem() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRadius);

        foreach (var hitCollider in hitColliders) {
            bool isPickupItem = hitCollider.CompareTag("PickupItem");
            if (!isPickupItem) {
                continue;
            }

            Item item = hitCollider.GetComponent<Item>();
            if (item && !item.isPickedUp) {
                PickUpItem(item);
                return;
            }
        }
    }

    private void PickUpItem(Item item) {
        if (heldItems.Count < maxItems) {
            AttachItemToHoldPoint(item);
            heldItems.Add(item.gameObject);
        }
    }

    private void AttachItemToHoldPoint(Item item) {
        item.transform.SetParent(holdPoint);
        item.transform.SetLocalPositionAndRotation(CalculateItemPosition(), Quaternion.identity);
        item.isPickedUp = true;

        SetItemKinematic(item, true);
    }

    private Vector3 CalculateItemPosition() {
        float currentStackHeight = 0f;

        foreach (var heldItem in heldItems) {
            Item item = heldItem.GetComponent<Item>();
            currentStackHeight += item.GetDimensions().y;
        }

        return new Vector3(0, currentStackHeight, 0);
    }

    private void SetItemKinematic(Item item, bool isKinematic) {
        Rigidbody rigidbody = item.GetComponent<Rigidbody>();
        
        if (rigidbody) {
            rigidbody.isKinematic = isKinematic;
        }
    }

    private void DropItem() {
        if (heldItems.Count > 0) {
            GameObject itemObject = heldItems[^1];
            Item item = itemObject.GetComponent<Item>();

            DetachItem(item);
            
            heldItems.RemoveAt(heldItems.Count - 1);
        }
    }

    private void DetachItem(Item item) {
        item.transform.SetParent(null);
        item.isPickedUp = false;

        SetItemKinematic(item, false);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
