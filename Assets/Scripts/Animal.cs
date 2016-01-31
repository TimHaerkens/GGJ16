using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PolyNavAgent))]
public class Animal : MonoBehaviour {

    private PolyNavAgent _agent;
    public PolyNavAgent agent
    {
        get
        {
            if (_agent == null)
                _agent = GetComponent<PolyNavAgent>();
            return _agent;
        }
    }

    public bool pickedUp = false;
    public GameObject carrier;
    float direction = 1;
    public bool sacrificed = false;

    Quaternion originRotation;

    public GameObject sprite;
    public SpriteRenderer ButtonA;

    void Awake()
    {
        originRotation = transform.rotation;
        spawn = true;
        StartCoroutine(WiggleNum());
        StartCoroutine(ChooseDestination());
    }

    bool spawn;
    float spawnMove = 1f;
    void Spawn()
    {
        if (spawnMove > 0)
        {
            if (transform.position.x < 0) transform.position += new Vector3(Time.deltaTime*3, 0, 0);
            if (transform.position.x > 0)
            {
                transform.position -= new Vector3(Time.deltaTime * 3, 0, 0);
                if (tag == "Animal1") anim.Play("bird_walk" + faceDirection);
                if (tag == "Animal2") anim.Play("boar_walk" + faceDirection);
                if (tag == "Animal3") anim.Play("bison_walk" + faceDirection);
            }
            spawnMove -= Time.deltaTime;
        }
        else
        {
            spawn = false;
            agent.enabled = true;
        }
    }

    //ANIMATIONS
    public Animator anim;
    [System.NonSerialized]public float walkDirection;
    [System.NonSerialized]public string faceDirection = "SE";
    public bool walking;
    void Face()
    {
        //FACE DIRECTION
        if (walkDirection < 360 && walkDirection > 360 - 90) faceDirection = "NE";
        else if (walkDirection < 180 + 90 && walkDirection > 180) faceDirection = "SE";
        else if (walkDirection < 180 && walkDirection > 90) faceDirection = "SW";
        else if (walkDirection < 90 && walkDirection > 0) faceDirection = "NW";

        if(walking)
        {
            if(tag=="Animal1")anim.Play("bird_walk" + faceDirection);
            if(tag=="Animal2")anim.Play("boar_walk" + faceDirection);
            if(tag=="Animal3")anim.Play("bison_walk" + faceDirection);
        }
        else
        {
            if (tag == "Animal1") anim.Play("bird_idle" + faceDirection);
            if (tag == "Animal2") anim.Play("boar_idle" + faceDirection);
            if (tag == "Animal3") anim.Play("bison_idle" + faceDirection);
        }


    }

    int speedChecker = 0;
    Vector2 posA;
    Vector2 posB;
    float movementSpeed = 0;
    void MovementSpeed()
    {
        switch (speedChecker)
        {
            case 1:
                posA = transform.position;
                break;
            case 10:
                posB = transform.position;
                break;
            case 11:
                movementSpeed = Vector2.Distance(posA, posB) * 10;
                speedChecker = 0;
                break;
        }
        speedChecker++;
    }

    bool intro = true;
    void Update ()
    {
        if (spawn) Spawn();
        if(intro && tag == "Animal1")
        {
            if (GameManager.instance.intensity > 0)
            {
                intro = false;
                ButtonA.enabled = false;
            }
        }

        //Wiggle();
        if (pickedUp || sacrificed || spawn)
        {
            agent.enabled = false;
            if (!spawn && tag == "Animal1") anim.Play("bird_carried");
        }
        else
        {
            //Calculate speed
            MovementSpeed();
            if (movementSpeed > 0.2f) walking = true;
            else walking = false;

            agent.enabled = true;
            if (tag == "Animal1" || tag == "Animal2" || tag == "Animal3")
                Face();
        }
        if (pickedUp)
        {
            Shake();
        }
        if (sacrificed)
        {
            Disappear();
            transform.position -= new Vector3(0, Time.deltaTime, 0);
        }

    }

    void Disappear()
    {
        Color currColor = sprite.GetComponent<SpriteRenderer>().color;
        sprite.GetComponent<SpriteRenderer>().color = new Color(currColor.r, currColor.g, currColor.b, currColor.a - Time.deltaTime*4);
        if(currColor.a <= 0)
        {
            GameManager.instance.Barf();
            GameManager.instance.Shake();
            Destroy(this.gameObject);
        }
    }

    void Wiggle()
    {
       transform.eulerAngles += new Vector3(0, 0, Time.deltaTime*direction*30);
       Debug.Log(transform.eulerAngles.z);
    }



    void Shake()
    {
        if (Random.Range(0, 50) == 1) direction = direction * -1;
        float rotation = 0 + direction * 0.01f;
        rotation = Mathf.Clamp(rotation, -10, 10);
        transform.rotation = new Quaternion(
        transform.rotation.x,
        transform.rotation.y,
        rotation,
        transform.rotation.w);
    }

    IEnumerator WiggleNum()
    {
        yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));

        direction = -direction;

        StartCoroutine(WiggleNum());
    }

    bool runAway;
    IEnumerator ChooseDestination()
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

        if (!pickedUp)
        {
            runAway = false;
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject p in players)
            {
                //Debug.Log("AAAAAAAHHHH");
                float xAway = (p.transform.position.x - transform.position.x) * -1;
                float yAway = (p.transform.position.y - transform.position.y) * -1;
                if (Vector2.Distance(p.transform.position, transform.position)<2)
                {
                    agent.maxSpeed = 6;
                    runAway = true;
                    agent.SetDestination(transform.position + new Vector3(xAway*3, yAway*3, 0));
                    break;
                }
            }
            if (!runAway)
            {
                agent.maxSpeed = 4;
                agent.SetDestination(transform.position + new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), 0));
            }
        }


        StartCoroutine(ChooseDestination());

    }
}
