using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Complex = System.Numerics.Complex;

namespace to.Lib.ProceduralMesh
{
	public class CylinderMesh : IMeshGenerator
	{
		public float height = 2;
		public float radius = 1;
		[SerializeField, Range(3, 30)]
		public int segments = 6;

		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		struct VertexLayout
		{
			public Vector3 pos;
			public Vector3 normal;
			public Vector2 uv0;
		}

		public Mesh Generate()
		{
			// set vertex attributes
			var layout = new[]
			{
				new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
				new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
				new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
			};

			var mesh = new Mesh();

			int vc = (1 + segments) * 2 + (3 + segments * 2);
			int ic = (3 * segments) * 2 + (6 * segments);
			var verts = new NativeArray<VertexLayout>(vc, Allocator.Temp);
			var indices = new NativeArray<int>(ic, Allocator.Temp);

			float r = Mathf.PI * 2 / segments;
			var rotate = new Complex(Mathf.Cos(r), Mathf.Sin(r));

			Cap(true, ref verts, ref indices);
			Cap(false, ref verts, ref indices);
			Tube(ref verts, ref indices);

			mesh.SetVertexBufferParams(vc, layout);
			mesh.SetVertexBufferData(verts, 0, 0, vc);

			mesh.SetIndexBufferParams(ic, IndexFormat.UInt32);
			mesh.SetIndexBufferData(indices, 0, 0, ic, MeshUpdateFlags.Default);
			mesh.subMeshCount = 1;
			mesh.SetSubMesh(0, new SubMeshDescriptor(0, ic, MeshTopology.Triangles));

			mesh.RecalculateNormals();
			mesh.RecalculateBounds();

			return mesh;
		}

		private void Cap(bool top, ref NativeArray<VertexLayout> verts, ref NativeArray<int> indices)
		{
			float y = top ? height / 2f : -height / 2f;
			int faceIdx = top ? 0 : segments;
			int vertIdx = top ? 0 : segments + 1;

			float r = (Mathf.PI * 2 / segments) * (top ? -1f : 1f);
			var rotate = new Complex(Mathf.Cos(r), Mathf.Sin(r));

			verts[vertIdx] = new VertexLayout
			{
				pos = new Vector3(0f, y, 0f),
			};
			Complex point = Complex.One * radius;
			for (int i = 0; i < segments; ++i)
			{
				verts[i + (vertIdx + 1)] = new VertexLayout
				{
					pos = new Vector3((float)point.Real, y, (float)point.Imaginary),
				};
				point *= rotate;

				indices[(faceIdx * 3) + 0 + i * 3] = (vertIdx);
				indices[(faceIdx * 3) + 2 + i * 3] = (vertIdx + 1) + ((1 + i) % segments);
				indices[(faceIdx * 3) + 1 + i * 3] = (vertIdx + 1) + ((0 + i) % segments);
			}
		}

		private void Tube(ref NativeArray<VertexLayout> verts, ref NativeArray<int> indices)
		{
			int vertOffset = (segments + 1) * 2;
			int faceOffset = segments * 2;

			float r = (Mathf.PI * 2 / segments);
			var rotate = new Complex(Mathf.Cos(r), Mathf.Sin(r));

			Complex point = Complex.One * radius;
			for (int i = 0; i < segments; ++i)
			{
				int v0 = vertOffset + (i * 2 + 0);
				int v1 = vertOffset + (i * 2 + 1);
				verts[v0] = new VertexLayout
				{
					pos = new Vector3((float)point.Real, height / 2f, (float)point.Imaginary),
				};
				verts[v1] = new VertexLayout
				{
					pos = new Vector3((float)point.Real, -height / 2f, (float)point.Imaginary),
				};
				point *= rotate;
			}

			for (int i = 0; i < segments; ++i)
			{
				int idx = i * 2;
				int v0 = vertOffset + (idx + 0);
				int v1 = vertOffset + (idx + 1);
				int v2 = vertOffset + ((idx + 2) % (segments * 2));
				int v3 = vertOffset + ((idx + 3) % (segments * 2));

				indices[(faceOffset * 3) + (i * 6) + 0] = v0;
				indices[(faceOffset * 3) + (i * 6) + 1] = v2;
				indices[(faceOffset * 3) + (i * 6) + 2] = v1;
				indices[(faceOffset * 3) + (i * 6) + 3] = v3;
				indices[(faceOffset * 3) + (i * 6) + 4] = v1;
				indices[(faceOffset * 3) + (i * 6) + 5] = v2;
			}
		}
	}
}