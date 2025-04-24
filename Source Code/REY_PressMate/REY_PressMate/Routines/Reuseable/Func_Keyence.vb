Imports Keyence.AutoID
Imports Keyence.AutoID.SDK
Imports REY_PressMate.REY_PressMate.Settings
Public Class Func_Keyence

    ''' <summary>
    ''' Value where we can set IP/Ports to read/write.
    ''' </summary>
    Public _reader751 As New ReaderAccessor With {
        .IpAddress = Settings_Keyence7XX.Default.Default_TCP_IP,
        .CommandPort = Settings_Keyence7XX.Default.Default_TCP_CommandPort,
        .DataPort = Settings_Keyence7XX.Default.Default_TCP_DataPort
    }

    Public _reader700 As New IO.Ports.SerialPort With {
        .BaudRate = Settings_Keyence7XX.Default.Default_RS232_BaudRate,
        .DataBits = Settings_Keyence7XX.Default.Default_RS232_DataBits,
        .Parity = System.IO.Ports.Parity.None,
        .StopBits = System.IO.Ports.StopBits.One,
        .PortName = Settings_Keyence7XX.Default.Default_RS232_PortName}

    Private readerResponse As String    'Internal handler for data we get back from the scanner during read events.

    ''' <summary>
    ''' Main call to try a read attempt(Self Handled)
    ''' </summary>
    ''' <returns>Barcode read value.</returns>
    Public Async Function Read751() As Task(Of String)

        'Function Locals
        Dim readCounter As Integer = 0
        Dim readString = ""
        readerResponse = ""

        Try '---Try to connect
            _reader751.Connect(AddressOf dataRecieved)
        Catch ex As Exception
            MessageBox.Show("Failed to connect to SR75X, check connection.")
        End Try

        'Turn on reader and try to read any non blank value for readtimeout.
        For i As Integer = 0 To Settings_Keyence7XX.Default.ReadTimeout Step Settings_Keyence7XX.Default.TimeBetweenReads

            'Check that we read something, if we did exit the loop.
            readString = _reader751.ExecCommand("LON")

            If readString = "OK,LON" & vbCr And readerResponse <> "" Then Exit For
            Await Task.Delay(Settings_Keyence7XX.Default.TimeBetweenReads)
        Next

        '---Turn off reader
        _reader751.ExecCommand("LOFF")

        '---Dispose of our resources
        _reader751.Disconnect()
        _reader751.Dispose()

        Return readerResponse  'Return the value we read.

    End Function

    'Read Event handler for data recieved from the scanner.
    Private Sub dataRecieved(data() As Byte)
        readerResponse = System.Text.Encoding.ASCII.GetString(data).Replace("2:", "").Replace(vbCr, "")
    End Sub



    ''' <summary>
    ''' Read Data from SR751 after a scan.
    ''' </summary>
    ''' <returns>Read Value</returns>
    Public Async Function Read700() As Task(Of String)

        'Function Locals
        Dim readCounter As Integer = 0
        Dim readString = ""

        Try '---Attempt to connect
            _reader700.Open()
        Catch ex As Exception
            MessageBox.Show("Keyence serial port appears to be unavailable")
        End Try

        'Turn on reader and try to read any non blank value for readtimeout.
        For i As Integer = 0 To Settings_Keyence7XX.Default.ReadTimeout Step Settings_Keyence7XX.Default.TimeBetweenReads

            'Check that we read something, if we did exit the loop.
            _reader700.Write("LON" + vbCr)
            readString = _reader700.ReadExisting
            If readString <> "" Then Exit For

            Await Task.Delay(Settings_Keyence7XX.Default.TimeBetweenReads)
        Next

        '---Turn off reader
        _reader700.Write("LOFF" + vbCr)

        '---Dispose of our resources
        _reader700.Close()
        _reader700.Dispose()

        Return readString.Replace(vbCr, "")  'Return the value we read.

    End Function

End Class
