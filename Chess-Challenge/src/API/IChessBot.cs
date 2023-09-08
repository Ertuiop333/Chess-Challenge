
using Chess_Challenge.src.ChatGpt;

namespace ChessChallenge.API
{
    public interface IChessBot
    {
		public NeuralNetwork Brain { get; set; }

		Move Think(Board board, Timer timer);
    }
}
