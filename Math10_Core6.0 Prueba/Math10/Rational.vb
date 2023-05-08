Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Security
Imports System.Globalization

Public Class Rational

    Public resto As New List(Of Term_2)
    Public divisor As New List(Of Term_2)
    Friend Shared bReduce As Boolean = True

    Public Sub New()
    End Sub
    Public Sub New(dbl As Double)
        resto.Add(New Term_2(dbl))
        divisor.Add(New Term_2(1.0))
    End Sub
    Public Sub New(t As Term_2)
        resto.Add(New Term_2(t))
        divisor.Add(New Term_2(1.0))
    End Sub
    Public Sub New(Term_2List As List(Of Term_2))
        For Each t In Term_2List
            resto.Add(New Term_2(t))
        Next
        divisor.Add(New Term_2(1.0))
    End Sub

    Public Sub New(db As Double, exp As Double)
        resto.Add(New Term_2(db, exp))
        divisor.Add(New Term_2(1.0))
    End Sub
    Public Sub New(rtnA As Rational)
        Try
            For i As Int32 = 0 To rtnA.resto.Count - 1
                resto.Add(New Term_2(rtnA.resto(i)))
            Next
            For i As Int32 = 0 To rtnA.divisor.Count - 1
                divisor.Add(New Term_2(rtnA.divisor(i)))
            Next
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Public Shared Function CopyFrom(rtnA As Rational) As Rational
        Dim eC As New Rational
        Try
            For i As Int32 = 0 To rtnA.resto.Count - 1
                eC.resto.Add(New Term_2(rtnA.resto(i)))
            Next
            For i As Int32 = 0 To rtnA.divisor.Count - 1
                eC.divisor.Add(New Term_2(rtnA.divisor(i)))
            Next
        Catch ex As Exception
            Throw
        End Try
        Return eC
    End Function
    Public Shared Function CopyFrom(lstT As List(Of Term_2)) As List(Of Term_2)
        Dim eClst As New List(Of Term_2)
        Try
            For i As Int32 = 0 To lstT.Count - 1
                eClst.Add(New Term_2(lstT(i)))
            Next
        Catch ex As Exception
            Throw
        End Try
        Return eClst
    End Function
    Public Sub CopyToMe(rtnA As Rational)
        Try
            resto.Clear()
            divisor.Clear()
            For i As Int32 = 0 To rtnA.resto.Count - 1
                resto.Add(New Term_2(rtnA.resto(i)))
            Next
            For i As Int32 = 0 To rtnA.divisor.Count - 1
                divisor.Add(New Term_2(rtnA.divisor(i)))
            Next
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Public ReadOnly Property Num() As Double
        Get
            Dim N As Double = 0.0
            For i As Int32 = 0 To resto.Count - 1
                N += resto(i).ToDouble
            Next
            Return N
        End Get
    End Property
    Public ReadOnly Property Den() As Double
        Get
            Dim D As Double = 0.0
            For i As Int32 = 0 To divisor.Count - 1
                D += divisor(i).ToDouble
            Next
            Return D
        End Get
    End Property
    Public ReadOnly Property Abs() As Double
        Get
            Return Math.Abs(ToDouble)
        End Get
    End Property
    Public Function IsZero() As Double
        Return (ToDouble() = 0.0)
    End Function
    Public Function IsRational() As Boolean
        Return divisor.Count = 1 AndAlso divisor(0).IsReal _
            AndAlso resto.Count = 1 AndAlso resto(0).IsReal
    End Function
    Public Sub OpChgSgn()
        For i = 0 To resto.Count - 1
            resto(i).OpChgSign()
        Next
    End Sub
    Public Function Sign() As Int32
        Return Math.Sign(ToDouble)
    End Function
    Public Sub OpChgSign()
        For i As Int32 = 0 To resto.Count - 1
            resto(i).OpChgSign()
        Next
    End Sub
    Public Function ToDouble() As Double
        Dim N As Double = 0.0
        For i As Int32 = 0 To resto.Count - 1
            N += resto(i).ToDouble
        Next
        Dim D As Double = 0.0
        For i As Int32 = 0 To divisor.Count - 1
            D += divisor(i).ToDouble
        Next
        Return N / D
    End Function
    Public Shared Operator =(eA As Rational, eB As Rational) As Boolean
        Return (eA.ToDouble = eB.ToDouble)
    End Operator
    Public Shared Operator <>(eA As Rational, eB As Rational) As Boolean
        Return Not (eA.ToDouble = eB.ToDouble)
    End Operator
    Public Shared Operator <(eA As Rational, eB As Rational) As Boolean
        Return Not (eA.ToDouble < eB.ToDouble)
    End Operator
    Public Shared Operator >(eA As Rational, eB As Rational) As Boolean
        Return Not (eA.ToDouble > eB.ToDouble)
    End Operator


    Public Shared Operator -(eA As Rational) As Rational
        Dim eC As Rational = Rational.CopyFrom(eA)
        eC.OpChgSign()
        Return eC
    End Operator
    Public Shared Operator -(eA As Rational, eB As Rational) As Rational
        Dim eC As Rational = Rational.CopyFrom(eB)
        Try
            eC.OpChgSign()
            eC = eA + eC
        Catch ex As Exception
            Throw
        End Try
        Return eC
    End Operator
    Public Shared Operator +(eA As Rational, eB As Rational) As Rational
        Return Add(eA, eB)
    End Operator
    Public Shared Operator +(eA As Rational, db As Double) As Rational
        Return Add(eA, New Rational(db))
    End Operator
    Public Shared Operator +(db As Double, eB As Rational) As Rational
        Return Add(New Rational(db), eB)
    End Operator
    Public Shared Function Substract(eAT As List(Of Term_2), eBT As List(Of Term_2)) As List(Of Term_2)
        Dim eCT As New List(Of Term_2)
        Try
            eCT.AddRange(eAT)
            For i As Int32 = 0 To eBT.Count - 1
                Dim t As New Term_2(eBT(i))
                t.OpChgSign()
                eCT.Add(t)
            Next
        Catch ex As Exception
            Throw
        End Try
        Return eCT
    End Function
    Public Shared Function Add(eAT As List(Of Term_2), eBT As List(Of Term_2)) As List(Of Term_2)
        Dim eCT As New List(Of Term_2)
        Try
            eCT.AddRange(eAT)
            eCT.AddRange(eBT)
            Dim ec As New Rational(eCT)
            ec.Reduce()
            eCT = ec.resto
        Catch ex As Exception
            Throw
        End Try
        Return eCT
    End Function
    Private Shared Function Add(eA As Rational, eB As Rational) As Rational
        Dim eC As Rational = Rational.CopyFrom(eA)
        Try
            eC.resto = Add(Mult(eA.resto, eB.divisor), Mult(eA.divisor, eB.resto))
            eC.divisor = Mult(eA.divisor, eB.divisor)
            eC.Reduce()
        Catch ex As Exception
            Throw
        End Try
        Return eC
    End Function
    Public Shared Operator *(db As Double, eB As Rational) As Rational
        Return Add(New Rational(db), eB)
    End Operator
    Public Shared Operator *(eA As Rational, db As Double) As Rational
        Return Add(eA, New Rational(db))
    End Operator
    Public Shared Operator *(eA As Rational, eB As Rational) As Rational
        Return Mult(eA, eB)
    End Operator
    Public Shared Function Mult(eAT As List(Of Term_2), eBT As List(Of Term_2)) As List(Of Term_2)
        Dim ecT As New List(Of Term_2)
        Try
            For i = 0 To eAT.Count - 1
                For j = 0 To eBT.Count - 1
                    If eAT(i) = eBT(j) Then
                        Dim t As New Term_2(eAT(i))
                        t.cf *= eBT(j).cf
                        For k = 0 To eAT(i).f.Count - 1
                            t.f(k).exp += eBT(j).f(k).exp
                        Next
                        ecT.Add(t)
                    Else
                        ecT.Add(New Term_2(eAT(i) * eBT(j)))
                    End If
                Next
            Next
        Catch ex As Exception
            Throw
        End Try
        Return ecT
    End Function
    Public Shared Function Mult(eA As Rational, eB As Rational) As Rational
        Dim eC As New Rational
        Try
            eC.resto = Mult(eA.resto, eB.resto)
            eC.divisor = Mult(eA.divisor, eB.divisor)

            eC.Reduce()
        Catch ex As Exception
            Throw
        End Try
        Return eC
    End Function
    Public Shared Function EqualListOfTerm_2s(eAT As List(Of Term_2), eBT As List(Of Term_2)) As Boolean
        If eAT.Count <> eBT.Count Then Return False
        For i = 0 To eAT.Count - 1
            If eAT(i) <> eBT(i) Then Return False
        Next
        Return True
    End Function
    Public Shared Operator /(eA As Rational, eB As Rational) As Rational
        Dim eC As New Rational(0.0)
        Try
            'If eA.Degree = eB.Degree Then
            'If EqualListOfTerm_2s(eA.divisor, eB.divisor) Then
            '    eC = New Rational(eA.resto)
            '    eC.divisor.Clear()
            '    For i As Int32 = 0 To eB.resto.Count - 1
            '        eC.divisor.Add(New Term_2(eB.resto(i)))
            '    Next
            '    eC.Reduce()
            '    Exit Try
            'End If
            'End If
            ' (ra/da)/(rb/db) = ra*db / (da * rb)
            eC.resto = Mult(eA.resto, eB.divisor)
            eC.divisor = Mult(eA.divisor, eB.resto)
            eC.Reduce()
        Catch ex As Exception
            Throw
        End Try
        Return eC
    End Operator
    Public Shared Operator /(eA As Rational, db As Double) As Rational
        Return eA / New Rational(db)
    End Operator
    Public Shared Operator ^(eA As Rational, db As Double) As Rational
        Return eA ^ New Rational(db)
    End Operator

    Public Shared Operator ^(rtnA As Rational, rtnB As Rational) As Rational
        Dim rtnC As Rational = Nothing
        Try
            rtnC = New Rational(rtnA.ToDouble, rtnB.ToDouble)
            rtnC.Reduce()
        Catch ex As Exception
            Throw
        End Try
        Return rtnC
    End Operator

    Public Shared Sub ReduceListOfTerm_2s(ByRef lstT As List(Of Term_2))
        For i As Int32 = lstT.Count - 1 To 1 Step -1
            If i >= lstT.Count Then i = lstT.Count - 1
            For j As Int32 = i - 1 To 0 Step -1
                If lstT(i) = lstT(j) Then
                    lstT(j).cf += lstT(i).cf
                    lstT.RemoveAt(i)
                    lstT.RemoveAt(j)
                    Exit For
                End If
            Next
        Next
        For i As Int32 = lstT.Count - 1 To 0 Step -1
            If lstT(i).ToDouble = 0.0 AndAlso lstT.Count > 1 Then
                lstT.RemoveAt(i)
            End If
        Next
    End Sub


    Public Sub Reduce(Optional ByRef IntegerPart As List(Of Term_2) = Nothing)
        Try
            If Not bReduce Then Exit Sub
            ReduceListOfTerm_2s(resto)
            ReduceListOfTerm_2s(divisor)
            For i As Int32 = resto.Count - 1 To 0 Step -1
                If resto(i).ToDouble = 0 Then
                    resto.RemoveAt(i)
                Else
                    For j As Int32 = resto(i).f.Count - 1 To 0 Step -1
                        If resto(i).f(j).exp = 0.0 Then
                            resto(i).f.RemoveAt(j)
                        End If
                    Next
                End If
            Next
            For i As Int32 = divisor.Count - 1 To 0 Step -1
                If divisor(i).ToDouble = 0 AndAlso divisor.Count > 1 Then
                    divisor.RemoveAt(i)
                Else
                    For j As Int32 = divisor(i).f.Count - 1 To 0 Step -1
                        If divisor(i).f(j).exp = 0.0 Then
                            divisor(i).f.RemoveAt(j)
                        End If
                    Next
                End If
            Next

            If resto.Count = 0 Then
                resto.Add(New Term_2(0.0))
                divisor.Clear()
                divisor.Add(New Term_2(1.0))
                Exit Try
            End If
            If divisor.Count = 0 Then
                divisor.Add(New Term_2(1.0))
            End If
            If New Rational(resto) = New Rational(divisor) Then
                resto.Clear()
                resto.Add(New Term_2(1.0))
                divisor.Clear()
                divisor.Add(New Term_2(1.0))
                Exit Sub
            End If

            ReduceListOfTerm_2s(resto)
            If resto.Count = 0 Then
                resto.Add(New Term_2(0.0))
                divisor.Clear()
                divisor.Add(New Term_2(1.0))
            Else
                ReduceListOfTerm_2s(divisor)
            End If
        Catch ex As Exception
            Throw
        End Try
    End Sub


    Public Overrides Function ToString() As String
        Return ToStringRational(G10.nDec, G10.CI)
    End Function
    Public Shared Function ToStringListOfTerm_2s(lstT As List(Of Term_2)) As String
        Dim sb As New StringBuilder
        Try
            Dim v() As Term_2 = lstT.ToArray
            Dim i As Int32
            Dim sIndep As String = ""
            For i = 0 To v.Count - 2
                sIndep = v(i).ToString
            Next
            If v.Count Then
                sb.Append(v(i).ToString())
            End If
            If Len(sIndep) Then
                If sb.Length Then
                    sb.Append("+")
                End If
                sb.Append(sIndep)
            End If
            If sb.Length = 0 Then
                sb.Append("0")
            End If
        Catch ex As Exception
            Throw
        End Try
        Return sb.ToString
    End Function
    Public Overloads Function ToStringRational(numDecimals As Int32,
                                                cultureInfo As Globalization.CultureInfo)
        Dim sb As New StringBuilder
        Try
            bReduce = True
            Reduce()
            Dim N As Double
            Dim i As Int32
            For i = 0 To resto.Count - 1
                If resto(i).IsReal Then N += resto(i).ToDouble
            Next
            If i >= resto.Count Then
                Dim D As Double
                For i = 0 To divisor.Count - 1
                    If divisor(i).IsReal Then D += divisor(i).ToDouble
                Next
                If i >= divisor.Count - 1 Then
                    Dim rA As New Rational_1(N)
                    Dim rB As New Rational_1(D)
                    Return (rA / rB).toString
                End If
            End If
            'Return ToDouble.ToString(cultureInfo)
            Dim sIntPart As String = ""
            Dim IntegerPart As New List(Of Term_2)
            Reduce(IntegerPart)
            sIntPart += ToStringListOfTerm_2s(IntegerPart)
            Dim pResto As New Rational(resto)
            Dim pDiv As New Rational(divisor)
            If pDiv.ToDouble = -1 Then
                pResto = -pResto
                pDiv = -pDiv
            End If
            Dim sResto As String
            sResto = ToStringListOfTerm_2s(pResto.resto)
            If sResto = "0" Then sResto = ""
            If sIntPart = "0" AndAlso Len(sResto) Then
                sIntPart = ""
            End If
            Dim sDiv As String = ToStringListOfTerm_2s(pDiv.resto)
            If Len(sResto) > 1 AndAlso Regex.IsMatch(Mid(sResto, 2), "\-|\+") AndAlso
            sDiv <> "1" AndAlso sDiv <> "-1" Then
                sResto = "(" + sResto + ")"
            End If
            If Regex.IsMatch(Mid(sDiv, 2), "\-|\+") Then
                sDiv = "(" + sDiv + ")"
            End If
            Dim sDiv2 As String = Regex.Replace(sDiv, "&h|&o| ", "")
            If sResto = "0" Then
                sResto = ""
                sDiv = ""
            ElseIf sDiv2 = "1" Then
                sDiv = ""
            ElseIf sDiv2 = "-1" Then
                sResto = "-" + sResto
                sDiv = ""
            Else
                If Regex.IsMatch(Mid(sDiv, 2), "\-|\+|\*") Then
                    sDiv = removeStartEndParentheses(sDiv)
                    sDiv = "(" + sDiv + ")"
                End If
                sDiv = "/" + sDiv
            End If
            If Len(sIntPart) Then
                sb.Append(sIntPart)
                If Len(sResto) Then sb.Append("+")
            End If
            sb.Append(sResto + sDiv)
        Catch ex As Exception
            Throw
        End Try
        Dim e1 As String = Regex.Replace(sb.ToString, "\+\s*\-", "-")
        Return e1
    End Function
    Public Shared Function removeStartEndParentheses(s As String) As String
        Try
            Do While Left(s, 1) = "(" AndAlso Right(s, 1) = ")"
                Dim s1 As String = Mid(s, 2, Len(s) - 2)
                Dim lp As Int32 = InStr(s1, "(")
                Dim rp As Int32 = InStr(s1, ")")
                If lp < rp OrElse lp + rp = 0 Then
                    s = s1
                Else
                    Exit Do
                End If
            Loop
        Catch ex As Exception

        End Try
        Return s
    End Function

End Class
Public Class Term_2
    Public f As New List(Of Factor_2)
    Public cf As Double
    Public Shared Infinity As New Term_2("∞", 1)
    Public Shared mnInfinity As Term_2 = -Infinity
    Public Shared IndeTerm_2inate As New Term_2("IndeTerm_2inate", 1)
    Public Sub New(dbl As Double)
        cf = dbl
    End Sub
    Public Sub New(t As Term_2)
        cf = t.cf
        For i = 0 To t.f.Count - 1
            f.Add(Factor_2.CopyFrom(t.f(i)))
        Next
    End Sub
    Public Sub New(f As Factor_2)
        cf = 1.0
        Me.f.Add(f)
    End Sub
    Public Sub New(db As Double, exp As Double)
        cf = 1.0
        f.Add(New Factor_2(db, exp))
    End Sub
    Public Shared Function CopyFrom(tA As Term_2) As Term_2
        Dim tC As New Term_2(tA.cf)
        For i As Int32 = 0 To tA.f.Count - 1
            tC.f.Add(Factor_2.CopyFrom(tA.f(i)))
        Next
        Return tC
    End Function
    Public Function ToDouble() As Double
        Dim db As Double = cf
        For i As Int32 = 0 To f.Count - 1
            db *= f(i).ToDouble
        Next
        Return db
    End Function
    Public Function IsReal() As Boolean
        For Each f1 As Factor_2 In f
            If Not f1.IsReal Then Return False
        Next
        Return True
    End Function

    Public Sub OpChgSign()
        cf = -cf
    End Sub
    Public Shared Operator =(tA As Term_2, tB As Term_2) As Boolean
        Return tA.ToDouble = tB.ToDouble
    End Operator
    Public Shared Operator <>(tA As Term_2, tB As Term_2) As Boolean
        Return Not (tA = tB)
    End Operator
    Public Shared Operator -(tA As Term_2) As Term_2
        Dim tC As New Term_2(tA)
        tC.OpChgSign()
        Return tC
    End Operator
    Public Shared Operator *(tA As Term_2, tB As Term_2) As Term_2
        Dim tC As New Term_2(tA)
        Dim tD As New Term_2(tB)
        Try
            tC.cf *= tB.cf
            For i As Int32 = tC.f.Count - 1 To 0 Step -1
                For j As Int32 = tD.f.Count - 1 To 0 Step -1
                    If tC.f(i).rtnl = tD.f(j).rtnl Then
                        tC.f(i).exp += tD.f(j).exp
                        tD.f.RemoveAt(j)
                        If tC.f(i).exp = 0 Then
                            tC.f.RemoveAt(i)
                        End If
                        Exit For
                    End If
                Next
            Next
            tC.f.AddRange(tD.f)
        Catch ex As Exception
            Throw
        End Try
        Return tC
    End Operator
    Public Shared Operator /(tA As Term_2, tB As Term_2) As Term_2
        Dim tC As New Term_2(tA)
        Dim tD As New Term_2(tB)
        Try
            tC.cf /= tB.cf
            For i As Int32 = tC.f.Count - 1 To 0 Step -1
                For j As Int32 = tD.f.Count - 1 To 0 Step -1
                    If tC.f(i).rtnl = tD.f(j).rtnl Then
                        tC.f(i).exp -= tD.f(j).exp
                        tD.f.RemoveAt(j)
                        If tC.f(i).exp = 0 Then
                            tC.f.RemoveAt(i)
                        End If
                        Exit For
                    End If
                Next
            Next
            For i = 0 To tD.f.Count - 1
                Dim f As New Factor_2(tD.f(i).rtnl.ToDouble, -tD.f(i).exp)
                tC.f.Add(f)
            Next
        Catch ex As Exception
            Throw
        End Try
        Return tC
    End Operator
    Public Overrides Function ToString() As String
        Return ToString(G10.nDec, G10.CI)
    End Function
    Public Overloads Function ToString(numDecimals As Int32, cultureInfo As Globalization.CultureInfo)
        Dim sb As New StringBuilder
        Try
            If cf = 0 Then
                sb.Append("0")
                Exit Try
            End If
            Dim sCf As String = ""
            If numDecimals < 15 Then
                sCf = Math.Round(cf, numDecimals).ToString(cultureInfo)
            Else
                sCf = cf.ToString(cultureInfo)
            End If
            Dim i As Int32
            For i = 0 To Me.f.Count - 1
                sb.Append(f(i).ToString(numDecimals, cultureInfo))
                If i < f.Count - 1 Then sb.Append("*")
            Next
            If Left(sb.ToString, 2) = "0*" Then sb.Clear()
            Dim scf2 As String = Regex.Replace(sCf, "&h|&o", "")
            If scf2 = "1" AndAlso sb.Length Then
            ElseIf scf2 = "-1" AndAlso sb.Length Then
                sb.Insert(0, "-")
            ElseIf sb.Length Then
                If Len(sCf) > 1 AndAlso Regex.IsMatch(Mid(sCf, 2), "[-+]") Then
                    sb.Insert(0, "(" + sCf + ")*")
                Else
                    sb.Insert(0, sCf + "*")
                End If
            Else
                sb.Append(sCf)
            End If
        Catch ex As Exception
            Throw
        End Try
        Return sb.ToString
    End Function
End Class

Public Class Factor_2
    Public rtnl As Rational_1 ' Double
    Public exp As Double = 1
    Public var As String
    Public Sub New(Factor_2 As Double)
        rtnl = New Rational_1(Factor_2)
    End Sub
    Public Sub New(rA As Rational_1)
        rtnl = New Rational_1(rA)
    End Sub
    Public Sub New(Factor_2 As Double, exponent As Double)
        rtnl = New Rational_1(Factor_2)
        exp = exponent
    End Sub
    Public Shared Function CopyFrom(fA As Factor_2) As Factor_2
        Dim fC As New Factor_2(fA.rtnl)
        fC.exp = fA.exp
        Return fC
    End Function
    Public Function ToDouble() As Double
        Return rtnl.ToDouble ^ exp
    End Function
    Public Function IsReal() As Boolean
        Return (var = "")
    End Function
    Public Shared Operator =(fA As Factor_2, fB As Factor_2) As Boolean
        Return fA.rtnl ^ fA.exp = fB.rtnl ^ fB.exp
    End Operator
    Public Shared Operator <>(fA As Factor_2, fB As Factor_2) As Boolean
        Return Not (fA = fB)
    End Operator
    Public Shared Operator ^(fA As Factor_2, dbl As Double) As Factor_2
        Dim fC As Factor_2 = Factor_2.CopyFrom(fA)
        Try
            fC.exp *= dbl
        Catch ex As Exception

        End Try
        Return fC
    End Operator

    Public Overrides Function ToString() As String
        Return ToString(G10.nDec, New Globalization.CultureInfo("en-US"))
    End Function
    Public Overloads Function ToString(numDecimals As Int32, cultureInfo As Globalization.CultureInfo)
        Dim sb As New StringBuilder
        Try
            sb.Append(rtnl)
            If exp <> 1 Then
                sb.Append("^" + exp.ToString)
            End If
        Catch ex As Exception
            Throw
        End Try
        Return sb.ToString
    End Function
End Class
Public Class Factor_2Comparer
    Implements IComparer
    Public Function Compare(x1 As Object, y1 As Object) As Integer Implements IComparer.Compare
        Dim x As Factor_2 = CType(x1, Factor_2)
        Dim y As Factor_2 = CType(y1, Factor_2)
        Dim sb1 As New StringBuilder
        Dim sb2 As New StringBuilder
        Try
            sb1.Append(x.rtnl)
            sb1.Append(String.Format("{0:000000}", 10 ^ 5 - x.exp))
            If sb1.Length = 0 Then
                sb1.Append(ChrW(255))
            Else
                sb1.Append("}")
            End If
            sb2.Append(y.rtnl)
            sb2.Append(String.Format("{0:000000}", 10 ^ 5 - y.exp))
            If sb2.Length = 0 Then
                sb2.Append(ChrW(255))
            Else
                sb2.Append("}")
            End If
        Catch ex As Exception
            Throw New NotImplementedException()
        End Try
        If sb1.ToString < sb2.ToString Then
            Return -1
        ElseIf sb1.ToString > sb2.ToString Then
            Return 1
        End If
        Return 0
    End Function
End Class
