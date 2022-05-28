using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace to.ProceduralMesh
{
	public class TorusMesh : IMeshGenerator
	{
		[SerializeField]
		public float radius = 1f;
		[SerializeField]
		public float width = 0.5f;
		[SerializeField, Range(3, 20)]
		public int majorSegments = 10;
		[SerializeField, Range(3, 20)]
		public int minorSegments = 8;

		public Mesh Generate()
		{
			int vc = minorSegments * majorSegments;
			int ic = minorSegments * majorSegments * 6;

			var verts = new NativeArray<MeshUtil.VertexLayout>(vc, Allocator.Temp);
			var ilist = new List<int>(ic);

			float majorDelta = Mathf.PI * 2 / majorSegments;
			float minorDelta = Mathf.PI * 2 / minorSegments;
			
			// circle
			Vector2[] circle = new Vector2[minorSegments];
			for (int i = 0; i < minorSegments; ++i)
			{
				float phi = Mathf.PI * 2 * i / minorSegments;
				circle[i] = new Vector2(Mathf.Cos(phi), Mathf.Sin(phi)) * width;
			}
			// vertices
			for (int i = 0; i < majorSegments; ++i)
			{
				float theta = Mathf.PI * 2 * i / majorSegments;
				float cos = Mathf.Cos(theta);
				float sin = Mathf.Sin(theta);
				for (int j = 0; j < minorSegments; ++j)
				{
					float dist = radius + circle[j].x;
					verts[i * minorSegments + j] = new MeshUtil.VertexLayout
					{
						pos = new Vector3(cos * dist, circle[j].y, sin * dist),
						uv0 = new Vector2((sin + 1f) * 0.5f, j / (minorSegments / 2f)),
					};
				}
			}
			// indices
			for (int i0 = 0; i0 < majorSegments; ++i0)
			{
				int i1 = (i0 + 1) % majorSegments;
				for (int j0 = 0; j0 < minorSegments; ++j0)
				{
					int j1 = (j0 + 1) % minorSegments;

					int a = i0 * minorSegments + j1;
					int b = i0 * minorSegments + j0;
					int c = i1 * minorSegments + j1;
					int d = i1 * minorSegments + j0;
					ilist.Add(a);
					ilist.Add(c);
					ilist.Add(b);
					ilist.Add(d);
					ilist.Add(b);
					ilist.Add(c);
				}
			}

			return MeshUtil.SetupTriangles(verts, ilist);
		}
	}
}