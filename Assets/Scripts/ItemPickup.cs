using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Transform holdPoint;
    public List<GameObject> heldItems;
    public int maxItems; 
    void Start() {
        heldItems = new List<GameObject>();  
        maxItems = 3;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            TryPickUpItem();
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            DropItem();
        }
    }

    void TryPickUpItem() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f);
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.CompareTag("PickupItem")) {
                Item item = hitCollider.GetComponent<Item>();
                if (item != null && !item.isPickedUp) {
                    PickUpItem(item);
                    return;
                }
            }
        }
    }

    void PickUpItem(Item item) {
        if (heldItems.Count < maxItems) {         
            item.transform.SetParent(holdPoint);
            float currentStackHeight = 0f;
            foreach (var heldItem in heldItems) {
               Item heldItemItem = heldItem.GetComponent<Item>();
              float itemHeight = heldItemItem.GetDimensions().y;
                currentStackHeight += itemHeight;
            } 
             
            item.transform.localPosition = new Vector3(0, currentStackHeight, 0);
            
            item.transform.localRotation = Quaternion.identity;
            item.isPickedUp = true;
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = true;
            }
            heldItems.Add(item.gameObject);
          
            Debug.Log("Item pego: " + item.name);
        }
    }
    void DropItem() {
        if (heldItems.Count > 0) {
            GameObject itemObject = heldItems[heldItems.Count - 1];
            Item item = itemObject.GetComponent<Item>();
            item.transform.SetParent(null);
            item.isPickedUp = false;
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = false;
            }
            heldItems.RemoveAt(heldItems.Count - 1);
            Debug.Log("Item solto: " + item.name);
        }
    }

    public void DropAllItem() {
      for (var i = 0; i < heldItems.Count; i++) {
        DropItem();
      }   
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
