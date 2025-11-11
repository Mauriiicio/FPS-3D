using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Alvo")]
    [Tooltip("Se vazio, o inimigo tentará encontrar um GameObject com tag 'Player' ou usará a Camera.main como fallback.")]
    public Transform player;

    [Header("Movimentação")]
    [Tooltip("Velocidade de movimento do fantasma (unidades por segundo).")]
    public float moveSpeed = 3.5f;
    [Tooltip("Velocidade de rotação ao olhar para o jogador.")]
    public float rotationSpeed = 5f;
    [Tooltip("Distância mínima até o jogador para parar de se aproximar.")]
    public float stoppingDistance = 1.2f;

    [Header("Comportamento de voo")]
    [Tooltip("Offset vertical que o fantasma busca em relação à posição do jogador.")]
    public float heightOffset = 1.5f;
    [Tooltip("Amplitude do movimento de 'bob' (oscilações verticais).")]
    public float bobAmplitude = 0.25f;
    [Tooltip("Frequência do movimento de 'bob'.")]
    public float bobFrequency = 2.0f;

    [Header("Detecção")]
    [Tooltip("Se verdadeiro, o inimigo só persegue quando o jogador estiver dentro dessa distância.")]
    public bool requireAggroRange = true;
    [Tooltip("Distância máxima para o fantasma começar a perseguir o jogador.")]
    public float aggroRange = 20f;

    [Header("Vida")]
    [Tooltip("Vida máxima do inimigo.")]
    public float maxHealth = 50f;
    [Tooltip("Efeito opcional ao morrer.")]
    public GameObject deathEffect;

    Rigidbody rb;
    Vector3 bobOffset;

    float currentHealth;
    bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (player == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go != null)
                player = go.transform;
            else if (Camera.main != null)
                player = Camera.main.transform; 
        }

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;
        if (player == null) return;

        float sqrDistance = (player.position - transform.position).sqrMagnitude;
        if (requireAggroRange && sqrDistance > aggroRange * aggroRange)
            return; 

        Vector3 targetBase = player.position + Vector3.up * heightOffset;
        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        Vector3 targetPos = new Vector3(targetBase.x, targetBase.y + bob, targetBase.z);

        Vector3 direction = targetPos - transform.position;
        float distance = direction.magnitude;

        if (distance > 0.001f)
        {
            Vector3 moveDir = direction.normalized;
            float speedThisFrame = moveSpeed;

            if (distance <= stoppingDistance)
                speedThisFrame = Mathf.Lerp(0f, moveSpeed, distance / stoppingDistance);

            Vector3 newPos = transform.position + moveDir * speedThisFrame * Time.deltaTime;

            if (rb != null)
                rb.MovePosition(newPos);
            else
                transform.position = newPos;

            Quaternion targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(direction, Vector3.up).normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        isDead = true;

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        foreach (var col in GetComponentsInChildren<Collider>())
            col.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        enabled = false;

        Destroy(gameObject, 0.2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        if (requireAggroRange)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, aggroRange);
        }

        if (player != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, player.position + Vector3.up * heightOffset);
        }
    }
}
