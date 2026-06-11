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
    public int heuristicType = 3; // 1 = H1, 2 = H2, 3 = H3

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
        //Debug.Log("ENTRA EN SELECT TILE - TURNO IA: " + turn);
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

        //Debug.Log("Movimiento elegido: " + selectableTiles[movimiento]);

        return selectableTiles[movimiento];

    }

    // N: Seleccionar heurística se usa para evalusr los nodos terminales
    private double CalculateUtility(Tile[] board)
    {
        if (heuristicType == 1)
        {
            return HeuristicH1(board);
        }
        else if (heuristicType == 2)
        {
            return HeuristicH2(board);
        }
        else
        {
            return HeuristicH3(board);
        }
    }

    // H1: Heurística básica
    // Evalúa únicamente la diferencia de fichas entre la IA y el rival.
    private double HeuristicH1(Tile[] board)
    {
        int myPieces = boardManager.CountPieces(board, turn);
        int enemyPieces = boardManager.CountPieces(board, -turn);

        return myPieces - enemyPieces;
    }

    // H2: Heurística estratégica
    // Evalúa cuántos movimientos tiene la IA frente al rival.
    private double HeuristicH2(Tile[] board)
    {
        int myMoves = boardManager.FindSelectableTiles(board, turn).Count;
        int enemyMoves = boardManager.FindSelectableTiles(board, -turn).Count;

        return myMoves - enemyMoves;
    }

    // H3: Heurística avanzada
    // Prioriza esquinas y penaliza casillas peligrosas cercanas a esquinas vacías.
    private double HeuristicH3(Tile[] board)
    {
        double score = 0;

        int[] corners = { 0, 7, 56, 63 };

        foreach (int corner in corners)
        {
            if (board[corner].value == turn)
            {
                score += 100;
            }
            else if (board[corner].value == -turn)
            {
                score -= 100;
            }
        }

        // Casillas peligrosas junto a esquinas.
        // Si una esquina está vacía, ocupar estas casillas puede permitir al rival capturarla.
        int[] dangerousTiles = { 1, 8, 9, 6, 14, 15, 48, 49, 57, 54, 55, 62 };

        foreach (int tile in dangerousTiles)
        {
            if (board[tile].value == turn)
            {
                score -= 25;
            }
            else if (board[tile].value == -turn)
            {
                score += 25;
            }
        }

        // Pequeño apoyo por diferencia de fichas para desempatar decisiones.
        score += HeuristicH1(board) * 0.5;

        return score;
    }

    // N: Implementa Minimax con poda alfa-beta
    // alpha guarda la mejor opción hasta ahora para MAX.
    // beta guarda la mejor opción hasta ahora para MIN.
    // Si beta <= alpha, se corta la exploración
    private double Minimax(Node node, int depth, int currentTurn, bool isMax, double alpha, double beta)
    {
        List<int> moves = boardManager.FindSelectableTiles(node.board, currentTurn);

        // Caso base: si llegamos a la profundidad máxima, evaluamos el tablero.
        if (depth == 0)
        {
            node.utility = CalculateUtility(node.board);
            return node.utility;
        }

        // Si el jugador actual no puede mover, comprobamos si el rival puede mover.
        if (moves.Count == 0)
        {
            List<int> opponentMoves = boardManager.FindSelectableTiles(node.board, -currentTurn);

            if (opponentMoves.Count == 0)
            {
                // Si ninguno de los dos puede mover, la partida ha terminado.
                node.utility = CalculateUtility(node.board);
                return node.utility;
            }
            else
            {
                // Si solo este jugador no puede mover, se representa el pase creando
                // un hijo con el mismo tablero y cambiando el turno.
                Node passChild = new Node(node.board);
                passChild.parent = node;
                passChild.type = isMax ? Constants.MIN : Constants.MAX;
                node.childList.Add(passChild);

                double utility = Minimax(passChild, depth - 1, -currentTurn, !isMax, alpha, beta);
                node.utility = utility;
                return utility;
            }
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
