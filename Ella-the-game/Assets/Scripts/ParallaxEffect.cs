using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    #region Try #1
    /*[SerializeField] Vector2 parallaxEffectAmount;
     Transform cameraTrans;
     Vector3 lastCameraPos;
     float textureUnitSize_X;
     void Start()
     {
         cameraTrans = Camera.main.transform;
         lastCameraPos = cameraTrans.position;
         Sprite sprite = GetComponent<SpriteRenderer>().sprite;
         Texture2D texture = sprite.texture;
         textureUnitSize_X = texture.width / sprite.pixelsPerUnit;
     }

     // Update is called once per frame
     void Update()
     {
         Vector3 deltaMovement = cameraTrans.position - lastCameraPos;
         transform.position += new Vector3(deltaMovement.x * parallaxEffectAmount.x, deltaMovement.y * parallaxEffectAmount.y);
         lastCameraPos = cameraTrans.position;

         if(Mathf.Abs(cameraTrans.position.x - transform.position.x) >= textureUnitSize_X)
         {
             float offsetPosX = (cameraTrans.position.x - transform.position.x) % textureUnitSize_X;
             transform.position = new Vector3(cameraTrans.position.x + offsetPosX, transform.position.y, 0);
         }
     }*/
    #endregion
    float length, startPos;
    Transform camTrans;
    public float parallaxEffect;
    public float offset;

    private void Start()
    {
        camTrans = Camera.main.transform;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    private void Update()
    {
        float tmp = camTrans.position.x * (1 - parallaxEffect);
        float dist = camTrans.position.x * parallaxEffect;

        Vector3 pos = transform.position;
        pos.x = startPos + dist;
        transform.position = pos;

        if (tmp > startPos + (length-offset)) startPos += length;
        else if (tmp < startPos - (length-offset)) startPos -= length;

    }
}
