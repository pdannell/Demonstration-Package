Module Func_Logging

    Private guiForm As Form_PressMate = My.Application.OpenForms("Form_PressMate")  'Global Passthrough for GUI calls.

    ''' <summary>
    ''' Add to log text on main window.
    ''' </summary>
    ''' <param name="txt">Any string to add a line to the log.</param>
    Sub Write(txt As String)
        guiForm.Txt_Log.AppendText(txt)
    End Sub

    'Clears the box for next use.
    Sub ClearTXT()
        guiForm.Txt_Log.Clear()
    End Sub

    'Saves to the box for next use.
    Sub SaveTXT(partNumber As String)
        guiForm.Txt_Log.SaveFile("C:\Logs\" + partNumber + "_" + DateTime.Now.ToString("dd-MM-yy-hh-mm-ss") + ".txt", RichTextBoxStreamType.PlainText)
    End Sub

    Public Async Function WriteRobotGUIText(txt As String) As Task

        For I As Integer = 0 To txt.Length
            guiForm.Lbl_SpeechBubble.Text = Strings.Left(txt, I)
            Await Task.Delay(200)
        Next

    End Function

End Module
