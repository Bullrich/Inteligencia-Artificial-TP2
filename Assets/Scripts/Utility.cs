using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public static class Utility {
	//ALUM: Crear los vectores de movimiento aquí

	

	//Trunca un vector a un largo máximo
	public static Vector3 Truncate(Vector3 vec, float maxMag) {
		var mag = vec.magnitude;
		if(mag < float.Epsilon) return vec;
		else return vec * Mathf.Min(1f, maxMag/mag);
	}

	//Baraja de elementos de un array dinámico
	public static void KnuthShuffle<T>(List<T> array) {
		for(int i = 0; i<array.Count-1; i++) {
			var j = Random.Range(i, array.Count);
			if(i != j) {
				var temp = array[j];
				array[j] = array[i];
				array[i] = temp;
			}
		}
	}

	//Dibuja una flecha gizmo con dirección (en vez de solo una linea)
	public static void GizmoArrow(Vector3 from, Vector3 to, float scale = 0.25f, float gap = 0.15f) {
		var dir = to - from;
		to -= dir.normalized * gap;
		var offset = Vector3.Cross(dir.normalized, Vector3.up) * scale;
		var arrowLeft = to - dir.normalized * scale + offset;
		var arrowRight = to - dir.normalized * scale - offset;

		Gizmos.DrawLine(from, to);
		Gizmos.DrawLine(to, arrowLeft);
		Gizmos.DrawLine(to, arrowRight);
	}
}
