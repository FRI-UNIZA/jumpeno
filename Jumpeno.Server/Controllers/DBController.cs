namespace Jumpeno.Server.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class DBController : ControllerBase {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    private const string BACKUP_SUFFIX = "backup";
    private static readonly Locker Lock = new();
    
    // Endpoints --------------------------------------------------------------------------------------------------------------------------
    [HttpGet][Role(ROLE.ADMIN)]
    [Produces(CONTENT_TYPE.OCTET_STREAM)][ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public FileContentResult Download() {
        // try {
        //     Lock.Lock();
        //     // 1) Prepare backup:
        //     var backupPath = Path.Join(DBContext.Dir, $"{DBContext.FileName}.{BACKUP_SUFFIX}");
        //     System.IO.File.Delete(backupPath);
        //     // 2) Create backup:
        //     {
        //         using var connection = new SqliteConnection(DBContext.ConnectionString);
        //         connection.Open();
        //         using var cmd = connection.CreateCommand();
        //         cmd.CommandText = $"VACUUM INTO '{backupPath}'";
        //         cmd.ExecuteNonQuery();
        //     }
        //     // 3) Download file:
        //     byte[] fileBytes = System.IO.File.ReadAllBytes(backupPath);
        //     System.IO.File.Delete(backupPath);
        //     Response.Headers.ContentType = CONTENT_TYPE.OCTET_STREAM;
        //     return File(fileBytes, CONTENT_TYPE.OCTET_STREAM, DBContext.FileName);
        // } catch {
        //     throw new CoreException().SetCode(500).SetMessage("Error reading the file!");
        // } finally {
        //     Lock.Unlock();
        // }
        throw new CoreException().SetCode(500).SetMessage("Not implemented yet!");
    }

    [HttpGet]
    public async Task<IActionResult> AdminTest() {
        // using (var connection = new MySqlConnection(
        //     $"Server={ServerSettings.Database.DB_HOST};"
        //     + $"Database={ServerSettings.Database.Database};"
        //     + $"User={ServerSettings.Database.User};"
        //     + $"Password={ServerSettings.Database.DB_PASSWORD};"
        //     + $"Port={ServerSettings.Database.DB_PORT}"
        // )) {
        //     await connection.OpenAsync();

        //     // Simple dummy query for testing
        //     string query = "SELECT 1"; // Just returns '1' as a result

        //     using (var command = new MySqlCommand(query, connection))
        //     {
        //         var result = await command.ExecuteScalarAsync();
        //         if (result != null)
        //         {
        //             Console.WriteLine($"Query Result: {result}");
        //         }
        //     }
        // }


        foreach (var email in ServerSettings.Admins) {
            Console.WriteLine($"admin email: {email}");
        }

        return Ok(new {
            DB_HOST = ServerSettings.Database.Host,
            DB_PORT = ServerSettings.Database.Port,
            DB_Database = ServerSettings.Database.Database,
            DB_USER = ServerSettings.Database.User,
            DB_PASSWORD = ServerSettings.Database.Password,

            EMAIL_HOST = ServerSettings.Email.Host,
            EMAIL_PORT = ServerSettings.Email.Port,
            EMAIL_ADDRESS = ServerSettings.Email.Address,
            EMAIL_PASSWORD = ServerSettings.Email.Password,
            MAILCATCHER = ServerSettings.Email.Mailcatcher,

            JWT_ACCESS_SECRET = ServerSettings.JWT.AccessSecret,
            JWT_REFRESH_SECRET = ServerSettings.JWT.RefreshSecret,
            token = JWT.GenerateAdmin("daniel.test@gmail.com")
        });
    }
}
