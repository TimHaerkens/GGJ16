using UnityEngine;
using System.Collections;
using FMODUnity;

public class AudioManager : MonoBehaviour {

    //FMOD
   
    public FMOD.Studio.EventInstance[] diePlayers = new FMOD.Studio.EventInstance[4];

    
    public FMOD.Studio.EventInstance[] pickupPlayers = new FMOD.Studio.EventInstance[4];

    public FMOD.Studio.EventInstance pickup;
    public FMOD.Studio.EventInstance drop;

    public FMOD.Studio.EventInstance[] playerready = new FMOD.Studio.EventInstance[4];


    public FMOD.Studio.EventInstance barf1;
    public FMOD.Studio.EventInstance barf2;
    public FMOD.Studio.EventInstance barf3;
    public FMOD.Studio.EventInstance barf4;



    FMOD.Studio.ParameterInstance pan;

    private static AudioManager Instance;
    public static AudioManager instance
    {
        get
        {
            if (Instance == null)
                Instance = GameObject.FindObjectOfType<AudioManager>();
            return Instance;
        }
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            if (this != Instance)
                Destroy(this.gameObject);
        }

        pickup = RuntimeManager.CreateInstance("event:/Sounds/Players/Pickup_sound");
        drop = RuntimeManager.CreateInstance("event:/Sounds/Players/Drop_sound");


        diePlayers[0] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player1_Die");
        diePlayers[1] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player2_Die");
        diePlayers[2] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player3_Die");
        diePlayers[3] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player4_Die");

        
        pickupPlayers[0] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player1_Pickup");
        pickupPlayers[1] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player2_Pickup");
        pickupPlayers[2] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player3_Pickup");
        pickupPlayers[3] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player4_Pickup");

        playerready[0] = RuntimeManager.CreateInstance("event:/Sounds/Animals/Announcer/Player 1");
        playerready[1] = RuntimeManager.CreateInstance("event:/Sounds/Animals/Announcer/Player 2");
        playerready[2] = RuntimeManager.CreateInstance("event:/Sounds/Animals/Announcer/Player 3");
        playerready[3] = RuntimeManager.CreateInstance("event:/Sounds/Animals/Announcer/Player 4");


        //diePlayers[3].start();
    }

}
