using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Population : MonoBehaviour {

    public int individuals = 100;

    private Individual[] population;

    public Individual individualPrefab;

    public Vector3 originPos;

    public int generation = 0;

    public int GetWeightedRandom(Individual[] individs, float totalWeight) {
        if (individs.Length == 0) return 0;

        float rand = Random.Range(0.0f, totalWeight);
        float weight = totalWeight;
        for (int i = 0; i < individs.Length; i++) {
            if (rand - individs[i].GetFitness() <= 0) {
                return i;
            }
            rand = rand - individs[i].GetFitness();
        }

        return -1;
    }

    void Start() {
        population = new Individual[individuals];

        for (int i = 0; i < individuals; i++) {
            population[i] = GameObject.Instantiate(individualPrefab, originPos, Quaternion.identity);
            population[i].Init();
            population[i].inputs = new float[] {1.5f, 3.0f};
            population[i].neuralNetwork.Init();
        }

        Time.fixedDeltaTime = 1.0f;
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
        
        List<Individual> newGeneration = new List<Individual>();
        float[] inputs = {Random.Range(0, 15), Random.Range(0, 15)};

        float totalWeight = 0;

        int fittest = 0;
            
        for (int i = 0; i < individuals; i++) {
            totalWeight += population[i].GetFitness();

            if (population[i].GetFitness() > population[fittest].GetFitness()) {
                fittest = i;
            }
        }
        
        do {

            

            Individual i1 = null;
            
            do {
                int weightedRand = GetWeightedRandom(population, totalWeight);
                if (weightedRand != -1)
                    i1 = population[weightedRand];
            } while (i1 == null);
            Individual i2 = null;

            do {
                int weightedRand = GetWeightedRandom(population, totalWeight);
                
                if (weightedRand != -1)
                    i2 = population[weightedRand];
            } while (i2 == null || i1 == i2);

            newGeneration.Add(Crossover(i1, i2, inputs));
        } while (newGeneration.Count != individuals);
        
        for (int i = 0; i < population.Length; i++) {
            Destroy(population[i]);
        }
        Debug.Log(string.Format("Generation {0}: (Max fitness: {1}, Input1: {2}, Input2: {3}, Output: {4})", generation, population[fittest].GetFitness(), inputs[0], inputs[1], population[fittest].neuralNetwork.Evaluate(inputs)[0]));
        generation += 1;
        population = newGeneration.ToArray();
    }

    public Individual Crossover(Individual i1, Individual i2, float[] inputs) {
        if (i2.GetFitness() > i1.GetFitness()) {
            Individual tmp = i1;
            i1 = i2;
            i2 = tmp;
        }

        int nodeInnovations = Mathf.Max(i1.neuralNetwork.nodeInnovation, i2.neuralNetwork.nodeInnovation);
        
        Individual child = Instantiate(individualPrefab, originPos, Quaternion.identity);
        child.Init();
        child.neuralNetwork.Init();

        //Debug.Log(child.neuralNetwork.numInputs + " " + child.neuralNetwork.numOutputs);
        child.inputs = inputs;
        for (int i = inputs.Length + 1; i < nodeInnovations; i++) {
            child.neuralNetwork.AddNode(0.0f, false);
        }

        child.neuralNetwork.disabledGenes = new HashSet<int>(i1.neuralNetwork.disabledGenes);

        foreach (int i in i1.neuralNetwork.weights.Keys) {
            NeuralNetwork.Connection conn = NeuralNetwork.connections[i];

            if (i2.neuralNetwork.weights.ContainsKey(i) && Random.Range(0, 2) == 1 && !i2.neuralNetwork.disabledGenes.Contains(i)) {
                child.neuralNetwork.AddConnection(conn.GetInputNode(), conn.GetOutputNode(), i2.neuralNetwork.weights[i]);
            } else {
                child.neuralNetwork.AddConnection(conn.GetInputNode(), conn.GetOutputNode(), i1.neuralNetwork.weights[i]);
            }
        }

        child.Mutate();

        return child;
    }
}
