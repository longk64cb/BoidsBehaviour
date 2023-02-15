using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private List<Detector> detectors;

    private AIContext seekContext;
    private AIContext strafeContext;
    private AIContext biasContext;
    private AIContext separationContext;
    private AIContext collisionContext;
    private AIContext weights;

    [SerializeField] private bool seek, strafe, bias, separation, collision;

    private Vector3 lastTargetPosition;
    [SerializeField] private float lastTargetPositionThreshhold = 0.5f;

    [SerializeField]
    private AIData aiData;

    [SerializeField]
    private float detectionDelay = 0.05f, aiUpdateDelay = 0.06f, attackDelay = 2f, attackTimeStamp = 1f;

    [SerializeField]
    private bool isAttacking = false;

    [SerializeField]
    private float biasFactor = 0.05f;

    [SerializeField]
    private float obstacleRadius = 2f, agentColliderSize = 0.6f;

    [SerializeField]
    private float neighbourRadius = 2f;

    [SerializeField]
    private float strafeDistance = 2f;

    [SerializeField]
    private float attackDeadZone = 0.5f;

    //Inputs sent from the Enemy AI to the Enemy controller
    public UnityEvent OnPrepareAttack;
    public UnityEvent OnAttackPressed;
    public UnityEvent<Vector2, bool> OnMovementInput;
    public UnityEvent<Vector2> OnPointerInput;

    [SerializeField]
    private Vector2 movementInput = new Vector2();

    bool following = false;

    private Rigidbody2D rgBody;
    public Transform target;

    public bool canMove = true;
    public bool canAttack = true;

    private void Awake()
    {
        rgBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        seekContext = new AIContext(Directions.GetDirectionCount);
        strafeContext = new AIContext(Directions.GetDirectionCount);
        biasContext = new AIContext(Directions.GetDirectionCount);
        separationContext = new AIContext(Directions.GetDirectionCount);
        collisionContext = new AIContext(Directions.GetDirectionCount);
        weights = new AIContext(Directions.GetDirectionCount);

        //Detecting Player and Obstacles around
        InvokeRepeating("PerformDetection", 0, detectionDelay);
    }

    private void PerformDetection()
    {
        foreach (Detector detector in detectors)
        {
            detector.Detect(aiData);
        }
    }

    private void FixedUpdate()
    {
        //Enemy AI movement based on Target availability
        if (aiData.currentTarget != null)
        {
            //Looking at the Target
            OnPointerInput?.Invoke(aiData.currentTarget != null ? (Vector2) aiData.currentTarget.position : (Vector2) transform.position + movementInput);
            if (!following && !isAttacking)
            {
                following = true;
                StartCoroutine(ChaseAndAttack());
                //ChaseAndAttackFunc();
            }
        }
        else if (aiData.GetTargetsCount() > 0)
        {
            //Target acquisition logic
            aiData.currentTarget = aiData.targets[0];
        }
        //Moving the Agent
        if (!canMove)
            movementInput = Vector2.zero;
        OnMovementInput?.Invoke(movementInput, following);
    }

    private IEnumerator ChaseAndAttack()
    {
        if (aiData.currentTarget == null)
        {
            //Stopping Logic
            Debug.Log("Stopping");
            movementInput = Vector2.zero;
            following = false;
            yield break;
        }
        else
        {
            if (aiData.targets != null && aiData.targets.Count > 0)
            {
                aiData.currentTarget = aiData.targets[0];
                lastTargetPosition = aiData.currentTarget.position;
                float distance = Vector2.SqrMagnitude(lastTargetPosition - transform.position);
                if (attackTimeStamp >= attackDelay && !isAttacking && distance <= strafeDistance * strafeDistance && canAttack)
                {
                    attackTimeStamp = 0f;
                    isAttacking = true;
                    StartCoroutine(Attack(aiData.currentTarget));
                    //yield break;
                }
                else
                {
                    /*if (distance < strafeDistance)*/ attackTimeStamp += (Time.deltaTime + aiUpdateDelay);
                    //else attackTimeStamp = attackDelay;
                    movementInput = GetChaseVelocity(aiData.currentTarget.position, strafeDistance, attackDeadZone);
                }
                yield return new WaitForSeconds(aiUpdateDelay);
                StartCoroutine(ChaseAndAttack());
            }
            else
            {
                isAttacking = false;
                attackTimeStamp = attackDelay;

                float distane = Vector3.Distance(lastTargetPosition, transform.position);
                if (distane <= lastTargetPositionThreshhold)
                {
                    aiData.currentTarget = null;
                    movementInput = Vector2.zero;
                    following = false;
                    yield break;
                }
                movementInput = GetLastTargetPositionVelocity(lastTargetPosition);
                yield return new WaitForSeconds(aiUpdateDelay);
                StartCoroutine(ChaseAndAttack());
            }

        }

    }

    private IEnumerator Attack(Transform target)
    {
        OnPrepareAttack?.Invoke();
        movementInput = Vector2.zero;
        yield return ExtensionClass.GetWaitForSeconds(0.6f);

        float attackTime = 0.5f;
        float timeStamp = 0f;
        while ((Vector2.SqrMagnitude(transform.position - target.position) > attackDeadZone) && timeStamp < attackTime)
        {
            movementInput = (target.transform.position - transform.position).normalized;
            movementInput *= 2.5f;
            timeStamp += Time.deltaTime;
            yield return null;
        }
        OnAttackPressed.Invoke();
        isAttacking = false;
    }

    public Vector2 GetLastTargetPositionVelocity(Vector3 lastTargetPosition)
    {
        var displacement = lastTargetPosition - transform.position;
        seekContext = GetDirectionContext(displacement, seekContext);
        collisionContext = GetCollisionContext();

        seekContext.factor = 1f;

        weights.Combine(new AIContext[] { seekContext, collisionContext });

        return weights.GetDesiredDirection();
    }

    public Vector2 GetChaseVelocity(Vector3 targetPosition, float seekDistance, float seekDeadZone)
    {
        Vector2 chaseVelocity = Vector2.zero;

        var displacement = targetPosition - transform.position;
        var distance = displacement.magnitude;

        seekContext = seek ? GetDirectionContext(displacement, seekContext) : new AIContext(Directions.GetDirectionCount);
        strafeContext = strafe ? GetStrafeContext(displacement) : new AIContext(Directions.GetDirectionCount);
        biasContext = bias ? GetDirectionContext(rgBody.velocity, biasContext) : new AIContext(Directions.GetDirectionCount);
        collisionContext = collision ? GetCollisionContext() : new AIContext(Directions.GetDirectionCount);
        separationContext = separation ? GetSeparationContext() : new AIContext(Directions.GetDirectionCount);

        seekContext.factor = 1f;
        if (seekDistance - seekDeadZone != 0f)
        {
            seekContext.factor = Utilities.MapValue(distance, seekDeadZone, seekDistance);
        }
        strafeContext.factor = 1f - seekContext.factor;
        biasContext.factor = biasFactor;

        weights.Combine(new AIContext[] { seekContext, strafeContext, biasContext, collisionContext, separationContext });

        return weights.GetDesiredDirection();
    }

    private AIContext GetDirectionContext(Vector2 displacement, AIContext aIContext)
    {
        if (displacement == Vector2.zero)
        {
            aIContext.ClearWeight();
            return aIContext;
        }

        for (int i = 0; i < aIContext.Size; i++)
        {
            //float weight = Vector2.Dot(displacement, Directions.detectDirections[i]);
            float weight = Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(displacement, Directions.detectDirections[i]));
            weight = weight * 0.5f + 0.5f;
            aIContext.SetWeight(i, weight);
        }

        return aIContext;
    }

    private AIContext GetSeparationContext()
    {
        separationContext.ClearWeight();
        Vector2 awayVector = new Vector2();
        float maxFactor = 0f;

        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll((Vector2)transform.position, neighbourRadius, 1 << gameObject.layer);

        for (int i = 0; i < enemyColliders.Length; i++)
        {
            if (enemyColliders[i].transform == transform) continue;

            Vector2 displacement = transform.position - enemyColliders[i].transform.position;
            float distance = displacement.magnitude;

            float factor = 1f - (distance / neighbourRadius);
            maxFactor = Mathf.Max(maxFactor, factor);

            awayVector += (displacement / distance) * factor;
        }

        if (awayVector != Vector2.zero)
        {
            for (int i = 0; i < Directions.GetDirectionCount; i++)
            {
                float value = Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(awayVector, Directions.detectDirections[i]));
                float weight = 1f - Mathf.Abs(value - 0.65f);
                separationContext.SetWeight(i, weight * maxFactor);
            }
        }
        return separationContext;
    }

    private AIContext GetStrafeContext(Vector2 displacement)
    {
        for (int i = 0; i < strafeContext.Size; i++)
        {
            //float value = Vector2.Dot(displacement, Directions.detectDirections[i]);
            float value = Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(displacement, Directions.detectDirections[i]));
            float weight = 1f - Mathf.Abs(value + 0.5f);
            strafeContext.SetWeight(i, weight);
        }
        return strafeContext;
    }

    private AIContext GetCollisionContext()
    {
        collisionContext.ClearWeight();
        foreach (Collider2D obstacleCollider in aiData.obstacles)
        {
            Vector2 directionToObstacle
                = obstacleCollider.ClosestPoint(transform.position) - (Vector2)transform.position;
            float distanceToObstacle = directionToObstacle.magnitude;

            //calculate weight based on the distance Enemy<--->Obstacle
            float weight
                = distanceToObstacle <= agentColliderSize
                ? 1
                : (obstacleRadius - distanceToObstacle) / obstacleRadius;

            Vector2 directionToObstacleNormalized = directionToObstacle.normalized;

            //Add obstacle parameters to the danger array
            for (int i = 0; i < collisionContext.Size; i++)
            {
                //float result = Vector2.Dot(directionToObstacleNormalized, Directions.detectDirections[i]);
                float result = Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(directionToObstacleNormalized, Directions.detectDirections[i]));

                float valueToPutIn = result * weight;

                valueToPutIn = -(valueToPutIn * 0.5f + 0.5f);

                //override value only if it is higher than the current one stored in the danger array
                if (valueToPutIn < collisionContext.GetWeight(i))
                {
                    collisionContext.SetWeight(i, valueToPutIn);
                }
            }
        }
        return collisionContext;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        //Gizmos.color = Color.green;
        for (int i = 0; i < weights.Size; i++)
        {
            Gizmos.color = weights.GetWeight(i) >= 0 ? Color.green : Color.red;
            Gizmos.DrawRay(
                transform.position,
                Directions.detectDirections[i] * Mathf.Abs(weights.GetWeight(i))
                );
        }
    }
}
