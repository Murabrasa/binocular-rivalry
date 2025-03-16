using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskAfterSequence : MonoBehaviour
{
    public int clicked;
    public GameObject afterFirstTaskCanvas;
    public GameObject taskCanvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnMouseDown(){
        int.TryParse(gameObject.tag, out int num);
        clicked = num;
        taskCanvas.SetActive(false);
        afterFirstTaskCanvas.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
