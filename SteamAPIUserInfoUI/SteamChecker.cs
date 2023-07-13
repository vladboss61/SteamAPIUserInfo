﻿using Steam.Models.SteamCommunity;
using SteamAPIUserInfoUI;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
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

    public SteamChecker()
    {
        _httpClient = new HttpClient();
    }

    public async Task AnalyzeIdsAsync(ulong[] ids, SteamWebInterfaceFactory steamWebInterfaceFactory)
    {
        var steamUserInterface = steamWebInterfaceFactory.CreateSteamWebInterface<SteamUser>(_httpClient);
        var now = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");

        using var writerAll = File.CreateText($"all_comments_and_screenshots_{now}.txt");
        using var writerOnlyComments = File.CreateText($"all_comments_{now}.txt");
        using var writerOnlyScreenshots = File.CreateText($"all_screenshots_{now}.txt");

        List<string> allItems = new List<string>();
        List<string> commentsItems = new List<string>();
        List<string> screenshotsItems = new List<string>();

        var countStop = 0;
        int number = 1;

        foreach (var id in ids)
        {
            Console.WriteLine($"Number: {number} | Processing id - {id}");

            try
            {
                var playerSummaryResponse = await steamUserInterface.GetPlayerSummaryAsync(id);
                var commentsPermission = playerSummaryResponse.Data.CommentPermission;

                var result = await (await _httpClient.GetAsync($"{SteamConstants.SteamUrl}/profiles/{id}/screenshots")).Content.ReadAsStringAsync();

                var comments = GetYesNoComments(commentsPermission);
                var screenshots = GetYesNoScreenshot(result);
                var allStr = $"Number: {number} | Id: {id} | Comments: {comments} | Screenshot: {screenshots}";
                await writerAll.WriteLineAsync(allStr);

                allItems.Add(allStr);

                if (screenshots == YesNo.Yes && comments == YesNo.Yes)
                {
                    await writerOnlyScreenshots.WriteLineAsync($"{id}");
                    screenshotsItems.Add($"{id}");
                }

                if (comments == YesNo.Yes && screenshots == YesNo.No)
                {
                    await writerOnlyComments.WriteLineAsync($"{id}");
                    commentsItems.Add($"{id}");
                }

                if (countStop == 100)
                {
                    await Task.Delay(new Random().Next(8000, 10000));

                    await writerAll.FlushAsync();
                    await writerOnlyComments.FlushAsync();
                    await writerOnlyScreenshots.FlushAsync();

                    countStop = 0;
                }
                countStop++;
                number++;
            }
            catch (Exception ex)
            {
                new ResultForm(commentsItems.ToArray(), "Не все Comments но те что смог проанализиовать").Show();
                new ResultForm(screenshotsItems.ToArray(), "Не все Screenshots").Show();
                new ResultForm(allItems.ToArray(), "Все и Comments и Screenshots но те что смог проанализиовать").Show();

                using var errorLogs = File.CreateText($"error_logs_{now}.txt");
                var msg = $"Something when wrong. Last have not been analyzed id: {id}.";

                await errorLogs.WriteLineAsync(msg);
                await errorLogs.WriteLineAsync("Stack Trace:");
                await errorLogs.WriteLineAsync(ex.StackTrace);
                await errorLogs.WriteLineAsync("Message:");
                await errorLogs.WriteLineAsync(ex.Message);
                throw;
            }
        }
        new ResultForm(commentsItems.ToArray(), "Comments").Show();
        new ResultForm(screenshotsItems.ToArray(), "Screenshots").Show();
        new ResultForm(allItems.ToArray(), "Все и Comments и Screenshots").Show();
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
