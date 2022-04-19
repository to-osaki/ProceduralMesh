using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace to.Lib.ProceduralMesh
{
	public sealed class QuadMesh : IMeshGenerator
	{
		[SerializeField]
		public Vector2Int size = Vector2Int.one;

		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		struct VertexLayout
		{
			public Vector3 pos;
			public Vector3 normal;
			public Vector2 uv0;
		}

		Mesh IMeshGenerator.Generate()
		{
			// set vertex attributes
			var layout = new[]
			{
				new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
				new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
				new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
			};

			int vc = 4;

			var mesh = new Mesh();
			mesh.SetVertexBufferParams(vc, layout);

			var verts = new NativeArray<VertexLayout>(vc, Allocator.Temp);

			verts[0] = new VertexLayout
			{
				pos = new Vector3(-size.x, size.y, 0f),
				uv0 = new Vector2(0f, 1f)
			};
			verts[1] = new VertexLayout
			{
				pos = new Vector3(size.x, size.y, 0f),
				uv0 = new Vector2(1f, 1f)
			};
			verts[2] = new VertexLayout
			{
				pos = new Vector3(size.x, -size.y, 0f),
				uv0 = new Vector2(1f, 0f)
			};
			verts[3] = new VertexLayout
			{
				pos = new Vector3(-size.x, -size.y, 0f),
				uv0 = new Vector2(0f, 0f)
			};

			mesh.SetVertexBufferData(verts, 0, 0, vc);

			mesh.SetIndexBufferParams(6, IndexFormat.UInt32);
			mesh.SetIndexBufferData(new int[] {
				0, 1, 2,
				2, 3, 0,
			}, 0, 0, 6, MeshUpdateFlags.Default);

			mesh.subMeshCount = 1;
			mesh.SetSubMesh(0, new SubMeshDescriptor(0, 6, MeshTopology.Triangles));

			mesh.RecalculateNormals();
			mesh.RecalculateBounds();

			return mesh;
		}
	}
}