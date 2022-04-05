using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace to.Lib.ProceduralMesh
{
	public class ConvexHullXYMesh : IMeshGenerator
	{
		[SerializeField]
		public List<Vector2> points;

		public Mesh Generate()
		{
			var hullPoints = GetHullPoints();

			var verts = new NativeArray<MeshUtil.VertexLayout>(hullPoints.Count, Allocator.Temp);
			for (int i = 0; i < hullPoints.Count; ++i)
			{
				verts[i] = new MeshUtil.VertexLayout
				{
					pos = points[hullPoints[i]],
				};
			}

			// Fan triangulation
			List<int> ilist = new List<int>((hullPoints.Count - 2) * 3);
			for (int i = 0; i < hullPoints.Count - 2; ++i)
			{
				ilist.Add(0);
				ilist.Add(i + 1);
				ilist.Add(i + 2);
			}

			return MeshUtil.SetupMesh(verts, ilist);
		}

		private List<int> GetHullPoints()
		{
			if (points == null || points.Count < 3) { return new List<int>(); }

			// Jarvis's March
			float minX = float.MaxValue;
			int beginIdx = -1;
			for (int i = 0; i < points.Count; i++)
			{
				Vector2 p = points[i];
				if (p.x < minX)
				{
					minX = p.x;
					beginIdx = i;
				}
			}
			if (beginIdx < 0) { return new List<int>(); }

			int currentIdx = beginIdx;
			Vector2 prev = points[beginIdx] + Vector2.down;

			List<int> hullPonts = new List<int> { beginIdx };
			do
			{
				int nextIdx = beginIdx;
				float maxAngle = float.MinValue;
				Vector2 v1 = (points[currentIdx] - prev).normalized;
				for (int i = 0; i < points.Count; i++)
				{
					if (i == currentIdx) { continue; }

					Vector2 v2 = (points[i] - points[currentIdx]).normalized;
					float angle = Vector2.SignedAngle(v1, v2);
					
					if (angle < 0f && angle > maxAngle)
					{
						maxAngle = angle;
						nextIdx = i;
					}
				}
				if (nextIdx != beginIdx)
				{
					hullPonts.Add(nextIdx);
					prev = points[currentIdx];
				}
				currentIdx = nextIdx;
			} while (beginIdx != currentIdx && hullPonts.Count < points.Count);

			return hullPonts;
		}
	}
}