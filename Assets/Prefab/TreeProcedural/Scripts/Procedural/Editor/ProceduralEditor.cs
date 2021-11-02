using UnityEditor;

namespace Procedural {

	[CustomEditor (typeof(ProceduralBase), true)]
	public class ProceduralEditor : Editor {

		public override void OnInspectorGUI() {

			EditorGUI.BeginChangeCheck();

			base.OnInspectorGUI();
			
			if(EditorGUI.EndChangeCheck()) {
				var pg = target as ProceduralBase;
				pg.Rebuild();
			}
		}

	}
		
}

