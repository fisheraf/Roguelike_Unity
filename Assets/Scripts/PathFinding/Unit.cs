using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public Transform target;
    public float speed = 1f;
    Vector3[] path;
    int targetIndex;
    Entity entity;
    Engine engine;


    void Start()
    {
        target = FindObjectOfType<Player>().transform;

        entity = GetComponent<Entity>();
        engine = FindObjectOfType<Engine>();
    }

    private void Update()
    {

    }

    public void RequestPathForUnit()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;

            engine.MoveToNextSpot(this, path[0]);
        }
    }


    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }


            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;

        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = new Color(0,1,0,.2f);
                Gizmos.DrawCube(new Vector3(path[i].x + .5f, path[i].y + .5f), Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}