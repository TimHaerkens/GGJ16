using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;
using FMODUnity;

public class GameManager : MonoBehaviour {

    //FMOD
    public FMOD.Studio.EventInstance music;
    public FMOD.Studio.EventInstance barf;
    FMOD.Studio.ParameterInstance tension;

    public GameObject[] animalSpawners;//animalSpawners in the game
    public GameObject[] spawnpoints;//Points a player can spawn

    //Players
    public List<GameObject> players;//All the players in the game
    public InputDevice[] controllers = new InputDevice[4] { null, null, null, null };


    //Resources
    public GameObject[] animalSpawns;//Animals that can be spawned
    public GameObject player;//A player that can be spawned

    

    #region singleton
    private static GameManager Instance;
    public static GameManager instance
    {
        get
        {
            if (Instance == null)
                Instance = GameObject.FindObjectOfType<GameManager>();
            return Instance;
        }
    }
    #endregion

    void Awake()
    {
        if (Instance == null)Instance = this;
        else
        {
            if (this != Instance)
                Destroy(this.gameObject);
        }

        StartCoroutine(SpawnAnimal());

        music = RuntimeManager.CreateInstance ("event:/Music");
        music.start();
        music.setParameterValue("Tension", 0);

        barf = RuntimeManager.CreateInstance("event:/Sounds/Erupt");

    }

    public bool characterSelect = true;
    float characterSelectTimer = 3;
    int playerAmount = 0;
    public GameObject[] charactersInMenu;
    public GameObject[] pressAInMenu;
    public GameObject menuCamera;
    float rotateTimer = 0;
    void CharacterSelect()
    {
        GameObject.Find("CharacterSelectTimer").transform.localScale = new Vector3(characterSelectTimer, 0.5f, 1);
        rotateTimer += Time.deltaTime;
        for(int i = 0; i < 4; i++)
        {
            if (controllers[i] == null)
            {
                charactersInMenu[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                pressAInMenu[i].GetComponent<SpriteRenderer>().enabled = true;
                if (Mathf.Round(rotateTimer) % 2 == 0) pressAInMenu[i].transform.eulerAngles += new Vector3(0, 0, Time.deltaTime*10);
                else pressAInMenu[i].transform.eulerAngles += new Vector3(0, 0, -Time.deltaTime*10);
            }
            else
            {
                charactersInMenu[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                pressAInMenu[i].GetComponent<SpriteRenderer>().enabled = false;
            }

        }

        if (InputManager.ActiveDevice.Action1.WasPressed)
        {
            int potentialSpot = -1;
            //Finding free spot
            for (int i = 0; i < 4; i++)
            {
                if (controllers[i] == InputManager.ActiveDevice)
                {
                    Debug.Log("Im already assigned");
                    potentialSpot = -1;
                    break;//Stop finding a spot
                }
                else
                {
                    if (controllers[i] == null)//Found spot
                    {
                        Debug.Log("This spot is free " + i);
                        potentialSpot = i;
                        break;//Stop finding a spot
                    }
                }
            }
            if (potentialSpot != -1)
            {
                AudioManager.instance.playerready[potentialSpot].start();
                controllers[potentialSpot] = InputManager.ActiveDevice;
                playerAmount += 1;
                Debug.Log("Assigned to " + potentialSpot);
            }
            else Debug.Log("Something went wrong");

        }

        if(controllers[0]!=null)characterSelectTimer -= Time.deltaTime;
        if (characterSelectTimer < 0)
        {
            GameObject.Find("CharacterSelectTimer").GetComponent<SpriteRenderer>().enabled = false;
            menuCamera.SetActive(false);
            Debug.Log("Times up");
            StartGame();
            characterSelect = false;
        }


    }

    void StartGame()
    {
        for(int i = 0; i < playerAmount;i++)
        {
            GameObject playerSpawn = Instantiate(player, spawnpoints[i].transform.position, transform.rotation) as GameObject;
            playerSpawn.name = "Player" + i;
            playerSpawn.GetComponent<Controls>().inputDevice = controllers[i];
            playerSpawn.GetComponent<Controls>().id = i;
            players[i] = playerSpawn;
        }
    }

    void Update()
    {
        SpriteLayering();
        ScreenShake();
        if(characterSelect)CharacterSelect();
    }

    void SpriteLayering()
    {
        //How every object overlaps eachother
        GameObject[] sprites = GameObject.FindGameObjectsWithTag("Sprites");
        
        foreach (GameObject o in sprites)
            o.GetComponent<SpriteRenderer>().sortingOrder = 20000 - ((int)Mathf.Round((o.transform.position.y + 100) * 100));
        

    }

    private Vector3 originPosition;
    private Quaternion originRotation;
    public float shake_decay;
    public float shake_intensity;

    public void Shake()
    {
        originPosition = Camera.main.transform.position;
        originRotation = Camera.main.transform.rotation;
        shake_intensity += .02f;
        shake_decay = 0.0003f;
    }

    public void ScreenShake()
    {
        if (shake_intensity > 0)
        {
            //Handheld.Vibrate();
            Camera.main.transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
            Camera.main.transform.rotation = new Quaternion(
            originRotation.x + Random.Range(-shake_intensity, shake_intensity) * .2f,
            originRotation.y + Random.Range(-shake_intensity, shake_intensity) * .2f,
            originRotation.z + Random.Range(-shake_intensity, shake_intensity) * .2f,
            originRotation.w + Random.Range(-shake_intensity, shake_intensity) * .2f);
            shake_intensity -= shake_decay;
        }
    }


    public GameObject barfVFX;
    public void Barf()
    {
        barfVFX.GetComponent<Animator>().Play("barf");
        barf.start();
    }

    //Spawning animals
    public bool unlock2 = false;//Unlock boars
    public bool unlock3 = false;//Unlock bizons
    public bool unlock4 = false;//Unlock more endbosses
    IEnumerator SpawnAnimal()
    {
        yield return new WaitForSeconds(Random.Range(1,4));

        GameObject[] animals1 = GameObject.FindGameObjectsWithTag("Animal1");
        GameObject[] animals2 = GameObject.FindGameObjectsWithTag("Animal2");
        GameObject[] animals3 = GameObject.FindGameObjectsWithTag("Animal3");

        float probability1 = 60-(animals1.Length*15); if (unlock2) probability1 = 40 - (animals1.Length * 20);
            float probability2 = 0; if(unlock2) probability2 = 20 - (animals2.Length * 7);
        float probability3 = 0; if(unlock3) probability3 = 3 -(animals3.Length*3); if (unlock4) probability3 = 10 - (animals3.Length * 10);

        

        float choiceRange = probability1 + probability2 + probability3; //85
        float choice = Random.Range(1, choiceRange);

        //if (choice > 0 && choice <= probability1) Debug.Log(choice+ ": Spawn Chicken");
        //if (choice > probability1 && choice <= probability1+probability2) Debug.Log(choice+ ": Spawn Boar");
        //if (choice > probability1+probability2 && choice < probability1+probability2+probability3) Debug.Log(choice+ ": Spawn Bull");


        if (choice > 0 && choice <= probability1)
        {
            GameManager.instance.animalSpawners[Random.Range(0, 6)].GetComponent<AnimalSpawner>().Spawn(GameManager.instance.animalSpawns[0]);
            StartCoroutine(SpawnAnimal());
        }
        else if (choice > probability1 && choice <= probability1 + probability2)
        {
            GameManager.instance.animalSpawners[Random.Range(0, 6)].GetComponent<AnimalSpawner>().Spawn(GameManager.instance.animalSpawns[1]);
            StartCoroutine(SpawnAnimal());
        }
        else if (choice > probability1 + probability2 && choice < probability1 + probability2 + probability3)
        {
            GameManager.instance.animalSpawners[Random.Range(0, 6)].GetComponent<AnimalSpawner>().Spawn(GameManager.instance.animalSpawns[2]);
            StartCoroutine(SpawnAnimal());
        }
        else
        {
            StartCoroutine(SpawnAnimal());
        }


    }

}
