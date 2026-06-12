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
    private const int MaxDepth = 4;
    private const double WinUtility = 100000;

    [Range(1, 3)]
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
        // Generamos el nodo raiz del arbol (MAX).
        Node root = new Node(board);
        root.type = Constants.MAX;

        // N: Generamos los movimientos posibles de la IA desde el tablero actual.
        List<int> selectableTiles = boardManager.FindSelectableTiles(board, turn);

        if (selectableTiles.Count == 0)
        {
            return -1;
        }

        int bestMoveIndex = 0;
        double bestUtility = double.NegativeInfinity;

        for (int i = 0; i < selectableTiles.Count; i++)
        {
            int move = selectableTiles[i];
            Node child = new Node(root.board);
            child.parent = root;
            child.type = Constants.MIN;
            root.childList.Add(child);

            boardManager.Move(child.board, move, turn);

            double utility = Minimax(
                child,
                MaxDepth - 1,
                -turn,
                false,
                double.NegativeInfinity,
                double.PositiveInfinity);

            if (utility > bestUtility)
            {
                bestUtility = utility;
                bestMoveIndex = i;
            }
        }

        return selectableTiles[bestMoveIndex];
    }

    // N: Selecciona la heuristica usada para evaluar los nodos limite.
    private double CalculateUtility(Tile[] board)
    {
        switch (heuristicType)
        {
            case 1:
                return HeuristicH1(board);
            case 2:
                return HeuristicH2(board);
            default:
                return HeuristicH3(board);
        }
    }

    // N: H1 basica: diferencia de fichas entre la IA y el rival.
    private double HeuristicH1(Tile[] board)
    {
        int myPieces = boardManager.CountPieces(board, turn);
        int enemyPieces = boardManager.CountPieces(board, -turn);

        return myPieces - enemyPieces;
    }

    // N: H2 estrategica: diferencia de movilidad.
    private double HeuristicH2(Tile[] board)
    {
        int myMoves = boardManager.FindSelectableTiles(board, turn).Count;
        int enemyMoves = boardManager.FindSelectableTiles(board, -turn).Count;

        return myMoves - enemyMoves;
    }

    // N: H3 avanzada: control posicional de esquinas y su entorno.
    private double HeuristicH3(Tile[] board)
    {
        double score = 0;
        int[] corners = { 0, 7, 56, 63 };
        int[,] adjacentToCorner =
        {
            { 1, 8, 9 },
            { 6, 14, 15 },
            { 48, 49, 57 },
            { 54, 55, 62 }
        };

        for (int i = 0; i < corners.Length; i++)
        {
            int corner = corners[i];

            if (board[corner].value == turn)
            {
                score += 100;
            }
            else if (board[corner].value == -turn)
            {
                score -= 100;
            }
            else
            {
                // Estas casillas solo son peligrosas mientras la esquina siga libre.
                for (int j = 0; j < adjacentToCorner.GetLength(1); j++)
                {
                    int tile = adjacentToCorner[i, j];

                    if (board[tile].value == turn)
                    {
                        score -= 25;
                    }
                    else if (board[tile].value == -turn)
                    {
                        score += 25;
                    }
                }
            }
        }

        // Desempate secundario; la estrategia principal sigue siendo posicional.
        score += HeuristicH1(board) * 0.5;

        return score;
    }

    // N: Devuelve un valor definitivo cuando la partida ha terminado.
    private double CalculateTerminalUtility(Tile[] board)
    {
        int pieceDifference = boardManager.CountPieces(board, turn)
            - boardManager.CountPieces(board, -turn);

        if (pieceDifference > 0)
        {
            return WinUtility + pieceDifference;
        }

        if (pieceDifference < 0)
        {
            return -WinUtility + pieceDifference;
        }

        return 0;
    }

    // N: Implementa Minimax con poda alfa-beta.
    // alpha guarda la mejor opción hasta ahora para MAX.
    // beta guarda la mejor opción hasta ahora para MIN.
    // Si beta <= alpha, se corta la exploración
    private double Minimax(Node node, int depth, int currentTurn, bool isMax, double alpha, double beta)
    {
        List<int> moves = boardManager.FindSelectableTiles(node.board, currentTurn);

        // Si el jugador actual no puede mover, comprobamos si el rival puede mover.
        if (moves.Count == 0)
        {
            List<int> opponentMoves = boardManager.FindSelectableTiles(node.board, -currentTurn);

            if (opponentMoves.Count == 0)
            {
                node.utility = CalculateTerminalUtility(node.board);
                return node.utility;
            }

            if (depth > 0)
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

        // Caso base: si llegamos a la profundidad maxima, evaluamos el tablero.
        if (depth == 0)
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

                alpha = System.Math.Max(alpha, bestUtility);

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

                beta = System.Math.Min(beta, bestUtility);

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

}
