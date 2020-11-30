using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class NeuralNetwork : MonoBehaviour {

    public Dictionary<int, Connection> connections;

    public HashSet<int> disabledGenes;
    
    public int hiddenNeurons = 5;

    public int numInputs;

    public int numOutputs;

    public Dictionary<int, Node> nodes;

    public float[] tests;
    
    public class Node {
        
        private static int innovation = 0;
        int id;

        float bias = 0.0f;
        public HashSet<int> connectionsIn;
        
        public Node(int[] connectionsIn, float bias) {
            this.connectionsIn = new HashSet<int>(connectionsIn);
            this.bias = bias;
            id = innovation++;
        }

        public Node(int[] connectionsIn) {
            this.connectionsIn = new HashSet<int>(connectionsIn);
        }

        public Node() {
            this.connectionsIn = new HashSet<int>();
        }

        public int GetID() {
            return id;
        }
        
    }

    public class Connection {

        private static int innovation = 0;

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

        public void SetWeight(float weight) {
            this.weight = weight;
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
        connections.Add(conn1.GetInnovationID(), conn1);
        connections.Add(conn2.GetInnovationID(), conn2);
        Node newNode = new Node(new int[] {conn1.GetInnovationID(), conn2.GetInnovationID()});
        nodes.Add(newNode.GetID(), newNode);
    }

    void AddConnection() {
        Node node1 = nodes[Random.Range(0, numInputs)];
        Node node2 = nodes[Random.Range(numInputs, nodes.Count)];
    }

    void MutateConnection() {
        int node1;
        do {
            node1 = nodes[Random.Range(0, nodes.Count)].GetID();
        } while (node1 >= numInputs && node1 < numInputs + numOutputs);
        
        int node2 = nodes[Random.Range(numInputs, nodes.Count)].GetID();

        if (node1 == node2) {
            return;
        }

        if (node1 > node2) {
            int tmp = node1;
            node1 = node2;
            node2 = tmp;
        }

        foreach (int connId in nodes[node2].connectionsIn) {

            if (connections[connId].GetInputNode() == node1) {
                connections[connId].SetWeight(Random.Range(-1.0f, 1.0f));
                return;
            }
        }

        Connection conn = new Connection(node1, node2, Random.Range(-1.0f, 1.0f));
        connections.Add(conn.GetInnovationID(), conn);
    }

    void Awake() {
        nodes = new Dictionary<int, Node>();

        for (int i = 0; i < numInputs + numOutputs; i++) {
            nodes.Add(i, new Node());
        }
    }

    void Start() {

    }

    void Update() {
        /*float[] result = Evaluate(tests);
        
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

        Debug.Log(output);*/
    }
    
}
