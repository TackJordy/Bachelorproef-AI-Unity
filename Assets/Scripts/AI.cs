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

    float fieldOfView = 100f;
    float viewDistance = 10f;
    float timeremaining = 0;
    float elapsedTime = 0;

    [SerializeField]
    private float maxShootSpread = 10f;
    [SerializeField]
    private float shelterDistance = 10f;
    [SerializeField]
    private GameObject cover = null;
    void Start()
    {
        self = GetComponent<Player>();
        destination = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

    }
    [Task]
    public bool ReachedCover()
    {
        if(cover==null)
            return false;

        if ((cover.transform.position - transform.position).sqrMagnitude < 7.0f)
        {
            cover=null;
            return false;
        }
        return true;
    }

    [Task]
    public bool HasCover()
    {
        if(cover==null)
            return false;

        return true;
    }
    [Task]
    public bool SearchCover()
    {
        if(self.health > 70)
            return false;

        int layerMask = 1 << 10;
        // Find all players within a radius of 10
        Collider[] cols = Physics.OverlapSphere(transform.position, shelterDistance, layerMask);
        Collider closesCollider = null;
        foreach (Collider collider in cols)
        {   
            if(closesCollider==null)
                closesCollider = collider;
            if(Vector3.Distance(collider.transform.position,transform.position)<Vector3.Distance(closesCollider.transform.position,transform.position)){
                closesCollider = collider;
            }
        }
        cover = closesCollider.gameObject;
        NavMeshHit hit;
        NavMesh.SamplePosition(cover.transform.position, out hit, 25f, 1);
        destination = hit.position;
        self.SetDestination(destination);
        return true;
    }
    [Task]
    public bool Fire()
    {
        if(self.ammo<=0)
            return false;

        Vector3 forward = transform.TransformDirection(Vector3.forward) * 5;
        Debug.DrawRay(transform.position, forward, Color.red);
        // self.ammo--;

         // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 9;

        Quaternion spreadAngle = Quaternion.AngleAxis(Random.Range(-maxShootSpread,maxShootSpread), new Vector3(1,1,0));
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, spreadAngle * transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity,layerMask))
        {
            hit.transform.GetComponent<Player>().DealDamage(20);
            if(hit.transform.GetComponent<Player>().health-20<=0){
                destination = transform.position;
                GetComponent<NavMeshAgent>().isStopped = false;
                enemy = null;
            }
        }
        //GameObject bullet = GameObject.Instantiate(bulletPrefab,)
        return true;
    }

    [Task]
    bool EnemyDetected()
    {
        return enemy!=null;
    }
    [Task]
    bool HoldDistance()
    {
        float distance = Vector3.Distance(enemy.transform.position, transform.position);
        // if (distance < viewDistance & distance > 5f)
        // {
        //     destination = enemy.transform.position;
        //     self.SetDestination(destination);
        // }
        if (distance < viewDistance)
        {
            // self.StopMoving();
            GetComponent<NavMeshAgent>().isStopped = true;
           
        }
        else
        {
            enemy = null;
            GetComponent<NavMeshAgent>().isStopped = false;
            return false;
        }
        return true;
    }
    
    float rotSpeed = 8f;

    [Task]
    void RotateToEnemy(){
        Vector3 lookAtGoal = new Vector3(enemy.transform.position.x, this.transform.position.y, enemy.transform.position.z);
        Vector3 direction = lookAtGoal - this.transform.position;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
        Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
    }
    [Task]
    bool FollowEnemy()
    {
        destination = enemy.transform.position;
        self.SetDestination(destination);
        // if(elapsedTime>1f){
        //     Debug.Log(elapsedTime);
        //     elapsedTime = 0;
        //     Vector3 randomDirection = Random.insideUnitSphere * 25f;
        //     NavMeshHit hit;
        //     NavMesh.SamplePosition(randomDirection, out hit, 5f, 1);
            
        //     return true;
        // }
        return true;
    }

    [Task]
    bool EnemyWithinViewRange()
    {
        if (enemy == null)
        {
            // Only layer with players is selected
            int layerMask = 1 << 9;
            // Find all players within a radius of 10
            Collider[] cols = Physics.OverlapSphere(transform.position, viewDistance, layerMask);
            foreach (Collider collider in cols)
            {
                if (collider.transform.root != transform)
                {
                    
                    Vector3 targetDir = collider.transform.position - transform.position;
                    float angleToEnemy = (Vector3.Angle(targetDir, transform.forward));
                    // Only detect players within the viewrange of 70° and if the player is not hidden after an object
                    if (Mathf.Abs(angleToEnemy) <= fieldOfView * 0.5f & IsInLineOfSight(collider.gameObject))
                    {
                        enemy = collider.gameObject;
                        // Go to the target
                        // self.SetDestination(enemy.transform.position);
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

    // Fire a raycast and return the first hit, if it's an enemy then the player is visible
    bool IsInLineOfSight(GameObject target){
        RaycastHit hit;
        if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit, Mathf.Infinity))
        {   
            if(hit.transform.gameObject.layer == 9){
                return true;
            } else {
                return false;
            }
        }
        return false;
    }


    [Task]
    bool RandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 20f;
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
    // void OnDrawGizmos()
    // {
    //     // Draw a green wire sphere to show destination positions
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireSphere(Vector3.zero, 25);
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position,10);
    //     // Draw a sphere to show the destination point
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawSphere(destination, 0.3f);
    //     DrawViewField();
    // }

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
