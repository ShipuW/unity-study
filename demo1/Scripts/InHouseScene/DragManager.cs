
using UnityEngine;
using System.Collections;

public class DragManager : MonoBehaviour
{


    #region public static 
    private static DragManager s_Instace;

    public static DragManager Instace { get { return s_Instace; } }
    #endregion

    #region public member for unity
    //public GameObject[] draggableObjects;

    #endregion

    #region private member
    private bool isLeftHandDrag;
    private GameObject draggedObject;
    private float draggedObjectDepth;
    private Vector3 draggedObjectOffset;
    private Material draggedObjectMaterial;
    private bool bInited = false;
    #endregion

    #region drag call back Delegate
    public delegate void VectorDelegate(GameObject go,Vector3 screenPixelPos);

    public delegate void DragDelegate(GameObject go,Vector3 screenPos);

    public delegate void VoidDelegate(GameObject go);

    public VoidDelegate onReleaseGameobject;
    public DragDelegate onDragGameObject;
    public VectorDelegate OnDropGameObject;
    #endregion

    public bool IsInit()
    {
        return bInited;
    }

    void Awake()
    {

        s_Instace = this;
    }

    public void Start()
    {
        bInited = true;
    }

    void Update()
    {
        //InteractionManager manager = InteractionManager.Instance;

        CKinect.CursorController controller = CKinect.CursorController.Instance;

        if (controller != null)
        {
            Vector3 screenNormalPos = Vector3.zero;
            Vector3 screenPixelPos = Vector3.zero;

            if (draggedObject == null)
            {
                // no object is currently selected or dragged.
                // if there is a hand grip, try to select the underlying object and start dragging it.

                if (controller.HasHandClosed)
                {
                    if (controller.GetHandLeftVisible())
                    {
                        isLeftHandDrag = true;
                        screenNormalPos = controller.GetHandLeftRawPos();

                    }
					else if (controller.GetHandRightVisible())
                    {
                        isLeftHandDrag = false;
                        screenNormalPos = controller.GetHandLeftRawPos();
                    }

                    Vector3 cursorPos = controller.GetCurrentCursorPos();
                    screenPixelPos.x = cursorPos.x;
                    screenPixelPos.y = cursorPos.y;
                }

                OnKeyDownOrHandClose(screenPixelPos);

            } else
            {
                Vector3 cursorPos = controller.GetCurrentCursorPos();
                OnKeyMoveOrHandMove(cursorPos);

                if (!controller.HasHandClosed)
                {
                    OnKeyUpOrHandOpen();
                }
            }
        }
    
    
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenNormalPos = Vector3.zero;
            Vector3 screenPixelPos = Vector3.zero;
            
            if (draggedObject == null)
            {
                // no object is currently selected or dragged.
                // if there is a hand grip, try to select the underlying object and start dragging it.

                Vector3 cursorPos = Input.mousePosition;
                screenPixelPos.x = cursorPos.x;
                screenPixelPos.y = cursorPos.y;

                OnKeyDownOrHandClose(screenPixelPos);
            } else
            {
                Vector3 cursorPos = Input.mousePosition;
                OnKeyMoveOrHandMove(cursorPos);
            }
        }

        // check if the object (hand grip) was released
        if (Input.GetMouseButtonUp(0) && draggedObject != null)
        {
            OnKeyUpOrHandOpen();
        }

    }

    private void OnKeyDownOrHandClose(Vector3 screenPixelPos)
    {
        // check if there is an underlying object to be selected
        if (!Utils.IsInvaildVec3(screenPixelPos) && screenPixelPos != Vector3.zero)
        {
            // convert the normalized screen pos to pixel pos
            //screenPixelPos.x = (int)(screenNormalPos.x * Camera.main.pixelWidth);
            //screenPixelPos.y = (int)(screenNormalPos.y * Camera.main.pixelHeight);
            //Ray ray = Camera.main.ScreenPointToRay(screenPixelPos);


            if( UICamera.lastHit.collider != null )
            {
                int layer = UICamera.lastHit.collider.gameObject.layer;
                if ( LayerMask.LayerToName(layer).Equals("2D GUI") || LayerMask.LayerToName(layer).Equals("3D GUI") )
                    return;
            }

			RaycastHit hit;
            // check for underlying objects
            // RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(screenPixelPos);
            //Debug.Log(screenNormalPos + "   " + screenPixelPos + "   " + Camera.main.pixelWidth + "    " + Camera.main.pixelHeight);
            
            if (Physics.Raycast(ray, out hit))
            {
                draggedObject = hit.collider.gameObject;
                draggedObjectDepth = draggedObject.transform.position.z - Camera.main.transform.position.z;
                draggedObjectOffset = hit.point - draggedObject.transform.position;
                if (onDragGameObject != null )
                {
                    
                    //Vector3 pos = isLeftHandDrag ? controller.GetHandLeftRawPos() : controller.GetHandRightRawPos();
                    Vector3 pos = Vector3.zero;
                    pos.x = screenPixelPos.x;
                    pos.y = screenPixelPos.y;
                    pos.z = draggedObjectDepth;
                    //pos.z = pos.z + draggedObjectDepth;

                    onDragGameObject(draggedObject,  pos);
                }
            }
        }
    }

    private void OnKeyMoveOrHandMove(Vector3 cursorCursorPos)
    {
        Vector3 screenPixelPos = Vector3.zero;

        screenPixelPos.x = cursorCursorPos.x;
        screenPixelPos.y = cursorCursorPos.y;
        screenPixelPos.z = draggedObjectDepth;
        
        if (OnDropGameObject != null)
        {
            OnDropGameObject(draggedObject, screenPixelPos);
        }
    }

    private void OnKeyUpOrHandOpen()
    {

        //Debug.Log("------------------->ReleaseGameobject   " + draggedObject.name);
        if (onReleaseGameobject != null)
        {
            onReleaseGameobject(draggedObject);
        }
        
        draggedObject = null;

    }

}
