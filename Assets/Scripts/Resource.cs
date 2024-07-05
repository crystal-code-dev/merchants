using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource : MonoBehaviour
{
    // Public variables
    public List<GameObject> resourceItems;
    public List<float> resourceRates;
    public int resourceCount = 5;

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

    private void Update() {
    }

    // Private custom methods
    private void InitializeItem()
    {
        itemCollider = GetComponent<Collider>();
    }

    public IEnumerator GenerateResourceItem() {
        yield return GenerateResourceItemCoroutine();
    }

    private IEnumerator GenerateResourceItemCoroutine() {
        if (resourceCount <= 0) yield break;
        resourceCount--;

        float elapsedTime = 0f;
        float waitTime = 2f;

        while (elapsedTime < waitTime) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        int randomIndex = Random.Range(0, resourceItems.Count);

        GameObject newItem = Instantiate(resourceItems[randomIndex], transform.position + Vector3.up * 2f, Quaternion.identity);

        if (!newItem.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
            rb = newItem.AddComponent<Rigidbody>();
        }

        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }
}
