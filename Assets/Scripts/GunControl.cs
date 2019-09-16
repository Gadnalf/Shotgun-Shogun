using System;
using UnityEngine;

public class Dist : MonoBehaviour
{
    public Vector3 mousePos { get; private set; }
    public Camera mainCamera;

    Transform leftShoulder;
    Transform rightShoulder;
    Transform leftPoint;
    Transform rightPoint;

    public Vector3 leftOffset;

    public void Start()
    {
        rightShoulder = transform.Find("Right Shoulder");
        rightPoint = rightShoulder.Find("Hand");
        leftShoulder = transform.Find("Left Shoulder");
        leftPoint = leftShoulder.Find("Gun").Find("Muzzle");
    }

	public void Update()
	{
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotate = new Vector3(0,0,3);
        rightShoulder.Rotate(rotate);
        leftShoulder.Rotate(rotate);
    }

    private double Dist2D(Vector3 a, Vector3 b)
    {
        return Math.Sqrt(Math.Pow(a.x - b.x, 2) * Math.Pow(a.y - b.y, 2));
    }

    private double FindAimAngle(Vector3 pivot, Vector3 elbow, Vector3 target)
    {

        return 0;
    }

    private int FindIntersectOfCircles(Vector3 center1, double radius1, Vector3 center2, double radius2, out Vector3 intersect1, out Vector3 intersect2)
    {
        double dist = Dist2D(center1, center2);

        if (dist > radius1 + radius2 | dist < Math.Abs(radius1 - radius2) | dist == 0) // There's some fuckery afoot
        {
            intersect1 = center1;
            intersect2 = center1;
            return 0;
        }
        else if (dist == radius1 + radius2) // One intersect
        {
            Vector3 direction = (center1 - center2).normalized;
            direction.Scale(new Vector3((float)radius1, (float)radius1, 0));
            intersect1 = center1 + direction;
            intersect2 = center1;
            return 1;
        }
        else
        {
            double a = (Math.Pow(radius1, 2) - Math.Pow(radius2, 2) + Math.Pow(dist, 2)) / (2 * dist);
            double h = Math.Sqrt(Math.Pow(radius1, 2) - Math.Pow(a, 2));

            Vector3 direction = (center1 - center2).normalized;
            direction.Scale(new Vector3((float) a, (float) a, 0));

            Vector3 vert = (center1 - center2).normalized;
            Vector3 avert = vert;
            vert.Scale(new Vector3((float) h, (float) -h, 0));
            avert.Scale(new Vector3((float) -h,(float) h, 0));
            
            intersect1 = center1 + direction + vert;
            intersect2 = center1 + direction + avert;
            return 2;
        }
        
    }

    //private void FindTarget(Transform shoulder, Transform point, Vector3 target)
    //{
    //    double distBetween = Math.Sqrt(Math.Pow(shoulder.position.x - target.position.x, 2) + Math.Pow(shoulder.position.x - target.position.x, 2));
    //    double angleToTarget = Math.Acos(1 / distBetween);
    //}
}
