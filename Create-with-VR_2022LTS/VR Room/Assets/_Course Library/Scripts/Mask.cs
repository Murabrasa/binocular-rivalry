using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Mask : MonoBehaviour
{
    public GameObject shape; // UI prefab
    public GameObject maskPanel;   // the panel where masks are created
    public GameObject cfsBackground; // the panel where CFS is projected
    public RawImage cfsBackgroundIm;
    public List<GameObject> Children = new List<GameObject>();
    public float contrast = 0.06f;
    Color[] greyscale = { new Color(0, 0, 0),
        new Color(0.3f,0.3f,0.3f),
        new Color(0.5f,0.5f,0.5f),
        new Color(0.7f,0.7f,0.7f),
        new Color(1,1,1) }; // list of different possible greys
    public RenderTexture renTex;
    public List<Texture2D> masks = new List<Texture2D>();
    public Texture2D cachedMask;
    Renderer rend;
    public Camera RenderTexCam;

    Color[] test = { new Color(0, 0, 0),
    new Color(1,1,1)};
    public RenderTexture rt;
    public List<int> randomIndexes = new List<int>();








    // Start is called before the first frame update
    private void Awake()
    {
        //cachedMask = new Texture2D(renTex.width, renTex.height, renTex.graphicsFormat, renTex.mipmapCount, UnityEngine.Experimental.Rendering.TextureCreationFlags.MipChain);
        //cachedMask = new Texture2D(renTex.width, renTex.height);

        rt = new RenderTexture(800, 400, 24, RenderTextureFormat.ARGB32);
        cfsBackgroundIm = cfsBackground.GetComponent<RawImage>();


        //rend = CFSPanel.GetComponent<Renderer>();
        //RenderTexCam.targetTexture = rt;
        //RenderTexCam.Render();
    }

    void Start()
    {
        // apply to each possible color the decided contrast
        for (int i = 0; i < greyscale.Length; i++)
        {
            greyscale[i] = ApplyContrast(greyscale[i], 0.06f);
        }

        //create 75 masks, thus repeat 75 times...
        for (int i = 0; i < 75; i++)
        {
            cachedMask = new Texture2D(800, 400, TextureFormat.ARGB32, false);
            //...after having a clean slate every time to draw shapes...
            /*if (Children.Count > 0)
            {
                for (int j = 0; j < Children.Count; j++)
                {
                    Destroy(Children[j]);

                }
            }*/
            //...the creation of shapes as decided in makeshapes
            if (Children.Count < 1)
            {
                MakeShapes();
            }
            else
            {
                MoveShapes(Children);
            }
            //then, every iteration, copy the slate into a texture2d
            //Graphics.CopyTexture(renTex,0,0, cachedMask,0,0);
            RenderTexCam.targetTexture = rt;
            RenderTexCam.Render();
            Graphics.CopyTexture(rt, cachedMask);

            //cachedMask = FromRenTexToTex2D(renTex);

            masks.Add(cachedMask);
            randomIndexes.Add(i);

            
            
        }
        StartCoroutine(Flashing());
        
    }

    
    public Color ApplyContrast(Color color, float contrast)
    {
        //this is just the formula verbatim, no need to think critically
        float red = color.r * contrast * 255 + ((255 - contrast * 255) / 2);
        float blue = color.b * contrast * 255 + ((255 - contrast * 255) / 2);
        float green = color.g * contrast * 255 + ((255 - contrast * 255) / 2);

        //and then each element of the color is converted back to a 0-1 format
        color[0] = red / 255;
        color[1] = blue / 255;
        color[2] = green / 255;
        return color;
    }

    //Randomizes the index list that we will use to access the masks list, i.e.: it randomizes the masks list
    public void Shuffle(List<int> indexList)
    {
        for (int i = 0; i < indexList.Count; i++)
        {
            int tmp = indexList[i];
            int randomNum = Random.Range(i, indexList.Count);
            indexList[i] = indexList[randomNum];
            indexList[randomNum] = tmp;
        }
    }

    public void setMask()
    {
        Shuffle(randomIndexes);
        for (int i = 0; i < randomIndexes.Count; i++)
        {
            cfsBackgroundIm.texture = masks[randomIndexes[i]];
        }
    }
    public void MakeShapes()
    {
        for (int i = 0; i < 400; i++)
        {
            //each iteration creates a single shape in a random position on a 2d canvas 800x400. It could be
            //arranged for a flexible size of canvas
            GameObject single_shape = Instantiate(shape, new Vector3(Random.Range(-400, 400), Random.Range(-400, 400), 0), Quaternion.identity);

            //then we make sure that the position is relative to the right canvas by setting it as the parent and
            //turning off global position (and on local position)
            single_shape.GetComponent<RectTransform>().SetParent(maskPanel.transform, false);


            //then, each shape will be set to a random size between 20% and 100% of the prefab shape
            single_shape.transform.localScale = Vector3.one * Random.Range(0.2f, 1f);





            //then, set the color to a random one on an already provided list
            single_shape.GetComponent<UnityEngine.UI.Image>().color = greyscale[Random.Range(0, greyscale.Length)];

            Children.Add(single_shape);
        }
    }

    public void MoveShapes(List<GameObject> children)
    {
        for (int i = 0; i < children.Count; i++)
        {
            children[i].transform.localPosition = new Vector3(Random.Range(-400, 400), Random.Range(-400, 400), 0);
        }

    }
    IEnumerator Flashing()
    {
        while (true)
        {
            Shuffle(randomIndexes);
            cfsBackgroundIm.texture = masks[randomIndexes[0]];
            yield return new WaitForSeconds(0.05f);
        }
        
    }

    private void FixedUpdate()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        


    }
}

