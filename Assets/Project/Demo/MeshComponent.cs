using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using to.ProceduralMesh;

namespace to.Demo
{
	[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
	[ExecuteInEditMode]
	public class MeshComponent : MonoBehaviour
	{
		[SerializeReference]
		private IMeshGenerator meshGenerator_;

#if UNITY_EDITOR
		[ContextMenu(nameof(CreateMeshAsset))]
		private void CreateMeshAsset()
		{
			UnityEditor.ProjectWindowUtil.CreateAsset(filter_.sharedMesh, $"{this.name}.mesh");
		}
#endif

		protected MeshFilter filter_;
		protected MeshRenderer renderer_;

		public Mesh GetMesh() => filter_ != null ? filter_.sharedMesh : null;

		private void Awake()
		{
			filter_ = GetComponent<MeshFilter>();
			renderer_ = GetComponent<MeshRenderer>();
		}

		private void OnEnable()
		{
			BuildMesh();
		}

		private void OnDisable()
		{
			DeleteMesh();
		}

		public void AssignMeshGenerator(IMeshGenerator gen)
		{
			meshGenerator_ = gen;
			this.BuildMesh();
		}

		public void BuildMesh()
		{
			DeleteMesh();
			if (filter_ != null)
			{
				filter_.sharedMesh = meshGenerator_?.Generate();
			}
		}

		public void DeleteMesh()
		{
			if (filter_ != null)
			{
				if (filter_.sharedMesh != null)
				{
					if (Application.isPlaying)
					{
						Destroy(filter_.sharedMesh);
					}
					else
					{
						DestroyImmediate(filter_.sharedMesh);
					}
					filter_.sharedMesh = null;
				}
			}
		}
	}
}