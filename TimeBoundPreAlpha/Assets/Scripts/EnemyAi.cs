using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyAi : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private Transform player;

    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    [SerializeField] private float health;

    //��������������
    private Vector3 _walkPoint;
    bool _walkPointSet;
    [SerializeField] private float _walkPointRange;

    //�����
    [SerializeField] private float timeBetweenAttacks;
    bool alreadyAttacked;
    [SerializeField] private GameObject projectile;

    //������
    [SerializeField] private float sightRange, attackRange;
    private bool _playerInSightRange, _playerInAttackRange;

    //���� - �������
    private List<GameObject> _activeProjectiles = new List<GameObject>();
    private int _maxProjectiles = 5;

    public AudioClip throwSound; // ���� ������
    public AudioClip hitSound;   // ���� ����� �� �����
    private AudioSource audioSource; // ��������� AudioSource

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();  // agent

        // ��������� AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = 1f; // ���������������� 3D ����
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 10f;
        audioSource.loop = false;
    }

    private void Update()
    {
        //������� ������ � �����
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
        //������ - ���
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
        // ��������� ��������
        agent.SetDestination(transform.position);

        // ������ - �� ������
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // �������
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        // �����
        if (!alreadyAttacked)
        {
            // ����������� ���� ������
            if (throwSound != null)
            {
                audioSource.PlayOneShot(throwSound);
            }

            // �������
            Rigidbody rb = Instantiate(projectile, transform.position + directionToPlayer, Quaternion.identity).GetComponent<Rigidbody>();

            // �������� �������
            rb.velocity = directionToPlayer * 32f;

            // ���������� ������� � ���� 
            _activeProjectiles.Add(rb.gameObject);

            // ������� ����� ������ ������
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
        TakeDamage(2); // ��������� �������� �� 2 ��� ������ �� �����.
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
