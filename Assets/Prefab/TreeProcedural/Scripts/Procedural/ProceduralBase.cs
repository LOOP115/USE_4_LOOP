﻿using UnityEngine;

namespace Procedural {

	[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
	[ExecuteInEditMode]
	public abstract class ProceduralBase : MonoBehaviour {

		public MeshFilter Filter {
			get {
				if(filter == null) {
					filter = GetComponent<MeshFilter>();
				}
				return filter;
			}
		}

		MeshFilter filter;

		protected virtual void Start () {
			Rebuild();
		}

		protected abstract Mesh Build();
		
		public void Rebuild() {
			if(Filter.sharedMesh != null) {
				if(Application.isPlaying) {
					Destroy(Filter.sharedMesh);
				} else {
					DestroyImmediate(Filter.sharedMesh);
				}
			} 
			Filter.sharedMesh = Build();
		}

	}
		
}

