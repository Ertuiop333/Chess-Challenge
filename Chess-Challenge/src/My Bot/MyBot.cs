using ChessChallenge.API;
using Chess_Challenge.src.MyAI;
using ChessChallenge.Application;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

public class MyBot : IChessBot {
	public ChessAI inpputDeserializer;
	Chess_Challenge.src.ChatGpt.NeuralNetwork _brain;
	Chess_Challenge.src.ChatGpt.NeuralNetwork IChessBot.Brain { get => _brain; set =>_brain = value; }

	public MyBot() {
		inpputDeserializer = new ChessAI(1000);
		_brain = new Chess_Challenge.src.ChatGpt.NeuralNetwork();
	}

	public MyBot(Chess_Challenge.src.ChatGpt.NeuralNetwork baseNeuralNetwork) {
		inpputDeserializer = new ChessAI(1000);
		_brain = baseNeuralNetwork;
	}


	public Move Think(Board board, Timer timer, ChallengeController challengeController) {
		inpputDeserializer.SetNewInputs(board);

		double theAiNumber = _brain.GetNeuralNetworkOutput(inpputDeserializer.newInput);
		Move[] move = board.GetLegalMoves();

		// if (challengeController.PlayerWhite.IsBot )

		int moveIndex = (int)Math.Round(theAiNumber * move.Length - 1);

		//double factor = AI.GenerateOutput(board);
		//Console.WriteLine(factor);

		if (moveIndex >= move.Length) {
			moveIndex = move.Length - 1;
		}

		if (moveIndex < 0) {
			moveIndex = 0;
		}

		return move[moveIndex];
	}

	public void ReverseValidMoves() {

	}
}