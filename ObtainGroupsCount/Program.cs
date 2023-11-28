using Newtonsoft.Json;
using System.Linq;
using System;
using SysFile = System.IO.File;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace ObtainGroupsCount;

[XmlRoot("memberList")]
public class MemberList
{
    [XmlElement("groupID64")]
    public string GroupID64 { get; set; }

    [XmlElement("memberCount")]
    public int MemberCount { get; set; }
}

internal sealed class HttpProxyConfig
{
    public string Host { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }
}

public class GroupInfo
{
    public string Group { get; set; }

    public int UsersCount { get; set; }
}

internal class Program
{
    public static string EndLinkPath = "/members";

    public static Regex Regex = new Regex("of (.*?) Member(s)?", RegexOptions.IgnoreCase & RegexOptions.Compiled);

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Started.");
        var appSettings = await SysFile.ReadAllTextAsync("app-settings.json");
        var groups = await SysFile.ReadAllLinesAsync("groups.txt");

        var options = JsonConvert.DeserializeObject<AppConfig>(appSettings);

        var now = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");

        using var writerGroups = SysFile.CreateText($"groups_{now}.txt");
        using var writerGroupsAndUser = File.CreateText($"groups_users_{now}.txt");
        using var writerUsers = File.CreateText($"users_count_{now}.txt");
        using var writerBroken = File.CreateText($"broken_user_ids_{now}.txt");

        var middle = groups.Length / 2;

        string[] firstPart = groups.Take(middle).ToArray();
        string[] secondPart = groups.Skip(middle).ToArray();

        var p1 = new HttpClient(new HttpClientHandler
        {
            Proxy = new WebProxy(options.Proxy1.Host)
            {
                Credentials = new NetworkCredential(options.Proxy1.UserName, options.Proxy1.Password)
            },
            UseProxy = true,
            PreAuthenticate = true,
            UseDefaultCredentials = false,
        })
        { Timeout = TimeSpan.FromSeconds(10) };

        var p2 = new HttpClient(new HttpClientHandler
        {
            Proxy = new WebProxy(options.Proxy2.Host)
            {
                Credentials = new NetworkCredential(options.Proxy2.UserName, options.Proxy2.Password)
            },
            UseProxy = true,
            PreAuthenticate = true,
            UseDefaultCredentials = false,
        })
        { Timeout = TimeSpan.FromSeconds(10) };

        Stopwatch stopwatch = Stopwatch.StartNew();
        var t1 = RunAsync(new HttpClient(), writerBroken, firstPart);
        var t2 = RunAsync(new HttpClient(), writerBroken, secondPart);

        Console.WriteLine("Working...");
        await Task.WhenAll(t1, t2);
        var orderedGroups = t1.Result.Concat(t2.Result).OrderByDescending(x => x.UsersCount);

        foreach (var group in orderedGroups)
        {
            await Task.WhenAll(
                writerGroups.WriteLineAsync(group.Group),
                writerGroupsAndUser.WriteLineAsync($"{group.Group}  -  {group.UsersCount}"),
                writerUsers.WriteLineAsync(group.UsersCount.ToString()));
        }
        stopwatch.Stop();
        // Get the elapsed time in hours and minutes
        TimeSpan elapsedTime = stopwatch.Elapsed;
        int hours = elapsedTime.Hours;
        int minutes = elapsedTime.Minutes;
        // Print or use the elapsed time
        Console.WriteLine($"Time: {hours} h | {minutes} m");
        Console.WriteLine("Done.");
    }

    public static async Task<List<GroupInfo>> RunAsync(
        HttpClient httpClient,
        StreamWriter broken,
        string[] groups)
    {
        var list = new List<GroupInfo>(10000000);
        foreach (var item in groups)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(item + EndLinkPath);
                string stringXmlContent = await response.Content.ReadAsStringAsync();
                if (!stringXmlContent.Contains("pageLinks"))
                {
                    broken.WriteLine(item);
                    continue;
                }

                var matcher = Regex.Match(stringXmlContent);

                if (!matcher.Success)
                {
                    broken.WriteLine(item);
                    continue;
                }
                string extractedContent = matcher.Groups[1].Value.Trim();
                if (extractedContent.Contains(","))
                {
                    extractedContent = extractedContent.Replace(",", "");
                }

                if (!int.TryParse(extractedContent, out int count))
                {
                    broken.WriteLine(item);
                    continue;
                }

                //var memberList = DeserializeMemberList(stringXmlContent);

                list.Add(new GroupInfo
                {
                    Group = item,
                    UsersCount = count
                });
            }
            catch
            {
                broken.WriteLine(item);
                continue;
            }
        }

        return list;
    }

    public static MemberList DeserializeMemberList(string xmlData)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MemberList));
            using (StringReader reader = new StringReader(xmlData))
            {
                MemberList memberList = (MemberList)serializer.Deserialize(reader);

                // ... and so on for other properties
                return memberList;
            }
        }
        catch
        {
            return null;
        }
    }
}
