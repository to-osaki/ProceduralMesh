using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Complex = System.Numerics.Complex;

namespace to.ProceduralMesh
{
	public class CylinderMesh : IMeshGenerator
	{
		public float height = 2;
		public float radius = 1;
		[SerializeField, Range(3, 30)]
		public int segments = 6;

		public Mesh Generate()
		{
			int vc = (1 + segments) * 2 + (3 + segments * 2);
			int ic = (3 * segments) * 2 + (6 * segments);
			var verts = new NativeArray<MeshUtil.VertexLayout>(vc, Allocator.Temp);
			var indices = new NativeArray<int>(ic, Allocator.Temp);

			float r = Mathf.PI * 2 / segments;
			var rotate = new Complex(Mathf.Cos(r), Mathf.Sin(r));

			Tube(ref verts, 0, ref indices, 0);
			Cap(true, ref verts, segments * 2, ref indices, segments * 2 * 3); // offset tube
			Cap(false, ref verts, (segments * 2) + (segments + 1), ref indices, (segments * 2 + segments) * 3); // offset tube&top

			var mesh = MeshUtil.SetupTriangles(verts, indices);
			return mesh;
		}

		private void Cap(bool top, ref NativeArray<MeshUtil.VertexLayout> verts, int vertStart, ref NativeArray<int> indices, int indexStart)
		{
			float y = top ? height / 2f : -height / 2f;

			float r = (Mathf.PI * 2 / segments) * (top ? -1f : 1f);
			var rotate = new Complex(Mathf.Cos(r), Mathf.Sin(r));

			verts[vertStart] = new MeshUtil.VertexLayout
			{
				pos = new Vector3(0f, y, 0f),
				uv0 = Vector2.one * 0.5f,
			};
			Complex point = Complex.One;
			for (int i = 0; i < segments; ++i)
			{
				float real = (float)point.Real;
				float imag = (float)point.Imaginary;
				verts[i + (vertStart + 1)] = new MeshUtil.VertexLayout
				{
					pos = new Vector3(real * radius, y, imag * radius),
					uv0 = new Vector2((real + 1f) * 0.5f, (imag + 1f) * 0.5f),
				};
				point *= rotate;

				indices[indexStart + 0 + i * 3] = (vertStart);
				indices[indexStart + 2 + i * 3] = (vertStart + 1) + ((1 + i) % segments);
				indices[indexStart + 1 + i * 3] = (vertStart + 1) + ((0 + i) % segments);
			}
		}

		private void Tube(ref NativeArray<MeshUtil.VertexLayout> verts, int vertStart, ref NativeArray<int> indices, int indexStart)
		{
			float r = (Mathf.PI * 2 / segments);
			var rotate = new Complex(Mathf.Cos(r), Mathf.Sin(r));

			Complex point = Complex.One;
			for (int i = 0; i < segments; ++i)
			{
				int v0 = vertStart + (i * 2 + 0);
				int v1 = vertStart + (i * 2 + 1);
				float real = (float)point.Real;
				float imag = (float)point.Imaginary;
				verts[v0] = new MeshUtil.VertexLayout
				{
					pos = new Vector3(real * radius, height / 2f, imag * radius),
					uv0 = new Vector2((imag + 1f) * 0.5f, 1f),
				};
				verts[v1] = new MeshUtil.VertexLayout
				{
					pos = new Vector3(real * radius, -height / 2f, imag * radius),
					uv0 = new Vector2((imag + 1f) * 0.5f, 0f),
				};
				point *= rotate;
			}

			for (int i = 0; i < segments; ++i)
			{
				int idx = i * 2;
				int v0 = vertStart + (idx + 0);
				int v1 = vertStart + (idx + 1);
				int v2 = vertStart + ((idx + 2) % (segments * 2));
				int v3 = vertStart + ((idx + 3) % (segments * 2));

				indices[indexStart + (i * 6) + 0] = v0;
				indices[indexStart + (i * 6) + 1] = v2;
				indices[indexStart + (i * 6) + 2] = v1;
				indices[indexStart + (i * 6) + 3] = v3;
				indices[indexStart + (i * 6) + 4] = v1;
				indices[indexStart + (i * 6) + 5] = v2;
			}
		}
	}
}