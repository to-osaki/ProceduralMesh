using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace to.ProceduralMesh
{
	public class EarClipingMesh : IMeshGenerator
	{
		[SerializeField]
		public Vector2[] points;

		public void SetPoints(Vector2[] points)
		{
			this.points = points;
		}

		public Mesh Generate()
		{
			Rect bounding = new Rect(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);
			foreach (var p in points)
			{
				if (p.x < bounding.xMin)
				{
					bounding.xMin = p.x;
				}
				if (p.x > bounding.xMax)
				{
					bounding.xMin = p.x;
				}
				if (p.y < bounding.yMin)
				{
					bounding.yMin = p.y;
				}
				if (p.y > bounding.yMax)
				{
					bounding.yMax = p.y;
				}
			}

			int[] indices = EarClippingTriangulation.GetTriangles(points, points.Length);

			var verts = new NativeArray<MeshUtil.VertexLayout>(points.Length, Allocator.Temp);
			for (int i = 0; i < points.Length; ++i)
			{
				var p = points[i];
				verts[i] = new MeshUtil.VertexLayout
				{
					pos = p,
					uv0 = new Vector2((p.x - bounding.xMin) / bounding.width, (p.y - bounding.yMin) / bounding.height),
				};
			}

			return MeshUtil.SetupTriangles(verts, indices);
		}
	}
}