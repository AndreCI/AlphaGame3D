using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BalisticProjectile : RangedAttackAnimation
{
    [Header("Projectile Data")]
    public int speed;
    public int resolution;
    public Vector3 gravity;
    public LineRenderer ray;
    public GameObject projectile;
    public ParticleSystem hitAnimation;
    public float projectileFadePerSecond;
    public float elevationOffset;

    [Header("Projectile initialization")]
    private List<Vector3> trajectory;
    private Vector3 direction;
    private float maxDistance;
    private bool shooting;
    private float timeIdx;
    private bool fadeProjectile;
    private MeshRenderer projectileRenderer;
    // Use this for initialization
    void Start()
    {
        trajectory = new List<Vector3>();
        shooting = false;
        fadeProjectile = false;
        projectileRenderer = projectile.GetComponentInChildren<MeshRenderer>();
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
                hitAnimation.Play();
                fadeProjectile = true;
                //projectile.SetActive(false);
            }
            else if(timeIdx>0)
            {
                int idx =Mathf.FloorToInt(resolution * timeIdx / animationDuration);
                projectile.transform.position = trajectory[idx] + source.Position;
                if (idx + resolution / 10 < resolution)
                {
                    projectile.transform.LookAt(trajectory[idx+Mathf.FloorToInt(resolution/10)] + source.Position);
                }
                else
                {
                    projectile.transform.LookAt(trajectory[resolution] + source.Position);
                }
            }

        }else if (fadeProjectile)
        {
            Color color = projectileRenderer.material.color; //Does not work
            projectileRenderer.material.color = new Color(color.r, color.g, color.b, color.a - (projectileFadePerSecond * Time.deltaTime));
            if (projectileRenderer.material.color.a <= 0)
            {
                fadeProjectile = false;
                projectile.SetActive(false);
            }
        }
    }

    public override void ShowAttackPreview(HexCell source_, HexCell target_)
    {
        source = source_;
        target = target_;
        SetTrajectory();
        RenderArc();
    }

    public override void HideAttackPreview()
    {
        ray.positionCount = 0;
    }

    void RenderArc()
    {
        transform.position = new Vector3(0, 0, 0) ;
        ray.positionCount=resolution +1;
        transform.position = source.Position;// new Vector3(0, 0, 0);
        ray.SetPositions(trajectory.ToArray());
        ray.materials[0].mainTextureScale = new Vector3(maxDistance, 1, 1);
    }

    public void SetTrajectory()
    {
        trajectory = new List<Vector3>();
        GetAngle();
        maxDistance = Vector3.Distance(source.Position, target.Position);
        for(int i =0; i<=resolution; i++)
        {
            float t = (float)i / (float)(resolution);
            trajectory.Add(CalculateArcPoint(t));
        }
    }

    private void GetAngle()
    {
        float bornes = 3f;
        System.Random randsys = new System.Random();
        Vector3 randomOffset = new Vector3((float)(randsys.NextDouble()*2-1)*bornes, 0, (float)(randsys.NextDouble() * 2 - 1)*bornes);
        direction = -0.5f * gravity + ((target.Position + randomOffset) - source.Position);
    }

    private Vector3 CalculateArcPoint(float t)
    {
        return (0.5f * gravity * Mathf.Pow(t, 2) + direction * t + new Vector3(0, elevationOffset, 0));
        
    }

    public override void PlayAnimation() {
        timeIdx = -delay;
        shooting = true;
        projectile.SetActive(true);
    }
}
