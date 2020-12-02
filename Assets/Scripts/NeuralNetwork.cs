using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class NeuralNetwork : MonoBehaviour {
    
    public static readonly float mutateConnectionChance = 0.05f;
    public static readonly float addConnectionChance = 0.01f;
    public static readonly float addNodeChance = 0.035f;

    public static Dictionary<int, Connection> connections = new Dictionary<int, Connection>();
    
    public Dictionary<int, float> weights;

    public HashSet<int> disabledGenes;

    public int numInputs = 2;

    public int numOutputs = 1;

    public int nodeInnovation = 0;
    private static int innovation = 0;

    public static int GetInnovation() {
        return innovation;
    }

    public Dictionary<int, Node> nodes;
    
    public class Node {
        
        int id;

        float bias = 0.0f;
        public HashSet<int> connectionsIn;
        
        public Node(int[] connectionsIn, float bias, int id) {
            this.connectionsIn = new HashSet<int>(connectionsIn);
            this.bias = bias;
            this.id = id;
        }

        public Node(int[] connectionsIn, int id) {
            this.connectionsIn = new HashSet<int>(connectionsIn);
            this.id = id;
        }

        public Node(int id) {
            this.connectionsIn = new HashSet<int>();
            this.id = id;
        }
        public Node(float bias, int id) {
            this.connectionsIn = new HashSet<int>();
            this.id = id;
            this.bias = bias;
        }

        public int GetID() {
            return id;
        }

        public float GetBias() {
            return bias;
        }
        
    }

    public class Connection {


        int innovationId;

        int input, output;

        public Connection(int input, int output) {
            this.input = input;
            this.output = output;
            this.innovationId = innovation++;
        }
        
        public int GetInputNode() {
            return input;
        }

        public int GetOutputNode() {
            return output;
        }

        public int GetInnovationID() {
            return innovationId;
        }

    }

    public float GetValue(HashSet<int> conns, float[] inputs, float threshold = 0.0f) {
        float result = 0.0f;

        //TODO: Use Sigmoid function instead of step function

        
        foreach (int i in conns) {
            Connection conn = connections[i];
            if (disabledGenes.Contains(i)) continue;

            if (conn.GetInputNode() < numInputs) {
                result += weights[i] * inputs[conn.GetInputNode()];
            } else {
                result += weights[i] * GetValue(nodes[conn.GetInputNode()].connectionsIn, inputs);
            }
        }

        return Sigmoid(result);
    }

    public float[] Evaluate(float[] inputs, float threshold = 0.0f) {
        float[] outputs = new float[numOutputs];

        for (int i = 0; i < numOutputs; i++) {
            Node node = nodes[i + numInputs];
            if (node.connectionsIn.Count != 0)
                outputs[i] = GetValue(node.connectionsIn, inputs, threshold);
        }

        return outputs;
    }

    public int AddNode(float bias = 0.0f, bool replaceConn=true) {
        Node newNode;
        if (replaceConn) {

            Connection oldConnection;

            do {
                oldConnection = connections[UnityEngine.Random.Range(0, connections.Count - 1)];
            } while (disabledGenes.Contains(oldConnection.GetInnovationID()));

            disabledGenes.Add(oldConnection.GetInnovationID());
            //Connection conn1 = new Connection(oldConnection.GetInputNode(), nodes.Count, oldConnection.GetWeight());
            //Connection conn2 = new Connection(nodes.Count, oldConnection.GetOutputNode(), 1.0f);
            int conn1 = AddConnection(oldConnection.GetInputNode(), nodes.Count, weights[oldConnection.GetInnovationID()]);
            int conn2 = AddConnection(nodes.Count, oldConnection.GetOutputNode(), 1.0f);

            newNode = new Node(new int[] {conn1, conn2}, bias, nodeInnovation++);

        } else {
            newNode = new Node(bias, nodeInnovation++);
        }
        nodes.Add(newNode.GetID(), newNode);
        return newNode.GetID();
    }

    public int AddConnection(int inputNode, int outputNode, float weight) {
        foreach (int id in connections.Keys) {
            if (connections[id].GetInputNode() == inputNode && connections[id].GetOutputNode() == outputNode) {
                weights[id] = weight;
                return id;
            }
        }

        

        Node node1 = nodes[inputNode];
        Node node2 = nodes[outputNode];

        Connection conn = new Connection(inputNode, outputNode);
        node2.connectionsIn.Add(conn.GetInnovationID());
        connections.Add(conn.GetInnovationID(), conn);
        weights[conn.GetInnovationID()] = weight;
        return conn.GetInnovationID();
    }

    void MutateConnection() {
        int node1;
        do {
            node1 = nodes[UnityEngine.Random.Range(0, nodes.Count)].GetID();
        } while (node1 >= numInputs && node1 < numInputs + numOutputs);
        
        int node2 = nodes[UnityEngine.Random.Range(numInputs, nodes.Count)].GetID();

        if (node1 == node2) {
            return;
        }

        if (node1 > node2 && node2 >= numInputs + numOutputs) {
            int tmp = node1;
            node1 = node2;
            node2 = tmp;
        }

        foreach (int connId in nodes[node2].connectionsIn) {

            if (connections[connId].GetInputNode() == node1) {
                weights[connId] = UnityEngine.Random.Range(-1.0f, 1.0f);
                return;
            }
        }

        Connection conn = new Connection(node1, node2);
        weights[conn.GetInnovationID()] = UnityEngine.Random.Range(-1.0f, 1.0f);
        connections.Add(conn.GetInnovationID(), conn);
    }

    public void Init() {
        nodes = new Dictionary<int, Node>();
        weights = new Dictionary<int, float>();
        disabledGenes = new HashSet<int>();

        for (int i = 0; i < numInputs; i++) {
            AddNode(0.0f, false);
            //nodes.Add(i, new Node(nodeInnovation++));
        }

        for (int i = numInputs; i < numInputs + numOutputs; i++) {
            AddNode(0.0f, false);
            //nodes.Add(i, new Node(nodeInnovation++));
        }
    }

    public void Mutate() {

        if (UnityEngine.Random.Range(0.0f, 1.0f) < mutateConnectionChance) {
            MutateConnection();
        }

        if (UnityEngine.Random.Range(0.0f, 1.0f) < addNodeChance) {
            AddNode(0.0f, false);
        }
    }
    
    public float Sigmoid(float x) {
        return 1 / (1 + Mathf.Pow(2.71828f, -x));
    }

}
