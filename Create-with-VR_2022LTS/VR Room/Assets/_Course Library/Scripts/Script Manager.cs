using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;
using System.Buffers;

public class Sequenza : MonoBehaviour
{
    //starting shape and stimulus it will turn into
    public Image startingImage;
    public Sprite stimulus;
    //expo phase images
    public List<Image> images;

    //reaction time variables
    public List<Image> rtImages;
    public List<float> reactionTime;
    public List<int> rtHitOrMiss; //for reaction time, 0 = mistake, 1 = right
    public float startTime;
    public float stopTime;


    //recognition trial variables
    public List<int> slicedList;
    public int startingIndex;
    public int recogTrials = 2;
    public List<int> recogHitOrMiss;

    //second order sequences as per Destrebecqz & cleeremans 2001
    public List<int> sequenza1 = new List<int> { 3, 4, 2, 3, 1, 2, 1, 4, 3, 2, 4, 1 };
    public List<int> sequenza2 = new List<int> { 3, 4, 1, 2, 4, 3, 1, 4, 2, 1, 3, 2 };


    public int repNum = 2; //number of sequence repetitions
    public int maxBlockNum = 2; //number of experimental blocks of repetitions
    public int myBlockNum; //the number of blocks elapsed


    public GameObject sequenceExpoPanel;
    public GameObject cfsPanel;
    public GameObject afterExpoPanel;
    public GameObject reactionTimePanel;
    public GameObject beforeRecognitionTaskPanel;
    public GameObject breakFromReactionTaskPanel;
    

    //checks
    public bool isReactionTimeFinished = false;
    public bool isRecognitionTaskFinished = false;
    public bool isExposureFinished = false;

    public TMP_Text message;
    
    public int taskNum = 0;
    public KeyCode pressedKey;

    // Start is called before the first frame update
    void Start()
    {

    }

    
    //checks the experimental condition:
    IEnumerator exposurePhase(int condition)
    {

        //if the condition involves the DETERMINED sequence, then for each number it
        //changes the corrispective sprite and then changes it back
        if (condition == 1 || condition == 2)
        {
            for (int i = 0; i < sequenza1.Count; i++)
            {

                /*//expose to stimulus for 200 ms
                images[sequenza1[i] - 1].sprite = stimulus;
                yield return new WaitForSeconds(0.2f);

                //then change it back, and show next stimulus after 50 ms
                images[sequenza1[i] - 1].sprite = startingImage.sprite;
                yield return new WaitForSeconds(0.05f);*/
                
                images[sequenza1[i] - 1].GetComponent<UnityEngine.UI.Image>().color = new Color (0.5f,0.5f,0.5f);
                yield return new WaitForSeconds(0.2f);

                //then change it back, and show next stimulus after 50 ms
                images[sequenza1[i] - 1].GetComponent<UnityEngine.UI.Image>().color = new Color(0.59f, 0.59f, 0.59f);
                yield return new WaitForSeconds(0.05f);

            }
        }

        //if it's the RANDOM sequence condition, then for each number it
        //changes a random sprite and then changes it back
        else if (condition == 3)
        {
            for (int i = 0; i < sequenza1.Count; i++)
            {
                //randomized item changes color for 0.2 secs
                int randomIndex = Random.Range(0, 3);
                images[randomIndex].GetComponent<UnityEngine.UI.Image>().color = new Color(0.5f, 0.5f, 0.5f);
                yield return new WaitForSeconds(0.2f);

                //then changes it back and shows next item after 50 ms
                images[randomIndex].GetComponent<UnityEngine.UI.Image>().color = new Color(0.59f, 0.59f, 0.59f);
                yield return new WaitForSeconds(0.05f);
            }
        }

        //then, after it finished the sequence, after 2 secs it switches to
        //the afterExpoPanel
        yield return new WaitForSeconds(2f);
        sequenceExpoPanel.SetActive(false);
        cfsPanel.SetActive(false);
        afterExpoPanel.SetActive(true);
        


        isExposureFinished = true;
    }

    //this coroutine starts the serial reacton time task
    IEnumerator ReactionTimeTask()
    {

        //250 ms dopo, nuovo stimolo, per un totale di 6 blocchi da 120 trials fatti da 10 ripetizioni

        for (int myBlockNum = 0; myBlockNum < maxBlockNum; myBlockNum++)
        {
            //repeat a single block of 10 repetitions of sequence
            for (int j = 0; j < repNum; j++)
            {
                //for each element of the sequence
                for (int i = 0; i < sequenza1.Count; i++)
                {

                    //trigger the stimulus, start the timer and wait for input
                    rtImages[sequenza1[i] - 1].sprite = stimulus;
                    startTime = Time.time;

                    //waits for input
                    while (!(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.R)
                          || Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.O)))

                    {
                        yield return null;
                    }

                    if (Input.GetKeyDown(KeyCode.W))
                    {
                        pressedKey = KeyCode.W;
                    }
                    else if (Input.GetKeyDown(KeyCode.R))
                    {
                        pressedKey = KeyCode.R;
                    }
                    else if (Input.GetKeyDown(KeyCode.U))
                    {
                        pressedKey = KeyCode.U;
                    }
                    else if (Input.GetKeyDown(KeyCode.O))
                    {
                        pressedKey = KeyCode.O;
                    }
                    if (rtImages[sequenza1[i] - 1].CompareTag(pressedKey.ToString()))
                    {
                        stopTime = Time.time - startTime;
                        rtHitOrMiss.Add(1);
                        reactionTime.Add(stopTime);
                        rtImages[sequenza1[i] - 1].sprite = startingImage.sprite;
                    }
               
                    //if it is the wrong, write down the mistake and the reaction time
                    else
                    {
                        stopTime = Time.time - startTime;
                        rtHitOrMiss.Add(0);
                        reactionTime.Add(stopTime);
                        rtImages[sequenza1[i] - 1].sprite = startingImage.sprite;
                    }
                    //yield return null is necessary with coroutines like this
                    //because in this way it WAITS for the KeyCode to
                    //turn back false before performing another check of the loop
                    //(without this, a single keypress would make the code loop for 120 times)
                    yield return null;
                }
            }

            if (myBlockNum < maxBlockNum)
            {
                breakFromReactionTaskPanel.SetActive(true);
            }

            while (breakFromReactionTaskPanel.activeInHierarchy == true)
            {
                yield return null;
            }
        }

        isReactionTimeFinished = true;
        reactionTimePanel.SetActive(false);
        beforeRecognitionTaskPanel.SetActive(true);
    }

    //this coroutine starts the completion task
    IEnumerator RecognitionTask()
    {

        //for 144 trials, divided in 72 trials with a sequence and 72 with the other/a random one,
        //take 6 item sequence, play it, then stop at the 7th item
        //wait for the input and confidence scale after each trial
        //then cycle again
        for (int j = 0; j < recogTrials; j++)
        {

            listExtract(sequenza1);

            for (int i = 0; i < slicedList.Count - 1; i++)
            {

                rtImages[slicedList[i] - 1].sprite = stimulus;
                yield return new WaitForSeconds(0.2f);

                rtImages[slicedList[i] - 1].sprite = startingImage.sprite;
                yield return new WaitForSeconds(0.05f);
            }

            while (!(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.R)
                      || Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.O)))

            {
                yield return null;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                pressedKey = KeyCode.W;
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                pressedKey = KeyCode.R;
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                pressedKey = KeyCode.U;
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                pressedKey = KeyCode.O;
            }
            if (rtImages[slicedList[6] - 1].CompareTag(pressedKey.ToString()))
            {
                recogHitOrMiss.Add(1);
            }

            //if it is the wrong, write down the mistake and the reaction time
            else
            {
                recogHitOrMiss.Add(0);
            }
            yield return new WaitForSeconds(0.25f);
        }

        isRecognitionTaskFinished = true;

    }


    //changes the task number
    public void taskCount(){
        taskNum++;
    }


    //if we reached the last block of SRT task, change to the Recognition task panel
    public void changeScene()
    {
        if (isReactionTimeFinished)
        {
            reactionTimePanel.SetActive(false);
            beforeRecognitionTaskPanel.SetActive(true);
        }

        
    }

    
    //extracts 7-item sequence from the 12-item sequence (needed for the completion/recognition task)
    public void listExtract(List<int> sequence)
    {
        slicedList = new List<int>();
        //we pick a random start in the sequence
        startingIndex = Random.Range(0, sequence.Count);

        //if we pick a start that doesn't let us just take 7 items because the sequence runs out
        if (startingIndex + 7 > sequence.Count)
        {
            //we check how many items are still needed after the end of the sequence
            //and add them from the start
            int rest = (startingIndex + 7) % sequence.Count;
            for (int i = startingIndex; i < sequence.Count; i++)
            {
                slicedList.Add(sequence[i]);
            }
            for (int i = 0; i < rest; i++)
            {
                slicedList.Add(sequence[i]);
            }
        }

        //if we picked a random start that lets us just extract 6 items
        //we just extract them without needing to loop the sequence
        else
        {
            for (int i = startingIndex; i < startingIndex + 7; i++)
            {
                slicedList.Add(sequence[i]);
            }
        }
    }

    // This starts the exposure phase
    public void StartExposure()
    {
        int condition = GetComponent<CambioCamera>().experimentalCondition;
        StartCoroutine(exposurePhase(condition));
    }

    //starts the Reaction time Task corutine
    public void StartReactionTask()
    {
        StartCoroutine(ReactionTimeTask());
    }
    
    //starts the Reaction time Task corutine
    public void StartRecognitionTask()
    {
        StartCoroutine(RecognitionTask());
    }


    void Update()
    {
        
            
            
       
        }
}
