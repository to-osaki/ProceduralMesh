using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using to.ProceduralMesh;
using Unity.Collections;
using UnityEngine;

namespace to.Demo
{
	[RequireComponent(typeof(MeshFilter))]
	public class DrawLine2Mesh : MonoBehaviour
	{
		Vector2[] buffer = new Vector2[256];
		int index = -1;
		bool isDrawing => index >= 0;

		[SerializeField]
		string path;

		List<string> outputs = new List<string>();

		private void OnDestroy()
		{
			File.WriteAllLines(path, outputs);
		}

		private void Update()
		{
			if (!isDrawing)
			{
				if (Input.GetMouseButtonDown(0))
				{
					Push(Input.mousePosition);
				}
			}
			else
			{
				if (Input.GetMouseButtonUp(0))
				{
					InitMesh();
				}
				else
				{
					Push(Input.mousePosition);
				}
			}
		}

		private void Push(Vector2 pos)
		{
			buffer[++index] = pos;
		}

		private void InitMesh()
		{
			try
			{
				if (index < 16 || index >= 128) { return; }

				List<Vector2> points = new List<Vector2>(index);
				points.Add(buffer[0]);
				for (int i = 1; i < index; ++i)
				{
					var current = points[points.Count - 1];
					if (Vector2.SqrMagnitude(current - buffer[i]) > 5 * 5)
					{
						points.Add(buffer[i]);
					}
				}
				Debug.Log($"{index} -> {points.Count}");

				var verts = new NativeArray<MeshUtil.VertexLayout>(points.Count, Allocator.Temp);
				for (int i = 0; i < points.Count; ++i)
				{
					verts[i] = new MeshUtil.VertexLayout { pos = points[i], };
				}
				int[] indices = EarClippingTriangulation.GetTriangles(points.ToArray(), points.Count);
				var mesh = MeshUtil.SetupTriangles(verts, indices);
				GetComponent<MeshFilter>().sharedMesh = mesh;

				outputs.Add(string.Join(",", Enumerable.Range(0, points.Count).SelectMany(i => new string[] { $"{points[i].x}", $"{points[i].y}" })));
			}
			finally
			{
				index = -1;
			}
		}
	}
}