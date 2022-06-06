using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using to.ProceduralMesh;
using Unity.Collections;

namespace to.Demo
{
	// フォーデルベルクの九角形のタイル
	public class VoderbergTiling : MonoBehaviour
	{
		[SerializeField]
		public Material m_mat;

		public Vector2[] m_tile;

		private Mesh m_mesh;
		private Matrix4x4[] m_matrices;

		private void Start()
		{
			m_tile = CreateTile();
			var verts = new NativeArray<MeshUtil.VertexLayout>(m_tile.Length, Allocator.Temp);
			for (int i = 0; i < m_tile.Length; ++i)
			{
				verts[i] = new MeshUtil.VertexLayout { pos = m_tile[i], };
			}

			int[] indices = EarClippingTriangulation.GetTriangles(m_tile, m_tile.Length);

			m_mesh = MeshUtil.SetupTriangles(verts, indices);

			var centers = Enumerable.Range(0, 15).Select(n => Matrix4x4.Rotate(Quaternion.Euler(0, 0, n * 24)));

			var translate = Matrix4x4.Translate(Vector3.left * Mathf.Tan(84 * Mathf.Deg2Rad));
			var arounds = Enumerable.Range(0, 30).Select(n =>
			{
				float deg = 12 * n + 6;
				return Matrix4x4.Rotate(Quaternion.Euler(0, 0, deg)) * translate * Matrix4x4.Rotate(Quaternion.Euler(0, 0, 6));
			});
			var arounds2 = Enumerable.Range(0, 30).Select(n =>
			{
				float deg = 12 * n + 6;
				return Matrix4x4.Rotate(Quaternion.Euler(0, 0, deg)) * translate * Matrix4x4.Rotate(Quaternion.Euler(0, 0, 6 + 12));
			});
			m_matrices = centers.Concat(arounds).Concat(arounds2).ToArray();
		}

		private void Update()
		{
			Graphics.DrawMeshInstanced(m_mesh, 0, m_mat, m_matrices);
		}

		// https://gihyo.jp/book/2021/978-4-297-12383-3VoderbergTiling
		Vector2[] CreateTile()
		{
			// 84, 84, 12の二等辺三角形
			Vector2 C = new Vector2(0, 0);
			Vector2 B = new Vector2(0, 2);
			Vector2 A = new Vector2(Mathf.Tan(84 * Mathf.Deg2Rad), 1);

			Vector2 F = A + new Vector2(0, 2);
			Vector2 D = F - new Vector2(2.5f, 0);
			Vector2 E = C + new Vector2(2.5f, 0);

			// D, E, F を、Aを中心に時計回りに12度回転
			float rad = -12 * Mathf.Deg2Rad;
			Vector2 D_ = A + Vector2DUtil.Rotate(D - A, rad);
			Vector2 E_ = A + Vector2DUtil.Rotate(E - A, rad);
			Vector2 F_ = A + Vector2DUtil.Rotate(F - A, rad);

			return new Vector2[9] { C, E, D, F, A, F_, D_, E_, B }.Select(p => p - A).ToArray();
		}
	}
}