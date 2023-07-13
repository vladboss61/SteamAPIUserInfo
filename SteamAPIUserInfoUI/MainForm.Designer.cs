using System.Windows.Forms;

namespace SteamAPIUserInfoUI;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.SteamApiKeysInput = new System.Windows.Forms.TextBox();
            this.APIKeys = new System.Windows.Forms.Label();
            this.IdsInput = new System.Windows.Forms.TextBox();
            this.Ids = new System.Windows.Forms.Label();
            this.StartButton = new System.Windows.Forms.Button();
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SteamApiKeysInput
            // 
            this.SteamApiKeysInput.Location = new System.Drawing.Point(326, 31);
            this.SteamApiKeysInput.Name = "SteamApiKeysInput";
            this.SteamApiKeysInput.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.SteamApiKeysInput.Size = new System.Drawing.Size(462, 23);
            this.SteamApiKeysInput.TabIndex = 0;
            // 
            // APIKeys
            // 
            this.APIKeys.AutoSize = true;
            this.APIKeys.Location = new System.Drawing.Point(326, 13);
            this.APIKeys.Name = "APIKeys";
            this.APIKeys.Size = new System.Drawing.Size(82, 15);
            this.APIKeys.TabIndex = 1;
            this.APIKeys.Text = "SteamAPIKeys";
            // 
            // IdsInput
            // 
            this.IdsInput.Location = new System.Drawing.Point(12, 31);
            this.IdsInput.MaxLength = 382136;
            this.IdsInput.Multiline = true;
            this.IdsInput.Name = "IdsInput";
            this.IdsInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.IdsInput.Size = new System.Drawing.Size(308, 377);
            this.IdsInput.TabIndex = 2;
            // 
            // Ids
            // 
            this.Ids.AutoSize = true;
            this.Ids.Location = new System.Drawing.Point(14, 13);
            this.Ids.Name = "Ids";
            this.Ids.Size = new System.Drawing.Size(22, 15);
            this.Ids.TabIndex = 3;
            this.Ids.Text = "Ids";
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(703, 384);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(75, 24);
            this.StartButton.TabIndex = 4;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Clicked);
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.AutoSize = true;
            this.ProgressLabel.Location = new System.Drawing.Point(532, 73);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(80, 15);
            this.ProgressLabel.TabIndex = 5;
            this.ProgressLabel.Text = "ProgressLable";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ProgressLabel);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.Ids);
            this.Controls.Add(this.IdsInput);
            this.Controls.Add(this.APIKeys);
            this.Controls.Add(this.SteamApiKeysInput);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private TextBox SteamApiKeysInput;
    private Label APIKeys;
    private TextBox IdsInput;
    private Label Ids;
    private Button StartButton;
    private Label ProgressLabel;
}
