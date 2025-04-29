using System.Collections.Generic;
using UnityEngine;

public class WallResetter : MonoBehaviour
{
    private List<WallInfo> walls = new List<WallInfo>();

    private void Start()
    {
        // Find all GameObjects tagged as "Wall" and store their initial positions
        GameObject[] wallObjects = GameObject.FindGameObjectsWithTag("Wall");

        foreach (GameObject wall in wallObjects)
        {
            walls.Add(new WallInfo(wall, wall.transform.position));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ResetWalls();
        }
    }

    private void ResetWalls()
    {
        foreach (WallInfo wallInfo in walls)
        {
            if (wallInfo.wallObject != null)
            {
                wallInfo.wallObject.transform.position = wallInfo.initialPosition;
            }
        }

        Debug.Log("Walls reset to initial positions!");
    }

    private struct WallInfo
    {
        public GameObject wallObject;
        public Vector3 initialPosition;

        public WallInfo(GameObject wallObject, Vector3 initialPosition)
        {
            this.wallObject = wallObject;
            this.initialPosition = initialPosition;
        }
    }
}
