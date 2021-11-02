using UnityEngine;

namespace Procedural {

	public class FrenetSerret {
		
		Vector3 normal, binormal, tangent;
		
		public Vector3 Normal { get { return normal; } }
		public Vector3 Binormal { get { return binormal; } }
		public Vector3 Tangent { get { return tangent; } }

		public FrenetSerret(Vector3 t, Vector3 n, Vector3 bn) {
			tangent = t;
			normal = n;
			binormal = bn;
		}
	}
		
}

