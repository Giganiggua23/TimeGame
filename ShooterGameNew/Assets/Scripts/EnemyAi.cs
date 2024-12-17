using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyAi : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private Transform player;

    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    [SerializeField] private float health;

    //Патрулирование
    private Vector3 _walkPoint;
    bool _walkPointSet;
    [SerializeField] private float _walkPointRange;

    //Атака
    [SerializeField] private float timeBetweenAttacks;
    bool alreadyAttacked;
    [SerializeField] private GameObject projectile;

    //Статус
    [SerializeField] private float sightRange, attackRange;
    private bool _playerInSightRange, _playerInAttackRange;

    //Лист - патроны
    private List<GameObject> _activeProjectiles = new List<GameObject>();
    private int _maxProjectiles = 5;

    public AudioClip throwSound; // Звук броска
    public AudioClip hitSound;   // Звук удара по врагу
    private AudioSource audioSource; // Компонент AudioSource

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();  // agent

        // Настройка AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = 1f; // Пространственный 3D звук
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 10f;
        audioSource.loop = false;
    }

    private void Update()
    {
        //Область зрение и атака
        _playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        _playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!_playerInSightRange && !_playerInAttackRange) Patroling();
        if (_playerInSightRange && !_playerInAttackRange) ChasePlayer();
        if (_playerInAttackRange && _playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!_walkPointSet) SearchWalkPoint();

        if (_walkPointSet)
            agent.SetDestination(_walkPoint);

        Vector3 distanceToWalkPoint = transform.position - _walkPoint;


        if (distanceToWalkPoint.magnitude < 1f)
            _walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Рандом - ход
        float randomZ = Random.Range(-_walkPointRange, _walkPointRange);
        float randomX = Random.Range(-_walkPointRange, _walkPointRange);

        _walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(_walkPoint, -transform.up, 2f, whatIsGround))
            _walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Остановка движения
        agent.SetDestination(transform.position);

        // Взгляд - на Игрока
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Поворот
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        // Атака
        if (!alreadyAttacked)
        {
            // Проигрываем звук броска
            if (throwSound != null)
            {
                audioSource.PlayOneShot(throwSound);
            }

            // Выстрел
            Rigidbody rb = Instantiate(projectile, transform.position + directionToPlayer, Quaternion.identity).GetComponent<Rigidbody>();

            // Скорость снаряда
            rb.velocity = directionToPlayer * 32f;

            // Добавление снаряда в Лист 
            _activeProjectiles.Add(rb.gameObject);

            // Удалить самый старый снаряд
            if (_activeProjectiles.Count > _maxProjectiles)
            {
                Destroy(_activeProjectiles[0]);
                _activeProjectiles.RemoveAt(0);
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        TakeDamage(2); // Уменьшить здоровье на 2 при щелчке по врагу.
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
