Imports System.Text
Imports System.Windows.Forms

Public Class Record

    Dim vRet2HTML(), vRet3(), vRetML() As String
    Dim lbRet2 As New ListBox
    Dim w As MainWindow
    Public Sub New(w As MainWindow, sHistResult2() As String, sHistResult3() As String)
        Try
            InitializeComponent()
            vRet2HTML = sHistResult2 ' HTML
            vRet3 = sHistResult3 ' text
            Me.w = w
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Record_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Try
            For i As Int32 = 0 To vRet3.Length - 1
                'Dim item As New Object
                'item.Content = Replace(vRet3(i), vbCrLf, " | ")
                'item.Height = 20
                'lbRet3.Items.Add(item)
                lbRet3.Items.Add(Replace(vRet3(i), vbCrLf, " | "))
            Next
            For i As Int32 = 0 To vRet2HTML.Length - 1
                'Dim item As New Object ' ListBoxItem
                'item.Content = vRet2HTML(i)
                'item.Height = 20
                'lbRet2.Items.Add(item)
                lbRet2.Items.Add(vRet2HTML(i))
            Next
        Catch ex As Exception

        End Try
    End Sub


    Private Sub Up_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Try
            Dim i As Int32
            For Each o In lbRet3.SelectedItems
                i = lbRet3.Items.IndexOf(o)
            Next
            If i > 0 Then
                Dim o As Object = lbRet3.Items(i)
                Dim o1 As Object = lbRet3.Items(i - 1)
                If i < lbRet2.Items.Count Then
                    o = lbRet2.Items(i)
                    o1 = lbRet2.Items(i - 1)
                    lbRet2.Items.RemoveAt(i)
                    lbRet2.Items.RemoveAt(i - 1)
                    lbRet2.Items.Insert(i - 1, o)
                    lbRet2.Items.Insert(i, o1)
                    lbRet2.SelectedItem = o
                End If
                If i < lbRet3.Items.Count Then
                    o = lbRet3.Items(i)
                    o1 = lbRet3.Items(i - 1)
                    lbRet3.Items.RemoveAt(i)
                    lbRet3.Items.RemoveAt(i - 1)
                    lbRet3.Items.Insert(i - 1, o)
                    lbRet3.Items.Insert(i, o1)
                    lbRet3.SelectedItem = o
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Down_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Try
            Dim i As Int32
            For Each o In lbRet3.SelectedItems
                i = lbRet3.Items.IndexOf(o)
            Next
            If i < lbRet3.Items.Count - 1 Then
                Dim o As Object = lbRet3.Items(i + 1)
                Dim o1 As Object = lbRet3.Items(i)
                If i < lbRet2.Items.Count Then
                    o = lbRet2.Items(i + 1)
                    o1 = lbRet2.Items(i)
                    lbRet2.Items.RemoveAt(i + 1)
                    lbRet2.Items.RemoveAt(i)
                    lbRet2.Items.Insert(i, o)
                    lbRet2.Items.Insert(i + 1, o1)
                    lbRet2.SelectedItem = o1
                End If
                If i < lbRet3.Items.Count Then
                    o = lbRet3.Items(i + 1)
                    o1 = lbRet3.Items(i)
                    lbRet3.Items.RemoveAt(i + 1)
                    lbRet3.Items.RemoveAt(i)
                    lbRet3.Items.Insert(i, o)
                    lbRet3.Items.Insert(i + 1, o1)
                    lbRet3.SelectedItem = o1
                End If

            End If
        Catch ex As Exception

        End Try

    End Sub


    Private Sub Del_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Try
            Dim iv(-1) As Int32, i As Int32
            For Each o In lbRet3.SelectedItems
                ReDim Preserve iv(i)
                iv(i) = lbRet3.Items.IndexOf(o)
                i += 1
            Next
            For i = iv.Length - 1 To 0 Step -1
                lbRet2.Items.RemoveAt(iv(i))
                lbRet3.Items.RemoveAt(iv(i))
            Next

        Catch ex As Exception

        End Try
    End Sub

    Private Sub Update_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Try
            Dim i As Int32 = lbRet2.Items.Count - 1
            ReDim vRet2HTML(i), vRet3(i), vRetML(i)
            For i = 0 To lbRet2.Items.Count - 1
                Dim o As Object = lbRet2.Items(i) ' .content
                o = lbRet2.Items(i) ' .content
                vRet2HTML(i) = o
                o = lbRet3.Items(i) '.content
                vRet3(i) = o
            Next
            w.setsTbQrysTbvar(vRet2HTML, vRet3)
            Me.Close()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub SelectionChanged(sender As Object,
        e As Windows.Controls.SelectionChangedEventArgs) Handles lbRet3.SelectionChanged
        Try
            Dim src As ListBox = CType(e.Source, ListBox)
            Dim i As Int32 = src.SelectedIndex
            Dim name As String = LCase(src.Name)
            If name <> "lbret2" Then
                lbRet2.SelectedIndex = i
            End If
            If name <> "lbret3" Then
                lbRet3.SelectedIndex = i
            End If
        Catch ex As Exception

        End Try

    End Sub
End Class
