using UnityEngine;
using System.Collections;
using FMODUnity;

public class AnimalSpawner : MonoBehaviour {

	public void Spawn(GameObject animal)
    {
        GameObject spawn = Instantiate(animal, transform.position, transform.rotation) as GameObject;
        if (spawn.tag == "Animal1") spawn.GetComponent<Animal>().sprite.GetComponent<SpriteRenderer>().color = GameManager.instance.color1[Random.Range(0, 3)];
        if (spawn.tag == "Animal2") spawn.GetComponent<Animal>().sprite.GetComponent<SpriteRenderer>().color = GameManager.instance.color2[Random.Range(0, 3)];
        if (spawn.tag == "Animal3")
        {
            if (GameManager.instance.firstBison)
            {
                Debug.Log("First bison");
                RuntimeManager.PlayOneShot("event:/Sounds/Animals/Bison_Appear", Camera.main.transform.position);
                GameManager.instance.firstBison = false;
            }
            spawn.GetComponent<Animal>().sprite.GetComponent<SpriteRenderer>().color = GameManager.instance.color3[Random.Range(0, 3)];

        }
    }
}
