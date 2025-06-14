using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAnimated : MonoBehaviour
{
    [SerializeField] float speedX;

    [SerializeField] float speedY;

     float currX;

     float currY;



    [SerializeField] GameObject mainGameObject;



    Renderer renderer = null;

    // Start is called before the first frame update

    void Start()

    {

        renderer = mainGameObject.GetComponent<Renderer>();



        currX = renderer.material.mainTextureOffset.x;

        currY = renderer.material.mainTextureOffset.y;

    }



    // Update is called once per frame

    void Update()

    {

        currX += Time.deltaTime * speedX;

        currY += Time.deltaTime * speedY;

        renderer.material.SetTextureOffset("_MainTex", new Vector2(currX, currY));

    }
}
