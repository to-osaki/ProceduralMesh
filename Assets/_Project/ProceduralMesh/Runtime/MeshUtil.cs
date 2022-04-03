using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Complex = System.Numerics.Complex;

namespace to.Lib.ProceduralMesh
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

		static public Mesh GenerateDomeMesh(Vector3 size, int split_r, int split_h, bool reverse)
		{
			var mesh = new Mesh();

			int vc = 1 + split_r * split_h;
			int ic = (split_r * 3) + ((split_h - 1) * split_r * 6);
			var verts = new NativeArray<VertexLayout>(vc, Allocator.Temp);
			var indices = new NativeArray<int>(ic, Allocator.Temp);

			reverse = size.y > 0f ? reverse : !reverse;
			int offset1 = reverse ? 0 : 1;
			int offset2 = reverse ? 1 : 0;

			verts[0] = new VertexLayout
			{
				pos = Vector3.up * size.y,
			};

			float rad_r = 2 * Mathf.PI / split_r;
			var rotate = new Complex(Mathf.Cos(rad_r), Mathf.Sin(rad_r));
			for (int floor = 0; floor < split_h; ++floor)
			{
				// vertices
				float rad_h = (Mathf.PI / 2) - (Mathf.PI / 2 / split_h) * (floor + 1);
				float y = Mathf.Sin(rad_h) * size.y;
				var point = Complex.One * Mathf.Cos(rad_h);
				for (int i = 0; i < split_r; ++i)
				{
					int index = (1 + floor * split_r) + i;
					verts[index] = new VertexLayout
					{
						pos = new Vector3((float)point.Real * size.x, y, (float)point.Imaginary * size.z),
					};
					point *= rotate;
				}
				// indexes
				if (floor == 0)
				{
					for (int i = 0; i < split_r; ++i)
					{
						indices[0 + i * 3] = 0;
						indices[1 + i * 3] = 1 + ((offset1 + i) % split_r);
						indices[2 + i * 3] = 1 + ((offset2 + i) % split_r);
					}
				}
				else
				{
					for (int i = 0; i < split_r; ++i)
					{
						int h_ = floor - 1;
						int i0 = (1 + floor * split_r) + ((offset1 + i) % split_r);
						int i1 = (1 + h_ * split_r) + ((offset1 + i) % split_r);
						int i2 = (1 + floor * split_r) + ((offset2 + i) % split_r);
						int i3 = (1 + h_ * split_r) + ((offset2 + i) % split_r);
						indices[(split_r * 3 + h_ * split_r * 6 + i * 6) + 0] = i0;
						indices[(split_r * 3 + h_ * split_r * 6 + i * 6) + 1] = i2;
						indices[(split_r * 3 + h_ * split_r * 6 + i * 6) + 2] = i1;
						indices[(split_r * 3 + h_ * split_r * 6 + i * 6) + 3] = i2;
						indices[(split_r * 3 + h_ * split_r * 6 + i * 6) + 4] = i3;
						indices[(split_r * 3 + h_ * split_r * 6 + i * 6) + 5] = i1;
					}
				}
			}
			mesh.SetVertexBufferParams(vc, VertexLayoutDescriptors);
			mesh.SetVertexBufferData(verts, 0, 0, vc);

			mesh.SetIndexBufferParams(ic, IndexFormat.UInt32);
			mesh.SetIndexBufferData(indices, 0, 0, ic, MeshUpdateFlags.Default);
			mesh.subMeshCount = 1;
			mesh.SetSubMesh(0, new SubMeshDescriptor(0, ic, MeshTopology.Triangles));

			mesh.RecalculateNormals();
			mesh.RecalculateBounds();

			return mesh;
		}
	}
}