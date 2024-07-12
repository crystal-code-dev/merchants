using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEditor;



public class Transformer : Container {
    [SerializeField]
    private List<KeyValuePair<GameObject, TransformerItemData>> transformMap = new List<KeyValuePair<GameObject, TransformerItemData>>();
    private Dictionary<GameObject, TransformerItemData> transformDictionary = new Dictionary<GameObject, TransformerItemData>();

    private void OnValidate() {
        UpdateDictionary();
    }

    private void UpdateDictionary() {
        transformDictionary.Clear();
        foreach (var kvp in transformMap)
        {
            transformDictionary[kvp.Key] = kvp.Value;
        }
    }

    private void Start() {
        // Exemplo de uso do dicionário
        foreach (var kvp in transformDictionary)
        {
            Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
        }
    }

    public IEnumerator TransformItem(Item item) {
        System.Collections.Generic.KeyValuePair<GameObject, TransformerItemData> pair = transformDictionary.FirstOrDefault(pair => pair.Key.name == item.name);

        if (pair.Key == null || pair.Value == null) yield break;

        TransformerItemData resultItemData = pair.Value;
        Destroy(item.gameObject);
        yield return new WaitForSeconds(resultItemData.time);

        float elapsedTime = 0f;
        float waitTime = 1f;

        while (elapsedTime < waitTime) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GameObject newItemGameObject = Instantiate(resultItemData.item, holdPoint.position, holdPoint.rotation);
        Item newItem = newItemGameObject.GetComponent<Item>();
        if (!newItem) yield break;
        AttachItemToHoldPoint(newItem, new List<GameObject>());
        yield return null;
    }



}
