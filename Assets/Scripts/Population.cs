using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour {

    public int individuals = 100;

    private NeuralNetwork[] population;

    void Start() {
        population = new NeuralNetwork[individuals];
    }

    void Update() {
        
    }
}
