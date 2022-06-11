using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace to.ProceduralMesh
{
	public sealed class PlaneMesh : IMeshGenerator
	{
		[SerializeField]
		public Vector2Int size = new Vector2Int(2, 2);
		[SerializeField]
		public Vector2Int segments = new Vector2Int(10, 10);
		[SerializeField]
		public float noiseHeight = 0.5f;
		[SerializeField]
		public Vector2 uvScale = new Vector2Int(2, 2);
		[SerializeField]
		public Vector2 uvOffset = Vector2.zero;

		Mesh IMeshGenerator.Generate()
		{
			int vc = 4 + segments.x * 2 + segments.y * 2 + segments.x * segments.y;
			int ic = (segments.x + 1) * (segments.y + 1) * 6;
			var verts = new NativeArray<MeshUtil.VertexLayout>(vc, Allocator.Temp);
			var indices = new NativeArray<int>(ic, Allocator.Temp);

			float winv = segments.x <= 0 ? 0f : 1f / segments.x;
			float hinv = segments.y <= 0 ? 0f : 1f / segments.y;
			for (int y = 0; y < segments.y + 1; ++y)
			{
				float ry = y * hinv;
				for (int x = 0; x < segments.x + 1; ++x)
				{
					float rx = x * winv;

					int index = y * (segments.x + 1) + x;
					float height = noiseHeight * Mathf.PerlinNoise(rx * uvScale.x + uvOffset.x, ry * uvScale.y + uvOffset.y);
					verts[index] = new MeshUtil.VertexLayout
					{
						pos = new Vector3((rx - 0.5f) * size.x, height, (0.5f - ry) * size.y),
						uv0 = new Vector2(rx, ry),
					};
				}
			}

			for (int y = 0; y < segments.y; ++y)
			{
				for (int x = 0; x < segments.x; ++x)
				{
					int index = y * (segments.x + 1) + x;
					int v0 = index;
					int v1 = index + 1;
					int v2 = index + 1 + (segments.x + 1);
					int v3 = index + (segments.x + 1);

					indices[index * 6 + 0] = v0;
					indices[index * 6 + 1] = v1;
					indices[index * 6 + 2] = v2;
					indices[index * 6 + 3] = v2;
					indices[index * 6 + 4] = v3;
					indices[index * 6 + 5] = v0;
				}
			}

			var mesh = MeshUtil.SetupTriangles(verts, indices);
			return mesh;
		}
	}
}