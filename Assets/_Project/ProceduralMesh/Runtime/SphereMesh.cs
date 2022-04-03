using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Complex = System.Numerics.Complex;

namespace to.Lib.ProceduralMesh
{
	public class SphereMesh : IMeshGenerator
	{
		[SerializeField]
		public Vector3 size = Vector3.one;
		[SerializeField, Range(3, 30)]
		public int split_r = 10;
		[SerializeField, Range(1, 30)]
		public int split_h = 10;
		[SerializeField]
		public bool reverse;

		public Mesh Generate()
		{
			var mesh1 = MeshUtil.GenerateDomeMesh(size, split_r, split_h, false);
			var mesh2 = MeshUtil.GenerateDomeMesh(new Vector3(size.x, -size.y, size.z), split_r, split_h, false);

			var mesh = new Mesh();
			mesh.CombineMeshes(new CombineInstance[]
			{
				new CombineInstance { mesh = mesh1, transform = Matrix4x4.identity},
				new CombineInstance { mesh = mesh2, transform = Matrix4x4.identity},
			});

			return mesh;
		}
	}
}