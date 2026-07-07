using System;

public class ObjectiveObject
{
    public string description;
    public bool isDone;

    public ObjectiveObject(string value)
    {
        this.description = value;
        this.isDone = false;
    }

    public bool isComplete()
    {
        return isDone;
    }

    public void SetComplete(bool value)
    {
        this.isDone = value;
    }
}
