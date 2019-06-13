using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnScript : MonoBehaviour
{
    public int amountOfNPCs;
    public GameObject NPC;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
        for(int i = 0; i<amountOfNPCs; i++)
        {
             Vector3 randomDirection = Random.insideUnitSphere * 20f;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 25f, 1);
            Instantiate(NPC,hit.position, Quaternion.Euler(0, Random.Range(0, 360),0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            Time.timeScale = 1f;
        }
    }
}
