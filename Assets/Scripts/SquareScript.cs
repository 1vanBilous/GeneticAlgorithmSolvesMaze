using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SquareScript : MonoBehaviour
{
    public Sprite grass_sprite;
    public Sprite selected_grass_sprite;
    public Sprite wall_sprite;
    public Sprite selected_wall_sprite;
    public Sprite start_sprite;
    public Sprite selected_start_sprite;
    public Sprite finish_sprite;
    public Sprite selected_finish_sprite;

    public GridManager grid_manager;

    void Start()
    {
        grid_manager = GameObject.Find("GridManager").GetComponent<GridManager>();
    }

    private void OnMouseEnter()
    {
        if (!grid_manager.grid_is_blocked)
        {
            switch (gameObject.tag)
            {
                case "Grass":
                    gameObject.GetComponent<SpriteRenderer>().sprite = selected_grass_sprite;
                    break;
                case "Wall":
                    gameObject.GetComponent<SpriteRenderer>().sprite = selected_wall_sprite;
                    break;
                case "Start":
                    gameObject.GetComponent<SpriteRenderer>().sprite = selected_start_sprite;
                    break;
                case "Finish":
                    gameObject.GetComponent<SpriteRenderer>().sprite = selected_finish_sprite;
                    break;
            }
        }
    }

    private void OnMouseExit()
    {
        switch (gameObject.tag)
        {
            case "Grass":
                gameObject.GetComponent<SpriteRenderer>().sprite = grass_sprite;
                break;
            case "Wall":
                gameObject.GetComponent<SpriteRenderer>().sprite = wall_sprite;
                break;
            case "Start":
                gameObject.GetComponent<SpriteRenderer>().sprite = start_sprite;
                break;
            case "Finish":
                gameObject.GetComponent<SpriteRenderer>().sprite = finish_sprite;
                break;
        }
    }
}
