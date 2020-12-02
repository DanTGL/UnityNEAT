using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Population : MonoBehaviour {

    public int individuals = 100;

    private Individual[] population;

    public Individual individualPrefab;

    public Vector3 originPos;

    public Individual GetWeightedRandom(Individual[] individs, float totalWeight) {
        if (individs.Length == 0) return 0;

        float rand = Random.Range(0.0f, totalWeight);

        for (int i = 0; i < individs.Length; i++) {
            if (rand - individs[i].GetFitness() <= 0) {
                return i;
            }
        }

        return -1;
    }

    void Start() {
        population = new Individual[individuals];

        for (int i = 0; i < individuals; i++) {
            population[i] = GameObject.Instantiate(individualPrefab, originPos, Quaternion.identity);
        }
    }

    void FixedUpdate() {
        bool allDead = true;
        for (int i = 0; i < population.Length; i++) {
            allDead &= population[i].IsDead();
        }

        if (allDead) {
            // TODO: Restart scene before start the next generation

            NextGeneration();
        }
    }

    public void NextGeneration() {

        Individual i1 = null;
        Individual i2 = null;

        float totalWeight = 0;
        
        for (int i = 0; i < individuals; i++) {
            totalWeight += population[i].GetFitness();
        }

        i1 = GetWeightedRandom(population, totalWeight);

        do {
            i2 = GetWeightedRandom(population, totalWeight);
        } while (i1 == i2);

        Crossover(i1, i2);
        
    }

    public void Crossover(Individual i1, Individual i2) {
        if (i2.GetFitness() > i1.GetFitness()) {
            Individual tmp = i1;
            i1 = i2;
            i2 = tmp;
        }

        int nodeInnovations = Mathf.Max(i1.neuralNetwork.nodeInnovation, i2.neuralNetwork.nodeInnovation);
        Dictionary<int, float> genes = new Dictionary<int, float>(i2.neuralNetwork.weights);
        HashSet<int> disabledGenes = new HashSet<int>();
        
        Individual child = Instantiate(individualPrefab, originPos, Quaternion.identity);

        for (int i = 0; i < NeuralNetwork.GetInnovation(); i++) {
            
            if (i2.neuralNetwork.weights.ContainsKey(i) && Random.Range(0, 2) == 1 && !i2.neuralNetwork.disabledGenes.Contains(i)) {
                genes.Add(i, i2.neuralNetwork.weights[i]);
            } else if (i1.neuralNetwork.weights.ContainsKey(i)) {
                genes.Add(i, i1.neuralNetwork.weights[i]);
                if (i1.neuralNetwork.disabledGenes.Contains(i)) {
                    disabledGenes.Add(i);
                }
            }
        }



    }
}
