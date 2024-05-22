namespace PiranhaCMS.Business.OpenAi.Models;

internal record Message(string Role, string Content)
{
    public override string ToString()
    {
        return $"Role: {Role}, Content: {Content}";
    }
}
