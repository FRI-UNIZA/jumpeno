namespace Jumpeno.Server.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController(IDbContextFactory<DB> myService) : ControllerBase {
    private readonly IDbContextFactory<DB> DBFactory = myService;

    [HttpPost(Name = "UserCreate")]
    public async Task Create([FromBody] Person person) {
        var db = await DBFactory.CreateDbContextAsync();
        db.Persons.Add(person);
        await db.SaveChangesAsync();
    }

    [HttpGet(Name = "UserRead")]
    public async Task<List<Person>> Read() {
        var db = await DBFactory.CreateDbContextAsync();
        var result = await db.Persons.ToListAsync();
        await db.DisposeAsync();
        return result;
    }

    [HttpPatch(Name = "UserUpdate")]
    public async Task Update([FromBody] int id) {
        var db = await DBFactory.CreateDbContextAsync();
        var person = db.Persons.FirstOrDefault(x => x.Id == id);
        if (person != null) {
            person.Name = "Igor";
            db.Persons.Update(person);
            await db.SaveChangesAsync();
        } else {
            await db.DisposeAsync();
        }
    }

    [HttpDelete(Name = "UserDelete")]
    public async Task Delete([FromBody] int id) {
        var db = await DBFactory.CreateDbContextAsync();
        db.Persons.Remove(new() { Id = id });
        await db.SaveChangesAsync();
    }
}
