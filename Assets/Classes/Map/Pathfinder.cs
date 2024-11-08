using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    #region Singleton
    public static Pathfinder Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private LineRenderer lineRenderer;
    private Dictionary<string, List<GameObject>> storedPaths = new Dictionary<string, List<GameObject>>();

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.blue;
    }

    // A* Algorithm to find a path between two tiles
    public List<GameObject> CalculatePath(GameObject startTile, GameObject endTile)
    {
        HexTile start = startTile.GetComponent<HexTile>();
        HexTile goal = endTile.GetComponent<HexTile>();

        if (start == null || goal == null)
        {
            Debug.LogError("Start or Goal tile is missing a HexTile component!");
            return null;
        }

        List<GameObject> path = new List<GameObject>();
        Dictionary<HexTile, HexTile> cameFrom = new Dictionary<HexTile, HexTile>();
        Dictionary<HexTile, float> costSoFar = new Dictionary<HexTile, float>();

        costSoFar[start] = 0;
        PriorityQueue<HexTile> openSet = new PriorityQueue<HexTile>();
        openSet.Enqueue(start, 0);

        while (!openSet.IsEmpty)
        {
            HexTile current = openSet.Dequeue();

            if (current == goal)
            {
                path = ReconstructPath(cameFrom, current);
                return path;
            }

            foreach (HexTile neighbor in GetNeighbors(current))
            {
                float newCost = costSoFar[current] + Vector3.Distance(current.GetTileCoordinates(), neighbor.GetTileCoordinates());

                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    costSoFar[neighbor] = newCost;
                    cameFrom[neighbor] = current;
                    openSet.Enqueue(neighbor, newCost);
                }
            }
        }

        return new List<GameObject>(); // Return empty if no path found
    }

    // Heuristic function using Euclidean distance
    private float Heuristic(HexTile a, HexTile b)
    {
        return Vector3.Distance(a.GetTileCoordinates(), b.GetTileCoordinates());
    }

    // Retrieves neighboring tiles for a given tile
    private List<HexTile> GetNeighbors(HexTile tile)
    {
        List<HexTile> neighbors = new List<HexTile>();
        Vector2Int[] directions = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, -1), new Vector2Int(-1, 1)
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborCoords = tile.HexCoords + direction;
            GameObject neighborObj = GridMap.Instance.GetTileByIndex(neighborCoords.x, neighborCoords.y);
            if (neighborObj != null)
            {
                neighbors.Add(neighborObj.GetComponent<HexTile>());
            }
        }

        return neighbors;
    }

    // Reconstruct the path by tracing back from the goal
    private List<GameObject> ReconstructPath(Dictionary<HexTile, HexTile> cameFrom, HexTile current)
    {
        List<GameObject> totalPath = new List<GameObject> { current.gameObject };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current.gameObject);
        }

        totalPath.Reverse();
        return totalPath;
    }

    // Visualizes the path and logs each waypoint's coordinates
    public void VisualizePath(List<GameObject> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No path to visualize.");
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = path.Count;
        Vector3[] positions = new Vector3[path.Count];

        Debug.Log("Path waypoints:");
        for (int i = 0; i < path.Count; i++)
        {
            HexTile tile = path[i].GetComponent<HexTile>();
            if (tile != null)
            {
                Vector3 waypointPosition = tile.GetTileCoordinates();
                positions[i] = waypointPosition + Vector3.up * 0.1f;  // Raise line slightly above tile surface

                // Log the waypoint coordinates
                Debug.Log($"Waypoint {i + 1}: {tile.HexCoords} at world position {waypointPosition}");
            }
        }

        lineRenderer.SetPositions(positions);
    }

    // Optional: Store and retrieve paths
    public void StorePath(string key, List<GameObject> path)
    {
        if (storedPaths.ContainsKey(key))
        {
            Debug.LogWarning($"Path with key '{key}' already exists. Overwriting.");
        }
        storedPaths[key] = path;
    }

    public List<GameObject> GetStoredPath(string key)
    {
        storedPaths.TryGetValue(key, out List<GameObject> path);
        return path;
    }

    public void RemovePath(string key)
    {
        if (storedPaths.ContainsKey(key))
        {
            storedPaths.Remove(key);
        }
        else
        {
            Debug.LogWarning($"No path found with key '{key}' to remove.");
        }
    }
}
