using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using SysFile = System.IO.File;


namespace SteamAPIUserInfo;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var appSettings = await SysFile.ReadAllTextAsync("app-settings.json");
        var ids = await SysFile.ReadAllLinesAsync("check_ids.txt");
        ulong[] parsedIds = null;

        try
        {
            parsedIds = ids.Select(x => ulong.Parse(x.Trim())).ToArray();
        }
        catch
        {
            Console.WriteLine("Не могу расспарсить Ids из файла check_ids.txt");
        }

        var options = JsonConvert.DeserializeObject<AppConfig>(appSettings);


        if (parsedIds == null)
        {
            Console.WriteLine("Что то пошло не так с ids.");
        }

        using (var checker = new SteamChecker(options))
        {
            await checker.AnalyzeIdsAsync(parsedIds);
        }
    }
}
