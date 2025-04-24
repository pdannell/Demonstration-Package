Imports Sres.Net.EEIP
Imports System.Collections

'Requires the EEIP Nuget Package by Stafan Rossman.
Public Class LIB_EIP


#Region "---Variables, Properties and structures"
    ''' <summary>
    ''' Main Class for holding autodiscovery device data.
    ''' </summary>
    Public Class ResultsList
        Public productName As New List(Of String)
        Public ipAddress As New List(Of String)
        Public port As New List(Of Integer)
        Public vendorID As New List(Of Integer)
        Public typeCode As New List(Of Integer)
    End Class


    'Private library wide globals.
    Private eeipClient As New EEIPClient                            'Private handler for all subscribe/unsubscribe/read/write etc...
    Private eeipIP As String = Settings_EIP.Default.EIP_DefaultIP   'Private IP for holding value.
    Private eeipSubscriptionSet As Boolean = False                  'Tells us if we have subscribed to a data event or not.
    Private lastSubscriptionIP As String = ""                       'Tells us if the user tried to switch our subscription.

    'Getter/setter for user IP.
    Public Property IP() As String

        '-Return our current EEIP value.
        Get
            If eeipIP = "" Then Return "IP Not Set"
            Return eeipIP
        End Get

        '-Check and set new value.
        Set(value As String)

            'Check if the user is trying to use an invalid or non IPv4 string.
            Dim stringTester As String = value.Replace(".", "")
            If IsNumeric(stringTester) = False Then
                Throw New Exception("Tried to set invalid IP or IPV6 format IP on EIP value.")
                Exit Property
            End If

        End Set
    End Property
#End Region


    ''' <summary>
    ''' Handler that checks if we need a new EIP connection. Implicit and Explicit Supported.
    ''' </summary>
    Private Sub SubscriptionHandler()

        'Check if user tries to swap IP for a new EIP handler and unregister if so.
        If eeipIP <> lastSubscriptionIP And eeipSubscriptionSet = True Then
            eeipClient.UnRegisterSession()
        ElseIf eeipSubscriptionSet = True Then  'Check if we're still subscribed to the same EIP.
            Return
        End If

        eeipClient.RegisterSession(eeipIP)  'Subscribe

        lastSubscriptionIP = eeipIP         'Update our last used IP
        eeipSubscriptionSet = True          'Set that we're subscribed.
    End Sub

    ''' <summary>
    ''' Calling this function returns a listing of all discovered EIP devices, their data and 
    ''' </summary>
    ''' <returns>ResultsList Class Structure is returned to the user.</returns>
    Public Function DiscoverDevices() As ResultsList

        'Definitions For Discoverer
        Dim resultList As List(Of Encapsulation.CIPIdentityItem)

        'Result Definition
        Dim results As New ResultsList

        'Get listing of EEIP responsive devices.
        Try
            resultList = eeipClient.ListIdentity()
        Catch ex As Exception   'Failed, throw an exception as we're doomed.
            Throw New System.Exception("Screwdriver Motor Module Connection Failed: Stopping Station")
        End Try

        'Wait for all devices to be found(Discover lacks a proper finish event).
        For I As Integer = 0 To Settings_EIP.Default.EIP_DiscoveryTime / 100
            System.Threading.Thread.Sleep(100)
        Next

        'Grab each devices data and return it as a Tuple list.
        For Each device In resultList
            results.productName.Add(device.ProductName1)
            results.ipAddress.Add(Sres.Net.EEIP.Encapsulation.CIPIdentityItem.getIPAddress(device.SocketAddress.SIN_Address))
            results.port.Add(device.SocketAddress.SIN_port)
            results.vendorID.Add(device.ProductCode1)
            results.typeCode.Add(device.ItemTypeCode)
        Next

        Return results  'Return our Results

    End Function

    ''' <summary>
    ''' Function that reads back either All Bytes on an address, a word, or bit.
    ''' </summary>
    ''' <param name="dataInstanceAddress">Required - Device Instance Address(Returns all data if only this is filled out)</param>
    ''' <param name="dataWord">Optional - Word In Data String(Returns the entire word if only this is fialled out)</param>
    ''' <param name="bitOnWord">Optional - Bit to read from our word</param>
    ''' <returns>Data Requested As a String List of either All Bytes, Single Byte, Single Bit</returns>
    Public Function Explicit_ReadInputDigital(dataInstanceAddress As Integer, Optional dataWord As Integer = -1, Optional bitOnWord As Integer = -1) As List(Of String)

        'Results storage vars
        Dim results As New List(Of String)
        Dim instanceResponse As Byte() = {}

        'Create a new EEIPclient tied to the requested IP.
        SubscriptionHandler()

        'Read the selected InstanceAddress and throw an exception if it fails.
        Try
            instanceResponse = eeipClient.AssemblyObject.getInstance(dataInstanceAddress)
        Catch ex As Exception
            Throw New System.Exception("Failed to read from instance: " + ex.Message)
        End Try

        'Parse out the data we need to return based on our users input.
        If dataWord = -1 And bitOnWord = -1 Then
            For Each dataByte As Byte In instanceResponse
                results.Add(dataByte.ToString())
            Next
        ElseIf (dataWord <> -1 And bitOnWord = -1) Then
            results.Add(instanceResponse(dataWord).ToString())
        ElseIf (dataWord <> -1 And bitOnWord <> -1) Then
            Dim bitResponse As BitArray = New BitArray(New Byte() {instanceResponse(dataWord)})
            results.Add(bitResponse(bitOnWord).ToString())
        Else
            'Add messagebox shown for invalid code use.
        End If

        Return results 'Feedback our completed Data request.
    End Function

    ''' <summary>
    ''' Function that sets any series of data you request.
    ''' </summary>
    ''' <param name="dataInstanceAddress">Instance To Set</param>
    ''' <param name="setByteSeries">Series of bytes that corrosponds to each of the max supported bytes.</param>
    Public Sub Explicit_SetOutput(dataInstanceAddress As String, setByteSeries As Byte())

        'Create a new EEIPclient tied to the requested IP.
        SubscriptionHandler()

        'Attempt to set the series of bytes requested.
        Try
            eeipClient.AssemblyObject.setInstance(150, setByteSeries)
        Catch ex As Exception
            Throw New System.Exception("Failed to set instance value: " + ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Test area used only for internal debug.
    ''' </summary>
    Private Sub TestFunction()

        'Discovery example
        'Dim discoveryResult As New ResultsList
        'discoveryResult = DiscoverDevices()   'Automatically finds all devices that are related.

        'Read Registers Example
        'Dim response As List(Of String) = ReadInput(100, 2, 4)

        'Set output example.
        'SetOutput(0, 1)
    End Sub

End Class
