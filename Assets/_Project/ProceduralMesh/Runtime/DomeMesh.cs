using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Complex = System.Numerics.Complex;

namespace to.Lib.ProceduralMesh
{
	public class DomeMesh : IMeshGenerator
	{
		[SerializeField]
		public Vector3 size = Vector3.one;
		[SerializeField, Range(3, 30)]
		public int split_r = 6;
		[SerializeField, Range(1, 30)]
		public int split_h = 6;

		public Mesh Generate()
		{
			return MeshUtil.GenerateDomeMesh(size, split_r, split_h, true);
		}
	}
}