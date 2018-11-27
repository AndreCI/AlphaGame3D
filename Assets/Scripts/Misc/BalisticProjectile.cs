using UnityEngine;
using System.Collections.Generic;

public class BalisticProjectile : MonoBehaviour
{
    [Header("Projectile Data")]
    public int speed;
    public int resolution;
    public Vector3 gravity;
    public LineRenderer ray;
    public GameObject projectile;
    public float animationDuration;

    [Header("Projectile initialization")]
    public Node source;
    public Node target;
    private List<Vector3> trajectory;
    private Vector3 direction;
    private float maxDistance;
    private bool shooting;
    private float timeIdx;

    // Use this for initialization
    void Start()
    {
        trajectory = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shooting)
        {
            timeIdx += Time.deltaTime;
            if (timeIdx > animationDuration)
            {
                shooting = false;
                projectile.SetActive(false);
            }
            else
            {
                int idx =Mathf.FloorToInt(resolution * timeIdx / animationDuration);
                projectile.transform.position = trajectory[idx];
                if (idx + resolution / 10 < resolution)
                {
                    projectile.transform.LookAt(trajectory[idx+Mathf.FloorToInt(resolution/10)]);
                }
                else
                {
                    projectile.transform.LookAt(target.position + new Vector3(0, 1.5f, 0));
                }
            }

        }
    }

    public void ShowArc(Node source_, Node target_)
    {
        source = source_;
        target = target_;
        SetTrajectory();
        RenderArc();
    }

    public void HideArc()
    {
        ray.positionCount = 0;
    }

    void RenderArc()
    {
        transform.position = new Vector3(0, 0, 0) ;
        ray.positionCount=resolution +1;
        ray.SetPositions(trajectory.ToArray());
        ray.materials[0].mainTextureScale = new Vector3(maxDistance, 1, 1);
         

    }

    public void SetTrajectory()
    {
        trajectory = new List<Vector3>();
        GetAngle();
        maxDistance = Vector3.Distance(source.position, target.position);
        for(int i =0; i<=resolution; i++)
        {
            float t = (float)i / (float)(resolution);
            trajectory.Add(CalculateArcPoint(t));
        }
    }

    private void GetAngle()
    {
        direction = -0.5f*gravity - source.position + target.position;
    }

    private Vector3 CalculateArcPoint(float t)
    {
        return 0.5f * gravity * Mathf.Pow(t, 2) + direction * t + source.position + new Vector3(0, 1.5f, 0);
        
    }

    public void Shoot() {
        timeIdx = 0;
        shooting = true;
        projectile.SetActive(true);
    }
}
