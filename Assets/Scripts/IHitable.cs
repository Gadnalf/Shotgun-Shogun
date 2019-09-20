using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitable
{
    void Hit(Vector3 direction, float force);
}
    
