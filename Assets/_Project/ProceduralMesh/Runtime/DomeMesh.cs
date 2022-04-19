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
			return GenerateDomeMesh(size, split_r, split_h, true);
		}

		static public Mesh GenerateDomeMesh(Vector3 size, int split_r, int split_h, bool reverse)
		{
			var mesh = new Mesh();

			int vc = 1 + split_r * split_h;
			int ic = (split_r * 3) + ((split_h - 1) * split_r * 6);
			var verts = new NativeArray<MeshUtil.VertexLayout>(vc, Allocator.Temp);
			var indices = new NativeArray<int>(ic, Allocator.Temp);

			reverse = size.y > 0f ? reverse : !reverse;
			int offset1 = reverse ? 0 : 1;
			int offset2 = reverse ? 1 : 0;

			// pole vertex
			verts[0] = new MeshUtil.VertexLayout
			{
				pos = Vector3.up * size.y,
				uv0 = Vector2.one * 0.5f,
			};

			float rad_r = 2 * Mathf.PI / split_r;
			var rotate = new Complex(Mathf.Cos(rad_r), Mathf.Sin(rad_r));
			for (int floor = 0; floor < split_h; ++floor)
			{
				// vertices
				float rad_h = (Mathf.PI / 2) - (Mathf.PI / 2 / split_h) * (floor + 1);
				float sin = Mathf.Sin(rad_h);
				float cos = Mathf.Cos(rad_h);
				var point = Complex.One;
				for (int i = 0; i < split_r; ++i)
				{
					int index = (1 + floor * split_r) + i;
					float real = (float)point.Real;
					float imag = (float)point.Imaginary;
					verts[index] = new MeshUtil.VertexLayout
					{
						pos = new Vector3(
							real * cos * size.x,
							sin * size.y,
							imag * cos * size.z),
						uv0 = new Vector2((real * cos + 1f) * 0.5f, (imag * cos + 1f) * 0.5f),
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
						int floor_ = floor - 1;
						int i0 = (1 + floor * split_r) + ((offset1 + i) % split_r);
						int i1 = (1 + floor_ * split_r) + ((offset1 + i) % split_r);
						int i2 = (1 + floor * split_r) + ((offset2 + i) % split_r);
						int i3 = (1 + floor_ * split_r) + ((offset2 + i) % split_r);
						int offset = (split_r * 3 + floor_ * split_r * 6 + i * 6);
						indices[offset + 0] = i0;
						indices[offset + 1] = i2;
						indices[offset + 2] = i1;
						indices[offset + 3] = i2;
						indices[offset + 4] = i3;
						indices[offset + 5] = i1;
					}
				}
			}
			mesh.SetVertexBufferParams(vc, MeshUtil.VertexLayoutDescriptors);
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