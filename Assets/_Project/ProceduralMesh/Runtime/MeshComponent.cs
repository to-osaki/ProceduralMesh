using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace to.Lib.ProceduralMesh
{
	[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
	[ExecuteInEditMode]
	public class MeshComponent : MonoBehaviour
	{
		private void MenuGenerateMesh<T>() where T : IMeshGenerator, new()
		{
			meshGenerator_ = new T();
			this.BuildMesh();
		}
		private void MenuQuad() => MenuGenerateMesh<QuadMesh>();
		private void MenuPlane() => MenuGenerateMesh<PlaneMesh>();
		private void MenuDome() => MenuGenerateMesh<DomeMesh>();
		private void MenuCylinder() => MenuGenerateMesh<CylinderMesh>();
		private void MenuSphere() => MenuGenerateMesh<IcoSphereMesh>();
		private void MenuConvexHullXY() => MenuGenerateMesh<ConvexHull2DMesh>();
		private void MenuTorus() => MenuGenerateMesh<TorusMesh>();

		[SerializeReference]
		[ContextMenuItem(nameof(MenuQuad), nameof(MenuQuad))]
		[ContextMenuItem(nameof(MenuPlane), nameof(MenuPlane))]
		[ContextMenuItem(nameof(MenuDome), nameof(MenuDome))]
		[ContextMenuItem(nameof(MenuCylinder), nameof(MenuCylinder))]
		[ContextMenuItem(nameof(MenuSphere), nameof(MenuSphere))]
		[ContextMenuItem(nameof(MenuConvexHullXY), nameof(MenuConvexHullXY))]
		[ContextMenuItem(nameof(MenuTorus), nameof(MenuTorus))]
		private IMeshGenerator meshGenerator_;

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

		public void BuildMesh()
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
				}

				filter_.sharedMesh = meshGenerator_?.Generate();
			}
		}
	}
}