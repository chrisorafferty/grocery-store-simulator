
public class GroceryListAssigner : HighlightableInteractable {

    public void AssignPlayerGroceryList() {
        if (GameManager.playerGroceryList != null) return;

        GameManager.NewPlayerGroceryList();
    }
}
