using System;
using UnityEngine;

public class GunControl : MonoBehaviour
{
    Vector3 mousePos;
    public Camera mainCamera;
    public GameObject bullet;
    public float spread;
    public int pellets;
    public float aimSpeed = 0.8f;
    public float reloadTime = 2f;
    public float recoilVelocity = 4;

    public bool facingLeft = false;
    private float remainingReloadTime = 0;
    private float ammo = 2;

    public Transform pivot;
    public Transform elbow;
    public Transform point;

    public void Update()
    {
        if(remainingReloadTime > 0)
        {
            remainingReloadTime = remainingReloadTime - Time.deltaTime;
            if(remainingReloadTime <= 0)
            {
                ammo = 2;
            }
        }
        
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Flip if flipping is needed
        if ((mousePos.x - transform.position.x) > 0 && facingLeft)
        {
            transform.Rotate(new Vector3(0, 180, 0));
            facingLeft = false;
        }
        else if ((mousePos.x - transform.position.x) < 0 && !facingLeft)
        {
            transform.Rotate(new Vector3(0, 180, 0));
            facingLeft = true;
        }

        //Debug.Log("squeak: " + mousePos.ToString());
        //Debug.Log("shrug: " + leftShoulder.position);
        //Debug.Log("elbow noise: " + leftElbow.position);

        double aimAngle = FindAimAngle(pivot.position, elbow.position, point.position, mousePos, facingLeft) * (180 / Math.PI);

        Vector3 rotate = new Vector3(0, 0, (float)aimAngle * aimSpeed);
        pivot.Rotate(rotate);
    }

    private double Dist2D(Vector3 a, Vector3 b)
    {
        return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
    }

    private double FindAimAngle(Vector3 pivot, Vector3 elbow, Vector3 point, Vector3 target, bool isFlipped)
    {
        // Aiming too close doesn't aim
        double aimDist = Dist2D(pivot, target);
        if (aimDist <= Dist2D(pivot, elbow))
        {
            return 0;
        }

        // Samurai isn't flexible enough to aim behind his shoulder, sorry.
        bool behindShoulder = (mousePos.x - pivot.x) <= 0;
        bool behindShoulderFlipped = (mousePos.x - pivot.x) >= 0;
        bool behind = (behindShoulderFlipped && isFlipped) || (behindShoulder && !isFlipped);

        if (behind)
        {
            return 0;
        }

        // A whole bunch of math to figure out how far to rotate our dude's arm.
        Vector3 intersect1, intersect2;
        Vector3 mid = new Vector3((pivot.x + target.x)/2, (pivot.y + target.y) / 2, 0);
        //Debug.Log("full distance: " + Dist2D(pivot, target));
        //Debug.Log("mid distance: " + Dist2D(pivot, mid));
        Vector3 pivot1 = pivot;
        double radius1 = Dist2D(pivot, elbow);
        Vector3 pivot2 = mid;
        double radius2 = Dist2D(pivot, mid);
        int numIntersects = FindIntersectOfCircles(pivot1, radius1, pivot2, radius2, out intersect1, out intersect2);

        double lengthToMidpoint;
        Vector3 newElbow;
        if (numIntersects == 2)
        {
            //Debug.Log("jabroni time: " + intersect1 + intersect2);
            double dist1 = Dist2D(elbow, intersect1);
            double dist2 = Dist2D(elbow, intersect2);
            if (intersect1.y < intersect2.y)
            {
                lengthToMidpoint = dist1 / 2;
                newElbow = intersect1;
            }
            else
            {
                lengthToMidpoint = dist2 / 2;
                newElbow = intersect2;
            }
        }
        else if (numIntersects == 1)
        {
            //Debug.Log("merry christmas: " + intersect1);
            //lengthToMidpoint = Dist2D(elbow, intersect1) / 2;
            //newElbow = intersect1;
            return 0;
        }
        else
        {
            return 0;
        }

        if (Dist2D(pivot, elbow) <= 0)
        {
            Debug.Log("this shouldn't be possible?");
            return 0;
        }
        //Debug.Log("We gotta angle here, between " + pivot + " and " + intersect1 + " or " + intersect2 + ". Length to mid is " + lengthToMidpoint);
        double returnAngle = ((90 * Math.PI / 180) - Math.Acos(lengthToMidpoint / Dist2D(pivot, elbow))) * 2;

        // Apply direction of rotation based on cross product
        double cross = ((elbow.x - pivot.x) * (newElbow.y - elbow.y)) - ((elbow.y - pivot.y) * (newElbow.x - elbow.x));

        if (cross < 0 && !isFlipped || cross > 0 && isFlipped)
        {
            returnAngle = returnAngle * -1;
        }

        return returnAngle;
    }

    private int FindIntersectOfCircles(Vector3 center1, double radius1, Vector3 center2, double radius2, out Vector3 intersect1, out Vector3 intersect2)
    {
        double dist = Dist2D(center1, center2);

        if (dist > radius1 + radius2 | dist < Math.Abs(radius1 - radius2) | dist == 0) // There's some fuckery afoot
        {
            //Debug.Log("there's fuckery afoot");
            //Debug.Log("too far? " + (dist > radius1 + radius2));
            //Debug.Log("one circle in the other somehow? " + (dist < Math.Abs(radius1 - radius2)));
            //Debug.Log("same pivot? " + (dist == 0));
            intersect1 = center1;
            intersect2 = center1;

            return 0;
        }
        else if (dist == radius1 + radius2) // One intersect
        {
            //Debug.Log("lucky you");
            Vector3 direction = new Vector3((center1 - center2).normalized.x * (float) radius1, (center1 - center2).normalized.y * (float) radius1, 0);
            intersect1 = center1 + direction;
            intersect2 = center1;
            return 1;
        }
        else
        {
            double a = (Math.Pow(radius1, 2) - Math.Pow(radius2, 2) + Math.Pow(dist, 2)) / (2 * dist);
            double h = Math.Sqrt(Math.Pow(radius1, 2) - Math.Pow(a, 2));

            Vector3 direction = new Vector3 ((float) ((center2.x - center1.x) / dist), (float) ((center2.y - center1.y) / dist), 0);
            Vector3 horizontal = new Vector3(direction.x * (float) a, direction.y * (float) a, 0);

            Vector3 vert = new Vector3(direction.y * (float)h, -1 * direction.x * (float)h, 0);
            Vector3 avert = new Vector3(-1 * direction.y * (float)h, direction.x * (float)h, 0);

            intersect1 = center1 + horizontal + vert;
            intersect2 = center1 + horizontal + avert;
            return 2;
        }

    }

    // Returns normalized vector representing recoil
    public Vector2 Shoot()
    {
        if(remainingReloadTime <= 0 || ammo > 0)
        {
            float increment = spread / pellets / 2f;

            if (pellets % 2 == 1)
            {
                for (int i = 1; i <= pellets; i++)
                {
                    if (i % 2 == 0)
                    {
                        Instantiate(bullet, point.position, Quaternion.Euler(pivot.rotation.eulerAngles - new Vector3(0, 0, increment * (i / 2))));
                    }
                    else
                    {
                        Instantiate(bullet, point.position, Quaternion.Euler(pivot.rotation.eulerAngles + new Vector3(0, 0, increment * (i / 2))));
                    }
                }
            }
            else
            {
                Debug.Log("Even");
            }

            remainingReloadTime = reloadTime;
            ammo = ammo - 1;

            // Return recoil for the controller to handle movement
            Vector3 recoil = (point.position - mousePos).normalized;
            return new Vector2(recoil.x * recoilVelocity, recoil.y * recoilVelocity);
        }
        else
        {
            return Vector2.zero;
        }
    }
}