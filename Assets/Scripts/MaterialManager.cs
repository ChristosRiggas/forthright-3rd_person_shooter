using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    private Renderer playerRenderer;
    private Color32 colorNew;
    [System.Obsolete]

    // Start is called before the first frame update
    void Start()
    {
        Random.seed = (int)System.DateTime.Now.Ticks;
        colorNew = Random.ColorHSV(0f, 1f, 0.5f, 0.5f, 0.8f, 0.8f);

        // Get Render Component
        playerRenderer = GetComponent<Renderer>();

        // Change Color of Material
        playerRenderer.material.SetColor("_BaseColor", colorNew);
    }

}
