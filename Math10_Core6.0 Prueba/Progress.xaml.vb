Imports System.Threading
Public Class Progress
    Public bCancelled As Boolean = False
    Private Sub BtCancelarProceso_Click(
             ByVal sender As System.Object,
             ByVal e As System.EventArgs) Handles BTNCancel.Click
        Me.Close()
    End Sub

    Private Sub BTNCancel_Click(sender As Object, e As Windows.RoutedEventArgs)
        Try
            bCancelled = True
        Catch ex As Exception

        End Try
    End Sub
End Class
