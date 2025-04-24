Imports System.Threading

Module Routine_Main

    'Module level Locals
#Region "---Module level locals"
    Private guiForm As Form_PressMate = My.Application.OpenForms("Form_PressMate")  'Global Passthrough for GUI calls.
    Dim oPCHandler As New Func_OPCDA        'OPCDA Handler
    Dim barCodeReader As New Func_Keyence   'Barcode Handler 
#End Region

    'Main Task Loop For Pressmate
    'sPT = Stop Program Token
    'Cancellation token is sPT.IsCancellationRequested  = true
    Public Async Function Maintask(sPT As CancellationToken) As Task
        Try
            'Timers for our cycles.
            Dim averageCycletime As Integer = 0
            Dim startTime As DateTime

            Do Until sPT.IsCancellationRequested = True
reStart:
#Region "---Function Locals"
                Dim barCode_PCB As String = Nothing     'PCB Barcode
                Dim barCode_Housing As String = Nothing 'Housing Barcode
                Dim fisResponse As String = ""          'Temp value for FIS
                Dim partPassFail As String = "FAIL"     'True = Pass, False = Fail
                Dim dataFMax As String = ""             'fMax pressure from press.
#End Region

                'Start Cycle Timer
                guiForm.Lbl_Indicator_Status.Text = "Running"
                guiForm.Lbl_Indicator_Status.BackColor = Color.Green
                startTime = DateTime.Now

#Region "---Make sure there's no part in the way on startup"
                'Read the presence sensor bit, 0 = Present, 1 = Not Present
                If oPCHandler.Read("machine_1.PLC.Inputs.ByNumber.0_to_99.2") <> "True" And sPT.IsCancellationRequested = False Then
                    ShowWI(My.Resources.PressGraphicResources.Step_NoCover, TXT_WI_RemovePart) 'Update WI And Text

                    'Read the presence sensor bit, 0 = Present, 1 = Not Present
                    Do While oPCHandler.Read("machine_1.PLC.Inputs.ByNumber.0_to_99.2") <> "True" And sPT.IsCancellationRequested = False
                        Await Task.Delay(1000)
                    Loop
                End If
#End Region
                If sPT.IsCancellationRequested = True Then GoTo fisBCMP
#Region "---Write OPC Press Lock Start"
                oPCHandler.Write("machine_1.PLC.Markers.ByNumber.1100_to_1199.1100", 0, "Integer")
#End Region
                If sPT.IsCancellationRequested = True Then GoTo fisBCMP
#Region "---Wait For Part Presence"
                ShowWI(My.Resources.PressGraphicResources.Step1_PushPart, TXT_WI_LoadPart) 'Update WI And Text

                'Read the presence sensor bit, 0 = Present, 1 = Not Present
                Do While oPCHandler.Read("machine_1.PLC.Inputs.ByNumber.0_to_99.2") <> "False" And sPT.IsCancellationRequested = False
                    Await Task.Delay(1000)
                Loop
#End Region
                If sPT.IsCancellationRequested = True Then GoTo fisBCMP
#Region "---Read PCB Barcode"
retryPCB:
                Func_Logging.Write("PCB Scan:" + vbLf)
                barCode_PCB = Await barCodeReader.Read700()
                Func_Logging.Write(barCode_PCB + vbLf)

                'Prompt and retry
                If barCode_PCB = Nothing Then
                    Func_Logging.Write("Failed To Read" + vbLf)

                    'See if they chose to rechose to retry or fail the part.
                    If MessageBox.Show("Failed to read PCB", "Scan Failure", MessageBoxButtons.RetryCancel) = DialogResult.Retry And sPT.IsCancellationRequested = False Then
                        GoTo retryPCB
                    Else
                        GoTo fisBCMP
                    End If
                End If
#End Region
                If sPT.IsCancellationRequested = True Then GoTo fisBCMP
#Region "---Send PCB Barcode To FIS"
retryPCBFIS:

                'Send and get the FIS Message
                Func_Logging.Write("Sending BREQ Message:" + vbLf + "BREQ|" + barCode_PCB + vbLf)
                fisResponse = Func_FIS.Send("BREQ|" + barCode_PCB)
                Func_Logging.Write("FIS Response:" + vbLf + fisResponse + vbLf)

                'See if they want to retry or give up.
                If fisResponse.Contains("PASS") = False And fisResponse <> "ByPass" Then
                    Func_Logging.Write("Part Failed FIS" + vbLf)

                    'See if they chose to rechose to retry or fail the part.
                    If MessageBox.Show("Part Failed FIS", "FIS Failure", MessageBoxButtons.RetryCancel) = DialogResult.Retry And sPT.IsCancellationRequested = False Then
                        GoTo retryPCBFIS
                    Else
                        GoTo fisBCMP
                    End If

                Else
                    Func_Logging.Write("FIS Passed" + vbLf)
                End If
#End Region
                If sPT.IsCancellationRequested = True Then GoTo fisBCMP
#Region "---Write OPC Barcode Tag"
                oPCHandler.Write("machine_1.Idents.Ident1", barCode_PCB, "String")
#End Region
                If sPT.IsCancellationRequested = True Then GoTo fisBCMP
#Region "---Write OPC Press Unlock Start"
                oPCHandler.Write("machine_1.PLC.Markers.ByNumber.1100_to_1199.1100", 1, "Integer")
#End Region
                If sPT.IsCancellationRequested = True Then GoTo fisBCMP
#Region "---Wait for start cycle then run then stop"
                ShowWI(My.Resources.PressGraphicResources.Step2_Press, TXT_WI_PressStart) 'Update WI And Text

                'Grab last run values for Start/Done
                Dim lastStartValue = oPCHandler.Read("machine_1.Press_1.Triggers.PressCycleStart")
                Dim lastDoneValue = oPCHandler.Read("machine_1.Press_1.Triggers.PressCycleDone")

                'Wait to see start signal.
                Do While oPCHandler.Read("machine_1.Press_1.Triggers.PressCycleStart") = lastStartValue And
                        sPT.IsCancellationRequested = False
                    Await Task.Delay(1000)
                Loop

                'Wait For Finish
                ShowWI(My.Resources.PressGraphicResources.Step_Wait, TXT_WI_Wait) 'Update WI And Text

                'Wait to see the start and done increment.
                Do While oPCHandler.Read("machine_1.Press_1.Triggers.PressCycleDone") = lastDoneValue And
                        sPT.IsCancellationRequested = False
                    Await Task.Delay(1000)
                Loop
#End Region
                If sPT.IsCancellationRequested = True Then GoTo fisBCMP
#Region "---Grab fMax Value"
                dataFMax = Int(oPCHandler.Read("machine_1.Press_1.Curve.Fmax")).ToString    'Converts String->Int->String to remove decimals.
                Func_Logging.Write("Pressed To:" + vbLf + dataFMax + vbLf)
#End Region
                If sPT.IsCancellationRequested = True Then GoTo fisBCMP
#Region "---Check for Failure or Press Error"
                If oPCHandler.Read("machine_1.PLC.Outputs.ByNumber.800_to_899.898") = "True" Then
                    ShowWI(My.Resources.PressGraphicResources.Step_ErrorFix, TXT_WI_FollowHMI)

                    'Wait till we reach TDC
                    Do While oPCHandler.Read("machine_1.PLC.Outputs.ByNumber.900_to_999.900") = "False"
                        Await Task.Delay(1000)
                    Loop

                    GoTo fisBCMP
                End If
#End Region
                If sPT.IsCancellationRequested = True Then GoTo fisBCMP
#Region "---Check for press interrupt"
                If oPCHandler.Read("machine_1.PLC.Outputs.ByNumber.900_to_999.905") = "False" Then
                    'oPCHandler.Read("machine_1.PLC.Outputs.ByNumber.900_to_999.902") = "False" Then
                    ShowWI(My.Resources.PressGraphicResources.Step_Unlock, TXT_WI_Unlock)

                    'Wait till we reach TDC
                    Do While oPCHandler.Read("machine_1.PLC.Outputs.ByNumber.900_to_999.900") = "False"
                        Await Task.Delay(1000)
                    Loop

                    GoTo reStart
                End If
#End Region
                If sPT.IsCancellationRequested = True Then GoTo fisBCMP
#Region "---Wait for removal"
                ShowWI(My.Resources.PressGraphicResources.Step3_Place2, TXT_WI_AddTop) 'Update WI And Text

                'Read the presence sensor bit, 0 = Present, 1 = Not Present
                Do While oPCHandler.Read("machine_1.PLC.Inputs.ByNumber.0_to_99.2") <> "True" And sPT.IsCancellationRequested = False
                    Await Task.Delay(1000)
                Loop
#End Region
                If sPT.IsCancellationRequested = True Then GoTo fisBCMP
#Region "---Read Housing Barcode"
retryHousing:
                Func_Logging.Write("Housing Scan:" + vbLf)
                barCode_Housing = Await barCodeReader.Read751()
                Func_Logging.Write(barCode_Housing + vbLf)

                'Prompt and retry
                If barCode_Housing = Nothing Then
                    Func_Logging.Write("Failed To Read" + vbLf)

                    'See if they chose to rechose to retry or fail the part.
                    If MessageBox.Show("Failed to read Housing", "Scan Failure", MessageBoxButtons.RetryCancel) = DialogResult.Retry And sPT.IsCancellationRequested = False Then
                        GoTo retryHousing
                    Else
                        GoTo fisBCMP
                    End If
                End If
#End Region
                If sPT.IsCancellationRequested = True Then GoTo fisBCMP
                partPassFail = "PASS"
#Region "---Send PCB+Housing Barcode To FIS with Force measurements"
fisBCMP:
                'Send and get the FIS Message
                Func_Logging.Write("Sending FIS Message:" + vbLf + "BCMP|" + "status=" + partPassFail + "|PCB=" + barCode_PCB + "|Housing=" + barCode_Housing + vbLf)
                fisResponse = Func_FIS.Send("BCMP|" + "status=" + partPassFail +
                                            "|PCB=" + barCode_PCB +
                                            "|Housing=" + barCode_Housing +
                                            "|testres=" + dataFMax +
                                            "|ftestres=" + dataFMax)
                Func_Logging.Write("FIS Response:" + vbLf + fisResponse + vbLf)

                'See if they want to retry or give up.
                If (fisResponse.Contains("PASS") = False Or fisResponse.Contains("FAIL") = False) And fisResponse <> "ByPass" Then

                    'Write and ask if they want to retry FIS as it failed to respond.
                    Func_Logging.Write("FIS Failed to respond." + vbLf)
                    If MessageBox.Show("Part Failed FIS", "FIS Failure", MessageBoxButtons.RetryCancel) = DialogResult.Retry Then GoTo fisBCMP
                    partPassFail = "FAIL"
                End If
#End Region

#Region "---Display Result"
                If partPassFail = "PASS" Then 'Part Passed
                    Func_Logging.Write("Part Passed Successfully!" + vbLf)
                    ShowWI(My.Resources.PressGraphicResources.Step_Pass, TXT_WI_Pass)

                    'Increment counters and timers for a well built part.
                    guiForm.Lbl_Ind_LastCycleTime.Text = Int((DateTime.Now - startTime).TotalSeconds)
                    averageCycletime += Int(guiForm.Lbl_Ind_LastCycleTime.Text)
                    guiForm.Lbl_Ind_PartsPressed.Value += 1
                    guiForm.Lbl_Ind_AvgCycletime.Text = averageCycletime / guiForm.Lbl_Ind_PartsPressed.Value

                Else 'Part Failed
                    Func_Logging.Write("Part Failed UnSuccessfully!" + vbLf)
                    ShowWI(My.Resources.PressGraphicResources.Step_Fail, TXT_WI_Fail)
                End If
#End Region

                Func_Logging.SaveTXT(barCode_PCB)
                Func_Logging.ClearTXT()

                Await Task.Delay(5000) 'Small delay till next run
            Loop

            'Cleanup Indicators etc...
            guiForm.Lbl_Indicator_Status.Text = "Not Running"
            guiForm.Lbl_Indicator_Status.BackColor = SystemColors.ControlDark
        Catch ex As Exception
            System.IO.File.WriteAllText("C:\ErrorLog.txt", ex.ToString)
        End Try

    End Function

    'Function to just throw on image resources into a WI
    Public Sub ShowWI(imageResource As Bitmap, imageText As String)
retryGUI:
        Try
            guiForm.Picture_WI.BackgroundImage = imageResource
            guiForm.LBL_WI.Text = imageText
            GC.Collect()
        Catch ex As Exception
            GoTo retryGUI
        End Try
    End Sub

End Module
