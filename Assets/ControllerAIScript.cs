using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ControllerAIScript : MonoBehaviour {

    public List<string> myLogText;

    private List<string> currentLoop;
    public TextMesh infoText;
    int full;
    string currentTag;
    string deathReason;


    private Dictionary<string, int> chest;

    public TextMesh Minion_Thoughts;
    public TextMesh DeathScreen;
    public int day;
    Light Light;
    Renderer Firepit;
    AudioSource fire;

    int loopIter;

    public Transform target;
    float MoveSpeed = 4;
    int MinDist = 3;

    bool notDead;

    bool EnterMode;

    int count;

    void updateMyLog(string entry)
    {
        myLogText.Add(entry);
       GameObject.Find("LogBook").GetComponent<TextMesh>().text =  System.String.Join("\n ", myLogText.ToArray());
        
    }
    // Use this for initialization
    void Start () {
        //init all the screens stuff
        tineToWalk = false;
        loopIter = 0;
        notDead = true;
        full = 3;
        currentLoop = new List<string>();
        myLogText = new List<string>();
        chest = new Dictionary<string, int>();
        infoText = GameObject.Find("Info_LoopText").GetComponent<TextMesh>();
        infoText.text = "Click the cubes in the order of desired action.";
        Minion_Thoughts = GameObject.Find("Thoughts").GetComponent<TextMesh>();
        Minion_Thoughts.text = "Hi";

        DeathScreen = GameObject.Find("DeathScreen").GetComponent<TextMesh>();
        deathReason = "";

        Firepit = GameObject.Find("Campfire").GetComponent<Renderer>();
        Firepit.enabled = false;

        fire = GameObject.Find("Campfire").GetComponent<AudioSource>();
        fire.volume = 0;

        Light = GameObject.Find("LightDN").GetComponent<Light>();

        day = 1;

        count = 0;

        EnterMode = true;

        currentTag = "nothing";

    }

    private void OnTriggerEnter(Collider other)
    {
        if (EnterMode)
        {

        if (other.tag == "AppleButton")
        {
            currentLoop.Add("Pick Apples");
        }
        else if (other.tag == "StoneButton")
        {
            currentLoop.Add("Collect Stones");
        }
        else if (other.tag == "StickButton")
        {
            currentLoop.Add("Gather Sticks");
        }
        else if (other.tag == "StoreInvButton")
        {
            currentLoop.Add("Store in Chest");
        }
        else if (other.tag == "EatingButton")
        {
            currentLoop.Add("Eat");

        }


        }

       
 if (other.tag.Contains("Button"))
        {
            Debug.Log(other.tag + " pressed");
        }


        if (other.tag == "ClearButton")
        {
            currentLoop.Clear();
            EnterMode = true;
        }
        if (other.tag == "RemoveButton")
        {
            if (currentLoop.Any())
            {
                EnterMode = true;
                currentLoop.RemoveAt(currentLoop.Count - 1);
            }
        }
        if (other.tag == "StartButton")
        {
            bool isItEmpty = !currentLoop.Any();

            if (isItEmpty)
            {
                Debug.Log("Empty list");
            }
            else
            {
                EnterMode = false;
                // startPlayer();
                runningIsland();

            }


        }

        bool isEmpty = currentLoop == null || !currentLoop.Any();
        if (isEmpty)
        {
            infoText.text = "Create a loop for your minion\n Click the cubes in the order of desired action.";
        }
        else
        {
            infoText.text = System.String.Join("\n ", currentLoop.ToArray());
        }


    }

    private void runningIsland()
    {
        if (EnterMode == false)
        {
            if (notDead)
            {
                string action = nextInLoop();

                GameObject.Find("CAText").GetComponent<TextMesh>().text = action;
                updateMyLog("Action: " + action);

                //Minion_Thoughts.text = action;



                switch (action)
                {
                    case "Pick Apples":
                        minionWalkTo("Oak_Tree");
                        checkifAlreadyHolding();
                        currentTag = "apple";
                        Minion_Thoughts.text = "Picking " + currentTag;

                        break;
                    case "Collect Stones":
                        minionWalkTo("Rock");
                        checkifAlreadyHolding();
                        currentTag = "stone";
                        Minion_Thoughts.text = "Looking for " + currentTag;

                        break;
                    case "Gather Sticks":
                        minionWalkTo("Bush");
                        checkifAlreadyHolding();
                        currentTag = "stick";
                        Minion_Thoughts.text = "Looking for " + currentTag;


                        break;
                    case "Store in Chest":
                        minionWalkTo("Crate");
                        if (currentTag != "nothing")
                        {
                            if (chest.ContainsKey(currentTag))
                            {
                                int newNumber = chest[currentTag] + 1;
                                chest[currentTag] = newNumber;
                            }

                         // GameObject.Find("ChestInv").GetComponent<TextMesh>().text = string.Join("\n", chest.ToString());
                         //TODO chest inv

                        }

                        Minion_Thoughts.text = "Storing " + currentTag + " in Chest.";

                        break;
                    case "Eat":
                        minionWalkTo("Eating_Spot");
                        if (currentTag == "apple")
                        {
                            full += 1;
                            currentTag = "nothing";
                            Minion_Thoughts.text = "Ate an apple";

                        }
                        else if (chest.ContainsKey("apple"))
                        {
                            if (chest["apple"] > 0)
                            {
                                Minion_Thoughts.text = "Ate an apple";
                                full += 1;
                                int newNumber = chest[currentTag] - 1;
                                if (newNumber < 1)
                                {
                                    chest.Remove("apple");
                                }
                                else
                                {
                                    chest[currentTag] = newNumber;
                                }
                            }
                        }
                        else
                        {
                            Minion_Thoughts.text = "No apples to eat.";
                            updateMyLog("No apples to eat.");
                        }

                        GameObject.Find("Fullness").GetComponent<TextMesh>().text = "Fullness " + (full / 3.0) * 100 + " %";



                        break;


                }

                checkCondition();

                DeathScreen.text = deathReason;

                if (deathReason != "")
                {
                    notDead = false;
                }

            }
            else
            {
                GameObject.Find("CAText").GetComponent<TextMesh>().text = "Dead!";

            }
        }


    }
    bool tineToWalk;
    private void minionWalkTo(string placeToGo)
    {
        //get fixed update to walk and only call runningIsland when finish walking
        currentTargetst = placeToGo;
        tineToWalk = true;
    }

    string currentTargetst;
    // Update is called once per frame
    void FixedUpdate () {
        //enter mode or doing mode
        if (tineToWalk)
        {
            Transform currentTarget = GameObject.Find(currentTargetst).GetComponent<Transform>();
            Transform minionTransform = GameObject.Find("Minion").GetComponent<Transform>();
            minionTransform.LookAt(currentTarget);

            if (Vector3.Distance(minionTransform.position, currentTarget.position) >= MinDist)
            {
                Debug.Log(Vector3.Distance(minionTransform.position, currentTarget.position));
                minionTransform.LookAt(currentTarget);

                minionTransform.position += minionTransform.forward * MoveSpeed * Time.deltaTime;

                //pause
            }
            else if (Vector3.Distance(minionTransform.position, currentTarget.position)== MinDist)
            {
                tineToWalk = false;
                runningIsland();
            }
        }

	}

    
    private string nextInLoop()
    {
        string action =
        currentLoop.ElementAt(loopIter);

        loopIter += 1;

        if (loopIter > currentLoop.Count() - 1)
        {
            loopIter = 0;
        }

        return action;
    }

    

    private void checkCondition()
    {


        count++;

        if (count == 12)
        { //get apple / eat / get apple / eat / get apple / eat / get stick/ store / get stick/ store / get stone / store - perfect sequence day one
            day += 1;
            count = 0;
            nightTrigger();
            Minion_Thoughts.text = "Time to rest for the day, I wonder if I have enough sticks and stones for the fire.";

            if (chest.ContainsKey("stone") && chest.ContainsKey("stick"))
            {
                if (chest["stone"] > 0 && chest["stick"] > 1)
                {
                    chest["stone"] = chest["stone"] - 1;
                    chest["stick"] = chest["stick"] - 2;
                    Firepit.enabled = true;
                    fire.volume = 0.25F;
                    //pause
                    Minion_Thoughts.text = "Yay! I can make fire!";
                    updateMyLog("Made Fire");
                    //fire pit appear

                    if (chest["stone"] > 0 && chest["stick"] > 0)
                    {
                        chest["stone"] = chest["stone"] - 1;
                        chest["stick"] = chest["stick"] - 1;
                        //pause

                        Minion_Thoughts.text = "Oh! I can make an axe too!";

                        chest["axe"] += 1;
                        updateMyLog("Made Axe");

                        //pause

                    }

                    full -= 3;
                    GameObject.Find("Fullness").GetComponent<TextMesh>().text = "Fullness " + (full / 3.0) * 100 + " %";

                }
                else
                {
                    deathReason = "Died from the cold.";
                    //fire pit hide
                    Firepit.enabled = false;
                    fire.volume = 0;
                }
            }


            dayTrigger();

        }

        if (full > 10)
        {
            //load death screen - overEat
            deathReason = "Died because he over ate";
        }
        else if (full > 5)
        {
            Minion_Thoughts.text = "I am getting too full";
        }
        else if (full > 3)
        {
            Minion_Thoughts.text = "Mmm I'm full";
        }
        else if (full == 0)
        {
            Minion_Thoughts.text = "Starving";
        }
        else if (full < 0)
        {
            // load death screen - starvation
            deathReason = "Died from starvation";

        }
    }

    private void dayTrigger()
    {
        Light.intensity = 1;
    }

    private void nightTrigger()
    {
        Light.intensity = 0.175F;
    }

    private void checkifAlreadyHolding()
    {
        if (currentTag != "nothing")
        {
            Minion_Thoughts.text = "Tossing away previous item: " + currentTag;
        }
    }
}
