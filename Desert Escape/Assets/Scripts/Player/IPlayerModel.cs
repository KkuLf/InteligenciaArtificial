using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerModel
{
    void Move(Vector3 dir);
    void LookDir(Vector3 dir);
}
