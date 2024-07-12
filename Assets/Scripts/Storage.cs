using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class Storage : Container {
    public List<GameObject> storedItems = new List<GameObject>();
    public int maxStoredItems = 3;
    private void Start() {}

    public IEnumerator StoreItem(Item item) {
        float elapsedTime = 0f;
        float waitTime = 1f;

        while (elapsedTime < waitTime) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        AttachItemToHoldPoint(item, storedItems);
        storedItems.Add(item.gameObject);
        yield return null;
    }
}
