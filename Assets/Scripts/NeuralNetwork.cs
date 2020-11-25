using UnityEngine;
using System;
using System.Collections.Generic;

public class NeuralNetwork : MonoBehaviour {
    
    public class Node {
        
        public float bias;

        public List<Connection> inputConnections;

        public Node(float bias) {
            this.bias = bias;
        }

        public float GetValue() {
            float result = bias;

            foreach (Connection conn in inputConnections) {
                result += conn.weight * conn.inputNode.GetValue();
            }

            return result;
        }
    }

    public class NeuralLayer {
        public List<Node> nodes;
    }

    public List<NeuralLayer> layers;

    public struct Connection {

        public bool enabled;

        public float weight;
        public Node inputNode;
    }

    void Start() {
    }

    void Update() {
        
    }
    
}
