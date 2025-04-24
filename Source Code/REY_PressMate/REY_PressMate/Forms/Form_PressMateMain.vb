Imports System.CodeDom.Compiler
Imports System.Drawing.Text
Imports System.Threading
Imports REY_PressMate.My.Resources
Imports REY_PressMate.REY_PressMate.Settings

Public Class Form_PressMate

    'Class Globals
    Private stopProgramToken As CancellationTokenSource
    Private lastAnimation As String = "Off"
    Private writeText As String = ""
    Private randomNumber As Random = New Random
    Private animCounter As Integer = 0


#Region "---General Events"
    'Load All Defaults for our form here.
    Private Sub Form_PressMate_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        SetLanguage()

        'Remove the Up Down Numerical Buttons
        Lbl_Ind_PartsPressed.Controls.RemoveAt(0)
        Image_SpeechBubble.SendToBack()

        'Debug functions
        '---OPC DA
        'Dim oPCHandler As New Func_OPCDA
        'Dim Test = oPCHandler.Read("Static.PsString1")
        'oPCHandler.Write("Static.PsString1", 6666)

        'Test = oPCHandler.Read("Static.PsString1")
        'oPCHandler.Write("Static.PsString1", 4321)

        'Test = oPCHandler.Read("Static.PsString1")
        'oPCHandler.Write("Static.PsString1", 1234)

        '---FIS
        'Dim Test = Func_FIS.Send("1234!")
        'Test = Func_FIS.Send("5432!")
        'Test = Func_FIS.Send("6666!")

        '---SR751
        'Dim Read As New Func_Keyence
        'Dim test = Await Read.Read751()

        '---SR700
        'Dim Read As New Func_Keyence
        'Dim test = Await Read.Read700()


    End Sub
#End Region

#Region "---General Buttons"

    'Start button click event.
    Private Async Sub Btn_StartStop_Click(sender As Object, e As EventArgs) Handles Btn_StartStop.Click

        If Btn_StartStop.Text = TXT_UI_Start Then    'Change button text and start the asyncronous task
            Btn_StartStop.Text = TXT_UI_Stop
            lastAnimation = "TurnOn"

            'Set a new cancellation token then run the task.
            stopProgramToken = New CancellationTokenSource
            Await Routine_Main.Maintask(stopProgramToken.Token)
        Else                                    'Change button text and set the program cancellation token to true.
            Btn_StartStop.Text = TXT_UI_Start
            lastAnimation = "TurnOff"
            stopProgramToken.Cancel()
        End If

    End Sub

    'Reset counters button clicked.
    Private Sub ResetPressCountersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ResetPressCountersToolStripMenuItem.Click
        Lbl_Ind_PartsPressed.Value = 0
        Lbl_Ind_AvgCycletime.Text = 0
        Lbl_Ind_LastCycleTime.Text = 0
    End Sub

    'On form X, send a cancellation token to force a BCMP fail.
    Private Sub Form_PressMate_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If stopProgramToken IsNot Nothing Then stopProgramToken.Cancel()
    End Sub

#End Region

#Region "---ToolStrip Options"
    'English Language Selection
    Private Sub EnglishToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TSM_LanguageEnglish.Click

        'Update Usersettings
        Settings_User.Default.Language = "English"
        Settings_User.Default.Save()

    End Sub

    'Spanish Language Selection
    Private Sub SpanishToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TSM_LanguageSpanish.Click

        'Update Usersettings
        Settings_User.Default.Language = "Spanish"
        Settings_User.Default.Save()

    End Sub

    'Polish Language Selection
    Private Sub PolishToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TSM_LanguagePolish.Click

        'Update Usersettings
        Settings_User.Default.Language = "Polish"
        Settings_User.Default.Save()

    End Sub

    'Exit Selection
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TSM_File_Exit.Click
        Application.Exit()  'Close the application.
    End Sub

    'Display the Contact Page with team information.
    Private Sub ContactToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TSM_Help_Contact.Click

    End Sub


#End Region

#Region "GUI Tick"

    'Timer used to update the GUI graphic when the station is idle. Mid run is going to be during an actual code change.
    Private Async Sub Timer_GuiRobotUpdater_Tick(sender As Object, e As EventArgs) Handles Timer_GuiRobotUpdater.Tick

        'Turn off timer to prevent refires
        Me.Timer_GuiRobotUpdater.Stop()

        'Determine if we need to see a speech bubble or not and set the size/location for our text.
        If lastAnimation = "On" Or lastAnimation = "Off" Or lastAnimation = "RandomIdle" Then
            Me.Image_SpeechBubble.Visible = False
        Else

            'Set bubble size and text location.
            If lastAnimation = "TurnOn" Or lastAnimation = "TurnOff" Or lastAnimation = "Angry" Then
                Me.Image_SpeechBubble.BackgroundImage = PressGraphicResources.Speech_Bubble_Long 'PressGraphicResources.Speech_Bubble_Small
                Me.Lbl_SpeechBubble.Location = New Point(101, 45)
            Else
                Me.Image_SpeechBubble.BackgroundImage = PressGraphicResources.Speech_Bubble_Long
                Me.Lbl_SpeechBubble.Location = New Point(23, 45)
            End If

            Me.Image_SpeechBubble.Visible = True
        End If

        Select Case lastAnimation
            Case "Off"  'Robot is off.
                Me.Image_Press.Image = PressGraphicResources.Default_Startup
                Await WriteRobotGUIText("")

                Me.Timer_GuiRobotUpdater.Interval = 1000
                Me.Timer_GuiRobotUpdater.Start()

                lastAnimation = "Off"
            Case "On"   'Robot is on/Idle
                Me.Image_Press.Image = PressGraphicResources.Default_Awake
                Await WriteRobotGUIText("")

                Me.Timer_GuiRobotUpdater.Interval = 1000
                Me.Timer_GuiRobotUpdater.Start()

                If animCounter Mod 5 = 0 And animCounter <> 0 Then lastAnimation = "RandomIdle"   'Every 10s or so, do a random animation when on.
                If animCounter Mod 10 = 0 And animCounter <> 0 Then lastAnimation = "RandomJoke"  'Every 100s or so, make a random joke.

                animCounter += 1 'Increment anim/joke counter.

            Case "TurnOn"   'Robot Turn On
                Me.Image_Press.Image = PressGraphicResources.Robot_PowerOn
                Await WriteRobotGUIText(TXT_Robot_Hello)

                Me.Timer_GuiRobotUpdater.Interval = 2680
                Me.Timer_GuiRobotUpdater.Start()

                lastAnimation = "On"
            Case "TurnOff"  'Robot Turn Off
                Me.Image_Press.Image = PressGraphicResources.Robot_PowerOff
                Await WriteRobotGUIText(TXT_Robot_Bye)

                Me.Timer_GuiRobotUpdater.Interval = 2680
                Me.Timer_GuiRobotUpdater.Start()

                lastAnimation = "Off"

            Case "Angry"
                Me.Image_Press.Image = PressGraphicResources.Robot_PowerOff
                Await WriteRobotGUIText(TXT_Robot_Anger)
                Me.Timer_GuiRobotUpdater.Start()

                Me.Timer_GuiRobotUpdater.Interval = 1480

                lastAnimation = "On"

            Case "RandomIdle"

                'Set Random Animation from our Library
                Select Case randomNumber.Next(1, 10)
                    Case 1  'Look Down
                        Me.Image_Press.Image = PressGraphicResources.Robot_LookD
                        Me.Timer_GuiRobotUpdater.Interval = 1430
                    Case 2  'Look Down Right
                        Me.Image_Press.Image = PressGraphicResources.Robot_LookDR
                        Me.Timer_GuiRobotUpdater.Interval = 1000
                    Case 3  'Look Right
                        Me.Image_Press.Image = PressGraphicResources.Robot_LookR
                        Me.Timer_GuiRobotUpdater.Interval = 1430
                    Case 4  'Look Up Right
                        Me.Image_Press.Image = PressGraphicResources.Robot_LookDR
                        Me.Timer_GuiRobotUpdater.Interval = 1000
                    Case 5  'Look Up
                        Me.Image_Press.Image = PressGraphicResources.Robot_LookU
                        Me.Timer_GuiRobotUpdater.Interval = 1430
                    Case 6  'Look Up Left
                        Me.Image_Press.Image = PressGraphicResources.Robot_LookUL
                        Me.Timer_GuiRobotUpdater.Interval = 1000
                    Case 7  'Look Left
                        Me.Image_Press.Image = PressGraphicResources.Robot_LookL
                        Me.Timer_GuiRobotUpdater.Interval = 1430
                    Case 8 'Look Down Left
                        Me.Image_Press.Image = PressGraphicResources.Robot_LookDL
                        Me.Timer_GuiRobotUpdater.Interval = 1000
                    Case 9 'Blink
                        Me.Image_Press.Image = PressGraphicResources.Robot_Blink
                        Me.Timer_GuiRobotUpdater.Interval = 50
                End Select

                Me.Timer_GuiRobotUpdater.Start()

                lastAnimation = "On"

                animCounter += 1 'Increment anim/joke counter.

                'I'm gonna press it

            Case "RandomJoke" 'Make Random Comment when we have been idle 100 times.

                Select Case randomNumber.Next(1, 6)
                    Case 1
                        Await WriteRobotGUIText(TXT_Robot_Bench)
                        Me.Timer_GuiRobotUpdater.Interval = 5000
                    Case 2
                        Await WriteRobotGUIText(TXT_Robot_Impatient)
                        Me.Timer_GuiRobotUpdater.Interval = 5000
                    Case 3
                        Await WriteRobotGUIText(TXT_Robot_Showoff)
                        Me.Timer_GuiRobotUpdater.Interval = 5000
                    Case 4
                        Await WriteRobotGUIText(TXT_Robot_Doctor)
                        Me.Timer_GuiRobotUpdater.Interval = 5000
                    Case 5
                        Await WriteRobotGUIText(TXT_Robot_Reps)
                        Me.Timer_GuiRobotUpdater.Interval = 5000
                End Select

                Me.Timer_GuiRobotUpdater.Start()

                animCounter = 0 ' Reset animation counter.
                lastAnimation = "On"

        End Select
        GC.Collect()
    End Sub





#End Region


End Class
