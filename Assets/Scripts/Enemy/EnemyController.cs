using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    public EnemyData data;
    
    [Header("Components")]
    [SerializeField] private Animator _animator; // Ссылка на аниматор

    private Transform _player;
    private NavMeshAgent _agent;
    private float _currentHealth;
    private float _nextAttackTime;
    private bool _isDead = false; // Флаг смерти, чтобы труп не атаковал

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = data.moveSpeed;
        _currentHealth = data.maxHealth;
        
        // Находим игрока
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;

        // Если забыли перетянуть аниматор в инспекторе, код найдет его сам
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Если игрок не найден или враг мертв — ничего не делаем
        if (_player == null || _isDead) return;

        float distance = Vector3.Distance(transform.position, _player.position);

        // --- ЛОГИКА АНИМАЦИИ ХОДЬБЫ ---
        // Если агент не остановлен и имеет скорость > 0.1, значит он идет
        bool isMoving = !_agent.isStopped && _agent.velocity.sqrMagnitude > 0.1f;
        _animator.SetBool("isWalking", isMoving);

        // 1. Преследование
        if (distance <= data.detectionRange && distance > data.attackRange)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_player.position);
        }
        // 2. Атака
        else if (distance <= data.attackRange)
        {
            _agent.isStopped = true;
            
            // Плавный поворот к игроку во время остановки/атаки
            Vector3 direction = (_player.position - transform.position).normalized;
            direction.y = 0; // Игнорируем разницу по высоте, чтобы враг не "клевал носом"
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

            TryAttack();
        }
        else
        {
            _agent.isStopped = true;
        }
    }

    private void TryAttack()
    {
        if (Time.time >= _nextAttackTime)
        {
            // ЗАПУСКАЕМ АНИМАЦИЮ АТАКИ
            _animator.SetTrigger("Attack");

            if (_player.TryGetComponent<PlayerRuntime>(out var player))
            {
                player.ApplyDamage(data.attackDamage); 
            }
            _nextAttackTime = Time.time + data.attackCooldown;
        }
    }

    public void TakeDamage(float amount)
    {
        if (_isDead) return; // Защита от получения урона после смерти

        _currentHealth -= amount;
        Debug.Log($"{data.enemyName} получил урон. ХП: {_currentHealth}");

        if (_currentHealth <= 0) Die();
    }

    private void Die()
    {
        _isDead = true;
        _agent.isStopped = true;
        _agent.enabled = false; // Отключаем NavMesh, чтобы труп не скользил
        
        // Отключаем коллайдер, чтобы игрок и другие враги не спотыкались о труп
        if (TryGetComponent<Collider>(out var col)) col.enabled = false; 

        // ЗАПУСКАЕМ АНИМАЦИЮ СМЕРТИ
        _animator.SetTrigger("Die");

        // Здесь можно вызвать спавн лута, как ты делал на боссе
        // LootSpawner.SpawnLoot();

        // Удаляем объект с задержкой в 3 секунды, чтобы игрок успел увидеть анимацию смерти
        Destroy(gameObject, 3f);
    }
}