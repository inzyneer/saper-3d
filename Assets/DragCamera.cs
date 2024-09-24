using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCamera : MonoBehaviour
{
    public float dragSpeed = -2;
    private Vector3 dragOrigin;

    public bool cameraDragging = true;

    public float cameraBoundsLeft = -10f;
    public float cameraBoundsRight = 10f;
    public float cameraBoundsUp = 10f;
    public float cameraBoundsDown = -13f;

    void Update()
    {
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        float left = Screen.width * 0.2f;
        float right = Screen.width - (Screen.width * 0.2f);
        float up = Screen.height * 0.2f;
        float down = Screen.height - (Screen.height * 0.2f);

        if (mousePosition.x < left)
        {
            cameraDragging = true;
        }
        else if (mousePosition.x > right)
        {
            cameraDragging = true;
        }
        if (mousePosition.y < up)
        {
            cameraDragging = true;
        }
        else if (mousePosition.y > down)
        {
            cameraDragging = true;
        }

        if (cameraDragging)
        {

            if (Input.GetMouseButtonDown(2))
            {
                dragOrigin = Input.mousePosition;
                return;
            }

            if (!Input.GetMouseButton(2)) return;

            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0);

            if (move.x > 0f)
            {
                if (this.transform.position.x < cameraBoundsRight)
                {
                    transform.Translate(new Vector3(move.x, 0, 0), Space.World);
                }
            }
            else
            {
                if (this.transform.position.x > cameraBoundsLeft)
                {
                    transform.Translate(new Vector3(move.x, 0, 0), Space.World);
                }
            }
            if (move.y > 0f)
            {
                if (this.transform.position.y < cameraBoundsUp)
                {
                    transform.Translate(new Vector3(0, move.y, 0), Space.World);
                }
            }
            else
            {
                if (this.transform.position.y > cameraBoundsDown)
                {
                    transform.Translate(new Vector3(0, move.y, 0), Space.World);
                }
            }
        }
    }

}
