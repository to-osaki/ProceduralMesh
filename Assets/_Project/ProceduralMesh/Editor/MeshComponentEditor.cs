using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace to.Lib.ProceduralMesh
{
	[CustomEditor(typeof(MeshComponent), editorForChildClasses: true)]
	public class MeshComponentEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var self = this.target as MeshComponent;

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