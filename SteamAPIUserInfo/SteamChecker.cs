using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SteamAPIUserInfo;

enum YesNo
{
    Yes,
    No
}

internal class SteamChecker : IDisposable
{
    private readonly string PatternNoScreenshot = "id=\"NoItemsContainer\"";
    private readonly HttpClient _httpClient;
    private readonly AppConfig _appConfig;

    public SteamChecker(AppConfig appConfig)
    {
        _appConfig = appConfig;
        _httpClient = new HttpClient();
    }

    public async Task AnalyzeIdsAsync(ulong[] ids)
    {
        var webInterfaceFactory = new SteamWebInterfaceFactory(_appConfig.SteamApiKey);
        var steamUserInterface = webInterfaceFactory.CreateSteamWebInterface<SteamUser>(_httpClient);
        var now = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");

        using var writerAll = File.CreateText($"all_comments_and_screenshots_{now}.txt");
        using var writerOnlyComments = File.CreateText($"all_comments_{now}.txt");
        using var writerOnlyScreenshots= File.CreateText($"all_screenshots_{now}.txt");

        var countStop = 0;
        int number = 1;

        foreach (var id in ids)
        {
            Console.WriteLine($"Number: {number} | Processing id - {id}");

            try
            {
                var playerSummaryResponse = await steamUserInterface.GetPlayerSummaryAsync(id);
                var commentsPermission = playerSummaryResponse.Data.CommentPermission;

                var result = await (await _httpClient.GetAsync($"{_appConfig.SteamUrl}/profiles/{id}/screenshots")).Content.ReadAsStringAsync();

                var comments = GetYesNoComments(commentsPermission);
                var screenshots = GetYesNoScreenshot(result);
                await writerAll.WriteLineAsync($"Number: {number} | Id: {id} | Comments: {comments} | Screenshot: {screenshots}");

                if (comments == YesNo.Yes)
                {
                    await writerOnlyComments.WriteLineAsync($"{id}");
                }

                if (screenshots == YesNo.Yes)
                {
                    await writerOnlyScreenshots.WriteLineAsync($"{id}");
                }

                if (countStop == 100)
                {
                    await Task.Delay(new Random().Next(8000, 10000));
                    countStop = 0;
                }
                countStop++;
                number++;
            }
            catch (Exception ex)
            {
                using var errorLogs = File.CreateText($"error_logs_{now}.txt");
                var msg = $"Something when wrong. Last have not been analyzed id: {id}.";

                await errorLogs.WriteLineAsync(msg);
                await errorLogs.WriteLineAsync("Stack Trace:");
                await errorLogs.WriteLineAsync(ex.StackTrace);
                await errorLogs.WriteLineAsync("Message:");
                await errorLogs.WriteLineAsync(ex.Message);

                Console.WriteLine(msg);
                return;
            }
        }

        Console.WriteLine("Done.");
    }

    private static YesNo GetYesNoComments(CommentPermission commentsPermission)
    {
        return (commentsPermission == CommentPermission.Public || commentsPermission == CommentPermission.FriendsOnly) ? YesNo.Yes : YesNo.No;
    }

    private YesNo GetYesNoScreenshot(string pageContent)
    {
        return pageContent.Contains(PatternNoScreenshot) ? YesNo.No : YesNo.Yes;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
