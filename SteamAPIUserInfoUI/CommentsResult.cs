using System;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace SteamAPIUserInfoUI;

public partial class ResultForm : Form
{
    public ResultForm(string[] items, string textLabel)
    {
        InitializeComponent();
        if(items is null || items.Length == 0)
        {
            OnlyComments.Text = $"Нету id {textLabel}";
            return;
        }
        label1.Text = textLabel;
        OnlyComments.Text = string.Join(Environment.NewLine, items);
    }
}
