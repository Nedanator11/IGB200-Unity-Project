using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option : MonoBehaviour
{
    [TextArea(4, 8)]
    public string Description;

    //Mouse Hover Scaling
    private GameObject SpriteObject;
    private GameObject HitBox;
    private Vector3 ScaleInitial;
    private Vector3 ScaleHovered;
    private float ScaleHoverFactor = 1.1f;
    private float ScaleDuration = 0.5f;

    private void Start()
    {
        //Retrieve sprite & hitbox objects
        SpriteObject = transform.Find("Sprite").gameObject;
        HitBox = SpriteObject.transform.Find("HitBox").gameObject;

        //Get the initial scale of this object and calculate the hovered scale
        ScaleInitial = SpriteObject.transform.localScale;
        ScaleHovered = ScaleInitial;
        ScaleHovered.x *= ScaleHoverFactor;
        ScaleHovered.y *= ScaleHoverFactor;
    }

    private void Update()
    {
        //Don't process if paused
        if (GameManager.instance.Paused)
            return;

        //Don't process outside of gameplay
        if (((LSGameManager)GameManager.instance).GameState != LSGameManager.GameStates.Gameplay)
            return;

        ScaleSprite();
    }

    private bool CheckMouseHover()
    {
        //Raycast from camera to mouse position to detect hover over object
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //Get the hit GameObject
            GameObject hitObject = hit.collider.gameObject;

            //If hitObject is this option's hitbox, return true
            if (hitObject == HitBox)
                return true;
        }

        //Otherwise, return false
        return false;
    }

    private void ScaleSprite()
    {
        //Calculate the scale step
        float scaleStepX = ScaleHovered.x - ScaleInitial.x * ScaleDuration * Time.deltaTime;
        float scaleStepY = ScaleHovered.y - ScaleInitial.y * ScaleDuration * Time.deltaTime;

        if (CheckMouseHover())
        {
            //Create new scale vector
            Vector3 newScale = SpriteObject.transform.localScale;
            newScale.x += scaleStepX;
            newScale.y += scaleStepY;
            if (newScale.x > ScaleHovered.x) newScale.x = ScaleHovered.x;
            if (newScale.y > ScaleHovered.y) newScale.y = ScaleHovered.y;

            //Set new scale vector
            SpriteObject.transform.localScale = newScale;
        }
        else
        {
            //Create new scale vector
            Vector3 newScale = SpriteObject.transform.localScale;
            newScale.x -= scaleStepX;
            newScale.y -= scaleStepY;
            if (newScale.x < ScaleInitial.x) newScale.x = ScaleInitial.x;
            if (newScale.y < ScaleInitial.y) newScale.y = ScaleInitial.y;

            //Set new scale vector
            SpriteObject.transform.localScale = newScale;
        }

        
    }
}
