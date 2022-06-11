using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace to.ProceduralMesh
{
	static public class Vector2DUtil
	{
		static public void Deconstruct(this Vector2 self, out float x, out float y)
		{
			x = self.x;
			y = self.y;
		}

		public struct Line
		{
			public Line(Vector2 start, Vector2 end)
			{
				this.start = start;
				this.v = end - start;
				this.vn = this.v.normalized;
				this.normal = new Vector2(-v.y, v.x).normalized; // 法線の方程式 -1/f'(a)*(x - a)+f(a)
			}

			public Vector2 start;
			public Vector2 v;
			public Vector2 vn;
			public Vector2 normal;

			public Vector2 end => start + v;
		}

		public struct HitInfo
		{
			public bool isHit;
			public Vector2 p;
			public float t1;
			public float t2;
		}


		static public float Cross(Vector2 v1, Vector2 v2)
		{
			return v1.x * v2.y - v1.y * v2.x;
		}
		static public float Dot(Vector2 v1, Vector2 v2)
		{
			return Vector2.Dot(v1, v2);
		}

		static public Vector2 CosSin(float rad)
		{
			return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
		}

		/// <summary>
		/// 反時計回りに回転
		/// </summary>
		static public Vector2 Rotate(Vector2 v, float rad)
		{
			float a = v.x;
			float b = v.y;
			float c = Mathf.Cos(rad);
			float d = Mathf.Sin(rad);
			return new Vector2(a * c - b * d, a * d + b * c);
		}

		/// <summary>
		/// 前後の頂点と比較してselfが凸頂点かどうか
		/// </summary>
		static public bool IsConvex(Vector2 prev, Vector2 self, Vector2 next)
		{
			return Cross(self - prev, next - prev) >= 0; // ベクトル同士が平行or衝突する
		}

		static public bool Intersect(Line l1, Line l2)
		{
			return Intersect(l1.start, l1.end, l2.start, l2.end);
		}

		static public bool Intersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
		{
			Vector2 p = p2 - p1;
			Vector2 q = q2 - q1;
			return Cross(p, q1 - p1) * Cross(p, q2 - p1) < 0 && Cross(q, p1 - q1) * Cross(q, p2 - q1) < 0;
		}

		static public HitInfo Hit(Line line1, Line line2)
		{
			if (Vector2.Dot(line1.normal, line2.normal) > 0f) // 法線同士が相対する場合のみ衝突とみなす
			{
				return default;
			}

			line1.Hit(line2, out HitInfo hit);
			return hit;
		}

		public static float Distance(this Line line, Vector2 p)
		{
			Vector2 begin = line.start;
			Vector2 end = line.end;

			Vector2 a = p - begin;
			Vector2 b = p - end;
			Vector2 s = line.v;

			float dbegin = Vector2.Dot(a, s);
			float dend = Vector2.Dot(b, s);
			// 直線との最近点が線分の始点より手前なら共に鈍角、終点より後なら共に鋭角となるため
			if (dbegin < 0 && dend < 0)
			{
				return a.magnitude;
			}
			else if (dbegin > 0 && dend > 0)
			{
				return b.magnitude;
			}
			else
			{
				float ld = Cross(s, a) / s.magnitude;// 直線とpとの距離
				return ld;
			}
		}

		// http://marupeke296.com/COL_2D_No10_SegmentAndSegment.html
		public static bool Hit(this Line line1, Line line2, out HitInfo hit)
		{
			hit = default;

			Vector2 v1 = line1.v;
			Vector2 v2 = line2.v;
			float c12 = Cross(v1, v2);
			if (c12 == 0f) // 平行なので交わらない
			{
				return false;
			}

			Vector2 v = line2.start - line1.start;
			float c1 = Cross(v, v1);
			float c2 = Cross(v, v2);

			float t1 = c2 / c12;
			float t2 = c1 / c12;

			hit.isHit = t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1;
			if (hit.isHit)
			{
				hit.p = line1.start + v1 * t1;
				hit.t1 = t1;
				hit.t2 = t2;
			}
			return hit.isHit;
		}
	}
}