using UnityEngine;
using System.Collections;

namespace Blue.Pathfinding
{
    public class Unit : MonoBehaviour
    {
        float speed = 20;
        Vector3[] path;
        int targetIndex;

        public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                path = newPath;
                targetIndex = 0;
                StopCoroutine(FollowPath());
                StartCoroutine(FollowPath());
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

        /// <summary>Navigate to a point in the map evading all the unwalkable objects</summary>
        public void NavigateToPoint(Vector3 pathStart, Vector3 pathEnd)
        {
            PathRequestManager.RequestPath(pathStart, pathEnd, OnPathFound);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //hit.collider.GetComponent<Renderer>().material.color = Color.red;
                    Debug.Log(hit.point);
                    NavigateToPoint(transform.position, hit.point);
                }

            }
        }

        public void OnDrawGizmos()
        {
            if (path != null)
            {
                for (int i = targetIndex; i < path.Length; i++)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(path[i], Vector3.one);

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
}