using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace to.ProceduralMesh
{
	[CustomEditor(typeof(MeshComponent), editorForChildClasses: true)]
	public class MeshComponentEditor : Editor
	{
		UnityEditor.TypeCache.TypeCollection Types_;

		private void OnEnable()
		{
			Types_ = UnityEditor.TypeCache.GetTypesDerivedFrom<IMeshGenerator>();
		}

		public override void OnInspectorGUI()
		{
			var self = this.target as MeshComponent;

			foreach (var type in Types_)
			{
				if (GUILayout.Button($"Create {type.Name}"))
				{
					self.AssignMeshGenerator(System.Activator.CreateInstance(type) as IMeshGenerator);
				}
			}

			using (var check = new EditorGUI.ChangeCheckScope())
			{
				base.OnInspectorGUI();
				if (check.changed)
				{
					self.BuildMesh();
				}
			}
		}
	}
}