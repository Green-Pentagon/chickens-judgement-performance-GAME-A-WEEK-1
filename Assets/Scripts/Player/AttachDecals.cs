using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AttachDecals : MonoBehaviour
{
    private const int POINTER_UNASSIGNED = -1;
    public Transform LeftWing, RightWing, LeftLeg, RightLeg, Headpiece;

    private SpriteShapeController spriteShapeController;
    private Spline spline;
    private Vector3 DepthOffset, WingPosOffset, HeadPosOffset;
    private Vector2 LegPosOffset;
    private int LWindex, RWindex, LLindex, RLindex, HEindex = POINTER_UNASSIGNED;//stores the corner's index from the Spline

    

    

    // Start is called before the first frame update
    void Start()
    {
        spriteShapeController = GetComponent<SpriteShapeController>();
        spline = spriteShapeController.spline;
        DepthOffset     = new Vector3(0.0f, 0.0f, -1.0f);
        WingPosOffset   = new Vector3(0.25f, 0.0f, 0.0f);
        LegPosOffset    = new Vector2(0.5f, -0.5f);
        HeadPosOffset   = new Vector3(0.0f, 0.25f, 0.0f);


        if (spline.GetPointCount() != 5)
        {
            Debug.LogError("AttachDecals.cs = Sprite Shape's points are not equal to 5!");
        }
        else
        {
            for (int i = 0; i < spline.GetPointCount(); i++)
            {
                //THIS SOLUTION IS HARD-CODED TO WORK WITH A PENTAGON WITH FOLLOWING POINTER COORDINATES:
                //Head:      y > 0
                //LeftWing:  y = 0, x < 0
                //RightWing: y = 0, x > 0
                //LeftLeg:   y < 0, x < 0
                //RightLeg:  y < 0, x > 0
                //Debug.Log("Pointer index:" + i + " | Position: " + spline.GetPosition(i));
                if (spline.GetPosition(i).y > 0.00f)
                {
                    HEindex = i;
                }else if (spline.GetPosition(i).y == 0.00f)
                {
                    if(spline.GetPosition(i).x < 0.00f)
                    {
                        LWindex = i;
                    }
                    else
                    {
                        RWindex = i;
                    }
                }
                else
                {
                    if (spline.GetPosition(i).x < 0.00f)
                    {
                        LLindex = i;
                    }
                    else
                    {
                        RLindex = i;
                    }
                }
            }

            if (LWindex == POINTER_UNASSIGNED || RWindex == POINTER_UNASSIGNED || LLindex == POINTER_UNASSIGNED || RLindex == POINTER_UNASSIGNED || HEindex == POINTER_UNASSIGNED)
            {
                Debug.LogError("AttachDecals.cs = Pointer(s) missing assigment in Start()!");
            }
        }
        


    }

    // Update is called once per frame
    void Update()
    {
        LeftWing.position   = spriteShapeController.transform.position + spline.GetPosition(LWindex) + DepthOffset - WingPosOffset;
        RightWing.position  = spriteShapeController.transform.position + spline.GetPosition(RWindex) + DepthOffset + WingPosOffset;
        LeftLeg.position    = spriteShapeController.transform.position + spline.GetPosition(LLindex) + DepthOffset - new Vector3(LegPosOffset.x,-LegPosOffset.y,0.0f);
        RightLeg.position   = spriteShapeController.transform.position + spline.GetPosition(RLindex) + DepthOffset + new Vector3(LegPosOffset.x, LegPosOffset.y, 0.0f);
        Headpiece.position  = spriteShapeController.transform.position + spline.GetPosition(HEindex) + DepthOffset + HeadPosOffset;
    }
}
