using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;
using FMODUnity;

public class GameManager : MonoBehaviour {

    //FMOD
    public FMOD.Studio.EventInstance music;
    public FMOD.Studio.EventInstance countdown;
    FMOD.Studio.ParameterInstance tension;

    public GameObject[] animalSpawners;//animalSpawners in the game
    public GameObject[] spawnpoints;//Points a player can spawn

    //Players
    public List<GameObject> players;//All the players in the game
    public InputDevice[] controllers = new InputDevice[4] { null, null, null, null };
    public Color[] playerColors1 = new Color[4];
    public Color[] playerColors2 = new Color[4];
    public Color[] playerColors3 = new Color[4];
    public Color[] playerColors4 = new Color[4];

    //Resources
    public GameObject[] animalSpawns;//Animals that can be spawned
    public GameObject player;//A player that can be spawned

    public GameObject pitButtonA;    public GameObject pixelCamera;//Vignette effect overlay    public bool endGame = false;    public bool firstBison;

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

    public bool intro;
    void Awake()
    {
        originalPosition = Camera.main.transform.position;
        firstBison = true;
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

        countdown = RuntimeManager.CreateInstance("event:/Sounds/Level_Select_Countdown");
        

        intro = true;

    }

    public bool characterSelect = true;
    float characterSelectTimer = 7;
    int playerAmount = 0;
    public GameObject[] charactersInMenu;
    public GameObject[] pressAInMenu;
    public GameObject pressYInMenu;
    public GameObject menuCamera;
    float rotateTimer = 0;

    public GameObject select1;
    public GameObject select2;
    public GameObject select3;
    public GameObject select4;

    public bool pause = false;
    float pauseCooldown = 0;
    void Pause()
    {
        //Time.timeScale = 0;

        //Go to character select
        if (InputManager.ActiveDevice.Action1.WasPressed)
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        //Go back to game
        if (InputManager.ActiveDevice.Action2.WasPressed || InputManager.MenuWasPressed)
        {
            pauseCooldown = 0.5f;
            pause = false;
        }

    }

    bool credits = false;
    public GameObject creditsScreen;
    public GameObject characterSelectTimerBar;
    void CharacterSelect()
    {
        if (InputManager.ActiveDevice.Action4.WasPressed)
        {
            if (credits) credits = false;
            else credits = true;
        }
        if(credits)
        {
            Color tempColor = creditsScreen.GetComponent<SpriteRenderer>().color;
            if(tempColor.a<1)tempColor.a += Time.deltaTime;
            creditsScreen.GetComponent<SpriteRenderer>().color = tempColor;
        }
        else
        {
            Color tempColor = creditsScreen.GetComponent<SpriteRenderer>().color;
            if(tempColor.a>0)tempColor.a -= Time.deltaTime;
            creditsScreen.GetComponent<SpriteRenderer>().color = tempColor;
        }

        //Move in
        select1.transform.position -= new Vector3(0, (select1.transform.position.y-1.65f) * Time.deltaTime * 2,0);
        select2.transform.position -= new Vector3(0, (select2.transform.position.y-1.65f) * Time.deltaTime * 2, 0);
        select3.transform.position -= new Vector3(0, (select3.transform.position.y-1.65f) * Time.deltaTime * 2, 0);
        select4.transform.position -= new Vector3(0, (select4.transform.position.y-1.65f) * Time.deltaTime * 2, 0);


        characterSelectTimerBar.transform.localScale = new Vector3(characterSelectTimer/2, 2f, 1);
        rotateTimer += Time.deltaTime;
        for(int i = 0; i < 4; i++)
        {
            if (controllers[i] == null)
            {
                
                charactersInMenu[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                pressAInMenu[i].GetComponent<SpriteRenderer>().enabled = true;
                if (Mathf.Round(rotateTimer) % 2 == 0)
                {
                    pressAInMenu[i].transform.eulerAngles += new Vector3(0, 0, Time.deltaTime * 10);
                    pressYInMenu.transform.eulerAngles += new Vector3(0, 0, Time.deltaTime * 2);
                }
                else
                {
                    pressAInMenu[i].transform.eulerAngles += new Vector3(0, 0, -Time.deltaTime * 10);
                    pressYInMenu.transform.eulerAngles += new Vector3(0, 0, -Time.deltaTime * 2);
                }
            }
            else
            {
                Color tempColor = playerColors1[i];
                tempColor.a = 1f;
                charactersInMenu[i].GetComponent<SpriteRenderer>().color = tempColor;
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
                        if(i==1) countdown.start();
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

        if (controllers[0] != null && controllers[1] != null)
        {
            //Timer begins
            characterSelectTimer -= Time.deltaTime * (1+playerAmount/4);
            Color tempColor = characterSelectTimerBar.GetComponent<SpriteRenderer>().color;
            tempColor.a += Time.deltaTime;
            characterSelectTimerBar.GetComponent<SpriteRenderer>().color = tempColor;

        }
        if (characterSelectTimer < 0)
        {
            GameObject.Find("CharacterSelectTimer").GetComponent<SpriteRenderer>().enabled = false;
            menuCamera.SetActive(false);
            Debug.Log("Times up");
            StartGame();
            characterSelect = false;
        }


    }

    public bool finished = false;
    public GameObject logo;
    public void Win()
    {
        MoreIntense(4);
        finished = true;
    }

    void StartGame()
    {
        RuntimeManager.PlayOneShot("event:/Sounds/Animals/Announcer/START", transform.position);

        countdown.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        for (int i = 0; i < playerAmount;i++)
        {
            GameObject playerSpawn = Instantiate(player, spawnpoints[i].transform.position, transform.rotation) as GameObject;
            playerSpawn.name = "Player" + i;
            playerSpawn.GetComponent<Controls>().inputDevice = controllers[i];
            playerSpawn.GetComponent<Controls>().id = i;
            playerSpawn.GetComponent<Controls>().UpdateVisuals();
            players[i] = playerSpawn;
        }
    }

    float finishTimer = 14;
    bool eagle = true;
    public GameObject pauseBlack;
    public GameObject pausePrompt;
    Vector3 originalPosition;
    void Update()
    {

        //Camera movement -9 to 9
        float averageX = 0;
        float averageAmount = 0;
        float totalX = 0;
        for(int i = 0; i < 4; i++)
        {
            if(players[i]!= null)
            {
                averageAmount++;
                totalX += players[i].transform.position.x;                
            }
        }
        if(averageAmount!=0)averageX = totalX / averageAmount;
        Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, originalPosition + new Vector3((averageX / 10), 0, 0), Time.deltaTime/5);



        if (pauseCooldown > 0) pauseCooldown -= Time.deltaTime;
        if(pause)
        {
            Pause();
            Color tempColor = pauseBlack.GetComponent<SpriteRenderer>().color;
            if (tempColor.a < 0.7f) tempColor.a += Time.deltaTime;
            pauseBlack.GetComponent<SpriteRenderer>().color = tempColor;

            pausePrompt.transform.position += new Vector3(0, (- pausePrompt.transform.position.y) * Time.deltaTime * 10, 0);
        }
        else
        {
            Color tempColor = pauseBlack.GetComponent<SpriteRenderer>().color;
            if (tempColor.a > 0) tempColor.a -= Time.deltaTime;
            pauseBlack.GetComponent<SpriteRenderer>().color = tempColor;

            if(pausePrompt.transform.position.y > -8)pausePrompt.transform.position -= new Vector3(0, Time.deltaTime * 20, 0);
        }

        if (InputManager.MenuWasPressed && !pause && !characterSelect && pauseCooldown<=0)
        {
            Debug.Log("pause");
            pause = true;
        }

        SpriteLayering();
        ScreenShake();
        if(characterSelect)CharacterSelect();

        if (finished)
        {
            finishTimer -= Time.deltaTime;
            if (finishTimer < 7 && finishTimer > 1) logo.transform.position -= new Vector3(0, (logo.transform.position.y - 1) / 50, 0);
            if (finishTimer < 7 && eagle)
            {
                RuntimeManager.PlayOneShot("event:/Sounds/Animals/Eagle", Camera.main.transform.position);
                eagle = false;
            }
            if (finishTimer < 1)
            {
                logo.transform.position -= new Vector3(80 * Time.deltaTime, 0, 0);
                foreach(GameObject g in players)
                {
                    if (g != null)
                    {
                        g.GetComponent<Controls>().agent.enabled = false;
                        g.transform.position -= new Vector3(80 * Time.deltaTime, 0, 0);
                    }
                }
            }

            if (finishTimer <= 0) Application.LoadLevel(Application.loadedLevel);
        }
    }

    void SpriteLayering()
    {
        //How every object overlaps eachother
        GameObject[] sprites = GameObject.FindGameObjectsWithTag("Sprites");
        
        foreach (GameObject o in sprites)
            o.GetComponent<SpriteRenderer>().sortingOrder = 20000 - ((int)Mathf.Round((o.transform.position.y + 100) * 100));
        

    }

    public int intensity = 0;
    public void MoreIntense(int newIntensity)
    {
        //Intensity can only increase
        if (newIntensity > intensity)
            music.setParameterValue("Tension", newIntensity);
        intensity = newIntensity;
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
    public void Barf(GameObject offer)
    {
        if (offer.tag == "Animal1")
        {
            barfVFX.GetComponent<Animator>().Play("barf1");
            RuntimeManager.PlayOneShot("event:/Sounds/Erupt_Chicken", transform.position);
        }
        if (offer.tag == "Animal2")
        {
            barfVFX.GetComponent<Animator>().Play("barf");
            RuntimeManager.PlayOneShot("event:/Sounds/Erupt_Pig", transform.position);
        }
        if (offer.tag=="Animal3")
        {
            barfVFX.GetComponent<Animator>().Play("barf3");
            RuntimeManager.PlayOneShot("event:/Sounds/Erupt_Dolyak", transform.position);
        }
        if (offer.tag=="Player")
        {
            barfVFX.GetComponent<Animator>().Play("barf4");
            RuntimeManager.PlayOneShot("event:/Sounds/Erupt_Human", transform.position);
        }
    }


    //Colors
    public Color[] color1;
    public Color[] color2;
    public Color[] color3;

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

        if (!GameManager.instance.finished)
        {
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

}
