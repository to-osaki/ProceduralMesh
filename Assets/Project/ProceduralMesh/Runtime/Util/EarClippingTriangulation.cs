using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace to.ProceduralMesh
{
	/// <summary>
	/// 耳刈り取り法による多角形の三角形分割 O(n^2)
	/// https://gist.github.com/sune2/eccd1d0576f2452f9cbe4df4759632fc#file-00_triangulation-cs
	/// </summary>
	public static class EarClippingTriangulation
	{
		/// <summary>
		/// 頂点情報
		/// </summary>
		public class PointInfo
		{
			public int index;
			public PointInfo prev;
			public PointInfo next;
			public Vector2 position;
			public bool isConvex;
			public bool isDefinitelyEar;
			public bool isInQueue;
		}

		/// <summary>
		/// 多角形vsを三角形分割して三角形情報を返す
		/// 多角形の頂点は反時計回りで与えられることを前提としている
		/// </summary>
		public static int[] GetTriangles(Vector2[] vs, int count)
		{
			// 点情報の初期化
			var ps = new PointInfo[count];
			for (int i = 0; i < count; i++)
			{
				ps[i] = new PointInfo();
				ps[i].index = i;
				ps[i].position = vs[i];
			}
			// リンクリストを構成
			for (int i = 0; i < count; i++)
			{
				ps[i].prev = ps[i >= 1 ? i - 1 : count - 1];
				ps[i].next = ps[i < count - 1 ? i + 1 : 0];
			}

			// 三角形情報の初期化
			var ears = new Queue<PointInfo>();
			for (int i = 0; i < count; i++)
			{
				UpdateInfo(ps[i], ears);
			}

			// インデックスバッファを作成
			int resultCount = (count - 2) * 3;
			var result = new int[resultCount];
			int cnt = 0;
			while (cnt < resultCount && ears.Count > 0)
			{
				var p = ears.Dequeue();

				if (p.isDefinitelyEar == false && IsEar(p) == false)
				{
					p.isInQueue = false;
					continue;
				}
				// pを含む三角形を登録
				result[cnt++] = p.prev.index;
				result[cnt++] = p.next.index;
				result[cnt++] = p.index;
				// 耳に当たるpを取り除いた状態にリンクリストを更新
				p.next.prev = p.prev;
				p.prev.next = p.next;
				// 取り除いた残りが耳になるかを判定し、繰り返す
				UpdateInfo(p.prev, ears);
				UpdateInfo(p.next, ears);
			}

			if (cnt != resultCount)
			{
				Debug.LogWarning("complex polygon detected.");
			}

			return result;
		}

		/// <summary>
		/// 頂点情報更新
		/// </summary>
		static void UpdateInfo(PointInfo p, Queue<PointInfo> ears)
		{
			p.isDefinitelyEar = false;
			if (p.isInQueue)
			{
				return;
			}
			if (p.isConvex == false)
			{
				p.isConvex = IsConvex(p);
			}
			if (p.isConvex)
			{
				if (IsEar(p))
				{
					// 耳なのでQueueに加える
					p.isDefinitelyEar = true;
					p.isInQueue = true;
					ears.Enqueue(p);
				}
			}
		}

		static float Cross(Vector2 v1, Vector2 v2)
		{
			return Vector2DUtil.Cross(v1, v2);
		}

		static bool IsConvex(PointInfo p)
		{
			return Vector2DUtil.IsConvex(p.prev.position, p.position, p.next.position);
		}

		/// <summary>
		/// 頂点pが耳かどうか
		/// </summary>
		static bool IsEar(PointInfo p)
		{
			// 耳＝内部に別の頂点を含まない
			Vector2 a = p.prev.position;
			Vector2 b = p.position;
			Vector2 c = p.next.position;
			Vector2 e1 = b - a;
			Vector2 e2 = c - b;
			Vector2 e3 = a - c;
			int endIndex = p.prev.index;
			p = p.next.next;
			while (p.index != endIndex) // 全ての点pについて、
			{
				// ３つの辺の左側＝三角形の中にはない
				var v = p.position;
				float c1 = Cross(e1, v - a);
				float c2 = Cross(e2, v - b);
				float c3 = Cross(e3, v - c);
				// 単純多角形なら、
				// c1 > 0 && c2 > 0 && c3 > 0
				// という条件を見ればいいだけなのだが、weakly simpleにできるだけ対応するために頑張った判定をしている
				if (c1 >= 0 && c2 >= 0 && c3 >= 0)
				{
					if (c1 > 0 && c2 > 0 && c3 > 0)
					{
						return false;
					}
					//if (IntersectSS(a, c, v, p.prev.position) ||
					//	IntersectSS(a, c, v, p.next.position))
					//{
					//	return false;
					//}
					//var center1 = (v + p.prev.position) * 0.5f;
					//var center2 = (v + p.next.position) * 0.5f;
					//if ((Cross(e1, a, center1) > 0 && Cross(e2, b, center1) > 0 && Cross(e3, c, center1) > 0) ||
					//	(Cross(e1, a, center2) > 0 && Cross(e2, b, center2) > 0 && Cross(e3, c, center2) > 0))
					//{
					//	return false;
					//}
				}
				p = p.next;
			}
			return true;
		}
	}
}