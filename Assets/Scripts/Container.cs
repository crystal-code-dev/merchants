using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class Container : MonoBehaviour {
    // Public variables
    public Transform holdPoint;

    private void Start() {}

    public void AttachItemToHoldPoint(Item item, List<GameObject> storedItems) {
        item.transform.SetParent(holdPoint);
        item.transform.SetLocalPositionAndRotation(CalculateItemPosition(storedItems), Quaternion.identity);
        SetItemKinematic(item, true);
    }

    public void SetItemKinematic(Item item, bool isKinematic) {
        Rigidbody rigidbody = item.GetComponent<Rigidbody>();
        
        if (rigidbody) {
            rigidbody.isKinematic = isKinematic;
        }
    }

    public Vector3 CalculateItemPosition(List<GameObject> storedItems) {
        float currentStackHeight = 0f;

        foreach (var storedItem in storedItems) {
            Item item = storedItem.GetComponent<Item>();
            currentStackHeight += item.GetDimensions().y;
        }

        return new Vector3(0, currentStackHeight, 0);
    }


}
