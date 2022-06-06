using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace to.ProceduralMesh
{
    public interface IMeshGenerator
    {
        Mesh Generate();
    }
}