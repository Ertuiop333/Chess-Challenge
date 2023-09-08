using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Challenge.src.ChatGpt {
	using System;
	using System.Linq;

	public class Neuron {
		private double[] weights;
		private double bias;
		private Random random;

		public Neuron(int numInputs) {
			random = new Random();
			weights = new double[numInputs];
			for (int i = 0; i < numInputs; i++) {
				weights[i] = random.NextDouble() * 2 - 1; // Initialize weights with random values between -1 and 1
			}
			bias = random.NextDouble() * 2 - 1; // Initialize bias with a random value between -1 and 1
		}

		public double Activate(double[] inputs) {
			double weightedSum = inputs.Zip(weights, (input, weight) => input * weight).Sum() + bias;
			return Sigmoid(weightedSum);
		}

		private double Sigmoid(double x) {
			return 1 / (1 + Math.Exp(-x));
		}

		internal void Alter() {
			for (int i = 0; i < weights.Length; i++) {
				weights[i] += random.NextDouble() * 2 - 1;
				weights[i] = Math.Max(-1, Math.Min(1, weights[i]));
			}

			bias += random.NextDouble() * 2 - 1;
			bias = Math.Max(-1, Math.Min(1, bias));
		}
	}

	public class NeuralNetwork {
		private Neuron[] hiddenLayer;
		private Neuron[] outputLayer;

		public NeuralNetwork(int numInputs, int numHiddenNeurons, int numOutputs) {
			hiddenLayer = new Neuron[numHiddenNeurons];
			for (int i = 0; i < numHiddenNeurons; i++) {
				hiddenLayer[i] = new Neuron(numInputs);
			}

			outputLayer = new Neuron[numOutputs];
			for (int i = 0; i < numOutputs; i++) {
				outputLayer[i] = new Neuron(numHiddenNeurons);
			}
		}

		public NeuralNetwork(Neuron[] hiddenLayer, Neuron[] outputLayer) {
			this.hiddenLayer = hiddenLayer;
			for (int i = 0; i < hiddenLayer.Length; i++) {
				hiddenLayer[i].Alter();
			}

			this.outputLayer = outputLayer;
			for (int i = 0; i < outputLayer.Length; i++) {
				outputLayer[i].Alter();
			}
		}

		public NeuralNetwork() {
			int numInputs = 64;
			int numHiddenNeurons = 150;
			int numOutputs = 1;

			hiddenLayer = new Neuron[numHiddenNeurons];
			for (int i = 0; i < numHiddenNeurons; i++) {
				hiddenLayer[i] = new Neuron(numInputs);
			}

			outputLayer = new Neuron[numOutputs];
			for (int i = 0; i < numOutputs; i++) {
				outputLayer[i] = new Neuron(numHiddenNeurons);
			}
		}

		public double[] Forward(double[] inputs) {
			double[] hiddenOutputs = new double[hiddenLayer.Length];
			for (int i = 0; i < hiddenLayer.Length; i++) {
				hiddenOutputs[i] = hiddenLayer[i].Activate(inputs);
			}

			double[] output = new double[outputLayer.Length];
			for (int i = 0; i < outputLayer.Length; i++) {
				output[i] = outputLayer[i].Activate(hiddenOutputs);
			}

			return output;
		}

		public double GetNeuralNetworkOutput(double[] input) {
			// Get the neural network's output
			double[] output = Forward(input);

			// sum
			double sum = 0;
			foreach (var value in output) {
				sum += value;
			}

			return Sigmoid(sum);
		}

		public double Sigmoid(double x) {
			return 1 / (1 + Math.Exp(-x));
		}

		public NeuralNetwork[] GiveBirth(int childCount) {
			Console.WriteLine("Giving birth to " + childCount + " children");

			NeuralNetwork[] children = new NeuralNetwork[childCount];
			for (int i = 0; i < childCount; i++) {
				children[i] = new NeuralNetwork(hiddenLayer, outputLayer);
			}

			return children;
		}
	}
}
