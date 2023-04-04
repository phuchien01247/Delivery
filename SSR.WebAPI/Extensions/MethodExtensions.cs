using System.Text.RegularExpressions;
using SSR.WebAPI.Models;
using SharpCompress.Common;
using SkiaSharp;

namespace SSR.WebAPI.Extensions;
public static class MethodExtensions
{
    public static string HideString(this string input)
    {
        return Regex.Replace(input, @"(\d(\d{0,5})$)|(\A[a-z]{1,4})", "xxxxxx");
    }

    public static IEnumerable<User> WithoutPasswords(this IEnumerable<User> users)
    {
        return users.Select(x => x.WithoutPassword());
    }

    public static User WithoutPassword(this User user)
    {
        //user.Password = null;
        user.PasswordHash = null;
        user.PasswordSalt = null;
        return user;
    }

    public static string ConvertVN(this string input)
    {
        const string FindText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
        const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
        int index = -1;
        char[] arrChar = FindText.ToCharArray();
        while ((index = input.IndexOfAny(arrChar)) != -1)
        {
            int index2 = FindText.IndexOf(input[index]);
            input = input.Replace(input[index], ReplText[index2]);
        }
        return input;
    }

    public static List<TreeVM> GetTree<TreeVM, Entity>(List<Entity> entities) where TreeVM : ITreeVM<TreeVM>, new() where Entity : IEntity<Entity>
    {
        var listDonVi = entities;

        var parents = listDonVi.Where(x => x.ParentId == null).ToList();
        var list = new List<TreeVM>();
        foreach (var item in parents)
        {
            var donVi = new TreeVM() { Id = item.Id, Label = item.Name, Opened = true};
            list.Add(donVi);
            GetLoopItem(ref list, listDonVi, donVi);
        }
        return list;
    }

    public static List<TreeVM> GetLoopItem<TreeVM, Entity>(ref List<TreeVM> list, List<Entity> items, TreeVM target) where TreeVM : ITreeVM<TreeVM>, new() where Entity : IEntity<Entity>
    {
        try
        {
            var entities = items.FindAll((item) => item.ParentId == target.Id).ToList();
            if (entities.Count > 0)
            {
                target.Children = new List<TreeVM>();
                foreach (var item in entities)
                {
                    var itemDV = new TreeVM() { Id = item.Id, Label = item.Name };
                    target.Children.Add(itemDV);
                    GetLoopItem(ref list, items, itemDV);
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            var message = ex.Message;
        }
        return null;
    }
}

public interface ITreeVM<T>
{
    string Id { get; set; }
    string Label { get; set; }
    bool Selected { get; set; }
    bool Opened { get; set; }
    List<T> Children { get; set; }
}

public class TreeVM : ITreeVM<TreeVM> {
   public string Id { get; set; }
    public string Label { get; set; }
    public bool Selected { get; set; }
    public bool Opened { get; set; }
    public List<TreeVM> Children { get; set; }
}

public interface IEntity<Entity>
{ 
    string Id { get; set; }
    string Name { get; set; }
    string ParentId { get; set; }
}