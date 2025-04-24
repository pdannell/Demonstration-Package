<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form_PressMate
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form_PressMate))
        Btn_StartStop = New Button()
        Lbl_UI_PartsPressed = New Label()
        SplitContainer1 = New SplitContainer()
        Lbl_SpeechBubble = New Label()
        Image_SpeechBubble = New PictureBox()
        Image_Press = New PictureBox()
        Lbl_Indicator_Status = New Label()
        Lbl_UI_Status = New Label()
        Label3 = New Label()
        Lbl_Ind_AvgCycletime = New Label()
        Lbl_Ind_LastCycleTime = New Label()
        Lbl_UI_AvgCycletime = New Label()
        Lbl_UI_LastCycletime = New Label()
        Txt_Log = New RichTextBox()
        Lbl_Ind_PartsPressed = New NumericUpDown()
        LBL_WI = New TextBox()
        Picture_WI = New PictureBox()
        MenuStrip_Main = New MenuStrip()
        TSM_File = New ToolStripMenuItem()
        TSM_File_Exit = New ToolStripMenuItem()
        TSM_Options = New ToolStripMenuItem()
        TSM_Options_Settings = New ToolStripMenuItem()
        TSM_Options_Login = New ToolStripMenuItem()
        ResetPressCountersToolStripMenuItem = New ToolStripMenuItem()
        TSM_Language = New ToolStripMenuItem()
        TSM_LanguageEnglish = New ToolStripMenuItem()
        TSM_LanguageSpanish = New ToolStripMenuItem()
        TSM_LanguagePolish = New ToolStripMenuItem()
        TSM_Help = New ToolStripMenuItem()
        TSM_Help_Contact = New ToolStripMenuItem()
        Timer_GeneralUse = New Timer(components)
        BackgroundWorker1 = New ComponentModel.BackgroundWorker()
        Timer_CycleTime = New Timer(components)
        Timer_GuiRobotUpdater = New Timer(components)
        CType(SplitContainer1, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer1.Panel1.SuspendLayout()
        SplitContainer1.Panel2.SuspendLayout()
        SplitContainer1.SuspendLayout()
        CType(Image_SpeechBubble, ComponentModel.ISupportInitialize).BeginInit()
        CType(Image_Press, ComponentModel.ISupportInitialize).BeginInit()
        CType(Lbl_Ind_PartsPressed, ComponentModel.ISupportInitialize).BeginInit()
        CType(Picture_WI, ComponentModel.ISupportInitialize).BeginInit()
        MenuStrip_Main.SuspendLayout()
        SuspendLayout()
        ' 
        ' Btn_StartStop
        ' 
        Btn_StartStop.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Bold)
        Btn_StartStop.Location = New Point(2, 274)
        Btn_StartStop.Name = "Btn_StartStop"
        Btn_StartStop.Size = New Size(270, 63)
        Btn_StartStop.TabIndex = 0
        Btn_StartStop.Text = "Start"
        Btn_StartStop.UseVisualStyleBackColor = True
        ' 
        ' Lbl_UI_PartsPressed
        ' 
        Lbl_UI_PartsPressed.AutoSize = True
        Lbl_UI_PartsPressed.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Bold)
        Lbl_UI_PartsPressed.Location = New Point(3, 349)
        Lbl_UI_PartsPressed.Name = "Lbl_UI_PartsPressed"
        Lbl_UI_PartsPressed.Size = New Size(97, 20)
        Lbl_UI_PartsPressed.TabIndex = 2
        Lbl_UI_PartsPressed.Text = "Parts Built:"
        ' 
        ' SplitContainer1
        ' 
        SplitContainer1.BorderStyle = BorderStyle.Fixed3D
        SplitContainer1.Location = New Point(12, 30)
        SplitContainer1.Name = "SplitContainer1"
        ' 
        ' SplitContainer1.Panel1
        ' 
        SplitContainer1.Panel1.Controls.Add(Lbl_SpeechBubble)
        SplitContainer1.Panel1.Controls.Add(Image_SpeechBubble)
        SplitContainer1.Panel1.Controls.Add(Image_Press)
        SplitContainer1.Panel1.Controls.Add(Lbl_Indicator_Status)
        SplitContainer1.Panel1.Controls.Add(Lbl_UI_Status)
        SplitContainer1.Panel1.Controls.Add(Label3)
        SplitContainer1.Panel1.Controls.Add(Lbl_Ind_AvgCycletime)
        SplitContainer1.Panel1.Controls.Add(Lbl_Ind_LastCycleTime)
        SplitContainer1.Panel1.Controls.Add(Lbl_UI_AvgCycletime)
        SplitContainer1.Panel1.Controls.Add(Lbl_UI_LastCycletime)
        SplitContainer1.Panel1.Controls.Add(Txt_Log)
        SplitContainer1.Panel1.Controls.Add(Lbl_Ind_PartsPressed)
        SplitContainer1.Panel1.Controls.Add(Btn_StartStop)
        SplitContainer1.Panel1.Controls.Add(Lbl_UI_PartsPressed)
        ' 
        ' SplitContainer1.Panel2
        ' 
        SplitContainer1.Panel2.Controls.Add(LBL_WI)
        SplitContainer1.Panel2.Controls.Add(Picture_WI)
        SplitContainer1.Size = New Size(1000, 661)
        SplitContainer1.SplitterDistance = 279
        SplitContainer1.TabIndex = 5
        ' 
        ' Lbl_SpeechBubble
        ' 
        Lbl_SpeechBubble.AutoSize = True
        Lbl_SpeechBubble.Location = New Point(23, 45)
        Lbl_SpeechBubble.Name = "Lbl_SpeechBubble"
        Lbl_SpeechBubble.Size = New Size(0, 15)
        Lbl_SpeechBubble.TabIndex = 16
        ' 
        ' Image_SpeechBubble
        ' 
        Image_SpeechBubble.BackgroundImage = My.Resources.PressGraphicResources.Speech_Bubble_Long
        Image_SpeechBubble.BackgroundImageLayout = ImageLayout.Stretch
        Image_SpeechBubble.Location = New Point(2, 31)
        Image_SpeechBubble.Name = "Image_SpeechBubble"
        Image_SpeechBubble.Size = New Size(269, 49)
        Image_SpeechBubble.TabIndex = 15
        Image_SpeechBubble.TabStop = False
        Image_SpeechBubble.Visible = False
        ' 
        ' Image_Press
        ' 
        Image_Press.BackgroundImageLayout = ImageLayout.Zoom
        Image_Press.Image = My.Resources.PressGraphicResources.Default_Startup
        Image_Press.InitialImage = Nothing
        Image_Press.Location = New Point(35, 86)
        Image_Press.Name = "Image_Press"
        Image_Press.Size = New Size(192, 182)
        Image_Press.TabIndex = 14
        Image_Press.TabStop = False
        ' 
        ' Lbl_Indicator_Status
        ' 
        Lbl_Indicator_Status.AutoSize = True
        Lbl_Indicator_Status.BackColor = SystemColors.ControlDark
        Lbl_Indicator_Status.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Bold)
        Lbl_Indicator_Status.Location = New Point(171, 409)
        Lbl_Indicator_Status.Name = "Lbl_Indicator_Status"
        Lbl_Indicator_Status.Size = New Size(102, 20)
        Lbl_Indicator_Status.TabIndex = 13
        Lbl_Indicator_Status.Text = "Not running"
        ' 
        ' Lbl_UI_Status
        ' 
        Lbl_UI_Status.AutoSize = True
        Lbl_UI_Status.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Bold)
        Lbl_UI_Status.Location = New Point(3, 409)
        Lbl_UI_Status.Name = "Lbl_UI_Status"
        Lbl_UI_Status.Size = New Size(72, 20)
        Lbl_UI_Status.TabIndex = 12
        Lbl_UI_Status.Text = "Status: "
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold)
        Label3.Location = New Point(3, 3)
        Label3.Name = "Label3"
        Label3.Size = New Size(186, 25)
        Label3.TabIndex = 11
        Label3.Text = "PressMate: V1.0"
        ' 
        ' Lbl_Ind_AvgCycletime
        ' 
        Lbl_Ind_AvgCycletime.AutoSize = True
        Lbl_Ind_AvgCycletime.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Bold)
        Lbl_Ind_AvgCycletime.Location = New Point(171, 389)
        Lbl_Ind_AvgCycletime.Name = "Lbl_Ind_AvgCycletime"
        Lbl_Ind_AvgCycletime.Size = New Size(19, 20)
        Lbl_Ind_AvgCycletime.TabIndex = 10
        Lbl_Ind_AvgCycletime.Text = "0"
        ' 
        ' Lbl_Ind_LastCycleTime
        ' 
        Lbl_Ind_LastCycleTime.AutoSize = True
        Lbl_Ind_LastCycleTime.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Bold)
        Lbl_Ind_LastCycleTime.Location = New Point(171, 369)
        Lbl_Ind_LastCycleTime.Name = "Lbl_Ind_LastCycleTime"
        Lbl_Ind_LastCycleTime.Size = New Size(19, 20)
        Lbl_Ind_LastCycleTime.TabIndex = 9
        Lbl_Ind_LastCycleTime.Text = "0"
        ' 
        ' Lbl_UI_AvgCycletime
        ' 
        Lbl_UI_AvgCycletime.AutoSize = True
        Lbl_UI_AvgCycletime.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Bold)
        Lbl_UI_AvgCycletime.Location = New Point(3, 389)
        Lbl_UI_AvgCycletime.Name = "Lbl_UI_AvgCycletime"
        Lbl_UI_AvgCycletime.Size = New Size(162, 20)
        Lbl_UI_AvgCycletime.TabIndex = 8
        Lbl_UI_AvgCycletime.Text = "Average Cycletime:"
        ' 
        ' Lbl_UI_LastCycletime
        ' 
        Lbl_UI_LastCycletime.AutoSize = True
        Lbl_UI_LastCycletime.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Bold)
        Lbl_UI_LastCycletime.Location = New Point(3, 369)
        Lbl_UI_LastCycletime.Name = "Lbl_UI_LastCycletime"
        Lbl_UI_LastCycletime.Size = New Size(131, 20)
        Lbl_UI_LastCycletime.TabIndex = 6
        Lbl_UI_LastCycletime.Text = "Last Cycletime:"
        ' 
        ' Txt_Log
        ' 
        Txt_Log.Location = New Point(3, 432)
        Txt_Log.Name = "Txt_Log"
        Txt_Log.Size = New Size(269, 222)
        Txt_Log.TabIndex = 5
        Txt_Log.Text = ""
        ' 
        ' Lbl_Ind_PartsPressed
        ' 
        Lbl_Ind_PartsPressed.BackColor = SystemColors.ControlText
        Lbl_Ind_PartsPressed.Enabled = False
        Lbl_Ind_PartsPressed.Font = New Font("Microsoft Sans Serif", 12F, FontStyle.Bold)
        Lbl_Ind_PartsPressed.ForeColor = Color.White
        Lbl_Ind_PartsPressed.InterceptArrowKeys = False
        Lbl_Ind_PartsPressed.Location = New Point(175, 343)
        Lbl_Ind_PartsPressed.Maximum = New Decimal(New Integer() {60000, 0, 0, 0})
        Lbl_Ind_PartsPressed.Name = "Lbl_Ind_PartsPressed"
        Lbl_Ind_PartsPressed.Size = New Size(98, 26)
        Lbl_Ind_PartsPressed.TabIndex = 3
        Lbl_Ind_PartsPressed.ThousandsSeparator = True
        ' 
        ' LBL_WI
        ' 
        LBL_WI.Enabled = False
        LBL_WI.Font = New Font("Microsoft Sans Serif", 36F, FontStyle.Bold)
        LBL_WI.Location = New Point(3, 592)
        LBL_WI.Name = "LBL_WI"
        LBL_WI.Size = New Size(707, 62)
        LBL_WI.TabIndex = 2
        LBL_WI.TextAlign = HorizontalAlignment.Center
        ' 
        ' Picture_WI
        ' 
        Picture_WI.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Picture_WI.BackgroundImageLayout = ImageLayout.Zoom
        Picture_WI.Location = New Point(3, 3)
        Picture_WI.Name = "Picture_WI"
        Picture_WI.Size = New Size(707, 583)
        Picture_WI.TabIndex = 0
        Picture_WI.TabStop = False
        ' 
        ' MenuStrip_Main
        ' 
        MenuStrip_Main.Items.AddRange(New ToolStripItem() {TSM_File, TSM_Options, TSM_Language, TSM_Help})
        MenuStrip_Main.Location = New Point(0, 0)
        MenuStrip_Main.Name = "MenuStrip_Main"
        MenuStrip_Main.Size = New Size(1028, 24)
        MenuStrip_Main.TabIndex = 6
        MenuStrip_Main.Text = "MenuStrip1"
        ' 
        ' TSM_File
        ' 
        TSM_File.DropDownItems.AddRange(New ToolStripItem() {TSM_File_Exit})
        TSM_File.Name = "TSM_File"
        TSM_File.Size = New Size(37, 20)
        TSM_File.Text = "File"
        ' 
        ' TSM_File_Exit
        ' 
        TSM_File_Exit.Name = "TSM_File_Exit"
        TSM_File_Exit.Size = New Size(93, 22)
        TSM_File_Exit.Text = "Exit"
        ' 
        ' TSM_Options
        ' 
        TSM_Options.DropDownItems.AddRange(New ToolStripItem() {TSM_Options_Settings, TSM_Options_Login, ResetPressCountersToolStripMenuItem})
        TSM_Options.Name = "TSM_Options"
        TSM_Options.Size = New Size(61, 20)
        TSM_Options.Text = "Options"
        ' 
        ' TSM_Options_Settings
        ' 
        TSM_Options_Settings.Name = "TSM_Options_Settings"
        TSM_Options_Settings.Size = New Size(183, 22)
        TSM_Options_Settings.Text = "Settings"
        ' 
        ' TSM_Options_Login
        ' 
        TSM_Options_Login.Name = "TSM_Options_Login"
        TSM_Options_Login.Size = New Size(183, 22)
        TSM_Options_Login.Text = "Login"
        ' 
        ' ResetPressCountersToolStripMenuItem
        ' 
        ResetPressCountersToolStripMenuItem.Name = "ResetPressCountersToolStripMenuItem"
        ResetPressCountersToolStripMenuItem.Size = New Size(183, 22)
        ResetPressCountersToolStripMenuItem.Text = "Reset Press Counters"
        ' 
        ' TSM_Language
        ' 
        TSM_Language.DropDownItems.AddRange(New ToolStripItem() {TSM_LanguageEnglish, TSM_LanguageSpanish, TSM_LanguagePolish})
        TSM_Language.Name = "TSM_Language"
        TSM_Language.Size = New Size(71, 20)
        TSM_Language.Text = "Language"
        ' 
        ' TSM_LanguageEnglish
        ' 
        TSM_LanguageEnglish.Name = "TSM_LanguageEnglish"
        TSM_LanguageEnglish.Size = New Size(115, 22)
        TSM_LanguageEnglish.Text = "English"
        ' 
        ' TSM_LanguageSpanish
        ' 
        TSM_LanguageSpanish.Name = "TSM_LanguageSpanish"
        TSM_LanguageSpanish.Size = New Size(115, 22)
        TSM_LanguageSpanish.Text = "Spanish"
        ' 
        ' TSM_LanguagePolish
        ' 
        TSM_LanguagePolish.Name = "TSM_LanguagePolish"
        TSM_LanguagePolish.Size = New Size(115, 22)
        TSM_LanguagePolish.Text = "Polish"
        ' 
        ' TSM_Help
        ' 
        TSM_Help.DropDownItems.AddRange(New ToolStripItem() {TSM_Help_Contact})
        TSM_Help.Name = "TSM_Help"
        TSM_Help.Size = New Size(44, 20)
        TSM_Help.Text = "Help"
        ' 
        ' TSM_Help_Contact
        ' 
        TSM_Help_Contact.Name = "TSM_Help_Contact"
        TSM_Help_Contact.Size = New Size(116, 22)
        TSM_Help_Contact.Text = "Contact"
        ' 
        ' Timer_CycleTime
        ' 
        Timer_CycleTime.Interval = 100000
        ' 
        ' Timer_GuiRobotUpdater
        ' 
        Timer_GuiRobotUpdater.Enabled = True
        Timer_GuiRobotUpdater.Interval = 1000
        ' 
        ' Form_PressMate
        ' 
        AutoScaleMode = AutoScaleMode.None
        ClientSize = New Size(1028, 703)
        Controls.Add(SplitContainer1)
        Controls.Add(MenuStrip_Main)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "Form_PressMate"
        Text = "PressMate Application"
        WindowState = FormWindowState.Maximized
        SplitContainer1.Panel1.ResumeLayout(False)
        SplitContainer1.Panel1.PerformLayout()
        SplitContainer1.Panel2.ResumeLayout(False)
        SplitContainer1.Panel2.PerformLayout()
        CType(SplitContainer1, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer1.ResumeLayout(False)
        CType(Image_SpeechBubble, ComponentModel.ISupportInitialize).EndInit()
        CType(Image_Press, ComponentModel.ISupportInitialize).EndInit()
        CType(Lbl_Ind_PartsPressed, ComponentModel.ISupportInitialize).EndInit()
        CType(Picture_WI, ComponentModel.ISupportInitialize).EndInit()
        MenuStrip_Main.ResumeLayout(False)
        MenuStrip_Main.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Btn_StartStop As Button
    Friend WithEvents Btn_ResetCounter As Button
    Friend WithEvents Lbl_UI_PartsPressed As Label
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents MenuStrip_Main As MenuStrip
    Friend WithEvents TSM_File As ToolStripMenuItem
    Friend WithEvents TSM_File_Exit As ToolStripMenuItem
    Friend WithEvents TSM_Options As ToolStripMenuItem
    Friend WithEvents TSM_Options_Settings As ToolStripMenuItem
    Friend WithEvents TSM_Options_Login As ToolStripMenuItem
    Friend WithEvents TSM_Language As ToolStripMenuItem
    Friend WithEvents TSM_LanguageEnglish As ToolStripMenuItem
    Friend WithEvents TSM_LanguageSpanish As ToolStripMenuItem
    Friend WithEvents TSM_LanguagePolish As ToolStripMenuItem
    Friend WithEvents Timer_GeneralUse As Timer
    Friend WithEvents Lbl_Ind_PartsPressed As NumericUpDown
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents Txt_Log As RichTextBox
    Friend WithEvents Lbl_Ind_AvgCycletime As Label
    Friend WithEvents Lbl_Ind_LastCycleTime As Label
    Friend WithEvents Lbl_UI_AvgCycletime As Label
    Friend WithEvents Lbl_UI_LastCycletime As Label
    Friend WithEvents Timer_CycleTime As Timer
    Friend WithEvents Lbl_WI2 As Label
    Friend WithEvents Picture_WI As PictureBox
    Friend WithEvents Label3 As Label
    Friend WithEvents TSM_Help As ToolStripMenuItem
    Friend WithEvents TSM_Help_Contact As ToolStripMenuItem
    Friend WithEvents Lbl_Indicator_Status As Label
    Friend WithEvents Lbl_UI_Status As Label
    Friend WithEvents Image_Press As PictureBox
    Friend WithEvents Lbl_SpeechBubble As Label
    Friend WithEvents Image_SpeechBubble As PictureBox
    Friend WithEvents Timer_GuiRobotUpdater As Timer
    Friend WithEvents ResetPressCountersToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LBL_WI As TextBox

End Class
