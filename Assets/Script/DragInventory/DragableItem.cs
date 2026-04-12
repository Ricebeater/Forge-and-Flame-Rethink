using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public InventoryItem item;
    public InventoryBase currentGrid;

    private Canvas canvas;
    private CanvasGroup canvasGroup;

    Vector2 originalPos;
    int oriGridX, oriGridY;
    InventoryBase originalGrid;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        item = GetComponent<InventoryItem>();
    }

    private void Start()
    {
        currentGrid = item.grid;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //save the original grid and position
        currentGrid = item.grid;
        originalGrid = currentGrid;
        oriGridX = item.gridX;
        oriGridY = item.gridY;
        originalPos = item.rectTransform.anchoredPosition;

        currentGrid.RemoveItem(item);
        
        item.transform.SetParent(canvas.transform);

        //visual
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.4f;

    }

    public void OnDrag(PointerEventData eventData)
    {
        item.rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        

        InventoryBase targetGrid = GetInventoryUnderMouse(eventData);

        if (targetGrid == null)
        {
            ReturnToOriginalPos();
            return;
        }

        int x, y;

        if(!targetGrid.GetMousePosInGrid(Input.mousePosition, out x, out y))
        {
            ReturnToOriginalPos();
            return;
        }

        if(targetGrid.CanPlaceItem(item, x, y))
        {
            targetGrid.PlaceItem(item, x, y);
            
            currentGrid = targetGrid;
            //item.rectTransform.anchoredPosition = currentGrid.GetItemPositionInUI(x, y);
        }
        else
        {
            ReturnToOriginalPos();
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    public void ReturnToOriginalPos()
    {
        canvasGroup.blocksRaycasts = true;

        originalGrid.PlaceItem(item,oriGridX,oriGridY);
    }

    private InventoryBase GetInventoryUnderMouse(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            InventoryBase grid = result.gameObject.GetComponent<InventoryBase>();
            if (grid != null)
            {
                Debug.Log("Found inventory grid under mouse: " + grid.name);
                return grid;
            }
        }

        Debug.Log("No inventory grid found under mouse.");
        return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            int x, y;
            if (currentGrid.GetMousePosInGrid(Input.mousePosition, out x, out y))
            {
                item.RotateItem();
            }
        }

    }
}
