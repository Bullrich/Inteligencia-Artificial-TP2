using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class AStar {
	static float Heuristic(MapNode node, MapNode end) {
		//ALUM: Funcion utilitaria. Calcula la heurística correspondiente. DEBE ser distinta de 0.
		return 0f;
	}

	static MapNode RemoveBest(HashSet<MapNode> nodes, Dictionary<MapNode, float> heuristicCosts) {
		//ALUM: Funcion utilitaria. Extrae el mejor candidato de los nodos
		return null;
	}

	static public Stack<MapNode> Run(MapNode start, MapNode end) {
		//ALUM: Implementar A* desde start a end aquí. Debe retornar un stack de los pasos para el mejor camino.
		return new Stack<MapNode>();
	}

}
