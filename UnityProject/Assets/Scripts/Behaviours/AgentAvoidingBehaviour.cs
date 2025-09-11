using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentAvoidingBehaviour : MonoBehaviour {
    
    [Header("General Settings")]
    public float repulsionRadius = 1.2f;     // How far to check for nearby agents
    public float repulsionStrength = 2.5f;   // How strongly agents push away from each other
    public bool enableDynamicObstacle = true;

    [Header("Stationary Detection")]
    public float idleThreshold = 0.1f;       // If agent speed < this, it counts as idle
    public float idleTimeToObstacle = 1.0f;  // After this time, we enable carving obstacle

    public NavMeshAgent agent;
    public NavMeshObstacle obstacle;
    private float idleTimer;
    private Vector3 lastPosition;

    private static readonly List<AgentAvoidingBehaviour> AllAgents = new ();

    void Awake() {
        // Set better defaults for smoother avoidance
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(30, 70); // Randomize to reduce deadlocks

        AllAgents.Add(this);
        lastPosition = transform.position;
    }

    void OnDestroy() { AllAgents.Remove(this); }

    void Update() {
        //Disabled due to not having that much agents
        //HandleStationaryObstacle();
        ApplyRepulsionForce();
    }

    private void HandleStationaryObstacle() {
        
        if (!enableDynamicObstacle) return;

        float movedDistance = Vector3.Distance(transform.position, lastPosition);

        // If the agent is idle for too long, turn it into a NavMeshObstacle to avoid blocking others.
        if (movedDistance < idleThreshold) {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTimeToObstacle && agent.enabled) {
                agent.enabled = false;
                obstacle.enabled = true;
            }
        //Else, ensure its not an obstacle anymore
        } else {
            idleTimer = 0f;
            if (!agent.enabled) {
                obstacle.enabled = false;
                agent.enabled = true;
            }
        }

        lastPosition = transform.position;
    }
    
    private void ApplyRepulsionForce() {
        if (!agent.enabled || !agent.isOnNavMesh) return;

        // Applies a soft repulsion force to keep agents from overlapping
        Vector3 repulsion = Vector3.zero;
        int count = 0;

        foreach (var other in AllAgents) {
            if (other == this || !other.agent.enabled) continue;

            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < repulsionRadius && dist > 0.001f) {
                Vector3 away = (transform.position - other.transform.position).normalized;
                repulsion += away / dist;
                count++;
            }
        }

        if (count > 0) {
            repulsion /= count;
            Vector3 newVelocity = agent.velocity + repulsion * repulsionStrength;
            agent.velocity = Vector3.Lerp(agent.velocity, newVelocity, Time.deltaTime * 5f);
        }
    }
}