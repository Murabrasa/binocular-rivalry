using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CambioCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject cameraLeft;
    public GameObject cfsScreen;
    public GameObject cameraRight;
    public GameObject sequenceScreen;
    public GameObject cameraCentral;
    public int experimentalCondition = 0; //experimentalCondition 1 = no CFS, 2 = CFS, 3 = Random sequence, no CFS
    void Start()
    {
        
    }
    //3 methods for the onClick buttons
    public void condition1(){
        experimentalCondition = 1;
    }
    public void condition2(){
        experimentalCondition = 2;
    }
    public void condition3(){
        experimentalCondition = 3;
    }



    public void CheckCondition(){

        //if no CFS, or if random sequence no CFS (basically this deals with the no CFS conditions)
        if (experimentalCondition == 1 || experimentalCondition == 3)
        {
            //turn off cameraLeft and cameraCentral
            cameraLeft.SetActive(false);
            cameraCentral.SetActive(false);

            //turn on cameraRight and make it so that it targets both eyes
            cameraRight.GetComponent<Camera>().enabled = true;
            cameraRight.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Both;

            //turn on the screen that cameraRight points at
            sequenceScreen.SetActive(true);
        }
        //if CFS
        if (experimentalCondition == 2)
        {
            //turn off cameraCentral
            cameraCentral.SetActive(false);

            //turn on both cameraEyes
            cameraLeft.GetComponent<Camera>().enabled = true;
            cameraRight.GetComponent<Camera>().enabled = true;

            //turn on both screens the cameras are pointing at
            sequenceScreen.SetActive(true);
            cfsScreen.SetActive(true);
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
