using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace to.Lib.ProceduralMesh
{
	public class ConvexHull2DMesh : IMeshGenerator
	{
		[SerializeField]
		public List<Vector2> points;

		public Mesh Generate()
		{
			var hullPoints = GetHullPoints(points);

			var verts = new NativeArray<MeshUtil.VertexLayout>(hullPoints.Count, Allocator.Temp);
			for (int i = 0; i < hullPoints.Count; ++i)
			{
				verts[i] = new MeshUtil.VertexLayout
				{
					pos = hullPoints[i],
				};
			}

			// fan triangulation
			List<int> ilist = new List<int>((hullPoints.Count - 2) * 3);
			for (int i = 0; i < hullPoints.Count - 2; ++i)
			{
				ilist.Add(0);
				ilist.Add(i + 1);
				ilist.Add(i + 2);
			}

			return MeshUtil.SetupMesh(verts, ilist);
		}

		static private List<Vector2> GetHullPoints(List<Vector2> points)
		{
			if (points == null || points.Count < 3) { return null; }

			// Jarvis's March
			// choose edge point by min(p.x)
			var sortedPoints = points.OrderBy(p => p.x).ToArray();
			List<Vector2> hullPonts = new List<Vector2> { sortedPoints[0] };
			int currentIdx = 0;
			Vector2 prev = sortedPoints[0] + Vector2.down;
			do
			{
				// find the least angle point on the right side
				int nextIdx = 0;
				float maxAngle = float.MinValue;
				Vector2 v1 = (sortedPoints[currentIdx] - prev);
				for (int i = 0; i < sortedPoints.Length; i++)
				{
					if (i == currentIdx) { continue; }

					Vector2 v2 = (sortedPoints[i] - sortedPoints[currentIdx]);
					float angle = Vector2.SignedAngle(v1, v2);
					// left side has positive value, right side has negative
					if (angle < 0f && angle > maxAngle)
					{
						maxAngle = angle;
						nextIdx = i;
					}
				}
				// then it makes hull
				if (nextIdx != 0)
				{
					hullPonts.Add(sortedPoints[nextIdx]);
					prev = sortedPoints[currentIdx];
				}
				currentIdx = nextIdx;
			} while (0 != currentIdx && hullPonts.Count < sortedPoints.Length);

			return hullPonts;
		}
	}
}