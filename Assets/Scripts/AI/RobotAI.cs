using System;
using UnityEngine;
using UnityEngine.AI;

public enum RobotState { Patrol, Chase, Destroy, Return }

[RequireComponent(typeof(NavMeshAgent))]
public class RobotAI : MonoBehaviour
{
    // Eventos: el robot avisa qué le pasa y quien quiera reaccionar se suscribe
    public event Action<RobotState> OnStateChanged;
    public event Action<Vector3> OnTargetDestroyed;
    [Header("Patrulla")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    [Header("Persecución")]
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float maxChaseTime = 20f; // si no llega en este tiempo, se rinde
    [SerializeField] private float ignoreTime = 8f;    // tiempo que ignora un clon al que no pudo llegar
    [Header("Visión")]
    [SerializeField] private float viewRadius = 10f;
    [SerializeField, Range(1f, 360f)] private float viewAngle = 90f;
    [SerializeField] private float eyeHeight = 1f;
    [SerializeField] private LayerMask obstacleMask;   // SOLO la layer Walls
    [SerializeField] private float scanInterval = 0.2f;
    [Header("Destrucción")]
    [SerializeField] private float destroyDistance = 1.5f;
    [SerializeField] private float destroyDelay = 1f;

    [Header("Estado actual (solo para mirar)")]
    [SerializeField] private RobotState state;
    private const string CloneTag = "Clone";
    private NavMeshAgent agent;
    private IClonePool clonePool;
    private int patrolIndex;
    private GameObject targetClone;
    private GameObject ignoredClone;
    private float ignoreUntil;
    private float scanTimer;
    private float chaseTimer;
    private float destroyTimer;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();   
        clonePool = FindFirstObjectByType<PlayerClone>();
        if (clonePool == null)
        {
            Debug.LogWarning($"{name}: no se encontró PlayerClone. Los clones se destruirán sin pasar por la cola.");
        }
    }
    private void Start()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning($"{name}: el robot no está sobre un NavMesh. ¿Horneaste la superficie y está apoyado sobre la zona azul?");
            enabled = false;
            return;
        }
        EnterPatrol();
    }
    private void Update()
    {
        switch (state)
        {
            case RobotState.Patrol: UpdatePatrol(); break;
            case RobotState.Chase: UpdateChase(); break;
            case RobotState.Destroy: UpdateDestroy(); break;
            case RobotState.Return: UpdateReturn(); break;
        }
    }
    private void SetState(RobotState newState)
    {
        state = newState;
        OnStateChanged?.Invoke(newState);
    }
    private void EnterPatrol()
    {
        SetState(RobotState.Patrol);
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        GoToCurrentPatrolPoint();
    }
    private void UpdatePatrol()
    {
        if (TryStartChase()) return;
        if (HasPatrolPoints() && HasArrived())
        {
            NextPatrolPoint();
            GoToCurrentPatrolPoint();
        }
    }
    private void EnterChase(GameObject clone)
    {
        SetState(RobotState.Chase);
        targetClone = clone;
        chaseTimer = 0f;
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        GoTo(clone.transform.position);
    }

    private void UpdateChase()
    {
        if (targetClone == null)
        {
            EnterReturn();
            return;
        }
        chaseTimer += Time.deltaTime;
        if (chaseTimer >= maxChaseTime)
        {
            GiveUp();
            return;
        }
        if (HorizontalDistance(transform.position, targetClone.transform.position) <= destroyDistance)
        {
            EnterDestroy();
            return;
        }
        // Llegó al destino y no logró acercarse lo suficiente al clon
        if (HasArrived())
        {
            GiveUp();
        }
    }
    private void GiveUp()
    {
        ignoredClone = targetClone;
        ignoreUntil = Time.time + ignoreTime;
        targetClone = null;
        EnterReturn();
    }
    private void EnterDestroy()
    {
        SetState(RobotState.Destroy);
        destroyTimer = 0f;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }
    private void UpdateDestroy()
    {
        // Alguien borró el clon mientras esperaba: no pasa nada, vuelve a patrullar
        if (targetClone == null)
        {
            EnterReturn();
            return;
        }
        Vector3 direction = targetClone.transform.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                5f * Time.deltaTime
            );
        }
        destroyTimer += Time.deltaTime;
        if (destroyTimer >= destroyDelay)
        {
            DestroyTarget();
        }
    }

    private void DestroyTarget()
    {
        OnTargetDestroyed?.Invoke(targetClone.transform.position);
        if (clonePool != null)
        {
            clonePool.RemoveClone(targetClone); // saca el clon de la cola y lo destruye
        }
        else
        {
            Destroy(targetClone);
        }
        targetClone = null;
        EnterReturn();
    }
    private void EnterReturn()
    {
        SetState(RobotState.Return);
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        GoToCurrentPatrolPoint();
    }
    private void UpdateReturn()
    {
        if (TryStartChase()) return;

        if (HasArrived())
        {
            if (HasPatrolPoints()) NextPatrolPoint();
            EnterPatrol();
        }
    }
    private bool TryStartChase()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer > 0f) return false;
        scanTimer = scanInterval;
        GameObject clone = FindNearestVisibleClone();
        if (clone == null) return false;
        EnterChase(clone);
        return true;
    }

    private GameObject FindNearestVisibleClone()
    {
        Vector3 eye = transform.position + Vector3.up * eyeHeight;
        Collider[] hits = Physics.OverlapSphere(transform.position, viewRadius, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        GameObject best = null;
        float bestSqrDistance = float.MaxValue;
        foreach (Collider hit in hits)
        {
            // Solo clones: el jugador real tiene tag Player, así que nunca entra acá
            if (!hit.CompareTag(CloneTag)) continue;
            if (hit.gameObject == ignoredClone && Time.time < ignoreUntil) continue;
            Vector3 toTarget = hit.bounds.center - eye;
            // Ángulo de visión (en el plano horizontal)
            Vector3 flat = new Vector3(toTarget.x, 0f, toTarget.z);
            if (flat.sqrMagnitude > 0.0001f && Vector3.Angle(transform.forward, flat) > viewAngle * 0.5f) continue;
            // Raycast: si hay una pared en el medio, no lo ve
            if (Physics.Raycast(eye, toTarget.normalized, toTarget.magnitude, obstacleMask, QueryTriggerInteraction.Ignore)) continue;
            float sqrDistance = toTarget.sqrMagnitude;
            if (sqrDistance < bestSqrDistance)
            {
                bestSqrDistance = sqrDistance;
                best = hit.gameObject;
            }
        }
        return best;
    }

    private bool HasPatrolPoints()
    {
        return patrolPoints != null && patrolPoints.Length > 0;
    }
    private void NextPatrolPoint()
    {
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }
    private void GoToCurrentPatrolPoint()
    {
        if (!HasPatrolPoints()) return;
        Transform point = patrolPoints[patrolIndex];
        if (point != null)
        {
            GoTo(point.position);
        }
    }
    private void GoTo(Vector3 position)
    {
        // Ajusta el destino al punto más cercano del NavMesh
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.SetDestination(position);
        }
    }
    private bool HasArrived()
    {
        if (agent.pathPending) return false;
        return agent.remainingDistance <= agent.stoppingDistance + 0.1f;
    }
    private float HorizontalDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }
    // dibuja el cono de vision cuando seleccionás el robot en la Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        Vector3 left = Quaternion.Euler(0f, -viewAngle * 0.5f, 0f) * transform.forward;
        Vector3 right = Quaternion.Euler(0f, viewAngle * 0.5f, 0f) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + left * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + right * viewRadius);
    }
}