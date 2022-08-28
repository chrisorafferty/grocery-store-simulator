
public class GroceryOrderDropoff : HighlightableInteractable {

    public void DropOffPlayerGroceryOrder() {
        if (GameManager.playerGroceryList?.IsFullyPicked != true) return;

        // MONEY+++++++
        GameManager.ClearPlayerGroceryList();
    }
}
