namespace CSharpStudies.Topic4.Examples;

public class User
{
    public string Name { get; set; }
    public bool IsActive { get; set; }

    public User(string name)
    {
        Name = name;
        IsActive = true;
    }
}
