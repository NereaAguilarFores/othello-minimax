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
    private int maxDepth = 4;

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
        //List<int> selectableTiles = boardManager.FindSelectableTiles(board, turn);

        // N: Generamos los movimientos posibles de la IA desde el tablero actual.
        List<int> selectableTiles = boardManager.FindSelectableTiles(board, turn);

        // Si no hay movimientos posibles, devolvemos -1.
        if (selectableTiles.Count == 0)
        {
            return -1;
        }

        int bestMoveIndex = 0;
        double bestUtility = double.NegativeInfinity;

        foreach (int s in selectableTiles)
        {
            Node child = new Node(root.board);
            child.parent = root;
            child.type = Constants.MIN;
            root.childList.Add(child);

            boardManager.Move(child.board, s, turn);

            // Después del movimiento de la IA, simulamos la respuesta del rival.
            double utility = Minimax(child, maxDepth - 1, -turn, false, double.NegativeInfinity, double.PositiveInfinity);

            if (utility > bestUtility)
            {
                bestUtility = utility;
                bestMoveIndex = selectableTiles.IndexOf(s);
            }
        }

        int movimiento = bestMoveIndex;

        Debug.Log("Movimiento elegido: " + selectableTiles[movimiento]);

        return selectableTiles[movimiento];

    }

    //N: Calcular utilidad del tablero. Cuanto mayor el valor mejor el tablero.
    //Tiene en cuenta la diferencia de fichas, la movilidad y las esquinas. LAs esquinas tienen más peso porque son posiciones muy estacbles.
    private double CalculateUtility(Tile[] board)
    {
        int myPieces = boardManager.CountPieces(board, turn);
        int enemyPieces = boardManager.CountPieces(board, -turn);
        
        int myMoves = boardManager.FindSelectableTiles(board, turn).Count;
        int enemyMoves = boardManager.FindSelectableTiles(board, -turn).Count;
        
        int[] corners = { 0, 7, 56, 63 };
        
        int cornerScore = 0;
        
        foreach (int corner in corners)
        {
            if (board[corner].value == turn)
            {
                cornerScore += 25;
            }
            else if (board[corner].value == -turn)
            {
                cornerScore -= 25;
            }
        }
        
        int pieceScore = myPieces - enemyPieces;
        int mobilityScore = myMoves - enemyMoves;
        
        return pieceScore + (mobilityScore * 2) + cornerScore;
    }

    // N: Implementa Minimax con poda alfa-beta
    // alpha guarda la mejor opción hasta ahora para MAX.
    // beta guarda la mejor opción hasta ahora para MIN.
    // Si beta <= alpha, se corta la exploración
    private double Minimax(Node node, int depth, int currentTurn, bool isMax, double alpha, double beta)
    {
        List<int> moves = boardManager.FindSelectableTiles(node.board, currentTurn);

        // profundidad máxima o sin movimientos posibles.
        if (depth == 0 || moves.Count == 0)
        {
            node.utility = CalculateUtility(node.board);
            return node.utility;
        }

        if (isMax)
        {
            double bestUtility = double.NegativeInfinity;

            foreach (int move in moves)
            {
                Node child = new Node(node.board);
                child.parent = node;
                child.type = Constants.MIN;
                node.childList.Add(child);

                boardManager.Move(child.board, move, currentTurn);

                double utility = Minimax(child, depth - 1, -currentTurn, false, alpha, beta);

                if (utility > bestUtility)
                {
                    bestUtility = utility;
                }

                alpha = Mathf.Max((float)alpha, (float)bestUtility);

                if (beta <= alpha)
                {
                    break;
                }
            }

            node.utility = bestUtility;
            node.alfa = alpha;
            node.beta = beta;

            return bestUtility;
        }
        else
        {
            double bestUtility = double.PositiveInfinity;

            foreach (int move in moves)
            {
                Node child = new Node(node.board);
                child.parent = node;
                child.type = Constants.MAX;
                node.childList.Add(child);

                boardManager.Move(child.board, move, currentTurn);

                double utility = Minimax(child, depth - 1, -currentTurn, true, alpha, beta);

                if (utility < bestUtility)
                {
                    bestUtility = utility;
                }

                beta = Mathf.Min((float)beta, (float)bestUtility);

                if (beta <= alpha)
                {
                    break;
                }
            }

            node.utility = bestUtility;
            node.alfa = alpha;
            node.beta = beta;

            return bestUtility;
        }
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
