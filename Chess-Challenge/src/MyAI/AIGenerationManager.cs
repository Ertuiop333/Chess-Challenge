using Chess_Challenge.src.ChatGpt;
using ChessChallenge.API;
using ChessChallenge.Application;
using ChessChallenge.Chess;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class AIGenerationManager {
	public List<NeuralNetwork> naturalSelection = new();
	public List<NeuralNetwork> childs = new();
	public StringBuilder aiData = new();
	private NeuralNetwork currentBestAI;
	private int lowerMoveCount = 1000;
	private int currentWinerMoveCount = 0;
	public List<NeuralNetwork> lastNaturalSelection = new();

	public void StoreAI(NeuralNetwork neuralNetwork) {
		// Store the AI in a file
		// TODO

		naturalSelection.Add(neuralNetwork);

		//string directoryPath = Path.Combine(FileHelper.AppDataPath, "Games");
		//Directory.CreateDirectory(directoryPath);
		
		//File.WriteAllText(fullPath, pgns);
		//ConsoleHelper.Log("Saved games to " + fullPath, false, ConsoleColor.Blue);

		

		if (currentWinerMoveCount < lowerMoveCount) {
			lowerMoveCount = currentWinerMoveCount;
			currentBestAI = neuralNetwork;
		}

		currentWinerMoveCount = 0;
	}

	private void SaveBestAIToText() {
		string directoryPath = Path.Combine(FileHelper.AppDataPath, "AI");
		Directory.CreateDirectory(directoryPath);
		string fileName = FileHelper.GetUniqueFileName(directoryPath, "best AI from batch # ", ".txt");
		string fullPath = Path.Combine(directoryPath, fileName);

		// serialize only the bot that has the least moves
		// Serialize the neural network to JSON
		string serializedNetwork = SerializeNeuralNetwork(currentBestAI);

		// Save the serialized network to a text file
		SaveToFile(fullPath, serializedNetwork);
	}

	public void SeriesEnded(int gamesPlayed) {
		SaveBestAIToText();

		if (naturalSelection.Count == 0) {
			if (lastNaturalSelection.Count > 0) {
				naturalSelection = lastNaturalSelection;
			}
			else
				return;
		}
		// #gamesPlayed bots to create
		int childCount = (gamesPlayed * 2) / naturalSelection.Count;
		for (int i = 0; i < naturalSelection.Count; i++) {
			childs.AddRange(naturalSelection[i].GiveBirth(childCount).ToList());
		}

		lastNaturalSelection.AddRange(naturalSelection);
		naturalSelection.Clear();
	}

	public void GameEnded(ChessPlayer botWhite, ChessPlayer botBlack, GameResult result, bool botAPlayingWhite, int blackNumberOfMoves, int whiteNumberOfMoves) {
		// store ai if won
		if (result == GameResult.BlackIsMated) {
			currentWinerMoveCount = whiteNumberOfMoves;
			StoreAI(botAPlayingWhite ? botWhite.Bot.Brain : botBlack.Bot.Brain);
		} else if (result == GameResult.WhiteIsMated) {
			currentWinerMoveCount = blackNumberOfMoves;
			StoreAI(botAPlayingWhite ? botBlack.Bot.Brain : botWhite.Bot.Brain);
		}
	}

	public ChessPlayer GetNextBot() {
		if (childs.Count == 0) return new ChessPlayer(new MyBot(), ChallengeController.PlayerType.MyBot, 60000);
		ChessPlayer player = new ChessPlayer(new MyBot(childs[0]), ChallengeController.PlayerType.MyBot, 60000);
		childs.RemoveAt(0);
		return player;
	}

	public MyBot GetSavedBot() {
		// this is for debug purposes, remove after test completed
		// this is where you input the file path of the bot
		string filePath = "C:/Users/Ertui/AppData/Local/ChessCodingChallenge/AI/.txt";

		if (File.Exists(filePath)) {
			// Read the JSON content from the file
			string jsonContent = File.ReadAllText(filePath);

			NeuralNetwork? nn = JsonSerializer.Deserialize(jsonContent, typeof(NeuralNetwork)) as NeuralNetwork;

			if (nn != null) {
				return new MyBot(nn);
			}
		}

		return new MyBot();
	}

	public static string SerializeNeuralNetwork(NeuralNetwork network) {
		JsonSerializerOptions options = new JsonSerializerOptions {
			WriteIndented = true, // To make the JSON more readable
		};

		// Serialize the neural network to JSON
		Console.WriteLine(network);
		string serializedNetwork = JsonSerializer.Serialize(network, options);

		return serializedNetwork;
	}

	public static void SaveToFile(string filePath, string content) {
		try {
			// Write the content to the specified file
			File.WriteAllText(filePath, content);
		} catch (IOException e) {
			Console.WriteLine($"Error writing to file: {e.Message}");
		}
	}
}

