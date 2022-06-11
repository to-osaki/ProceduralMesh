using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace to.ProceduralMesh
{
	static public class MeshUtil
	{
		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		public struct VertexLayout
		{
			public Vector3 pos;
			public Vector3 normal;
			public Vector2 uv0;
		}

		// set vertex attributes
		static public readonly VertexAttributeDescriptor[] VertexLayoutDescriptors = new[]
		{
			new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
			new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
		};

		static public Mesh SetupTriangles(in NativeArray<VertexLayout> verts, List<int> ilist)
		{
			var mesh = new Mesh();
			mesh.SetVertexBufferParams(verts.Length, MeshUtil.VertexLayoutDescriptors);
			mesh.SetVertexBufferData(verts, 0, 0, verts.Length);

			mesh.SetIndexBufferParams(ilist.Count, IndexFormat.UInt32);
			mesh.SetIndexBufferData(ilist, 0, 0, ilist.Count, MeshUpdateFlags.Default);
			mesh.subMeshCount = 1;
			mesh.SetSubMesh(0, new SubMeshDescriptor(0, ilist.Count, MeshTopology.Triangles));

			mesh.RecalculateNormals();
			mesh.RecalculateBounds();

			return mesh;
		}

		static public Mesh SetupTriangles(in NativeArray<VertexLayout> verts, int[] indices)
		{
			var mesh = new Mesh();
			mesh.SetVertexBufferParams(verts.Length, MeshUtil.VertexLayoutDescriptors);
			mesh.SetVertexBufferData(verts, 0, 0, verts.Length);

			mesh.SetIndexBufferParams(indices.Length, IndexFormat.UInt32);
			mesh.SetIndexBufferData(indices, 0, 0, indices.Length, MeshUpdateFlags.Default);
			mesh.subMeshCount = 1;
			mesh.SetSubMesh(0, new SubMeshDescriptor(0, indices.Length, MeshTopology.Triangles));

			mesh.RecalculateNormals();
			mesh.RecalculateBounds();

			return mesh;
		}

		static public Mesh SetupTriangles(in NativeArray<VertexLayout> verts, in NativeArray<int> indices)
		{
			var mesh = new Mesh();
			mesh.SetVertexBufferParams(verts.Length, MeshUtil.VertexLayoutDescriptors);
			mesh.SetVertexBufferData(verts, 0, 0, verts.Length);

			mesh.SetIndexBufferParams(indices.Length, IndexFormat.UInt32);
			mesh.SetIndexBufferData(indices, 0, 0, indices.Length, MeshUpdateFlags.Default);
			mesh.subMeshCount = 1;
			mesh.SetSubMesh(0, new SubMeshDescriptor(0, indices.Length, MeshTopology.Triangles));

			mesh.RecalculateNormals();
			mesh.RecalculateBounds();

			return mesh;
		}

		static public Mesh SetupPoints(in NativeArray<VertexLayout> verts)
		{
			int length = Mathf.Min(verts.Length, s_numbers.Length);

			var mesh = new Mesh();
			mesh.SetVertexBufferParams(length, MeshUtil.VertexLayoutDescriptors);
			mesh.SetVertexBufferData(verts, 0, 0, length);

			mesh.SetIndexBufferParams(length, IndexFormat.UInt32);

			mesh.SetIndexBufferData(s_numbers, 0, 0, length, MeshUpdateFlags.Default);
			mesh.subMeshCount = 1;
			mesh.SetSubMesh(0, new SubMeshDescriptor(0, length, MeshTopology.Points));

			mesh.RecalculateBounds();

			return mesh;
		}

		static private int[] s_numbers;
		static MeshUtil()
		{
			s_numbers = new int[65536];
			for (int i = 0; i < s_numbers.Length; ++i)
			{
				s_numbers[i] = i;
			}
		}
	}
}