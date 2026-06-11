using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Tile[] board = new Tile[Constants.NumTiles];
    public Node parent;
    public List<Node> childList = new List<Node>();
    public int type;//Constants.MIN o Constants.MAX
    public double utility;
    public double alfa;
    public double beta;

    public Node(Tile[] tiles)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            this.board[i] = new Tile();
            this.board[i].value = tiles[i].value;
        }

    }    

}

public class Player : MonoBehaviour
{
    public int turn;    
    private BoardManager boardManager;

    void Start()
    {
        boardManager = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
    }
       
    /*
     * Entrada: Dado un tablero
     * Salida: Posición donde mueve  
     */
    public int SelectTile(Tile[] board)
    {        
        Debug.Log("ENTRA EN SELECT TILE - TURNO IA: " + turn);
        //Generamos el nodo raíz del árbol (MAX)
        Node root = new Node(board);
        root.type = Constants.MAX;

        //Generamos primer nivel de nodos hijos
        List<int> selectableTiles = boardManager.FindSelectableTiles(board, turn);

        foreach (int s in selectableTiles)
        {
            //Creo un nuevo nodo hijo con el tablero padre
            Node n = new Node(root.board);
            //Lo añadimos a la lista de nodos hijo
            root.childList.Add(n);
            //Enlazo con su padre
            n.parent = root;
            //En nivel 1, los hijos son MIN
            n.type = Constants.MIN;
            //Aplico un movimiento, generando un nuevo tablero con ese movimiento
            boardManager.Move(n.board, s, turn);
            //si queremos imprimir el nodo generado (tablero hijo)
            //boardManager.PrintBoard(n.board);

            // N: Generamos los movimientos posibles del rival a partir de este tablero.
            List<int> enemyMoves = boardManager.FindSelectableTiles(n.board, -turn);
            
            foreach (int enemyMove in enemyMoves)
            {
                Node grandChild = new Node(n.board);
                
                n.childList.Add(grandChild);
                
                grandChild.parent = n;
                grandChild.type = Constants.MAX;
                
                boardManager.Move(grandChild.board, enemyMove, -turn);
            }
        }

        //Selecciono un movimiento aleatorio. Esto habrá que modificarlo para elegir el mejor movimiento según MINIMAX
        //int movimiento = Random.Range(0, selectableTiles.Count);


        //N: Minimax a profundidad 2
        //Primero evaluamos los nietos.
        //Después, cada nodo MIN se queda con la menor utilidad de sus hijos.
        foreach (Node child in root.childList)
        {
            if (child.childList.Count == 0)
            {
                // Si el rival no se puede mover, evaluamos directamente este tablero
                child.utility = CalculateUtility(child.board);
            }
            else
            {
                double minUtility = child.childList[0].utility;
                
                for (int i = 0; i < child.childList.Count; i++)
                {
                    child.childList[i].utility = CalculateUtility(child.childList[i].board);
                    
                    if (i == 0 || child.childList[i].utility < minUtility)
                    {
                        minUtility = child.childList[i].utility;
                    }
                }
                
                child.utility = minUtility;
            }
        }
        
        // Elegimos el hijo con mayor utilidad, ya que la raíz es un nodo MAX.
        
        int movimiento = GetBestChildIndex(root);

        return selectableTiles[movimiento];

    }

    //N: Calcular utilidad del tablero. Cuanto mayor el valor mejor el tablero.
    //Utilidad = numero fichas propias - numero fichas rival
    private double CalculateUtility(Tile[] board)
    {
        int myPieces = boardManager.CountPieces(board, turn);
        int enemyPieces = boardManager.CountPieces(board, -turn);

        return myPieces - enemyPieces;
    }

    //N: Devuelve el hijo con mejor utilidad para un nodo MAX.
    // Se usa en la raíz para escoger el movimieto más favorable para la IA.
    private int GetBestChildIndex(Node root)
    {
        int bestIndex = 0;
        double bestUtility = root.childList[0].utility;

        for (int i = 1; i < root.childList.Count; i++)
        {
            if (root.childList[i].utility > bestUtility)
            {
                bestUtility = root.childList[i].utility;
                bestIndex = i;
            }
        }

        return bestIndex;
    }
}
