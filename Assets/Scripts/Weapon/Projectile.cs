using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _damage;
    private float _speed;

    public void Init(float damage, float speed)
    {
        _damage = damage;
        _speed = speed;
        // Уничтожаем пулю через 3 секунды, чтобы не засорять память
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyController>(out var enemy))
        {
            enemy.TakeDamage(_damage);
            Debug.Log("Hit enemy");
            Destroy(gameObject);
        }
    }
}