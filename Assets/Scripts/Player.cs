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
    public int ammo = 20;
    // startAmmo is needed to know how much bullets the gun will have when reload happens
    private int startAmmo;

    [SerializeField]
    public int health = 100;
    public bool regenerate = true;
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
        if(health<100 && regenerate){
            StartCoroutine(IncreaseHealth());
            regenerate = false;
        }
        if(health<=0){
            gameObject.SetActive(false);
            Vector3 randomDirection = Random.insideUnitSphere * 20f;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 25f,1);
            transform.position = hit.position;
            //transform.rotation = Quaternion.Euler(0, Random.Range(0, 360),0);
            health = startHealth;
            regenerate = true;
            gameObject.SetActive(true);
            agent.destination = destination;
        }
    }
    IEnumerator IncreaseHealth()
    {
        yield return new WaitForSeconds(5);
        health+=20;
        regenerate = true;
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
  

}
