using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.FilePathAttribute;

public class PlacementHandler : MonoBehaviour
{
    // The object to be placed
    [SerializeField] GameObject placeObject;

    // The object that shows where you are placing
    [SerializeField] GameObject cursor;

    // The Layer mask of placed objects so that they can be removed
    [SerializeField] LayerMask objectLayer;

    // Holds all objects so the hierarchy doesn't get cluttered
    [SerializeField] Transform objectParent;

    Vector2 mousePos;

    // The mouse position converted to the grid position
    Vector3Int gridPos;

    // A shorthand for cursor.transform.position
    Vector3 cursorPos;

    // Current that objects are set to be when placed
    int rotation;

    RaycastHit2D hit;

    Grid grid;
    SpriteRenderer cursorSprite;
    SpriteRenderer objectSprite;

    InputAction place;
    InputAction delete;
    InputAction rotate;

    private void Start()
    {
        place = InputSystem.actions.FindAction("Place");
        delete = InputSystem.actions.FindAction("Delete");
        rotate = InputSystem.actions.FindAction("Rotate");

        cursorSprite = GetComponentInChildren<SpriteRenderer>();

        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Converts the mouse position to a position on the grid
        gridPos = grid.WorldToCell(mousePos);

        // Sets the cursor to the center of a grid cell according to the converted mousePos
        cursor.transform.position = grid.GetCellCenterWorld(gridPos);

        // Cursor position shorthand
        cursorPos = cursor.transform.position;


        hit = Physics2D.Raycast(new Vector3(cursorPos.x, cursorPos.y, -10), Vector3.forward, Mathf.Infinity, objectLayer);

        cursor.transform.rotation = Quaternion.Euler(0, 0, rotation);

        Place();

        Delete();

        Rotate();

        Cursor();
    }

    void Place()
    {
        // Instantiates the object to be placed at the position of the cursor and as a child of the object holder
        if (place.WasPerformedThisFrame())
        {
            // Checks if the spot is occupied
            if (hit.collider == null)
            {
                Instantiate(placeObject, cursorPos, Quaternion.Euler(0, 0, rotation), objectParent);
            }
        }
    }

    void Delete()
    {
        // Deletes the placed object that the cursor is hovering over
        if (delete.WasPerformedThisFrame())
        {
            // Checks if there actually is something at the cursor before trying to delete
            if (hit.collider != null)
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }

    void Rotate()
    {
        if (rotate.WasPerformedThisFrame())
        {
            rotation -= 90;

            if (rotation >= 360)
            {
                rotation = 0;
            }
        }
    }

    void Cursor()
    {
        objectSprite = placeObject.GetComponentInChildren<SpriteRenderer>();

        print(objectSprite.sprite);

        cursorSprite.sprite = objectSprite.sprite;
    }
}
