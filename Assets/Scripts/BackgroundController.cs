using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private Vector3 initialPos;
    public GameObject cam;
    public Vector2 parallaxEffect;

    private Renderer rend;

    private void Start()
    {
        initialPos = transform.position;
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        if (rend == null) return;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        bool isVisible = GeometryUtility.TestPlanesAABB(planes, rend.bounds);

        if (isVisible)
        {
            float distX = (cam.transform.position.x - initialPos.x) * parallaxEffect.x;
            float distY = (cam.transform.position.y - initialPos.y) * parallaxEffect.y;

            transform.position = new Vector3(initialPos.x + distX, initialPos.y + distY, initialPos.z);
        }
    }
}




