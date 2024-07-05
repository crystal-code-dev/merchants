using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {
    // Public variables
    public Transform holdPoint;
    public List<GameObject> heldItems = new List<GameObject>();
    public int maxItems = 3;

    // Built-in methods
    private void Start() {
        Debug.Log("Max items definido para: " + maxItems);
    }

    private void Update() {
  
    }

    // Public methods
    public void DropAllItems() {
        while (heldItems.Count > 0) {
            DropItem();
        }
    }

    public void PickUpItem(Item item) {
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

    public void DropItem() {
        if (heldItems.Count > 0) {
            GameObject itemObject = heldItems[^1];
            Item item = itemObject.GetComponent<Item>();

            DetachItem(item);
            
            heldItems.RemoveAt(heldItems.Count - 1);
        }
    }

    public void ThrowItem() {
        if (heldItems.Count > 0)
        {
            GameObject itemObject = heldItems[^1];
            Item item = itemObject.GetComponent<Item>();

            // Detach the item from the player (if needed)
            DetachItem(item);
            heldItems.RemoveAt(heldItems.Count - 1);

            // Add Rigidbody component if not already present
            Rigidbody rb = itemObject.GetComponent<Rigidbody>();
            if (!rb)
            {
                rb = itemObject.AddComponent<Rigidbody>();
            }

            // Calculate throw direction based on player's forward direction
            Vector3 throwDirection = transform.forward;

            // Apply throw force
            rb.AddForce(throwDirection * 10f, ForceMode.Impulse);
        }
    }


    private void DetachItem(Item item) {
        item.transform.SetParent(null);
        item.isPickedUp = false;

        SetItemKinematic(item, false);
    }


}
