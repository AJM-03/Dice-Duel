using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float smoothing;
    public Vector2 maxPosition;
    public Vector2 minPosition;
    public Vector3 targetPosition;
    public float xMouseClamp;
    public float yMouseClamp;
    public bool mouseMovement;


    private void LateUpdate()
    {
        if (target != null)
        {
            if (mouseMovement == true)
            {
                Vector2 mousePos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                Vector3 playerPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

                Vector3 p = new Vector3(mousePos.x + playerPosition.x, mousePos.y + playerPosition.y, playerPosition.z);
                float x = p.x / 2;
                float y = p.y / 2;
                x = Mathf.Clamp(x, playerPosition.x - xMouseClamp, playerPosition.x + xMouseClamp);
                y = Mathf.Clamp(y, playerPosition.y - yMouseClamp, playerPosition.y + yMouseClamp);

                targetPosition = new Vector3(x, y, -1);
            }
            else
            {
                targetPosition = new Vector3(target.position.x, target.position.y, -1);
            }

            if (transform.position != targetPosition)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
            }
        }
    }
}
