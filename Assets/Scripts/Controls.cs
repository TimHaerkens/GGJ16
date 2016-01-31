using UnityEngine;
using System.Collections;
using InControl;

[RequireComponent(typeof(PolyNavAgent))]
public class Controls : MonoBehaviour
{
    public InputDevice inputDevice;

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

    public GameObject sprite;

    //Character stats
    public int id = 0;
    float speed = 5f; //How fast the player can go
    float range = 1f; //Pickup range
    public float level = 0;
    float progress = 0;

    public GameObject carry;


    float hThrottle = 0;
    float vThrottle = 0;

    public bool pickedUp = false;
    public bool sacrificed = false;
    public GameObject carrier = null;

    float pickUpCooldown = 0;

    GameObject hole;

    void Awake()
    {
        hole = GameObject.Find("hole");
    }

    
    void UpdateVisuals()
    {
        if(level==0)
        {
            sprite.GetComponent<SpriteRenderer>().color = Color.white;
            sprite.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        }
        if (level == 1)
        {
            sprite.GetComponent<SpriteRenderer>().color = new Color(1.0f,1.0f,0f);
            sprite.transform.localScale = new Vector3(3f, 3f, 3f);
        }
        if (level == 2)
        {
            sprite.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.6f, 0.2f);
            sprite.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
        }
        if (level == 3)
        {
            sprite.GetComponent<SpriteRenderer>().color = Color.red;
            sprite.transform.localScale = new Vector3(4f, 4f, 4f);
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

    //ANIMATIONS
    public Animator anim;
    [System.NonSerialized]
    public float walkDirection;
    [System.NonSerialized]
    public string faceDirection = "SE";
    public bool walking;
    void Face()
    {
        //FACE DIRECTION
        if (walkDirection < 360 && walkDirection > 360 - 90) faceDirection = "NE";
        else if (walkDirection < 180 + 90 && walkDirection > 180) faceDirection = "SE";
        else if (walkDirection < 180 && walkDirection > 90) faceDirection = "SW";
        else if (walkDirection < 90 && walkDirection > 0) faceDirection = "NW";

        if (walking)
        {
            if(carry)anim.Play("player_tilWalk" + faceDirection);
            else anim.Play("player_walk" + faceDirection);
        }
        else
        {
            //anim.Play("player_idle" + faceDirection);
        }


    }



    float rotateSpeed = 550;
    public GameObject pointer;
    public Vector2 lookDirection = new Vector2(0.93f, -0.35f);
    void Direction()
    {
        lookDirection = new Vector2(inputDevice.LeftStickX, inputDevice.LeftStickY);
        float rot = -Mathf.Atan2(lookDirection.x, lookDirection.y) * 180 / Mathf.PI;
        float newZ = Mathf.MoveTowardsAngle(pointer.transform.localEulerAngles.z, rot, rotateSpeed * Time.deltaTime);
        pointer.transform.localEulerAngles = new Vector3(pointer.transform.localEulerAngles.x, pointer.transform.localEulerAngles.y, newZ);
        walkDirection = newZ;
    }

    float freedom = 0;
    float freedomMinimum = 50;
    void Update()
    {


        if (pickedUp || sacrificed)
        {
            //if (carry) PutDown(carry);
            agent.enabled = false;

            if (!sacrificed)
            {
                //Wiggle free
                freedom += Mathf.Abs(inputDevice.LeftStickY) + Mathf.Abs(inputDevice.LeftStickX);
                if (freedom > freedomMinimum) Free();
                Carry();
            }
        }
        else
        {
            Direction();
            Movement();
            Carry();

            //Calculate speed
            MovementSpeed();
            if (movementSpeed > 0.2f) walking = true;
            else walking = false;

            agent.enabled = true;
            Face();
        }

        if (pickUpCooldown > 0) pickUpCooldown -= Time.deltaTime;
        if (pickUpCooldown < 0) pickUpCooldown = 0;

        //Spawn test
        if (inputDevice.GetControl(InputControlType.Action4) && pickUpCooldown == 0)
        {
            //GameManager.instance.Barf();
        }

        //Pickup button
        if (inputDevice.GetControl(InputControlType.Action3) && pickUpCooldown==0)
        {
            pickUpCooldown = 0.5f;
            if (carry != null)
            {
                PutDown(carry);
            }
            else
            {
                
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                GameObject[] animals1 = GameObject.FindGameObjectsWithTag("Animal1");
                GameObject[] animals2 = GameObject.FindGameObjectsWithTag("Animal2");
                GameObject[] animals3 = GameObject.FindGameObjectsWithTag("Animal3");

                if (carry == null)
                {
                    foreach (GameObject p in players)
                    {
                        if (p!=this.gameObject && p != null && Vector2.Distance(p.transform.position, transform.position) < range && level >= 3)
                        {
                            AudioManager.instance.pickupPlayers[p.GetComponent<Controls>().id].start();
                            PickUp(p);
                            break;
                        }
                    }
                }
                if (carry == null)
                {
                    foreach (GameObject a in animals1)
                    {
                        if (a != null && Vector2.Distance(a.transform.position, transform.position) < range && level >= 0)
                        {
                            AudioManager.instance.pickup1.start();
                            PickUp(a);
                            break;
                        }
                    }
                }
                if (carry == null)
                {
                    foreach (GameObject a in animals2)
                    {
                        if (a != null && Vector2.Distance(a.transform.position, transform.position) < range && level >= 1)
                        {
                            AudioManager.instance.pickup2.start();
                            PickUp(a);
                            break;
                        }
                    }
                }
                if (carry == null)
                {
                    foreach (GameObject a in animals3)
                    {
                        if (a != null && Vector2.Distance(a.transform.position, transform.position) < range && level >= 2)
                        {
                            AudioManager.instance.pickup3.start();
                            PickUp(a);
                            break;
                        }
                    }
                }
            }
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
        sprite.GetComponent<SpriteRenderer>().color = new Color(currColor.r, currColor.g, currColor.b, currColor.a - Time.deltaTime * 4);
        if (currColor.a <= 0)
        {
            if(carry!=null)PutDown(carry);
            GameManager.instance.Barf();
            GameManager.instance.Shake();
            Destroy(this.gameObject);
        }
    }




    void Carry()
    {
        if (carry != null)
        {
            if (carry.tag == "Animal1") speed = 4.5f;
            if (carry.tag == "Animal2") speed = 4.0f;
            if (carry.tag == "Animal3") speed = 3.5f;
            carry.transform.position = transform.position + new Vector3(0, 1.4f, 0);
        }
        else speed = 5;
    }

    float handicapX = 0;
    float handicapY = 0;
    void Movement()
    {

        hThrottle += inputDevice.LeftStickX;
        hThrottle = Mathf.Clamp(hThrottle, -1.0f, 1.0f);
        vThrottle += inputDevice.LeftStickY;
        vThrottle = Mathf.Clamp(vThrottle, -1.0f, 1.0f);


        //Speed
        float hSpeed = 0;
        float vSpeed = 0;

        //Random handicap movement
        
        if (carry != null)
        {
            if (Random.Range(0, 15) == 5)
            {
                handicapX = Random.Range(-0.01f, 0.01f);//Change add sometimes
                handicapY = Random.Range(-0.01f, 0.01f);//Change add sometimes
            }
        }
        if (Mathf.Abs(inputDevice.LeftStickX)>0.2f)hSpeed = (inputDevice.LeftStickX) * Time.deltaTime * speed;
        if(Mathf.Abs(inputDevice.LeftStickY)>0.2f)vSpeed = (inputDevice.LeftStickY) * Time.deltaTime * speed;

        transform.position += new Vector3(hSpeed + handicapX, vSpeed + handicapY, 0);
    }

    bool nearHole()
    {
        if (Mathf.Abs(transform.position.x - hole.transform.position.x) < 2.5f && Mathf.Abs(transform.position.y - hole.transform.position.y) < 1.5f)
            return true;

        return false;
    }


    void LevelUp()
    {
        level += 1;
        Debug.Log("level up to: " + level);
        progress = 0;
        UpdateVisuals();
        if (level == 1) { GameManager.instance.unlock2 = true; Debug.Log("Unlock 2"); }
        if (level == 2) { GameManager.instance.unlock3 = true; Debug.Log("Unlock 3"); }
        if (level == 3) { GameManager.instance.unlock4 = true; Debug.Log("Unlock 4"); }
        if (level == 3) GameManager.instance.music.setParameterValue("Tension", 1);
        
    }


    void PickUp(GameObject who)
    {
        AudioManager.instance.pickup.start();

        if (who.tag == "Player")
        {
            GameObject whoTemp = who;
            if (who.GetComponent<Controls>().pickedUp) whoTemp = who.GetComponent<Controls>().carrier;

            whoTemp.GetComponent<Controls>().pickedUp = true;
            whoTemp.GetComponent<Controls>().carrier = this.gameObject;
            whoTemp.GetComponent<Controls>().agent.Stop();
        }
        else //picking up animal
        {
            if (who.GetComponent<Animal>().pickedUp)//Someone is carrying it already
            {
                //Steal
                who.GetComponent<Animal>().carrier.GetComponent<Controls>().carry = null;
                who.GetComponent<Animal>().pickedUp = true;
                who.GetComponent<Animal>().carrier = this.gameObject;
                who.GetComponent<Animal>().agent.Stop();
            }
            else
            {
                who.GetComponent<Animal>().pickedUp = true;
                who.GetComponent<Animal>().carrier = this.gameObject;
                who.GetComponent<Animal>().agent.Stop();
            }
        }

        carry = who;
    }

    void PutDown(GameObject who)
    {
        handicapY = 0;
        handicapX = 0;

        AudioManager.instance.drop.start();

        transform.eulerAngles = new Vector3(0, 0, 0);

        //IN THE HOLE????
        #region throwing in the hole
        if (nearHole())
        {
            if (who.tag == "Player")//Threw in player
            {
                //Player dies
                AudioManager.instance.diePlayers[who.GetComponent<Controls>().id].start();
                who.GetComponent<Controls>().sacrificed = true;
                who.transform.position = hole.transform.position + new Vector3(0, 0.1f, 0);

                //Let go
                who.GetComponent<Controls>().pickedUp = false;
                who.GetComponent<Controls>().carrier = null;
                carry = null;
                //This player is now out of the game                
            }
            if (who.tag == "Animal1" || who.tag == "Animal2" || who.tag == "Animal3")
            {
                who.GetComponent<Animal>().sacrificed = true;
                who.transform.position = hole.transform.position + new Vector3(0, 0.1f, 0);
                who.GetComponent<Animal>().pickedUp = false;
                carry = null;
            }
            //Leveling
            if(who.tag=="Animal1")//Threw in chicken
            {
                AudioManager.instance.die1.start();
                if (level == 0)
                {
                    progress += 0.34f;
                    if (progress >= 1) LevelUp();
                }
                else
                {
                    //Wrong animal
                }
            }
            if (who.tag == "Animal2")//Threw in boar
            {
                AudioManager.instance.die2.start();
                if (level == 1)
                {
                    progress += 0.5f;
                    if (progress >= 1) LevelUp();
                }
                else
                {
                    //Wrong animal
                }
            }
            if (who.tag == "Animal3")//Threw in bull
            {
                AudioManager.instance.die3.start();
                if (level == 2)
                {
                    progress += 1f;
                    if (progress >= 1) LevelUp();
                }
                else
                {
                    //Wrong animal
                }
            }
            
        }
        #endregion 
        else
        {
            if (who.tag == "Animal1" || who.tag == "Animal2" || who.tag == "Animal3")
            {
                who.transform.position = transform.position + new Vector3(0.18f, -0.1f, 0);
                who.GetComponent<Animal>().pickedUp = false;
                carry = null;
            }
            if (who.tag == "Player")
            {
                who.transform.position = transform.position + new Vector3(0.18f, -0.1f, 0);
                who.GetComponent<Controls>().pickedUp = false;
                who.GetComponent<Controls>().carrier = null;
                carry = null;
            }
        }
    }

    void Free()
    {
        freedom = 0;
        transform.position = transform.position + new Vector3(0.18f, -0.1f, 0);
        pickedUp = false;
        carrier.GetComponent<Controls>().carry = null;
        carrier = null;
    }

    




}
