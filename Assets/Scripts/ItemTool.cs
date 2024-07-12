using UnityEngine;

public class ItemTool : Item
{
    // Enum for the types of tools
    public enum ItemToolType
    {
        PICKAXE,
        HAMMER,
        AXE
    }

    // Public variable to hold the type of tool
    public ItemToolType itemToolType;

    // Start is called before the first frame update
    private void Start()
    {
        // Initialization logic here
    }
}
