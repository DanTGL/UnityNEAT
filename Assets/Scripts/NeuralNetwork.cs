using UnityEngine;
using System.Collections.Generic;

public class NeuralNetwork : MonoBehaviour {

    public static int innovation = 0;

    public List<Connection> connections;

    public HashSet<int> disabledGenes;
    
    public int hiddenNeurons = 5;

    public int numInputs;

    public int numOutputs;

    public List<Node> nodes;

    public float[] tests;
    
    public class Node {
        
        float bias = 0.0f;
        public HashSet<int> connectionsIn;
        
        public Node(int[] connectionsIn, float bias) {
            this.connectionsIn = new HashSet<int>(connectionsIn);
            this.bias = bias;
        }

        public Node(int[] connectionsIn) {
            this.connectionsIn = new HashSet<int>(connectionsIn);
        }

        public Node() {
            this.connectionsIn = new HashSet<int>();
        }
        
    }

    public class InputNode : Node {}
    public class OutputNode : Node {}

    public class Connection {

        int innovationId;

        int input, output;

        float weight;

        public Connection(int input, int output, float weight) {
            this.input = input;
            this.output = output;
            this.weight = weight;
            this.innovationId = innovation++;
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

    public float GetValue(HashSet<int> conns, float[] inputs) {
        float result = 0.0f;

        foreach (int i in conns) {
            Connection conn = connections[i];
            if (disabledGenes.Contains(conn.GetInnovationID())) continue;

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
        Connection oldConnection;

        do {
            oldConnection = connections[Random.Range(0, connections.Count - 1)];
        } while (disabledGenes.Contains(oldConnection.GetInnovationID()));

        disabledGenes.Add(oldConnection.GetInnovationID());
        Connection conn1 = new Connection(oldConnection.GetInputNode(), nodes.Count, oldConnection.GetWeight());
        Connection conn2 = new Connection(nodes.Count, oldConnection.GetOutputNode(), 1.0f);
        connections.Add(conn1);
        connections.Add(conn2);

        nodes.Add(new Node(new int[] {conn1.GetInnovationID(), conn2.GetInnovationID()}));
    }

    void Awake() {
        nodes = new List<Node>();

        for (int i = 0; i < numInputs; i++) {
            nodes.Add(new Node());
        }

        for (int i = 0; i < numOutputs; i++) {
            List<Connection> conns = new List<Connection>();
            for (int j = 0; j < numInputs; j++) {
                conns.Add(new Connection(j, i, Random.Range(-1.0f, 1.0f)));
            }
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
