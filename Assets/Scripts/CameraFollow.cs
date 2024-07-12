using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // O Transform do personagem
    public Vector3 offset;   // Deslocamento da câmera em relação ao personagem
    public float smoothSpeed = 0.125f; // Velocidade de suavização

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Mantém a rotação original da câmera
        transform.LookAt(target);
    }
}
