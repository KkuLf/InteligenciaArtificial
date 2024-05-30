using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magikarp : Player, IBoid
{
    public Vector3 Position => transform.position;
    public Vector3 Front => transform.forward;
}
