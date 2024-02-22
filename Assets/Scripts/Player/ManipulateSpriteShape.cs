//CODE TAKEN & MODIFIED FROM https://gamedev.stackexchange.com/questions/176439/manipulating-sprite-shape-rendered
//ORIGINAL CODE BY USER: https://gamedev.stackexchange.com/users/133238/jorayen

//Alterations:
//- instead of adding a new point, grabs an existing point on the shape
//- the object's offset to the camera is taken into account
//- THE SCRIPT BREAKS WHEN THE SHAPE IS ROTATED IN THE Z-AXIS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ManipulateSpriteShape : MonoBehaviour
{
    const int INVALID_POINT_INDEX = -1;

    private SpriteShapeController spriteShapeController;
    private Spline spline;
    private int inseretedPointIndex = INVALID_POINT_INDEX;
    private float snappingDistance = 1.5f; //minimum distance the cursor click needs to occur near a corner for it to snap to the cursor's location, set to int.MaxValue to make any closest point snap when clicking anywhere in the shape;
    private Vector3[] DefaultPointPositions;
    

    Vector3 cameraPosition;


    void ResetShape()
    {
        for (var i = 0; i < spline.GetPointCount(); i++)
        {
            spline.SetPosition(i, DefaultPointPositions[i] - spriteShapeController.transform.position);
        }
    }

    void Start()
    {
        spriteShapeController = GetComponent<SpriteShapeController>();
        spline = spriteShapeController.spline;
        cameraPosition = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.0f);

        DefaultPointPositions = new Vector3[spline.GetPointCount()];

        for (var i = 0; i < spline.GetPointCount(); i++)
        {
            DefaultPointPositions[i] = spline.GetPosition(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // changes the grabbed point's position while mouse is held down
        if (inseretedPointIndex != INVALID_POINT_INDEX)
        {
            spline.SetPosition(inseretedPointIndex, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z)) - (spriteShapeController.transform.position));
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetShape();
            }
        }
    }

    void OnMouseDown()
    {
        Vector3 mouseDownPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z)) - cameraPosition;
        int pointCount = spline.GetPointCount();
        int closestPointIndex = INVALID_POINT_INDEX;
        float minDistance = snappingDistance;

        //Debug.Log("Mouse Down Position:" + Input.mousePosition);
        //Debug.Log("World Position: " + mouseDownPos);

        //finds which of the point is closest to the position clicked on
        
        //loop finds the closest point to the mouse position, preparing it to be snapped to the mouse position.
        for (var i = 0; i < pointCount; i++)
        {
            Vector3 currentPointPos = spline.GetPosition(i) + spriteShapeController.transform.position ;//- (cameraPosition - spriteShapeController.transform.position);
            float distance = Vector3.Distance(currentPointPos, mouseDownPos);
            //Debug.Log("\nPoint " + i + " pos: " + currentPointPos);

            if (distance < minDistance)
            {
                //Debug.Log("Point closer to mouse than previous point!");
                minDistance = distance;
                closestPointIndex = i;
            }
        }

        if (closestPointIndex != INVALID_POINT_INDEX)//if a closest point has been found (if the user clicked close enough to a point)
        {
            spline.SetTangentMode(closestPointIndex, ShapeTangentMode.Linear);
            inseretedPointIndex = closestPointIndex;
            //Debug.Log("Selected point index: " + inseretedPointIndex);
        }

    }

    void OnMouseUp()
    {
        //Debug.Log("Mouse Up");
        inseretedPointIndex = INVALID_POINT_INDEX;
    }
}
