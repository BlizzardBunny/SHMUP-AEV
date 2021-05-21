using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    [SerializeField] WaveConfig waveConfig;
    List<Transform> waypoints;
    [SerializeField] float moveSpeed = 2f;

    int waypointIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (waypointIndex <= waypoints.Count - 1)
        {
            Vector2 currPos = transform.position;
            Vector2 targetPos = waypoints[waypointIndex].transform.position;
            var movement = moveSpeed * Time.deltaTime;

            transform.position = Vector2.MoveTowards
                (transform.position, targetPos, movement);

            if (currPos == targetPos)
            {
                waypointIndex++;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}