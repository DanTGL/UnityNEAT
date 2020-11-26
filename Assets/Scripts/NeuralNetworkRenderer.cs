using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageEffectAllowedInSceneView]
public class NeuralNetworkRenderer : MonoBehaviour {

    public NeuralNetwork nn;
    public Material mat;

    void OnPostRender() {

        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();

        GL.Begin(GL.QUADS);

        for (int i = 0; i < nn.layers.Count; i++) {
            NeuralNetwork.NeuralLayer layer = nn.layers[i];
            float layerX = (1 / ((float)nn.layers.Count + 1.0f)) * ((float)i + 1.0f);

            for (int j = 0; j < layer.nodes.Count; j++) {
                float nodeY = (1 / ((float)layer.nodes.Count + 1.0f)) * ((float)j + 1.0f);

                GL.Color(Color.red);
                
                GL.Vertex3(layerX, nodeY, 0);
                GL.Vertex3(layerX, nodeY + 0.05f, 0);
                GL.Vertex3(layerX + 0.05f, nodeY + 0.05f, 0);
                GL.Vertex3(layerX + 0.05f, nodeY, 0);
            }
        }
        GL.End();
        GL.PopMatrix();
        
    }

}
