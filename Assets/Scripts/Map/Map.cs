using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Map : MonoBehaviour {
	public int width=10, height=10;					//Ancho y alto de la grilla en celdas (enteros)
	public float startX, startZ;					//Inicio de la grilla en coordenadas de mundo
	public float raycastStartY = 120f;				//Donde iniciar los raycasts que "llueven"
	public float cellSize = 1f;						//Tamaño (ancho y largo) de una celda
	public float maxAngle = 40f;					//Angulo maximo entre un nodo y otro respecto del piso (para conectarlo)
	public float safetyRadius = 20f;				//Radio de seguridad (evitar spawn y patrol en el mismo)
	public Vector3 nodeOffset = Vector3.up;			//Offset para el nodo (cuanto correrlo desde el punto de intersección entre rayo y terreno)

	public MapNode[,] grid { get; private set; }	//Grilla [X,Z]

	Vector3 _gizmoSafetyCenter;

	public void Build(Vector3 safetyCenter) {
		if(cellSize < 1f) cellSize = 1f;		//Por seguridad para evitar loops infinitos.
		_gizmoSafetyCenter = safetyCenter;
		
		//ALUM: ¡Recuerde float.MaxValue para valores muy grandes al hacer raycasts!

		//ALUM: Creación de grilla acorde a lo configurado
		//ALUM: "Lluvia" de raycasts para crear los nodos en la altura correspondiente (no se olvide de "nodeOffset")

		

		//ALUM: Por cada nodo, realizar una conexión de aristas si el ángulo es correcto (NOTA: utilizar un vector de movimiento de 8 direcciones)
		

		//ALUM: Recorrer desde el nodo de la grilla en [0,0] (cerca de la casa del granjero) para descartar nodos inaccesibles
		

		//ALUM: Marcar nodos en el radio seguro también como inaccesibles
		
	}
	
	public MapNode FindClosestNode(Vector3 point) {
		//ALUM: Convertir un punto del mundo en el nodo que esté mas cercano (Pista: Debe redondear). NUNCA retorne null.
		return null;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.black;
		Gizmos.DrawWireCube(
			  new Vector3(startX+width/2*cellSize, raycastStartY/2f, startZ+height/2*cellSize)
			, new Vector3(width*cellSize, raycastStartY, height*cellSize)
		);

		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(_gizmoSafetyCenter, safetyRadius);

		//ALUM: Dibujar todos los nodos y sus aristas en cyan para accesibles y rojo para inaccesibles
		
	}
}
