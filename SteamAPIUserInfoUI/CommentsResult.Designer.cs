namespace SteamAPIUserInfoUI;

partial class ResultForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.OnlyComments = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // OnlyComments
            // 
            this.OnlyComments.Location = new System.Drawing.Point(29, 29);
            this.OnlyComments.Multiline = true;
            this.OnlyComments.Name = "OnlyComments";
            this.OnlyComments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.OnlyComments.Size = new System.Drawing.Size(483, 396);
            this.OnlyComments.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Comments";
            // 
            // CommentsResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OnlyComments);
            this.Name = "CommentsResult";
            this.Text = "CommentsResult";
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox OnlyComments;
    private System.Windows.Forms.Label label1;
}