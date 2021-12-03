Imports PCSC
Imports PCSC.Monitoring

Public Class Form1

    Public oMonitorFactory
    Public oMonitor As ISCardMonitor

    Private Sub oMonitor_StatusChanged(sender As Object, e As StatusChangeEventArgs)
        'MsgBox(e.NewState)

    End Sub

    Private Sub oMonitor_CardInserted(sender As Object, e As CardStatusEventArgs)
        SetText(lbl_status, "card inserita")
    End Sub

    Private Sub oMonitor_CardRemoved(sender As Object, e As CardStatusEventArgs)

        SetText(lbl_status, "card rimossa o in attesa carta")
    End Sub

    Private Sub SetText(ByVal ctl As Control, ByVal cText As String)
        If ctl.InvokeRequired Then
            ctl.BeginInvoke(New Action(Of Control, String)(AddressOf SetText), ctl, cText)
        Else
            ctl.Text = cText
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim oContextFactory = ContextFactory.Instance
        Dim oReaders As String()

        oMonitorFactory = MonitorFactory.Instance
        oMonitor = oMonitorFactory.Create(SCardScope.System)

        AddHandler oMonitor.StatusChanged, AddressOf oMonitor_StatusChanged
        AddHandler oMonitor.CardInserted, AddressOf oMonitor_CardInserted
        AddHandler oMonitor.CardRemoved, AddressOf oMonitor_CardRemoved


        Using oContext As ISCardContext = oContextFactory.Establish(SCardScope.System)
            oReaders = oContext.GetReaders

            If oReaders.Length > 1 Then
                For Each readername In oReaders
                    lbl_status.Text = "inserire card nel lettore " & readername
                Next
            Else
                lbl_status.Text = "inserire card nel lettore " & oReaders(0)
            End If

            oMonitor.Start(oReaders(0))
            'oScCardMonitor.Start(oReaders(0))

            'Dim oCardReader As ICardReader = oContext.ConnectReader(oReaders(0), SCardShareMode.Shared, SCardProtocol.Any)

            'MsgBox(oCardReader.GetStatus.Protocol)



            'oCardReader.Disconnect(SCardReaderDisposition.Reset)
            'oCardReader.Dispose()

            MsgBox(oMonitor.GetCurrentStateValue(0))




        End Using




    End Sub

    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        oMonitor.Cancel()
        oMonitor.Dispose()
    End Sub
End Class
