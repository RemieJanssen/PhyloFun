using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseHandling : MonoBehaviour
{
    // the time a potential tap has started (i.e., the time of the last mousedown event.)
    float tapTime;
    // a boolean that tells the program whether we are currently checking if a mouse-down event is a tap.
    bool tapListen;
    // the node that is currently being dragged.
    GraphNode draggedNode;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
        tapTime = 0;
        draggedNode = null;
    }

    // Sets the bool value for taplisten: used for whether we are checking if a mouse event is a tap.
    public bool TapListen
    {
        set { tapListen = value;}
    }

    // Returns the node that is currently being dragged.
    public GraphNode DraggedNode
    {
        get { return draggedNode; }
    }

    // Update is called once per frame
    void Update()
    {
        //mouse down handling
        if (Input.GetMouseButtonDown(0))
        {
            tapListen = false;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.collider!= null)
            {
                //maybe I should restrict hitting to only graph nodes, so I cant accidentally hit edges.
                GraphNode hitNode = hit.collider.gameObject.GetComponent<GraphNode>();
                if(!GameState.InRearrangementMode || hitNode.RearrangementEndpoint)
                {
                    hitNode.OnMouseDownHandling();
                    draggedNode = hitNode;
                }
                else
                {
                    tapTime = Time.time;
                    tapListen = true;
                }
            }
            else
            {
                tapTime = Time.time;
                tapListen = true;
            }

        }
        //mouse up handling
        if (Input.GetMouseButtonUp(0))
        {
            /*
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                GraphNode hitNode = hit.collider.gameObject.GetComponent<GraphNode>();
                hitNode.OnMouseUpHandling();
            }
            */
            if(draggedNode!=null)
            {
                draggedNode.OnMouseUpHandling();
                draggedNode = null;
            }

            if (Time.time - tapTime < GameConstants.TapDelay && GameState.InRearrangementMode)
            {
                GameState.EndRearrangement();
            }
        }
    }

}
