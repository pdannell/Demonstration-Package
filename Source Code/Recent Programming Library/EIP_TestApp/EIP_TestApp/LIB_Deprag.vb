Imports EIP_TestApp.Settings_Deprag

''' <summary>
''' Wrapper for All Deprag based modules. Requires LIB_EIP as a dependency when in use.
''' </summary>
Public Class LIB_Deprag     'Deprag wrapper that requires LIP_EIP to function for some applications.

#Region "--- Start of all progrmaming related to the Deprag ESFM Module(Up Down Z Axis on Screwdriver)"
    'Class that should be defined anytime we want to use the SFM module.
    Public Class SFM_Module

        Private sfmClient As New LIB_EIP    'Handler for all EIP calls.

        'Structure for holding all read only results.
        Public Class SFM_DepragResults
            Public ProgramNumber As Integer
            Public AxisMoving As Boolean
            Public MotorActive As Boolean
            Public RefPositionReached As Boolean
            Public SuperVisionRange As Boolean
            Public EStopped As Boolean
            Public AlarmOn As Boolean
            Public ControllerReady As Boolean
            Public CurrentPosition As Double    '0.01mm in Hex
            Public CurrentSpeed As Integer      '1mm/s in Hex
            Public CurrentThrust As String      'Unknown Conversion
            Public TargetPosition As String
            Public Alarm1 As String
            Public Alarm2 As String
            Public Alarm3 As String
            Public Alarm4 As String
        End Class

        'Enumeration used to determine which program to run.
        Enum SFM_Command
            Program             'Tells us to run a program(All Inclusive)
            HoldPosition        'Lets us toggle the hold position on/off
            MotorStart          'Lets us turn the motor on/off
            Reset               'Resets the SFM
            ReferenceRun        'Causes the reference program to run.
            UpJog               'Absolutely Not
            DownJog             'Absolutely Not
            FLGH                'What is this?
        End Enum

        ''' <summary>
        ''' Function which reads all the motor status state bits and feeds them back into a SFM_DepragResults array.
        ''' </summary>
        ''' <returns>SFM_DepragResults array</returns>
        Public Function SFM_ReadInputs() As SFM_DepragResults

            Dim results As New SFM_DepragResults    'Holder for reading all our data into.
            Dim programString As String = ""         'Binary string
            sfmClient.IP = Settings_Deprag.Default.SFM_IP   'Set our IP for use(Can update settings file to run multiple as the handler will autohandle each if a new class isn't made)

            Try

#Region "-Grab our Word 0 Results"
                'Read all the data from Word 0, byte 0 and 1
                For J As Integer = 0 To 1
                    For I As Integer = 0 To 7

                        'Parse out our result to a binary string.
                        If J = 0 And I <= 5 Then
                            If sfmClient.Explicit_ReadInputDigital(100, J, I)(0).ToString = True Then
                                programString += "1"
                            Else
                                programString += "0"
                            End If
                        End If

                        'Grab our Axis moving result.
                        If J = 1 And I <= 0 Then results.AxisMoving = sfmClient.Explicit_ReadInputDigital(100, J, I)(0).ToString

                        'Grab our Motor Active result.
                        If J = 1 And I <= 1 Then results.MotorActive = sfmClient.Explicit_ReadInputDigital(100, J, I)(0).ToString

                        'Grab our Axis moving result.
                        If J = 1 And I <= 2 Then results.RefPositionReached = sfmClient.Explicit_ReadInputDigital(100, J, I)(0).ToString

                        'Grab our Axis moving result.
                        If J = 1 And I <= 3 Then results.TargetPosition = sfmClient.Explicit_ReadInputDigital(100, J, I)(0).ToString

                        'Grab our Axis moving result.
                        If J = 1 And I <= 4 Then results.SuperVisionRange = sfmClient.Explicit_ReadInputDigital(100, J, I)(0).ToString

                        'Grab our Axis moving result.
                        If J = 1 And I <= 6 Then results.EStopped = sfmClient.Explicit_ReadInputDigital(100, J, I)(0).ToString

                        'Grab our Axis moving result.
                        If J = 1 And I <= 7 Then results.AlarmOn = sfmClient.Explicit_ReadInputDigital(100, J, I)(0).ToString

                    Next
                Next

                'Add results from above to the results array.
                'Convert our program selection from Binary to Decimal
                Dim nexp As Long = 0
                Dim digit As String

                For n = Len(programString) To 1 Step -1
                    digit = Mid(programString, n, 1)
                    results.ProgramNumber = results.ProgramNumber + (CInt(digit) * (2 ^ nexp))
                    nexp = nexp + 1
                Next n
#End Region

                'Grab our Bit from Word 1, Bit 4 - Controller Ready Bit
                results.ControllerReady = sfmClient.Explicit_ReadInputDigital(100, 2, 4)(0).ToString

            Catch ex As Exception
                Throw New System.Exception("Error Occured When Reading SFM Input: " + ex.Message)
            End Try

            Return results
        End Function


        ''' <summary>
        ''' Main run program command function that autohandles the motor to do stuff.
        ''' </summary>
        ''' <param name="command">Use class built in ENUM for this.</param>
        ''' <param name="programNumber">Tells us which program to use when using Enum.Program</param>
        ''' <param name="onOff">Tells us the state to set on/off based items to such as Motor On/Off</param>
        Public Sub SFM_RunProgram(command As Integer, Optional programNumber As Integer = Nothing, Optional onOff As Boolean = Nothing)

            Dim motorStatus As New SFM_DepragResults    'Function call for reading inputs as we go along.

            'Fill our commands byte structure with 36 bytes for whatever reason(32 incorrectly defined in customer documentation).
            Dim databyte(35) As Byte
            For I = 0 To databyte.Length - 1
                databyte(I) = 0
            Next

            'Command hander which can either run a routine or prep a message for our send that occurs after.
            Select Case command
                Case SFM_Module.SFM_Command.Program

                    'Check for false program calls.
                    If programNumber < 0 Or programNumber > 30 Then
                        Throw New System.Exception("Error, Invalid program selected.")
                    End If

                    'Read our motor inputs then reset it if there is an error followed by waiting some time.
                    motorStatus = SFM_ReadInputs()
                    If motorStatus.AlarmOn = True Then
                        databyte(0) = 0                                 'Program Selection
                        databyte(1) = 8                                 'Command

                        sfmClient.Explicit_SetOutput(150, databyte)     'Send Command

                        databyte(0) = 0                                 'Program Selection
                        databyte(1) = 0                                 'Command
                        sfmClient.Explicit_SetOutput(150, databyte)     'Send Command
                        System.Threading.Thread.Sleep(20)
                    End If

                    databyte(0) = programNumber                     'Program Selection
                    databyte(1) = 2                                 'Command
                    sfmClient.Explicit_SetOutput(150, databyte)     'Send Command
                    System.Threading.Thread.Sleep(10)               'Required wait time for the SFM motor to actuate based on hardware spec.
                    databyte(1) = 6                                 'Command

                Case SFM_Module.SFM_Command.HoldPosition
                    If onOff = True Then
                        databyte(0) = 0                                 'Program Selection
                        databyte(1) = 1                                 'Command
                    ElseIf onOff = False Then
                        databyte(0) = 0                                 'Program Selection
                        databyte(1) = 0                                 'Command
                    Else
                        Throw New System.Exception("Error, onOff bit was not set for command")
                    End If

                Case SFM_Module.SFM_Command.MotorStart
                    If onOff = True Then
                        databyte(0) = 0                                 'Program Selection
                        databyte(1) = 4                                 'Command
                    ElseIf onOff = False Then
                        databyte(0) = 0                                 'Program Selection
                        databyte(1) = 0                                 'Command
                    Else
                        Throw New System.Exception("Error, onOff bit was not set for command")
                    End If

                Case SFM_Module.SFM_Command.Reset
                    databyte(0) = 0                                 'Program Selection
                    databyte(1) = 8                                 'Command

                    sfmClient.Explicit_SetOutput(150, databyte)     'Send Command

                    databyte(0) = 0                                 'Program Selection
                    databyte(1) = 0                                 'Command
                Case SFM_Module.SFM_Command.ReferenceRun
                    databyte(0) = 0                                 'Program Selection
                    databyte(1) = 18                                'Command
                Case SFM_Module.SFM_Command.UpJog
                    Throw New System.Exception("Error, user tried to use banned command.")
                Case SFM_Module.SFM_Command.DownJog
                    Throw New System.Exception("Error, user tried to use banned command.")
                Case SFM_Module.SFM_Command.FLGH
                    Throw New System.Exception("Error, user tried to use unknown use command.")
                Case Else
                    Throw New System.Exception("Invalid command.")
            End Select

            sfmClient.Explicit_SetOutput(150, databyte)     'Send Command

            'If a program movement was commanded, we will wait and monitor for completion.
            If command = SFM_Command.Program Then

                'Grab motor status results and keep looping till we see our target was reached.
                Do While motorStatus.AxisMoving = True Or motorStatus.RefPositionReached = False
                    motorStatus = SFM_ReadInputs()
                    If motorStatus.AlarmOn = True Then Throw New System.Exception("Alarm triggered for SFM module during movement.")
                    System.Threading.Thread.Sleep(10)
                    'If this is inside the microbase application, use Application.DoEvents here due to poor Threading/Architecture.
                Loop

            End If


        End Sub
    End Class
#End Region

End Class
