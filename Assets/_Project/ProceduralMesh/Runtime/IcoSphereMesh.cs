using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace to.Lib.ProceduralMesh
{
	public class IcoSphereMesh : IMeshGenerator
	{
		[SerializeField]
		public float radius = 1f;
		[SerializeField, Range(0, 4)]
		public int LOD = 1;

		private struct Edge
		{
			public int i0, i1;
			public System.UInt64 EdgeKey => ((uint)i0 << 16) + (uint)i1;
			public Edge(int i0, int i1)
			{
				(this.i0, this.i1) = i0 < i1 ? (i0, i1) : (i1, i0);
			}
		}

		private struct Face
		{
			public int i0, i1, i2;
			public Edge e0, e1, e2;

			public Face(int i0, int i1, int i2)
			{
				this.i0 = i0;
				this.i1 = i1;
				this.i2 = i2;
				e0 = new Edge(i0, i1);
				e1 = new Edge(i1, i2);
				e2 = new Edge(i2, i0);
			}
		}

		// https://suzulang.com/cpp-code-ico-q-1/
		// http://blog.andreaskahler.com/2009/06/creating-icosphere-mesh-in-code.html
		public Mesh Generate()
		{
			var vlist = new List<Vector3>();
			var ilist = new List<int>();
			CalcVertices(vlist, ilist);

			var verts = new NativeArray<MeshUtil.VertexLayout>(vlist.Count, Allocator.Temp);
			for (int i = 0; i < vlist.Count; ++i)
			{
				// Ico sphere is inscribed
				var nv = vlist[i].normalized;
				float angle = Vector2.Angle(Vector2.one, new Vector2(nv.x, nv.z)) / 180f;
				verts[i] = new MeshUtil.VertexLayout
				{
					pos = nv * radius,
					uv0 = new Vector2((angle + 1f) * 0.5f, (nv.y + 1f) * 0.5f),
				};
			}

			return MeshUtil.SetupMesh(verts, ilist);
		}

		private void CalcVertices(List<Vector3> vlist, List<int> ilist)
		{
			// regular icosahedron = 30 edges, 12 vertices
			//(int i, int j)[] tries = new[] {
			//	(1, 2), (2, 3), (1, 0),
			//	(1, 0), (2, 3), (0, 1),
			//	(1, 0), (0, 1), (0, 3),
			//	(1, 0), (0, 3), (2, 2), 
			//	(1, 0), (2, 2), (1, 2),

			//	(1, 2), (2, 2), (0, 2),
			//	(1, 2), (0, 2), (0, 0),
			//	(1, 2), (0, 0), (2, 3),
			//	(2, 3), (0, 0), (2, 1),
			//	(2, 3), (2, 1), (0, 1),

			//	(0, 1), (2, 1), (1, 1),
			//	(0, 1), (1, 1), (0, 3),
			//	(0, 3), (1, 1), (2, 0),
			//	(0, 3), (2, 0), (2, 2),
			//	(2, 2), (2, 0), (0, 2),

			//	(0, 2), (2, 0), (1, 3),
			//	(0, 2), (1, 3), (0, 0),
			//	(0, 0), (1, 3), (2, 1),
			//	(2, 1), (1, 3), (1, 1),
			//	(1, 1), (1, 3), (2, 0),
			//};
			Face[] faces = new[]
			{
				new Face(6,11,4),
				new Face(4,11,1),
				new Face(4,1,3),
				new Face(4,3,10),
				new Face(4,10,6),

				new Face(6,10,2),
				new Face(6,2,0),
				new Face(6,0,11),
				new Face(11,0,9),
				new Face(11,9,1),

				new Face(1,9,5),
				new Face(1,5,3),
				new Face(3,5,8),
				new Face(3,8,10),
				new Face(10,8,2),

				new Face(2,8,7),
				new Face(2,7,0),
				new Face(0,7,9),
				new Face(9,7,5),
				new Face(5,7,8),
			};

			vlist.Clear();
			ilist.Clear();

			// three planes of 1 : (1+Å„5)/2
			float e = (1 + Mathf.Sqrt(5f)) / 2;

			int totalVertices = 12;
			int totalFaces = 20;
			{
				int add = 30;
				for (int lod = 0; lod < LOD; ++lod)
				{
					totalVertices += add;
					add *= 4;
					totalFaces *= 4;
				}
			}
			vlist.Capacity = totalVertices;
			vlist.AddRange(new Vector3[]
			{
				new Vector3(e, 0, -1), new Vector3(-e, 0, -1),new Vector3(e, 0, 1),new Vector3(-e, 0, 1),
				new Vector3(-1, e, 0), new Vector3(-1, -e, 0), new Vector3(1, e, 0), new Vector3(1, -e, 0),
				new Vector3(0, -1, e), new Vector3(0, -1, -e),new Vector3(0, 1, e),new Vector3(0, 1, -e),
			});

			int totalEdges = totalFaces + totalVertices - 2;
			var viByEdge = new Dictionary<System.UInt64, int>(totalEdges);
			for (int lod = 0; lod < LOD; ++lod)
			{
				Face[] populatedFaces = new Face[faces.Length * 4];
				for (int fi = 0; fi < faces.Length; ++fi)
				{
					var face = faces[fi];
					// populate 1 face to 4 faces
					Edge[] edges = new Edge[3] { face.e0, face.e1, face.e2 };
					int[] populatedVertexIndexes = new int[3];
					for (int ei = 0; ei < edges.Length; ei++)
					{
						Edge edge = edges[ei];
						System.UInt64 edgeKey = edge.EdgeKey;
						int mi = -1;
						if (!viByEdge.TryGetValue(edgeKey, out mi))
						{
							var newV = vlist[edge.i0] * 0.5f + vlist[edge.i1] * 0.5f;
							vlist.Add(newV);
							mi = vlist.Count - 1;
							viByEdge.Add(edgeKey, mi);
						}
						populatedVertexIndexes[ei] = mi;
					}
					populatedFaces[fi * 4 + 0] = new Face(face.i0, populatedVertexIndexes[0], populatedVertexIndexes[2]);
					populatedFaces[fi * 4 + 1] = new Face(face.i1, populatedVertexIndexes[1], populatedVertexIndexes[0]);
					populatedFaces[fi * 4 + 2] = new Face(face.i2, populatedVertexIndexes[2], populatedVertexIndexes[1]);
					populatedFaces[fi * 4 + 3] = new Face(populatedVertexIndexes[0], populatedVertexIndexes[1], populatedVertexIndexes[2]);
				}
				faces = populatedFaces;
			}

			ilist.Capacity = faces.Length * 3;
			for (int i = 0; i < faces.Length; ++i)
			{
				ilist.Add(faces[i].i0);
				ilist.Add(faces[i].i1);
				ilist.Add(faces[i].i2);
			}
		}
	}
}