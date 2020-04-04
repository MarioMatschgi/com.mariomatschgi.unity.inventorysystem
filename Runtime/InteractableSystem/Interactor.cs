namespace MM.Systems.InventorySystem
{
    public interface IInteractor
    {
        int interactorId { get; set; }

        InteractorInventoryUi inventoryUi { get; set; }
    }
}