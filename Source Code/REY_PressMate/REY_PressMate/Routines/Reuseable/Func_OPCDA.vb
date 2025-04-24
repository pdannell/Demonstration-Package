'OPC DA Library functions for use.
'This is utilizing the godsharp OPC Nuget Package as its baseline.
'Documentation can be found here : https://github.com/godsharp/opcsample
Imports GodSharp.Opc.Da
Imports GodSharp.Opc.Da.Options
Imports REY_PressMate.REY_PressMate.Settings

Public Class Func_OPCDA

#Region "---Class Locals/Definitions"
    ''' <summary>
    ''' Placeholder List of Groups
    ''' </summary>
    Private _dataGroups = New List(Of GroupData)

    ''' <summary>
    ''' Server data class for IP/ProgramName.
    ''' </summary>
    ''' <remarks>Modify host if you want to change the server I.P Post Run</remarks>
    ''' <remarks>Modify ProgID if you want to change virtual server</remarks>
    ''' <remarks>Don't touch .Groups or bad things happen</remarks>
    Public _server As New ServerData With {
                .Host = Settings_OPCDA.Default.Default_Server_IP,                 'ServerIP
                .ProgId = Settings_OPCDA.Default.Default_Server_ProgramName,      'Program Name - Important
                .Groups = _dataGroups                                          'Groups we're making available which contain tags to read.
            }

    ''' <summary>
    ''' 'Client options, events and server settings.
    ''' </summary>
    Private _clientOptions As New DaClientOptions With {
        .Data = _server,
        .OnDataChangedHandler = AddressOf OnDataChangedHandler,
        .OnServerShutdownHandler = AddressOf OnShutdownHandler,
         .OnAsyncReadCompletedHandler = AddressOf OnAsyncReadCompletedHandler,
        .OnAsyncWriteCompletedHandler = AddressOf OnAsyncWriteCompletedHandler
        }

    ''' <summary>
    ''' Client used for autohandling.
    ''' </summary>
    Private _client As OpcNetApiClient


    ''' <summary>
    ''' This value is used as a return when we have the OPCDA function disabled for test purposes.
    ''' </summary>
    Public _testValue As String = "ByPass"


#End Region

    ''' <summary>
    ''' Connect function which autohandles connections.
    ''' </summary>
    Private Sub Connect()

        'Define our client for first time use if it is currently null or connect if we aren't already connected.
        If _client Is Nothing Then              'Check for null
            Try
                _client = DaClientFactory.Instance.CreateOpcNetApiClient(_clientOptions)
                _client.Connect()
            Catch ex As Exception
                MessageBox.Show("Error connecting to OPC Server")
            End Try
        ElseIf _client.Connected = False Then   'Check if we're connected
            Try
                _client.Connect()
            Catch ex As Exception
                MessageBox.Show("Error connecting to OPC Server")
            End Try
        End If
    End Sub


#Region "---Event Handlers - These are only used if we want a continuous data feedback and instantaneous unrequested responses."


    Private Shared Sub OnDataChangedHandler(ByVal output As DataChangedOutput)
    End Sub

    Private Shared Sub OnAsyncReadCompletedHandler(ByVal output As AsyncReadCompletedOutput)
    End Sub

    Private Shared Sub OnAsyncWriteCompletedHandler(ByVal output As AsyncWriteCompletedOutput)
    End Sub

    Private Shared Sub OnShutdownHandler(ByVal server As Server, ByVal reason As String)
    End Sub
#End Region


    ''' <summary>
    ''' Read from any given Tag.
    ''' </summary>
    ''' <param name="tagName">Name of tag you want to read.</param>
    ''' <returns>Return value from said tag request</returns>
    Public Function Read(tagName As String) As String
readRetry:
        If Settings_OPCDA.Default.Enabled = False Then Return _testValue 'Return a bypass value for dry runs.

        Dim readValue As String = Nothing 'Value we read from the OPC server and used in the return for it.

        '---Check if we are currently connected or not, if we're not run our connection function.
        If _client Is Nothing Then
            Connect()
        ElseIf _client.Connected = False Then
            Connect()
        End If

        '---Add Tags/Groups to Read.
        '-Add Group for Reading
        If _client.CurrentGroupName <> "default" Then
            _client.Add(New Group With {
                        .Name = "default",
                        .UpdateRate = 100,
                        .ClientHandle = 10})
        End If

        '-Add Tag for Reading to the group.
        If _client.Groups("default").Tags.ContainsKey(tagName) = False Then
            _client.Groups("default").Add(New Tag(tagName, 1))
        End If

        For i As Integer = 0 To _client.Groups.Values.Count - 1
            For Each Tag In _client.Groups.Values(i).Tags
                If Tag.Value.ItemName = tagName Then
                    Dim results = _client.Groups.Values(i).Read(Tag.Value.ItemName)
                    readValue = results.Result.Value.ToString
                End If
            Next
        Next

        If readValue = Nothing Then GoTo readRetry
        Return readValue.ToString

    End Function

    ''' <summary>
    ''' Write to any given tag by name and what you want.
    ''' </summary>
    ''' <param name="tagName">Any given tag name.</param>
    ''' <param name="writeValue">Object that should handle any realistic tag type.</param>
    Public Sub Write(tagName As String, writeValue As Object, valueType As String)

        If Settings_OPCDA.Default.Enabled = False Then Exit Sub 'Just exit the function for dry runs if set true.

        '---Check if we are currently connected or not, if we're not run our connection function.
        If _client Is Nothing Then
            Connect()
        ElseIf _client.Connected = False Then
            Connect()
        End If

        '---Add Tags/Groups to Read.
        '-Add Group for Reading
        If _client.CurrentGroupName <> "default" Then
            _client.Add(New Group With {
                        .Name = "default",
                        .UpdateRate = 100,
                        .ClientHandle = 10})
        End If

        '-Add Tag for Reading to the group.
        If _client.Groups("default").Tags.ContainsKey(tagName) = False Then
            _client.Groups("default").Add(New Tag(tagName, 1))
        End If

        '---Write all Tags Block
        Select Case valueType
            Case "Integer"
                For i As Integer = 0 To _client.Groups.Values.Count - 1
                    For Each Tag In _client.Groups.Values(i).Tags
                        If Tag.Value.ItemName = tagName Then
                            Dim results = _client.Groups.Values(i).Write(Tag.Value.ItemName, writeValue)
                        End If
                    Next
                Next
            Case "String"
                For i As Integer = 0 To _client.Groups.Values.Count - 1
                    For Each Tag In _client.Groups.Values(i).Tags
                        If Tag.Value.ItemName = tagName Then
                            Dim results = _client.Groups.Values(i).Write(Tag.Value.ItemName, writeValue)
                        End If
                    Next
                Next
        End Select

        '---Dispose of our resources.
        'Disconnect()

    End Sub

    ''' <summary>
    ''' Autohandler that disposes of our connection when not needed.
    ''' </summary>
    Private Sub Disconnect()

        'Free our resources after disconnection.
        _client.RemoveAll()
        _client.Remove("default")
        _client.Disconnect()
        GC.Collect()

    End Sub


End Class
