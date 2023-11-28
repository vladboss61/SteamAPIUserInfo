using Newtonsoft.Json;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SysFile = System.IO.File;

namespace SteamGroupSearcher;

internal class Program
{
    public static string Link = "https://steamcommunity.com/gid/";

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

        var now = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
        using var writerAllGroups = File.CreateText($"all_groups_{now}.txt");
        using var writerBroken = File.CreateText($"broken_user_ids{now}.txt");

        var middle = parsedIds.Length / 2;

        ulong[] firstPart = parsedIds.Take(middle).ToArray();
        ulong[] secondPart = parsedIds.Skip(middle).ToArray();

        var t1 = Task.Run(async () => await RunAsync(firstPart, options.SteamApiKey1, writerAllGroups, writerBroken));
        var t2 = Task.Run(async () => await RunAsync(secondPart, options.SteamApiKey1, writerAllGroups, writerBroken));

        await Console.Out.WriteLineAsync("Wait...");
        await Task.WhenAll(t1, t2);
        await Console.Out.WriteLineAsync("Done.");
    }

    public static async Task RunAsync(ulong[] parsedIds, string steamApiKey, StreamWriter writerAllGroups, StreamWriter writerBroken)
    {
        var webInterfaceFactory = new SteamWebInterfaceFactory(steamApiKey);
        var steamUserInterface = webInterfaceFactory.CreateSteamWebInterface<SteamUser>();
        for (int i = 0; i < parsedIds.Length; i++)
        {
            var currentUserId = parsedIds[i];
            try
            {
                var groups = await steamUserInterface.GetUserGroupsAsync(currentUserId);

                await Task.Delay(30);

                if (groups is null || groups.Data is null || groups.Data.Count == 0)
                {
                    continue;
                }

                foreach (var group in groups.Data)
                {
                    await writerAllGroups.WriteLineAsync(Link + group);
                };

                await writerAllGroups.FlushAsync();
            }
            catch (Exception ex)
            {
                writerBroken.WriteLine(currentUserId);
                await writerBroken.FlushAsync();
                continue;
            }
        }
    }
}
