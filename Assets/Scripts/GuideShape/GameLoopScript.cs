using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class GameLoopScript : MonoBehaviour
{
    public Transform playerTransform;
    public SpriteShapeController playerSpriteShapeController;
    public TextMeshProUGUI scoreReadout;

    private Transform selfTransform;
    private SpriteShapeController selfSpriteShapeController;
    private SpriteShapeRenderer selfSpriteShapeRenderer;
    private Spline selfSpline;
    private Spline playerSpline;

    private bool roundInProgress = false;
    private float leeway = 1.5f;        //how close the player has to match the corners to gain a point, adjust to a lower value to increase precision needed
    private float roundTime = 10.0f;    //time for the player to do their actions for chance of gaining points
    private float nextRoundDelay = 5.0f;//time between rounds
    private int score = 0;
    private float colourShiftTimer;



    private Vector2[] positionRange = {new Vector2(-7.4f,-9.5f), new Vector2(18.0f, -1.0f) };//the minimum and maximum co-ordinate that the shape can be formed in

    IEnumerator RoundStart()
    {
        selfSpriteShapeRenderer.enabled = true;
        roundInProgress = true;
        
        
        yield return new WaitForSeconds(roundTime); //Round Time

        ClosenessCheck();
        scoreReadout.text = "Score: " + score;
        selfSpriteShapeRenderer.enabled = false;


        yield return new WaitForSeconds(nextRoundDelay); //Time between Rounds
        roundInProgress = false;
    }

    void ClosenessCheck()
    {
        bool[] selfPointTriggered = new bool[selfSpline.GetPointCount()]; //prevents player from gaining additional points if they put mulitple corners of their object next to a single corner of the shape object.

        for (int i = 0; i < selfSpline.GetPointCount(); i++)
        {
            for (int j = 0; j < playerSpline.GetPointCount(); j++)
            {
                float pointToPointDistance = (2 * Vector3.Distance((selfSpriteShapeController.transform.position + selfSpline.GetPosition(i)), (playerTransform.position + playerSpline.GetPosition(j)))) / 2;
                //multiplied and divided by two to quickly get the absolute value

                if (pointToPointDistance <= leeway && !selfPointTriggered[i])
                {
                    Debug.Log("distance of shape point " + i + " and player point " + j + " = " + pointToPointDistance);
                    Debug.Log("Shape point " + i + " matched with player point " + j);
                    score += 1;
                    selfPointTriggered[i] = true;
                }
            }
        }
    }

    void ShiftColour(float timestamp)
    {
        if (timestamp <= 0.0f)
        {
            selfSpriteShapeRenderer.color = new Color(0.0f, 1.0f, 0.0f, selfSpriteShapeRenderer.color.a); //set to green
        }
        else
        {
            
            selfSpriteShapeRenderer.color = new Color((1.0f * ((roundTime - timestamp) / roundTime)), (1.0f * (timestamp / roundTime)), 0.0f, selfSpriteShapeRenderer.color.a);
            //the percent of current time left compared to the round time total
            //red increasing, green decreasing by a ratio
            //red decreases by timestamp over roundTime, green increases by that much.
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        colourShiftTimer = roundTime;
        selfSpriteShapeController = GetComponent<SpriteShapeController>();
        selfSpriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
        selfTransform = GetComponent<Transform>();
        selfSpline = selfSpriteShapeController.spline;


        playerSpline = playerSpriteShapeController.spline;


        //DEBUG
        //int playerPointCount = playerSpline.GetPointCount();
        //int selfPointCount = selfSpline.GetPointCount();

        //for (var i = 0; i < playerPointCount; i++)
        //{
        //    Vector3 currentPointPos = playerSpline.GetPosition(i);
        //    Debug.Log("Player Point " + i + " position: " + currentPointPos);
        //}

        //for (var i = 0; i < selfPointCount; i++)
        //{
        //    Vector3 currentPointPos = selfSpline.GetPosition(i);
        //    Debug.Log("Self Point " + i + " position: " + currentPointPos);
        //}
    }




    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {

        if (!roundInProgress)
        {
            //randomise the location of the shape just before the round begins
            selfTransform.position = new Vector3(Random.Range(positionRange[0].x , positionRange[1].x),Random.Range(positionRange[0].y, positionRange[1].y), selfTransform.position.z);
            colourShiftTimer = 0.0f; //due to delay, the timer has to be set to 0.0f to ensure that it starts fresh in the next round
            StartCoroutine(RoundStart());
            
        }
        else if (roundInProgress)
        {
            //shifts the colour of the sprite shape over time
            ShiftColour(colourShiftTimer);
            if (colourShiftTimer <= 0.0f)
            {
                colourShiftTimer = roundTime;
            }
            else
            {
                colourShiftTimer -= Time.fixedDeltaTime;
            }
        }
    }
}
