using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInScript : MonoBehaviour
{

    Vector3 originalCamPos;
    float originalSize;
    Camera camRef;

    // Start is called before the first frame update
    void Start()
    {
        camRef = FindObjectOfType<Camera>();
        originalCamPos = camRef.transform.position;
        originalSize = camRef.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnMouseDown()
    {
        if(originalSize == FindObjectOfType<Camera>().orthographicSize)
        {
            Vector3 pos = camRef.transform.position;
            pos.x = this.transform.position.x;
            pos.y = this.transform.position.y;
            camRef.transform.position = pos;
            camRef.orthographicSize = this.transform.parent.localScale.x/1.8f;
        }
        else
        {
            camRef.transform.position = originalCamPos;
            camRef.orthographicSize = originalSize;

        }
    }
}
