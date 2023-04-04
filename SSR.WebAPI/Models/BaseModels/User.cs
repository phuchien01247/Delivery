using SSR.WebAPI.ViewModels;
using MongoDB.Bson.Serialization.Attributes;
using SSR.WebAPI.Models;
using SSR.WebAPI.ViewModels;

namespace SSR.WebAPI.Models;
public class User : Audit, TEntity<string>
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }

    public string UserName { get; set; }
    public byte[] PasswordSalt { get; set; }
    public byte[] PasswordHash { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Note { get; set; }
    public List<FileShort> Avatar { get; set; }
    public List<Role> Role { get; set; }
    [BsonIgnore] public string Password { get; set; }
    [BsonIgnore] public List<string> Permissions { get; set; }
    [BsonIgnore] public List<NavMenuVM> Menu { get; set; }
    [BsonIgnore] public IEnumerable<Menu> MenuItems { get; set; }
}

public class UserPhone
{
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
}