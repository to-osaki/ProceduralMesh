using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace to.Lib.ProceduralMesh
{
    public interface IMeshGenerator
    {
        Mesh Generate();
    }
}