using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Node currentNode;        // The node the player is currently standing on
    private bool isMoving = false;
    public Camera playerCamera; // Assign in inspector

    void Update()
    {
        if (!isMoving && Input.GetMouseButtonDown(0))
        {
            Node targetNode = GetNodeUnderMouse();
            if (targetNode != null && targetNode != currentNode)
            {
                List<Node> path = FindShortestPath(currentNode, targetNode);
                if (path != null)
                {
                    StartCoroutine(MoveAlongPath(path));
                }
            }
        }
    }

    // Cast a ray from camera to see if we hit a node
    Node GetNodeUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.GetComponent<Node>();
        }
        return null;
    }

    // Breadth-first search for shortest path
    List<Node> FindShortestPath(Node start, Node goal)
    {
        Queue<Node> queue = new Queue<Node>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

        queue.Enqueue(start);
        cameFrom[start] = null;

        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();

            if (current == goal)
            {
                return ReconstructPath(cameFrom, goal);
            }

            foreach (Node neighbor in current.neighbors)
            {
                if (!cameFrom.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        return null; // No path found
    }

    // Rebuild the path from BFS results
    List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node end)
    {
        List<Node> path = new List<Node>();
        Node current = end;

        while (current != null)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }

    // Move the player step by step
    IEnumerator MoveAlongPath(List<Node> path)
    {
        isMoving = true;
        float stepDuration = 0.3f; // Time to travel between nodes

        for (int i = 1; i < path.Count; i++)
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = path[i].transform.position;
            float elapsed = 0f;

            while (elapsed < stepDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / stepDuration);
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            transform.position = targetPos;
            currentNode = path[i];

            UpdateCameraMask();
            SwitchScene();
        }

        isMoving = false;
    }

    void SwitchScene()
    {
        if(currentNode.changeScene)
        {
            SceneManager.LoadScene("Test 1");
        }
    }

    void UpdateCameraMask()
    {
        if (playerCamera == null || currentNode == null) return;

        int layer1 = 1 << 6;
        int layer2 = 1 << 7;

        if (currentNode.enableExtraLayer)
        {
            playerCamera.cullingMask = layer1 | layer2; // Show both
        }
        else
        {
            playerCamera.cullingMask = layer2; // Only player
        }
    }
}