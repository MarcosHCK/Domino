public abstract class Action
{
    public string autor {get; private set;}
    public Action(string autor)
    {
        this.autor = autor;
    }
}