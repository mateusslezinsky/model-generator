namespace ModelGenerator.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Genre { get; set; }
    public decimal Price { get; set; }
    public Person Person { get; set; }
    public Enum Enum { get; set; }
    public List<Person> PeopleInvolved { get; set; }
}