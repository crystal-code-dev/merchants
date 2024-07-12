using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Emote : MonoBehaviour
{
  public Image emoteImage; // Referência para a UI Image que exibirá o emote
  public Sprite emote1; // Frames do primeiro emote
  private Coroutine currentEmoteCoroutine;
  private Transform cameraTransform;

  void Start()
  {
    cameraTransform = Camera.main.transform;
    emoteImage.gameObject.SetActive(false);
    ShowEmote(1);
  }

  private void LateUpdate()
  {
    transform.rotation = cameraTransform.rotation;
  }

  public void ShowEmote(int emoteIndex)
  {
    if (currentEmoteCoroutine != null)
    {
      StopCoroutine(currentEmoteCoroutine);
    }

    StartCoroutine(PlayEmote(emote1));
  }

  public void HideEmote()
  {
    if (currentEmoteCoroutine != null)
    {
      StopCoroutine(currentEmoteCoroutine);
    }

    emoteImage.gameObject.SetActive(false);
  }

  private IEnumerator PlayEmote(Sprite sprite)
  {
    emoteImage.gameObject.SetActive(true);
    int frameIndex = 0;
    while (true)
    {
      emoteImage.sprite = sprite;
      yield return new WaitForSeconds(0.1f); // Tempo entre os frames
    }
  }
}
