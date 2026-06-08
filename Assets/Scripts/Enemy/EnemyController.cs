using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    public LootSpawner lootSpawner;
    public EnemyData data;
    
    [Header("Components")]
    [SerializeField] private Animator _animator;
    private Transform _player;
    private NavMeshAgent _agent;
    private float _currentHealth;
    private float _nextAttackTime;
    private bool _isDead = false;
    private PlayerRuntime _playerRuntime;
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = data.moveSpeed;
        _currentHealth = data.maxHealth;
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
        _playerRuntime = playerObj.GetComponent<PlayerRuntime>(); 
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (_player == null || _isDead) return;

        float distance = Vector3.Distance(transform.position, _player.position);
        bool isMoving = !_agent.isStopped && _agent.velocity.sqrMagnitude > 0.1f;
        _animator.SetBool("isWalking", isMoving);
        if (distance <= data.detectionRange && distance > data.attackRange)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_player.position);
        }
        else if (distance <= data.attackRange)
        {
            _agent.isStopped = true;
            Vector3 direction = (_player.position - transform.position).normalized;
            direction.y = 0;
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
        if (_isDead) return;

        _currentHealth -= amount;
        Debug.Log($"{data.enemyName} получил урон. ХП: {_currentHealth}");

        if (_currentHealth <= 0) Die();
    }

    private void Die()
    {

        _isDead = true;
        _agent.isStopped = true;
        _agent.enabled = false;
        if (TryGetComponent<Collider>(out var col)) col.enabled = false; 
        _animator.SetTrigger("Die");
        if (_playerRuntime != null && data.coinsReward > 0)
        {
            _playerRuntime.CollectCoin(data.coinsReward);
            Debug.Log($"Получено {data.coinsReward} монет с {data.enemyName}");
        }

        Destroy(gameObject, 3f);
        if (data.isBoss) lootSpawner.SpawnLoot();
    }
}