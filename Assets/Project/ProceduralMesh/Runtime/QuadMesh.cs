using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace to.ProceduralMesh
{
	public sealed class QuadMesh : IMeshGenerator
	{
		[SerializeField]
		public Vector2Int size = Vector2Int.one;

		Mesh IMeshGenerator.Generate()
		{
			int vc = 4;

			var verts = new NativeArray<MeshUtil.VertexLayout>(vc, Allocator.Temp);

			verts[0] = new MeshUtil.VertexLayout
			{
				pos = new Vector3(-size.x, size.y, 0f),
				uv0 = new Vector2(0f, 1f)
			};
			verts[1] = new MeshUtil.VertexLayout
			{
				pos = new Vector3(size.x, size.y, 0f),
				uv0 = new Vector2(1f, 1f)
			};
			verts[2] = new MeshUtil.VertexLayout
			{
				pos = new Vector3(size.x, -size.y, 0f),
				uv0 = new Vector2(1f, 0f)
			};
			verts[3] = new MeshUtil.VertexLayout
			{
				pos = new Vector3(-size.x, -size.y, 0f),
				uv0 = new Vector2(0f, 0f)
			};

			var indices = new int[] {
				0, 1, 2,
				2, 3, 0,
			};
			var mesh = MeshUtil.SetupTriangles(verts, indices);
			return mesh;
		}
	}
}