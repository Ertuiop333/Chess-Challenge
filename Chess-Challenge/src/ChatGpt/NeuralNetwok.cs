using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Challenge.src.ChatGpt {
	using System;
	using System.Linq;

	public class Neuron {
		private double[] _weights;
		public double[] Weights { get { return _weights; } }
		public double _bias;
		public double Bias { get { return _bias; } }
		public Random random;
		public int percentageToHaveGreatAlternation = 5; // on 100

		public Neuron(int numInputs) {
			random = new Random();
			_weights = new double[numInputs];
			for (int i = 0; i < numInputs; i++) {
				_weights[i] = random.NextDouble(); // Initialize weights with random values between 0 and 1
			}

			_bias = (random.NextDouble() * 2 - 1) * 10; // Initialize bias with a random value between -10 and 10
		}

		public double Activate(double[] inputs) {
			double weightedSum = inputs.Zip(_weights, (input, weight) => input * weight).Sum() + _bias;
			return Sigmoid(weightedSum);
		}

		private double Sigmoid(double x) {
			return 1 / (1 + Math.Exp(-x));
		}

		internal void Alter() {
			for (int i = 0; i < _weights.Length; i++) {
				float magnitude = random.NextInt64(0, 100) < percentageToHaveGreatAlternation ? 0.5f : 0.1f;
				_weights[i] += (random.NextDouble() * 2 - 1) * magnitude; // modifies the weight with a magnitude of 0.1 and sometimes 0.5
				_weights[i] = Math.Max(0, Math.Min(1, _weights[i]));
			}

			_bias += random.NextDouble() * 2 - 1; // bias can change by the magnitude of 1
			_bias = Math.Max(-10, Math.Min(10, _bias));
		}
	}

	public class NeuralNetwork {
		private Neuron[] _hiddenLayer;
		public Neuron[] HiddenLayer { get { return _hiddenLayer; } }
		public Neuron[] _outputLayer;
		public Neuron[] OutputLayer { get { return _outputLayer; } }

		public NeuralNetwork(int numInputs, int numHiddenNeurons, int numOutputs) {
			_hiddenLayer = new Neuron[numHiddenNeurons];
			for (int i = 0; i < numHiddenNeurons; i++) {
				_hiddenLayer[i] = new Neuron(numInputs);
			}

			_outputLayer = new Neuron[numOutputs];
			for (int i = 0; i < numOutputs; i++) {
				_outputLayer[i] = new Neuron(numHiddenNeurons);
			}
		}

		public NeuralNetwork(Neuron[] hiddenLayer, Neuron[] outputLayer) {
			this._hiddenLayer = hiddenLayer;
			for (int i = 0; i < hiddenLayer.Length; i++) {
				hiddenLayer[i].Alter();
			}

			this._outputLayer = outputLayer;
			for (int i = 0; i < outputLayer.Length; i++) {
				outputLayer[i].Alter();
			}
		}

		public NeuralNetwork() {
			int numInputs = 64;
			int numHiddenNeurons = 150;
			int numOutputs = 1;

			_hiddenLayer = new Neuron[numHiddenNeurons];
			for (int i = 0; i < numHiddenNeurons; i++) {
				_hiddenLayer[i] = new Neuron(numInputs);
			}

			_outputLayer = new Neuron[numOutputs];
			for (int i = 0; i < numOutputs; i++) {
				_outputLayer[i] = new Neuron(numHiddenNeurons);
			}
		}

		public double[] Forward(double[] inputs) {
			double[] hiddenOutputs = new double[_hiddenLayer.Length];
			for (int i = 0; i < _hiddenLayer.Length; i++) {
				hiddenOutputs[i] = _hiddenLayer[i].Activate(inputs);
			}

			double[] output = new double[_outputLayer.Length];
			for (int i = 0; i < _outputLayer.Length; i++) {
				output[i] = _outputLayer[i].Activate(hiddenOutputs);
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
				children[i] = new NeuralNetwork(_hiddenLayer, _outputLayer);
			}

			return children;
		}
	}
}
