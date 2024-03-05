namespace PiranhaCMS.Search.Models;

public record MusicSearchHit
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Tags { get; set; }
}