using SteamAPIUserInfo;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Forms;

namespace SteamAPIUserInfoUI;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        ProgressLabel.Text = "Not Started";
    }

    private async void StartButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(SteamApiKeysInput.Text))
        {
            MessageBox.Show("Введите Steam API Keys через запятую.");
            return;
        }

        if (string.IsNullOrEmpty(IdsInput.Text))
        {
            MessageBox.Show("Введите Ids разделенные enter.");
            return;
        }

        ulong[] ids = PrepareIds(IdsInput.Text);
        string[] apiKeys = PrepareApiKeys(SteamApiKeysInput.Text);

        var apiKesQueue = new Queue<string>(apiKeys);

        var checker = new SteamChecker();
        var retry = 0;
        string currentApiKey = apiKesQueue.Dequeue();
        ProgressLabel.Text = "Woking...";

        while (retry < 2)
        {
            try
            {
                var webInterfaceFactory = new SteamWebInterfaceFactory(currentApiKey);
                await checker.AnalyzeIdsAsync(ids, webInterfaceFactory, ProgressLabel);
                break;
            }
            catch
            {
                retry++;
                if (retry == 2)
                {
                    if(apiKesQueue.Count == 0)
                    {
                        MessageBox.Show("Все Api ключи не подходят");
                        break;
                    }
                    retry = 0;
                    currentApiKey = apiKesQueue.Dequeue();
                }
            }
        }

        ProgressLabel.Text = "Done";
    }

    private static ulong[] PrepareIds(string ids)
    {
        return ids.Split('\n').Select(x => ulong.Parse(x.Trim())).ToArray();
    }

    private static string[] PrepareApiKeys(string apiKeys)
    {
        return apiKeys.Split(",").Select(x => x.Trim()).ToArray();
    }
}
