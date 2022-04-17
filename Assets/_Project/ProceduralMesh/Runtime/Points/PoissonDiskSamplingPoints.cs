using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace to.Lib.ProceduralMesh
{
	public class PoissonDiskSamplingPoints : IMeshGenerator
	{
		[SerializeField]
		public Vector2 size = new Vector2(3, 3);
		[SerializeField, Range(0.25f, 1f)]
		public float distance = 0.1f;
		[SerializeField, Range(1, 30)]
		public int iterationLimit = 10;

		public Mesh Generate()
		{
			List<Vector2> points;
			if (size.x > 0 && size.y > 0)
			{
				points = GetSamples();
			}
			else
			{
				points = new List<Vector2> { Vector2.zero };
			}

			var verts = new NativeArray<MeshUtil.VertexLayout>(points.Count, Allocator.Temp);
			for (int i = 0; i < points.Count; i++)
			{
				verts[i] = new MeshUtil.VertexLayout
				{
					pos = points[i]
				};
			}
			return MeshUtil.SetupPoint(verts);
		}

		private List<Vector2> GetSamples()
		{
			// Fast Poisson Disk Sampling
			// https://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf
			// make grid
			float cellSize = distance / Mathf.Sqrt(2);
			int w = Mathf.CeilToInt(size.x / cellSize);
			int h = Mathf.CeilToInt(size.y / cellSize);
			Rect area = new Rect(0, 0, size.x, size.y);
			List<Vector2> points = new List<Vector2>(w * h);
			List<Vector2> actives = new List<Vector2>(w * h);
			var grid = new Vector2?[w + 1, h + 1];

			// put first point randomly
			{
				var first = (size / 2) + GetPointInCircle(0f, Mathf.Min(size.x, size.y) / 2);
				points.Add(first);
				actives.Add(first);
				var gi = GetGridIndex(first, cellSize);
				grid[gi.x, gi.y] = first;
			}

			while (actives.Count > 0 && actives.Count < w * h)
			{
				// choose target point randomly
				int targetIdx = Random.Range(0, actives.Count);
				var target = actives[targetIdx];
				// sample around target point
				bool sampled = false;
				for (int i = 0; i < iterationLimit; ++i)
				{
					// search around sample point
					var sample = target + GetPointInCircle(distance, 2 * distance);
					if (!area.Contains(sample)) { continue; }

					var sampleIdx = GetGridIndex(sample, cellSize);
					if (grid[sampleIdx.x, sampleIdx.y].HasValue) { continue; }

					Vector2Int dim = new Vector2Int(2, 2);
					var min = sampleIdx - dim;
					var max = sampleIdx + dim;
					float sqrDist = distance * distance;

					bool discard = false;
					for (int x = Mathf.Max(min.x, 0); x < Mathf.Min(max.x, w) && !discard; ++x)
					{
						for (int y = Mathf.Max(min.y, 0); y < Mathf.Min(max.y, h) && !discard; ++y)
						{
							var tmp = grid[x, y];
							if (tmp.HasValue && (tmp.Value - sample).sqrMagnitude < sqrDist)
							{
								discard = true;
							}
						}
					}

					if (discard)
					{
						continue;
					}
					else
					{
						Debug.Assert(!grid[sampleIdx.x, sampleIdx.y].HasValue);
						points.Add(sample);
						actives.Add(sample);
						grid[sampleIdx.x, sampleIdx.y] = sample;
						sampled = true;
					}
				}
				// not sampled, disable target point
				if (!sampled)
				{
					actives.RemoveAt(targetIdx);
				}
			}
			Debug.Assert(actives.Count == 0);
			return points;
		}

		static Vector2Int GetGridIndex(Vector2 p, float cellSize)
		{
			return new Vector2Int((int)(p.x / cellSize), (int)(p.y / cellSize));
		}

		static private Vector2 GetPointInCircle(float minR, float maxR)
		{
			float theta = Random.Range(0f, Mathf.PI * 2);
			float r = Random.Range(minR, maxR);
			return new Vector2(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
		}
	}
}