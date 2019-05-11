using System.Collections;
using System.Collections.Generic;
using Panda;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject enemy;
    Player self;

    [SerializeField]
    Vector3 destination;

    float fieldOfView = 70f;
    float viewDistance = 10f;
    void Start()
    {
        self = GetComponent<Player>();
        destination = transform.position;

    }

    // Update is called once per frame
    void Update()
    {


    }
    [Task]
    bool EnemyDetected()
    {
        if (enemy != null)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if (distance < viewDistance)
            {
                destination = enemy.transform.position;
                self.SetDestination(destination);
            }
            else if (distance < 5f)
            {
                self.StopMoving();
                Vector3 lookAtGoal = new Vector3(enemy.transform.position.x, this.transform.position.y, enemy.transform.position.z);
                Vector3 direction = lookAtGoal - this.transform.position;
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
            }
            else
            {
                enemy = null;
            }
            return true;
        }
        return false;
    }
    [Task]
    bool EnemyWithinViewRange()
    {
        if (enemy == null)
        {
            int layerMask = 1 << 9;
            Collider[] cols = Physics.OverlapSphere(transform.position, viewDistance, layerMask);
            foreach (Collider collider in cols)
            {
                if (collider.transform.root != transform)
                {
                    Vector3 targetDir = collider.transform.position - transform.position;
                    float angleToEnemy = (Vector3.Angle(targetDir, transform.forward));
                    if (Mathf.Abs(angleToEnemy) <= fieldOfView * 0.5f & IsInLineOfSight(collider.gameObject))
                    {
                        enemy = collider.gameObject;
                        self.SetDestination(enemy.transform.position);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        return false;
    }

    bool IsInLineOfSight(GameObject target){
        int layerMask = 1 << 9;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {   
            return true;
        }
        return false;
    }

    float rotSpeed = 5f;
    [Task]
    bool AimAtEnemy()
    {
        if(!enemy)
            return false;
            
        Vector3 lookAtGoal = new Vector3(enemy.transform.position.x,
                                    this.transform.position.y,
                                    enemy.transform.position.z);
        Vector3 direction = lookAtGoal - this.transform.position;

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
        return true;
    }

    [Task]
    bool RandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 25f;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 25f, 1);
        destination = hit.position;
        self.SetDestination(destination);
        return true;
    }
    [Task]
    bool ReachedDestination()
    {
        if ((destination - transform.position).sqrMagnitude < 7.0f)
        {
            return true;
        }
        return false;
    }
    void OnDrawGizmos()
    {
        // Draw a green wire sphere to show destination positions
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, 25);

        // Draw a sphere to show the destination point
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(destination, 1);
        DrawViewField();
    }

    void DrawViewField()
    {
    
        float rayRange = 20.0f;
        float halfFOV = fieldOfView / 2.0f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.DrawRay(transform.position, leftRayDirection * rayRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * rayRange);
    }
}
