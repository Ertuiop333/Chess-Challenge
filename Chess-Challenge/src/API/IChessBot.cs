
using Chess_Challenge.src.ChatGpt;
using ChessChallenge.Application;

namespace ChessChallenge.API
{
    public interface IChessBot
    {
		public NeuralNetwork Brain { get; set; }

		Move Think(Board board, Timer timer, ChallengeController challengeController);
    }
}
