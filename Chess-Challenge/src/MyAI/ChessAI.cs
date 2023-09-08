using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Chess_Challenge.src.MyAI {
	public class ChessAI {
		public Tile[] input = new Tile[64];

		public double[] newInput = new double[64];

		public NeuralNetwork brain;

		public ChessAI(int brainSize) {
			// random generator
			Random rng = new Random();

			// clamp brain size
			if (brainSize > 1000) {
				brainSize = 1000;
			}

			// generate number of layers
			int numberOfLayers = rng.Next(2, brainSize / 2);
			Layer[] layers = new Layer[numberOfLayers];

			// generate number of neurons per layer
			int numberOfNeuronsPerLayer = brainSize / numberOfLayers;

			// generate neurons on each layer
			for (int i = 0; i < numberOfLayers; i++) {
				List<Neuron> neuronsToAddInLayer = new List<Neuron>();

				for (int j = 0; j < numberOfNeuronsPerLayer; j++) {
					// what we will be generating
					Neuron neuron;
					float[] weights;
					double bias;

					// find out how many neurons will have a connection to this neuron
					int numberOfInputs = numberOfNeuronsPerLayer;
					if (i == 0) numberOfInputs = 64;

					// generate weights
					weights = new float[numberOfInputs];
					for (int k = 0; k < weights.Length; k++) {
						weights[k] = rng.NextSingle() * 2 - 1;
					}

					// generate bias
					bias = rng.NextDouble() * 2 - 1;

					// generate neuron
					neuron = new Neuron(weights, bias);

					// add neuron to layer
					neuronsToAddInLayer.Add(neuron);
				}

				// generate layer
				layers[i] = new Layer(neuronsToAddInLayer.ToArray());
			}

			// Create the brain
			brain = new NeuralNetwork(layers);
		}

		private void SetInputs(Board board) {
			string fenString = board.GetFenString().Split(" ")[0];
			string[] rowFen = fenString.Split("/");

			int rowCounter = 0;

			// loop through each row
			foreach (string line in rowFen) {
				int tileCounter = 0;

				// loop through each character in the fen row, the numbers are the amount of empty tiles
				for (int i = 0; i < line.Length; i++) {
					char character = line[i];

					if (char.IsDigit(character)) {
						int skippingTileCounter = int.Parse(character.ToString());

						for (int j = 0; j < skippingTileCounter; j++) {
							// set tile to empty
							input[(input.Length - 1) - (tileCounter + ((rowCounter) * 8))] = new Tile(new Vector2(tileCounter, rowCounter), 0);
						}

						continue;
					} else {
						int pieceType = DeSerializeFenNotation(character);

						// set tile to the piece 
						input[(input.Length - 1) - (tileCounter + (rowCounter * 8))] = new Tile(new Vector2(tileCounter, rowCounter), pieceType);
					}

					tileCounter++;
				}

				rowCounter++;
			}
		}

		public void SetNewInputs(Board board) {
			string fenString = board.GetFenString().Split(" ")[0];
			string[] rowFen = fenString.Split("/");

			int rowCounter = 0;

			// loop through each row
			foreach (string line in rowFen) {
				int squareCounter = 0;

				// loop through each character in the fen row, the numbers are the amount of empty squares
				for (int i = 0; i < line.Length; i++) {
					char character = line[i];

					if (char.IsDigit(character)) {
						int skippingTileCounter = int.Parse(character.ToString());

						for (int j = 0; j < skippingTileCounter; j++) {
							// set value to 0
							newInput[(newInput.Length - 1) - (squareCounter + ((rowCounter) * 8))] = 0;
						}

						continue;
					} else {
						float pieceValue = NewDeSerializeFenNotation(character);

						// set value to the piece 
						newInput[(newInput.Length - 1) - (squareCounter + (rowCounter * 8))] = pieceValue;
					}

					squareCounter++;
				}

				rowCounter++;
			}
		}

		private static int DeSerializeFenNotation(char character) {
			// 0 = empty, 1 = white rook, 2 = white knight, 3 = white bishop, 4 = white queen, 5 = white king, 6 = white pawn
			// 7 = black rook, 8 = black knight, 9 = black bishop, 10 = black queen, 11 = black king, 12 = black pawn
			int pieceType = 0;

			if (character.ToString() == ("R")) { // white rook
												 //Console.WriteLine("white Rook");
				pieceType = 1;
			} else if (character.ToString() == ("N")) { // white Knight
														//Console.WriteLine("white Knight");
				pieceType = 2;
			} else if (character.ToString() == ("B")) { // white Bishop
														//Console.WriteLine("white Bishop");
				pieceType = 3;
			} else if (character.ToString() == ("Q")) { // white Queen
														//Console.WriteLine("white Queen");
				pieceType = 4;
			} else if (character.ToString() == ("K")) { // white King
														//Console.WriteLine("white King");
				pieceType = 5;
			} else if (character.ToString() == ("P")) { // white Pawn
														//Console.WriteLine("white Pawn");
				pieceType = 6;
			} else if (character.ToString() == ("r")) { // black rook
														//Console.WriteLine("black Rook");
				pieceType = 7;
			} else if (character.ToString() == ("n")) { // black Knight
														//Console.WriteLine("black Knight");
				pieceType = 8;
			} else if (character.ToString() == ("b")) { // black Bishop
														//Console.WriteLine("black Bishop");
				pieceType = 9;
			} else if (character.ToString() == ("q")) { // black Queen
														//Console.WriteLine("black Queen");
				pieceType = 10;
			} else if (character.ToString() == ("k")) { // black King
														//Console.WriteLine("black King");
				pieceType = 11;
			} else if (character.ToString() == ("p")) { // black Pawn
														//Console.WriteLine("black Pawn");
				pieceType = 12;
			}

			return pieceType;
		}

		private static float NewDeSerializeFenNotation(char character) {
			// 0 = empty, 1 = white rook, 2 = white knight, 3 = white bishop, 4 = white queen, 5 = white king, 6 = white pawn
			// 7 = black rook, 8 = black knight, 9 = black bishop, 10 = black queen, 11 = black king, 12 = black pawn
			float pieceValue = 0;

			if (character.ToString() == ("R")) { // white rook
												 //Console.WriteLine("white Rook");
				pieceValue = 1f;
			} else if (character.ToString() == ("N")) { // white Knight
														//Console.WriteLine("white Knight");
				pieceValue = 1;
			} else if (character.ToString() == ("B")) { // white Bishop
														//Console.WriteLine("white Bishop");
				pieceValue = 1;
			} else if (character.ToString() == ("Q")) { // white Queen
														//Console.WriteLine("white Queen");
				pieceValue = 1;
			} else if (character.ToString() == ("K")) { // white King
														//Console.WriteLine("white King");
				pieceValue = 1;
			} else if (character.ToString() == ("P")) { // white Pawn
														//Console.WriteLine("white Pawn");
				pieceValue = 1;
			} else if (character.ToString() == ("r")) { // black rook
														//Console.WriteLine("black Rook");
				pieceValue = -1;
			} else if (character.ToString() == ("n")) { // black Knight
														//Console.WriteLine("black Knight");
				pieceValue = -1;
			} else if (character.ToString() == ("b")) { // black Bishop
														//Console.WriteLine("black Bishop");
				pieceValue = -1;
			} else if (character.ToString() == ("q")) { // black Queen
														//Console.WriteLine("black Queen");
				pieceValue = -1;
			} else if (character.ToString() == ("k")) { // black King
														//Console.WriteLine("black King");
				pieceValue = -1;
			} else if (character.ToString() == ("p")) { // black Pawn
														//Console.WriteLine("black Pawn");
				pieceValue = -1;
			}

			return pieceValue;
		}

		private static string SerializeOutput(Output output) {
			string moveName = "";

			//from x
			if (output.from.X == 0) {
				moveName += "a";
			} else if (output.from.X == 1) {
				moveName += "b";
			} else if (output.from.X == 2) {
				moveName += "c";
			} else if (output.from.X == 3) {
				moveName += "d";
			} else if (output.from.X == 4) {
				moveName += "e";
			} else if (output.from.X == 5) {
				moveName += "f";
			} else if (output.from.X == 6) {
				moveName += "g";
			} else if (output.from.X == 7) {
				moveName += "h";
			}

			//from y
			moveName += (output.from.Y + 1).ToString();

			//to x
			if (output.to.X == 0) {
				moveName += "a";
			} else if (output.to.X == 1) {
				moveName += "b";
			} else if (output.to.X == 2) {
				moveName += "c";
			} else if (output.to.X == 3) {
				moveName += "d";
			} else if (output.to.X == 4) {
				moveName += "e";
			} else if (output.to.X == 5) {
				moveName += "f";
			} else if (output.to.X == 6) {
				moveName += "g";
			} else if (output.to.X == 7) {
				moveName += "h";
			}

			moveName += (output.to.Y + 1).ToString();

			//promotion piece
			// 0 = empty, 1 = white rook, 2 = white knight, 3 = white bishop, 4 = white queen, 5 = white king, 6 = white pawn
			// 7 = black rook, 8 = black knight, 9 = black bishop, 10 = black queen, 11 = black king, 12 = black pawn
			if (output.promotionPiece == 0) {
				// no promotion
			} else if (output.promotionPiece == 1) {
				moveName += "R";
			} else if (output.promotionPiece == 2) {
				moveName += "N";
			} else if (output.promotionPiece == 3) {
				moveName += "B";
			} else if (output.promotionPiece == 4) {
				moveName += "Q";
			} else if (output.promotionPiece == 5) {
				moveName += "K";
			} else if (output.promotionPiece == 6) {
				moveName += "P";
			} else if (output.promotionPiece == 7) {
				moveName += "r";
			} else if (output.promotionPiece == 8) {
				moveName += "n";
			} else if (output.promotionPiece == 9) {
				moveName += "b";
			} else if (output.promotionPiece == 10) {
				moveName += "q";
			} else if (output.promotionPiece == 11) {
				moveName += "k";
			} else if (output.promotionPiece == 12) {
				moveName += "p";
			}

			return moveName;
		}

		public double GenerateOutput(Board board) {
			// SetInputs(board);
			SetNewInputs(board);

			// TODO: implement AI here
			// Output output = new Output(new Vector2(0, 1), new Vector2(0, 3), 0);

			// clamp output to board
			// output.from.X = Math.Clamp(output.from.X, 0, 7);
			// output.from.Y = Math.Clamp(output.from.Y, 0, 7);
			// output.to.X = Math.Clamp(output.to.X, 0, 7);
			// output.to.Y = Math.Clamp(output.to.Y, 0, 7);

			// return SerializeOutput(output);
			return brain.GetOutput(newInput);
		}
	}

	public class Tile {
		// (0, 0) is the top left corner
		// (7, 7) is the botom right corner
		// (7, 0) is the top right corner
		public Vector2 index;
		// 0 = empty, 1 = white rook, 2 = white knight, 3 = white bishop, 4 = white queen, 5 = white king, 6 = white pawn
		// 7 = black rook, 8 = black knight, 9 = black bishop, 10 = black queen, 11 = black king, 12 = black pawn
		public int piece;

		public Tile(Vector2 index, int piece) {
			this.index = index;
			this.piece = piece;
		}
	}

	public class Output {
		public Vector2 from;
		public Vector2 to;

		// 0 = empty, 1 = white rook, 2 = white knight, 3 = white bishop, 4 = white queen, 5 = white king, 6 = white pawn
		// 7 = black rook, 8 = black knight, 9 = black bishop, 10 = black queen, 11 = black king, 12 = black pawn
		public int promotionPiece;

		public Output(Vector2 from, Vector2 to, int promotionPiece) {
			this.from = from;
			this.to = to;
			this.promotionPiece = promotionPiece;
		}
	}

	// new neural network

	public class Neuron {
		public float[] weights; // ranges from -1 to 1
		public double bias; // ranges from -10 to 10

		public double value;

		public Neuron(float[] weights, double bias) {
			this.weights = weights;
			this.bias = bias;
		}

		private double Sigmoid(double x) {
			return 1 / (1 + Math.Exp(-x));
		}

		public void Activate(double[] value) {
			double sum = 0;
			for (int i = 0; i < weights.Length; i++) {
				sum += value[i] * weights[i];
			}
			this.value = Sigmoid(sum + bias);
		}
	}

	public class Layer {
		public Neuron[] neurons;

		public Layer(Neuron[] neurons) {
			this.neurons = neurons;
		}
	}

	public class NeuralNetwork {
		public Layer[] layers;

		public NeuralNetwork(Layer[] layers) {
			this.layers = layers;
		}

		private double Sigmoid(double x) {
			return 1 / (1 + Math.Exp(-x));
		}

		public double GetOutput(double[] inputs) {
			// work inputs through first layer
			for (int i = 0; i < layers[0].neurons.Length; i++) {
				layers[0].neurons[i].Activate(inputs);
			}

			// work inputs through all other layers
			for (int i = 1; i < layers.Length; i++) {
				for (int o = 0; o < layers[i].neurons.Length; o++) {
					double[] output = new double[layers[i - 1].neurons.Length];
					for (int j = 0; j < layers[i - 1].neurons.Length; j++) {
						output[j] = layers[i - 1].neurons[j].value;
					}
					layers[i].neurons[o].Activate(output);
				}
			}


			// gather output value
			double outputValue = 0;
			for (int i = 0; i < layers[^1].neurons.Length; i++) {
				outputValue += layers[^1].neurons[i].value;
			}

			return Sigmoid(outputValue);
		}
	}
}
