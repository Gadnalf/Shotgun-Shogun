using System;
using UnityEngine;

public class GunControl : MonoBehaviour
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
        Vector3 elbow = new Vector3(rightShoulder.x, ) 
        rightShoulder.Rotate(new Vector3(0, 0, 2f));
        leftShoulder.Rotate(new Vector3(0, 0, 2f));
    }

    private double Dist(Vector3 a, Vector3 b)
    {
        return Math.Sqrt(Math.Pow(a.x - b.x, 2) * Math.Pow(a.y - b.y, 2));
    }

    private double FindAimAngle(Vector3 pivot, Vector3 elbow, Vector3 target)
    {

        return 0;
    }

    //private void FindTarget(Transform shoulder, Transform point, Vector3 target)
    //{
    //    double distBetween = Math.Sqrt(Math.Pow(shoulder.position.x - target.position.x, 2) + Math.Pow(shoulder.position.x - target.position.x, 2));
    //    double angleToTarget = Math.Acos(1 / distBetween);
    //}
}
