using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Bot
{

	private static void tryGenerateMove(int loc, int offset, Game src, ref List<Game> into){
		Game move = new Game(src); //copy game
		Board.makeMove(loc, loc + offset, ref move); //try generate move
		if (move.turn != src.turn) //if move is valid, turn would progress
			into.Add(move); //add to possible moves
	}

	private static void generateNewBoards(Game src, ref List<Game> into)
	{
		ulong m = 1;
		for (int loc = 0; loc < 64; loc++)
		{
			m <<= 1;
			if (src.turn % 2 == 0)
			{
				if ((src.bottom & (m >> 1)) == 0) //dont try if piece is'nt there
					continue;
			}
            else
            {
				if ((src.top & (m >> 1)) == 0) //dont try if piece is'nt there
					continue;
			}
			tryGenerateMove(loc, 7, src, ref into); //try to generate all moves
			tryGenerateMove(loc, 9, src, ref into);
			tryGenerateMove(loc, -7, src, ref into);
			tryGenerateMove(loc, -9, src, ref into);
			tryGenerateMove(loc, 14, src, ref into);
			tryGenerateMove(loc, 18, src, ref into);
			tryGenerateMove(loc, -14, src, ref into);
			tryGenerateMove(loc, -18, src, ref into);
		}
    }


	private static int eval(Game g)
	{
		if (g.bottom == 0) //loss
			return int.MinValue;
		if (g.top == 0) //win
			return int.MaxValue;

		int pieces = (Util.count(g.bottom) - Util.count(g.top)) * 20;
		int kings = (Util.count(g.bottom & g.kings) - Util.count(g.top & g.kings)) * 30;

		ulong center = 0xC3C3C3C30000;
		int centerControl = (Util.count(center & g.bottom) - Util.count(center & g.top)) * 2;

		return pieces + kings + centerControl;
	}

	private static int minimax(Game g, int depth, int alpha, int beta)
	{
		if (depth <= 0)
		{
			if (g.hasEaten) //for trading pieces
				depth = 1;
			else
				return eval(g);
		}

		List<Game> possibilities = new List<Game>();
		generateNewBoards(g, ref possibilities);

		if (possibilities.Count == 0)
			return eval(g);

		if (g.turn % 2 == 0) //sort efficiently moves by eval for better pruning
			possibilities = possibilities
				.Select(m => new { Game = m, Eval = eval(m) })
				.OrderByDescending(x => x.Eval)
				.Select(x => x.Game)
				.ToList();
		
		else
			possibilities = possibilities
				.Select(m => new { Game = m, Eval = eval(m) })
				.OrderBy(x => x.Eval)
				.Select(x => x.Game)
				.ToList();
		

		if (g.turn % 2 == 0) //bottom turn so max
		{
			int maxEval = int.MinValue;
			foreach (Game move in possibilities)
			{
				int currentEval = minimax(move, depth - 1, alpha, beta);
				maxEval = Mathf.Max(maxEval, currentEval);
				alpha = Mathf.Max(alpha, currentEval);

				if (beta <= alpha) 
					break;
			}
			return maxEval;
		}
		else //top turn so min
		{
			int minEval = int.MaxValue;
			foreach (Game move in possibilities)
			{
				int currentEval = minimax(move, depth - 1, alpha, beta);
				minEval = Mathf.Min(minEval, currentEval);
				beta = Mathf.Min(beta, currentEval);

				if (beta <= alpha) 
					break;
			}
			return minEval;
		}

	}

	public static Game bestMove(Game g, float maxSeconds)
	{
		List<Game> possibilities = new List<Game>();
		generateNewBoards(g, ref possibilities);

		if (possibilities.Count == 0) return null;

		possibilities = possibilities //sort moves by eval
			.Select(m => new { Game = m, Eval = eval(m) })
			.OrderBy(x => x.Eval)
			.Select(x => x.Game)
			.ToList();

		List<Game> bestMoves = new List<Game>(); //best moves found
		int bestValue = int.MaxValue;

		var timer = System.Diagnostics.Stopwatch.StartNew();
		int depth;

		for (depth = 1; depth <= maxSeconds; depth++) //start with depth 1, and keep trying deeper
		{
			int currentBestValue = int.MaxValue;
			List<Game> currentBestMoves = new List<Game>(); //current depth best
			int alpha = int.MinValue;
			int beta = int.MaxValue;

			foreach (Game move in possibilities)
			{
				if (timer.Elapsed.TotalSeconds >= maxSeconds) break; //out of time

				int currentEval = minimax(move, depth - 1, alpha, beta);

				if (currentEval < currentBestValue) //best move
				{
					currentBestValue = currentEval;
					currentBestMoves.Clear();
					currentBestMoves.Add(move);
				}
				else if (currentEval == currentBestValue) //same eval move
					currentBestMoves.Add(move);
				

				beta = Mathf.Min(beta, currentBestValue);
				if (beta <= alpha) break;
			}

			if (currentBestValue < bestValue) //new best move found
			{
				bestValue = currentBestValue;
				bestMoves = new List<Game>(currentBestMoves);
			}
			
			if (timer.Elapsed.TotalSeconds >= maxSeconds) break;
		}

		Debug.Log($"depth {depth} in {timer.Elapsed.TotalSeconds:F2}s");
		return bestMoves[Random.Range(0, bestMoves.Count)];
			
	}
}
