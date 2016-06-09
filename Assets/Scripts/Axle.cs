using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class AxleInfo
{
    public List<WheelCollider> Wheels;
    public bool motor;
    public bool steering;
}
