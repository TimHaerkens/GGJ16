using UnityEngine;
using System.Collections;

public class AnimalSpawner : MonoBehaviour {

	public void Spawn(GameObject animal)
    {
        GameObject spawn = Instantiate(animal, transform.position, transform.rotation) as GameObject;
    }
}
