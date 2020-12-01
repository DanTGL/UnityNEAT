using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour {

    public int individuals = 100;

    private Individual[] population;

    public Individual individualPrefab;

    public Vector3 originPos;

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
        Crossover();
    }

    public void Crossover() {

    }
}
