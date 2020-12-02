using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NeuralNetwork))]
public class Individual : MonoBehaviour {

    public NeuralNetwork neuralNetwork;

    private bool dead = false;

    private float fitness = 0;

    public float[] inputs;

    public void Init() {
        neuralNetwork = GetComponent<NeuralNetwork>();
        /*neuralNetwork.AddNode(0.5f, false);
        neuralNetwork.AddNode(-1.5f, false);

        neuralNetwork.AddConnection(0, 3, 1.0f);
        neuralNetwork.AddConnection(0, 4, -1.0f);
        neuralNetwork.AddConnection(1, 3, 1.0f);
        neuralNetwork.AddConnection(1, 4, -1.0f);
        neuralNetwork.AddConnection(3, 2, 1.0f);
        neuralNetwork.AddConnection(4, 2, 1.0f);*/

        /*neuralNetwork.AddConnection(0, 3, 1);
        neuralNetwork.AddConnection(1, 3, 1);
        neuralNetwork.AddConnection(3, 2, 1);*/
        

        //Debug.Log(neuralNetwork.Evaluate(inputs, 1.5f)[0]);
    }

    public void Update() {
        if (!dead) {
            float output = neuralNetwork.Evaluate(inputs)[0];
            if (Mathf.Sign(output) == 1) {
                fitness = -(output - (inputs[0] * inputs[1]));
            } else {
                fitness = -1;
            }

            dead = true;
            //Debug.Log(neuralNetwork.Evaluate(inputs)[0]);
        }
    }

    public void Mutate() {
        
    }

    public bool IsDead() {
        return dead;
    }

    public float GetFitness() {
        return fitness;
    }

}
