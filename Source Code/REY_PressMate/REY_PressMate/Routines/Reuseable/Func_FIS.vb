Imports System.Net.Http
Imports System.Net.Sockets
Imports REY_PressMate.REY_PressMate.Settings

Public Module Func_FIS

#Region "Function Locals"
    Private _tcpHandler As TcpClient                        'Client
    Private _tcpMessageHandler As NetworkStream             'DataHandler
    Private _port As Integer = Settings_FIS.Default.Port    'Port
    Private _iP As String = Settings_FIS.Default.IP         'IP To Use
    Public _testResponse As String = "ByPass"               'Default response for Static Testing.
#End Region

    ''' <summary>
    ''' Connect Autohandler
    ''' </summary>
    Private Sub Connect()

        'Exit if we were falsely called.
        If _tcpHandler IsNot Nothing Then
            If _tcpHandler.Connected = True Then Exit Sub
        End If


        'Call a new client
        _tcpHandler = New TcpClient(_iP, _port)
        Try 'Try to connect
        Catch ex As Exception ' Display error.
            Throw New System.Exception("Failed to connect to FIS - " + ex.Message.ToString())
        End Try

        _tcpMessageHandler = _tcpHandler.GetStream()  'Set stream data

        'Set connection properties.
        _tcpHandler.NoDelay = Settings_FIS.Default.NoDelay
        _tcpHandler.ReceiveTimeout = Settings_FIS.Default.RecieveTimeout
        _tcpHandler.SendTimeout = Settings_FIS.Default.SendTimeout

    End Sub

    ''' <summary>
    ''' Send message to FIS Server and return any response they send.
    ''' </summary>
    ''' <param name="msg">Accepts any string to send.</param>
    ''' <returns>Returns FIS response to our said message</returns>
    Public Function Send(msg As String) As String

        If Settings_FIS.Default.Enabled = False Then Return _testResponse   'if FIS is disabled hand back our bypass response.

        'Data handler and response for our call.
        Dim data As Byte()
        Dim fisResponse As String

        'Format our message for sending over our network.
        data = System.Text.Encoding.ASCII.GetBytes(msg + vbLf)

        'Check that we're still connected and reconnect if needed or if it was never initialized.
        If _tcpHandler Is Nothing Then
            Connect()
        ElseIf _tcpHandler.Connected = False Then
            Connect()
        End If

        Try 'Attempt to send data.
            _tcpMessageHandler.Write(data, 0, data.Length)
        Catch ex As Exception   'Throw exception for an error.
            Throw New System.Exception("Failed to send to Mini Robot - " + ex.Message.ToString())
        End Try

        'Wait a delay/Read Response/Cleanup/Return Answer
        System.Threading.Thread.Sleep(Settings_FIS.Default.SendRecieveDelay)
        fisResponse = Read()
        Disconnect()
        Return fisResponse

    End Function

    ''' <summary>
    ''' Read a response from FIS(Called after a send)
    ''' </summary>
    ''' <returns>Returns anything we see on the dataport from FIS.</returns>
    Private Function Read() As String

        'Initialize our Robot response.
        Dim response As String = ""
        Dim Count As Integer = 0

        Try 'Read till our robot says it has finished.
            Do While response.Contains(vbCrLf) = False 'Read string and count till we see a  vbnullchar for the response
                response += Chr(_tcpMessageHandler.ReadByte())
                Count = System.Text.RegularExpressions.Regex.Split(response, vbNullChar.ToString).Length - 1
                System.Threading.Thread.Sleep(10)
            Loop
        Catch ex As Exception
            Throw New System.Exception("FIS failed to send back data - " + ex.Message.ToString())
        End Try

        Return response 'Return the response

    End Function

    ''' <summary>
    ''' Closes connection and cleans up resources used.
    ''' </summary>
    Private Sub Disconnect()
        _tcpHandler.Close()
        _tcpHandler.Dispose()
    End Sub

End Module
