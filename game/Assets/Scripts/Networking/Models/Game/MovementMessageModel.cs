
[System.Serializable]
public class MovementMessageModel
{
    public enum Action
    {
        Forward,
        Backward,
        Rot_left,
        Rot_right
    }
    public Action action;
    public bool pressed;
}
