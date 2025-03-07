namespace Jumpeno.Server.Controllers;

using Microsoft.Data.Sqlite;

[ApiController]
[Route("[controller]/[action]")]
public class DBController : ControllerBase {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    private const string BACKUP_SUFFIX = "backup";
    private static readonly Locker Lock = new();
    
    // Endpoints --------------------------------------------------------------------------------------------------------------------------
    [HttpGet]
    public FileContentResult Download() {
        try {
            Lock.Lock();
            // 1) Prepare backup:
            var backupPath = Path.Join(DB.Dir, $"{DB.FileName}.{BACKUP_SUFFIX}");
            System.IO.File.Delete(backupPath);
            // 2) Create backup:
            {
                using var connection = new SqliteConnection(DB.ConnectionString);
                connection.Open();
                using var cmd = connection.CreateCommand();
                cmd.CommandText = $"VACUUM INTO '{backupPath}'";
                cmd.ExecuteNonQuery();
            }
            // 3) Download file:
            byte[] fileBytes = System.IO.File.ReadAllBytes(backupPath);
            System.IO.File.Delete(backupPath);
            return File(fileBytes, CONTENT_TYPE.OCTET_STREAM, DB.FileName);
        } catch {
            throw new CoreException().SetCode(500).SetMessage("Error reading the file!");
        } finally {
            Lock.Unlock();
        }
    }
}
