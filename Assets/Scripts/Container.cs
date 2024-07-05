using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class Container : MonoBehaviour {
    // Public variables

    public Transform holdPoint;
    public List<GameObject> storedItems = new List<GameObject>();
    public int maxStoredItems = 3;
    private void Start() {
        
    }

    public IEnumerator StoreItem(Item item) {
        Debug.Log("store2");
        float elapsedTime = 0f;
        float waitTime = 1f;



        while (elapsedTime < waitTime) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        AttachItemToHoldPoint(item);
        storedItems.Add(item.gameObject);
        yield return null;
    }

    private void AttachItemToHoldPoint(Item item) {
        item.transform.SetParent(holdPoint);
        item.transform.SetLocalPositionAndRotation(CalculateItemPosition(), Quaternion.identity);
        item.isPickedUp = true;

        SetItemKinematic(item, true);
    }

    private void SetItemKinematic(Item item, bool isKinematic) {
        Rigidbody rigidbody = item.GetComponent<Rigidbody>();
        
        if (rigidbody) {
            rigidbody.isKinematic = isKinematic;
        }
    }

    private Vector3 CalculateItemPosition() {
        float currentStackHeight = 0f;

        foreach (var heldItem in storedItems) {
            Item item = heldItem.GetComponent<Item>();
            currentStackHeight += item.GetDimensions().y;
        }

        return new Vector3(0, currentStackHeight, 0);
    }


}
