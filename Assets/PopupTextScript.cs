using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum IconType { heart,hunger,food};
public class PopupTextScript : MonoBehaviour
{
    public bool isRef = false;


    float lifeTime = 0;

    public SpriteRenderer iconRef;

    public Sprite heartSprite;
    public Sprite hungerSprite;
    public Sprite foodSprite;

    public Transform popupTextRef;

    public void Setup(string Text, IconType icon)
    {
        this.gameObject.GetComponent<TextMeshPro>().text = Text;
        switch (icon)
        {
            case IconType.heart:
                iconRef.sprite = heartSprite;
                break;
            case IconType.hunger:
                iconRef.sprite = hungerSprite;
                break;
            case IconType.food:
                iconRef.sprite = foodSprite;
                break;
        }
    }


    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;

        this.transform.Translate(0,Time.deltaTime,0);

        if(lifeTime > 1)
        {
            if (!isRef)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
