using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Splines;

namespace to.ProceduralMesh
{
	public class SplineTubularMesh : IMeshGenerator
	{
		[SerializeField]
		public float radius = 1f;
		[SerializeField]
		public AnimationCurve radiusCurve = AnimationCurve.Constant(0, 1, 1);
		[SerializeField, Range(3, 50)]
		public int tubuarSegments = 20;
		[SerializeField, Range(3, 20)]
		public int radialSegments = 6;
		[SerializeField]
		public SplineContainer spline;

		public struct FrenetFrame
		{
			public Vector3 pos;
			public Vector3 tangent;
			public Vector3 normal;
			public Vector3 binormal;

			public FrenetFrame(Vector3 pos, Vector3 tangent, Vector3 normal) : this()
			{
				this.pos = pos;
				this.tangent = tangent;
				this.normal = normal;
				this.binormal = Vector3.Cross(tangent, normal);
			}
		}

		public Mesh Generate()
		{
			int vc = (tubuarSegments + 1) * (radialSegments + 1);
			int ic = 6 * radialSegments * tubuarSegments;
			var verts = new NativeArray<MeshUtil.VertexLayout>(vc, Allocator.Temp);
			var indices = new NativeArray<int>(ic, Allocator.Temp);

			FrenetFrame[] frenetFrames = new FrenetFrame[tubuarSegments + 1];
			for (int ti = 0; ti <= tubuarSegments; ++ti)
			{
				float t = (float)ti / tubuarSegments;
				if (spline.Evaluate(t, out var _pos, out var _tangent, out var _normal))
				{
					frenetFrames[ti] = new FrenetFrame(
						new Vector3(_pos.x, _pos.y, _pos.z),
						new Vector3(_tangent.x, _tangent.y, _tangent.z).normalized,
						new Vector3(_normal.x, _normal.y, _normal.z).normalized);
				}
			}

			// generate verts
			for (int ti = 0; ti <= tubuarSegments; ++ti)
			{
				float t = (float)ti / tubuarSegments;
				var pos = frenetFrames[ti].pos;
				var normal = frenetFrames[ti].normal;
				var binormal = frenetFrames[ti].binormal;
				for (int ri = 0; ri <= radialSegments; ++ri)
				{
					float r = (float)ri / radialSegments;
					float rad = r * Mathf.PI * 2;
					(float cos, float sin) = Vector2DUtil.CosSin(rad);

					verts[ti * radialSegments + ri] = new MeshUtil.VertexLayout
					{
						pos = pos + (cos * normal + sin * binormal) * (radius * radiusCurve.Evaluate(t)),
						uv0 = new Vector2(t, r),
					};
				}
			}

			// calculate indices
			for (int ti = 0; ti < tubuarSegments; ++ti)
			{
				int vertStart = ti * radialSegments;
				for (int ri = 0; ri < radialSegments; ++ri)
				{
					int v0 = vertStart + ri;
					int v1 = vertStart + radialSegments + ri;
					int v2 = vertStart + ri + 1;
					int v3 = vertStart + radialSegments + ri + 1;

					int indexStart = (ti * radialSegments + ri) * 6;
					indices[indexStart + 0] = v0;
					indices[indexStart + 1] = v2;
					indices[indexStart + 2] = v1;
					indices[indexStart + 3] = v3;
					indices[indexStart + 4] = v1;
					indices[indexStart + 5] = v2;
				}
			}

			var mesh = MeshUtil.SetupTriangles(verts, indices);
			return mesh;
		}
	}
}