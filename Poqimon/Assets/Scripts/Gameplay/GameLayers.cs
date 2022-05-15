using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask longGrassLayer;
    [SerializeField] LayerMask trainerFoVLayer;

    public static GameLayers i { get; set;}

    private void Awake() {
        i = this;
    }

    public LayerMask SolidObjectsLayer {
        get => solidObjectsLayer;
    }

    public LayerMask InteractableLayer {
        get => interactableLayer;
    }
    
    public LayerMask LongGrassLayer {
        get => longGrassLayer; 
    }

    public LayerMask TrainerFoVLayer {
        get => trainerFoVLayer;
    }

}
