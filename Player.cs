using System.Collections;
using System.Collections.Generic;
using Panda;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField]
    private Vector3 destination;
    [SerializeField]
    private int ammo = 20;
    // startAmmo is needed to know how much bullets the gun will have when reload happens
    private int startAmmo;

    [SerializeField]
    private int health = 100;

    private int startHealth;
    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        startAmmo = ammo;
        startHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if(health<=0)
            Destroy(gameObject);
    }

    public void SetDestination(Vector3 dest)
    {
        destination = dest;
        agent.destination = destination;
    }

    public void DealDamage(int damage)
    {
        health -= damage;
    }

    public void StopMoving()
    {
        agent.isStopped = true;
    }
    [Task]
    public bool Fire()
    {
        if(ammo<=0)
            return false;

        Vector3 forward = transform.TransformDirection(Vector3.forward) * 30;
        Debug.DrawRay(transform.position, forward, Color.red);
        ammo--;

         // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 9;


        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity,layerMask))
        {
            hit.transform.GetComponent<Player>().DealDamage(20);
            if(hit.transform.GetComponent<Player>().health-20<=0){
                GetComponent<AI>().enemy = null;
            }
        }
        //GameObject bullet = GameObject.Instantiate(bulletPrefab,)
        return true;
    }

}
