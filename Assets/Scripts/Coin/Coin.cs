using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Settings")]
    public int coinValue = 1;
    public float rotationSpeed = 100f;
    public float floatAmplitude = 0.25f;
    public float floatFrequency = 2f;

    private Vector3 _startPos;

    private void Start()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        // Визуальный эффект: вращение
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Визуальный эффект: парение вверх-вниз
        float newY = _startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(_startPos.x, newY, _startPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, есть ли на объекте наш скрипт PlayerRuntime
        if (other.TryGetComponent<PlayerRuntime>(out PlayerRuntime player))
        {
            // Обращаемся к методу подбора монетки
            player.CollectCoin(coinValue);
            
            // Здесь можно заспавнить эффект частиц перед удалением
            Destroy(gameObject);
        }
    }
}