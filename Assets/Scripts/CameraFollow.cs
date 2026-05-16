using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // Ссылка на игрока
    [SerializeField] private Vector3 offset;    // Смещение, которое ты уже настроил
    [SerializeField] private float smoothSpeed = 0.125f;

    private void LateUpdate()
    {
        if (target == null) return;

        // Желаемая позиция с учетом смещения
        Vector3 desiredPosition = target.position + offset;
        
        // Плавный переход от текущей позиции к желаемой
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        transform.position = smoothedPosition;
    }
}