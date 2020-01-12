using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class WayPointPatrol : MonoBehaviour
{
    Animator m_Animator;
    public NavMeshAgent navMeshAgent;
    public Transform[] waypoints;

    bool isWalking = true;
    int m_CurrentWaypointIndex = 0;

    private void scriptGenerateWayPoints()
    {
        int numberWaypoints = Random.Range(2, 4);
        waypoints = new Transform[numberWaypoints];

        int y = 15; //standard

        GameObject beginObject = new GameObject();
        beginObject.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        waypoints[0] = beginObject.transform;

        for (int i = 1; i < numberWaypoints; i += 1)
        {
            while (true)
            {
                int x = Random.Range(5, 100);
                int z = Random.Range(400, 500);

                GameObject emptyObject = new GameObject();
                emptyObject.transform.position = new Vector3(x, y, z);

                NavMeshPath path = new NavMeshPath();
                navMeshAgent.CalculatePath(emptyObject.transform.position, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    waypoints[i] = emptyObject.transform;
                    break;
                }
                else
                {
                    Destroy(emptyObject);
                }

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        if (waypoints.Length == 0)
        {
            this.scriptGenerateWayPoints();
        }

        navMeshAgent.SetDestination(waypoints[0].position);
    }

    // Update is called once per frame
    void Update()
    {
        if (isWalking)
        {
            if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
            {
                StartCoroutine(WaitIdling());
            }
        }
    }

    IEnumerator WaitIdling()
    {
        isWalking = false;
        m_Animator.SetBool("IsWalking", isWalking);
        navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(2);

        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        Debug.Log("Index: " + m_CurrentWaypointIndex);

        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        navMeshAgent.isStopped = false;
        isWalking = true;
        m_Animator.SetBool("IsWalking", isWalking);
    }
}
