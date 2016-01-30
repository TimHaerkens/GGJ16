using UnityEngine;
using System.Collections;
using FMODUnity;

public class AudioManager : MonoBehaviour {

    //FMOD
    public FMOD.Studio.EventInstance die1;
    public FMOD.Studio.EventInstance die2;
    public FMOD.Studio.EventInstance die3;
    public FMOD.Studio.EventInstance[] diePlayers = new FMOD.Studio.EventInstance[4];

    public FMOD.Studio.EventInstance pickup1;
    public FMOD.Studio.EventInstance pickup2;
    public FMOD.Studio.EventInstance pickup3;
    public FMOD.Studio.EventInstance[] pickupPlayers = new FMOD.Studio.EventInstance[4];

    public FMOD.Studio.EventInstance pickup;
    public FMOD.Studio.EventInstance drop;



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


        die1 = RuntimeManager.CreateInstance("event:/Sounds/Animals/Chicken_Die");
        die2 = RuntimeManager.CreateInstance("event:/Sounds/Animals/Pig_Die");
        die3 = RuntimeManager.CreateInstance("event:/Sounds/Animals/Bison_Die");
        diePlayers[0] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player1_Die");
        diePlayers[1] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player2_Die");
        diePlayers[2] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player3_Die");
        diePlayers[3] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player4_Die");

        pickup1 = RuntimeManager.CreateInstance("event:/Sounds/Animals/Chicken_Pickup");
        pickup2 = RuntimeManager.CreateInstance("event:/Sounds/Animals/Pig_Pickup");
        pickup3 = RuntimeManager.CreateInstance("event:/Sounds/Animals/Bison_Pickup");
        pickupPlayers[0] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player1_Pickup");
        pickupPlayers[1] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player2_Pickup");
        pickupPlayers[2] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player3_Pickup");
        pickupPlayers[3] = RuntimeManager.CreateInstance("event:/Sounds/Players/Player4_Pickup");

        //diePlayers[3].start();
    }

}
