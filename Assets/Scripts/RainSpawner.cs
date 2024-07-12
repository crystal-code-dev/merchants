using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using System.Linq;


public class RainSpawner : MonoBehaviour
{
  public GameObject rainObjectPrefab;
  public Collider spawnAreaCollider;
  public GameObject warningPrefab;
  public float spawnInterval = 1f;
  public float warningDelay = 2f; // Atraso antes de mostrar o aviso
  public float warningGrowSpeed = 0.5f; // Velocidade de crescimento do aviso
  public float warningScaleMax = 1.5f; // Tamanho máximo do aviso em relação ao objeto final

  private GameObject currentWarning;

  private void Start()
  {
    StartCoroutine(Rain());
  }

  private IEnumerator Rain()
  {
    while (true)
    {
      yield return new WaitForSeconds(spawnInterval);

      // Gerar aviso visual
      Vector3 spawnPoint = SpawnWarning();

      yield return new WaitForSeconds(warningDelay);

      // Gerar objeto
      GameObject rainObject = SpawnRainObject(spawnPoint);

      yield return new WaitForSeconds(3f);

      Destroy(rainObject);
    }
  }

  private Vector3 SpawnWarning()
  {
    Vector3 spawnPoint = GetRandomPointInCollider(spawnAreaCollider);
    float originalY = spawnPoint.y;
    spawnPoint.y = GetGroundHeight(spawnPoint);

    // Instancia o aviso no ponto onde a pedra vai cair
    currentWarning = Instantiate(warningPrefab, spawnPoint, Quaternion.identity);
    StartCoroutine(GrowWarning(currentWarning.transform));

    spawnPoint.y = originalY;
    return spawnPoint;
  }

  private IEnumerator GrowWarning(Transform warningTransform)
  {
    Vector3 initialScale = warningTransform.localScale;
    Vector3 targetScale = initialScale * warningScaleMax;

    float t = 0f;
    while (t < 1f)
    {
      t += Time.deltaTime * warningGrowSpeed;
      Vector3 currentScale = Vector3.Lerp(initialScale, targetScale, t);
      currentScale.y = initialScale.y; // Mantém a escala em y
      warningTransform.localScale = currentScale;
      yield return null;
    }
  }

  private GameObject SpawnRainObject(Vector3 spawnPoint)
  {
    Destroy(currentWarning); // Remove o aviso antes de gerar o objeto

    GameObject rainObject = Instantiate(rainObjectPrefab, spawnPoint, Quaternion.identity);
    Rigidbody rb = rainObject.GetComponent<Rigidbody>();
    rb.AddForce(Vector3.down * 20f, ForceMode.Impulse);
    return rainObject;
  }

  private Vector3 GetRandomPointInCollider(Collider collider)
  {
    Vector3 center = collider.bounds.center;
    Vector3 size = collider.bounds.size;

    float x = UnityEngine.Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
    float z = UnityEngine.Random.Range(center.z - size.z / 2f, center.z + size.z / 2f);

    return new Vector3(x, center.y, z);
  }


  private float GetGroundHeight(Vector3 position)
  {
    // Faz um raycast para encontrar a altura do chão abaixo da posição fornecida
    RaycastHit hit;
    if (Physics.Raycast(new Vector3(position.x, spawnAreaCollider.bounds.max.y + 1f, position.z), Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
    {
      return hit.point.y;
    }
    else
    {
      Debug.LogWarning("Ground not found below the spawn point. Using default height.");
      return position.y;
    }
  }

  private void OnDrawGizmos()
  {
    if (spawnAreaCollider != null)
    {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireCube(spawnAreaCollider.bounds.center, spawnAreaCollider.bounds.size);
    }
  }
}
