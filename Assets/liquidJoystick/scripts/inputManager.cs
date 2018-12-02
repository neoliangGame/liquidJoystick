using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inputManager : MonoBehaviour {

    public liquidJoystick joystick = null;

    Vector2 startPosition = Vector2.zero;

    //scale
    float hideScale = 0f;
    float showScale = 1f;
    bool isToShow = false;
    float scaleSpeed = 3.2f;

    //shape change rate
    float changeDistanceReference = Screen.width * 0.2f;
    float changedRate = 0f;
    bool shouldReturnChange = false;
    float changeReturnSpeed = 40f;

    float angle = 0;

    //test cube
    public GameObject cube;
    Vector2 cubeLeftBottom = new Vector2(-50f,0f);
    Vector2 cubeRightTop = new Vector2(50f, 40f);
    float cubeMoveSpeed = 40f;

    void Update () {
        WhenTouched();
        ChangeScale();
        if (shouldReturnChange)
        {
            ShapeChangeReturn();
        }
    }

    void WhenTouched()
    {
        //Touch touch = Input.GetTouch(0);
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            isToShow = true;
            shouldReturnChange = false;
            joystick.SetLiquidJoystick(startPosition, 0f, 0f);
        }else if (Input.GetMouseButtonUp(0))
        {
            shouldReturnChange = true;
            isToShow = false;
        }else if (Input.GetMouseButton(0))
        {
            Vector2 diff = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - startPosition;
            float distance = Mathf.Pow(diff.x * diff.x + diff.y * diff.y, 0.5f);
            angle = 0;
            changedRate = 0;
            float minDistance = 1f;
            if (distance > minDistance)
            {
                changedRate = distance / changeDistanceReference;
                angle = 180f - Vector2.SignedAngle(diff, Vector2.right);
                MoveCube(diff / distance);
            }
            joystick.SetLiquidJoystick(startPosition, angle, changedRate);
            
        }
    }

    void ChangeScale()
    {
        float scaleStep = scaleSpeed * Time.deltaTime;
        float nextScale;
        RectTransform rectTran = GetComponent<RectTransform>();
        if (isToShow)
        {
            nextScale = rectTran.localScale.x + scaleStep;
            if(nextScale > showScale)
            {
                nextScale = showScale;
            }
            rectTran.localScale = new Vector3(nextScale, nextScale, 1f);
        }
        else
        {
            nextScale = rectTran.localScale.x - scaleStep;
            if (nextScale < hideScale)
            {
                nextScale = hideScale;
            }
            rectTran.localScale = new Vector3(nextScale, nextScale, 1f);
        }
    }

    void ShapeChangeReturn()
    {
        float nextRate = changedRate - Time.deltaTime * changeReturnSpeed;
        if(nextRate < 0f)
        {
            nextRate = 0f;
        }
        joystick.SetLiquidJoystick(startPosition, angle, nextRate);
    }

    void MoveCube(Vector2 direction)
    {
        Vector3 position = cube.transform.position;
        float x = position.x + direction.x * cubeMoveSpeed * Time.deltaTime;
        x = (x < cubeLeftBottom.x) ? cubeLeftBottom.x : x;
        x = (x > cubeRightTop.x) ? cubeRightTop.x : x;

        float y = position.y + direction.y * cubeMoveSpeed * Time.deltaTime;
        y = (y < cubeLeftBottom.y) ? cubeLeftBottom.y : y;
        y = (y > cubeRightTop.y) ? cubeRightTop.y : y;

        cube.transform.position = new Vector3(x, y, position.z);
    }
}
