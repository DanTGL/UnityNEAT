using UnityEngine;
using System.Collections.Generic;

public class NeuralNetwork : MonoBehaviour {

    public static int innovation = 0;

    public int hiddenNeurons = 5;

    public int numInputs;

    public int numOutputs;

    public List<Node> nodes;

    public float[] tests;

    public class Node {
        
        public Connection[] connectionsIn;

        public float bias = 0.0f;

        public Node(Connection[] connectionsIn, float bias) {
            this.connectionsIn = connectionsIn;
            this.bias = bias;
        }

        public Node(Connection[] connectionsIn) {
            this.connectionsIn = connectionsIn;
        }

        public Node() {

        }

    }

    public class Connection {

        int innovationId;

        int input, output;

        float weight;

        bool disabled = false;

        public Connection(int input, int output, float weight, int innovation) {
            this.input = input;
            this.output = output;
            this.weight = weight;
            this.innovationId = innovation;
        }

        public void SetDisabled(bool disabled) {
            this.disabled = disabled;
        }

        public bool GetDisabled() {
            return disabled;
        }
        
        public int GetInputNode() {
            return input;
        }

        public int GetOutputNode() {
            return output;
        }

        public float GetWeight() {
            return weight;
        }

        public int GetInnovationID() {
            return innovationId;
        }
        
    }

    public float GetValue(Connection[] connections, float[] inputs) {
        float result = 0.0f;

        for (int i = 0; i < connections.Length; i++) {
            Connection conn = connections[i];
            if (conn.GetDisabled()) continue;

            if (conn.GetInputNode() < numInputs) {
                result += conn.GetWeight() * inputs[conn.GetInputNode()];
            } else {
                result += conn.GetWeight() * GetValue(nodes[conn.GetInputNode()].connectionsIn, inputs);
            }
        }

        return result;
    }

    public float[] Evaluate(float[] inputs) {
        float[] outputs = new float[numOutputs];

        for (int i = 0; i < numOutputs; i++) {
            Node node = nodes[i + numInputs];
            outputs[i] = GetValue(node.connectionsIn, inputs);
        }

        return outputs;
    }

    void AddNode() {
    }

    void Awake() {
        nodes = new List<Node>();

        for (int i = 0; i < numInputs; i++) {
            nodes.Add(new Node());
        }

        for (int i = 0; i < numOutputs; i++) {
            List<Connection> conns = new List<Connection>();
            for (int j = 0; j < numInputs; j++) {
                conns.Add(new Connection(j, i, Random.Range(-1.0f, 1.0f), ++innovation));
            }
            nodes.Add(new Node(conns.ToArray()));
        }
    }

    void Start() {

    }

    void Update() {
        float[] result = Evaluate(tests);
        
        string output = "{ ";
        for (int i = 0; i < tests.Length; i++) {
            if (i > 0) {
                output += ", ";
            }

            output += tests[i].ToString();
        }

        output += " }, { ";

        for (int i = 0; i < result.Length; i++) {
            if (i > 0) {
                output += ", ";
            }

            output += result[i].ToString();
        }

        output += " }";

        Debug.Log(output);
    }
    
}
