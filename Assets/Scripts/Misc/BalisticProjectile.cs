using UnityEngine;
using System.Collections.Generic;

public class BalisticProjectile : MonoBehaviour
{
    [Header("Projectile Data")]
    public int speed;
    public int resolution;
    //public int gravity;
    public LineRenderer ray;
    public float correction;

    [Header("Projectile initialization")]
    public Node source;
    public Node target;
    private List<Vector3> trajectory;
    private Vector3 direction;
    private float maxDistance;
    private float alpha = 2f;

    // Use this for initialization
    void Start()
    {
        trajectory = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(Node source_, Node target_)
    {
       
        source = source_;
        target = target_;
        Debug.Log(source.ToString());
        Debug.Log(target.ToString());
        SetTrajectory();
        RenderArc();
    }

    void RenderArc()
    {
        ray.positionCount=resolution +1;
        ray.SetPositions(trajectory.ToArray());
        ray.materials[0].mainTextureScale = new Vector3(maxDistance, 1, 1);


    }

    public void SetTrajectory()
    {
        //throw new System.NotImplementedException();
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
        float x = target.position.x - source.position.x;
        float z = target.position.z - source.position.z;
        float y = 0;
        direction = new Vector3(x, y, z);
    }

    private Vector3 CalculateArcPoint(float t)
    {
        float x = t * direction.x;
        float z = t * direction.z;
        float y = -Mathf.Pow((t * maxDistance - maxDistance/2), alpha) + Mathf.Pow(maxDistance/2,alpha);
        //float y = Mathf.Tan(radianAngle) - ((gravity * x * x) / (2 * speed * speed * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle));
        return new Vector3(x, y/correction, z);
    }
}
