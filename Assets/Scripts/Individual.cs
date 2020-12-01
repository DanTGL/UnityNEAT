using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Individual : MonoBehaviour {

    public NeuralNetwork neuralNetwork;

    private bool dead = false;

    private int fitness = 0;

    void Start() {
        neuralNetwork = GetComponent<NeuralNetwork>();
    }

    public void Update() {
        if (!dead) {

        }
    }

    public void Mutate() {
        
    }

    public bool IsDead() {
        return dead;
    }

    public int GetFitness() {
        return fitness;
    }

}
