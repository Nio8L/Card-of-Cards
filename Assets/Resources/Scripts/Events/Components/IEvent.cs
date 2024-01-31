public interface IEvent
{
    //Called when a Lost Soul is placed in a card slot
    void LostSoulCase();

    //Called when a Lost Soul is removed from a card slot
    void RevertLostSoulCase();
}
