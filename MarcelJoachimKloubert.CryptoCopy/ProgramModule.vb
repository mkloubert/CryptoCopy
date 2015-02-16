'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Security.Cryptography

''' <summary>
''' The program module.
''' </summary>
Module ProgramModule

#Region "Methods (3)"

    ''' <summary>
    ''' Entry point.
    ''' </summary>
    ''' <param name="args">Command line arguments.</param>
    Sub Main(args As String())
        Try
            Dim actionToInvoke As Action = Nothing

            Dim normalizedArgs As String() = args.Select(Function(x)
                                                             Return x.Trim()
                                                         End Function) _
                                                 .Where(Function(x)
                                                            Return x <> String.Empty
                                                        End Function) _
                                                .ToArray()

            If normalizedArgs.Length > 0 Then
                '' extract data
                Dim dirs As List(Of String) = New List(Of String)
                Dim opts As List(Of String) = New List(Of String)
                For Each a As String In normalizedArgs
                    If a.StartsWith("/") Then
                        opts.Add(a)
                    Else
                        dirs.Add(a)
                    End If
                Next

                If dirs.Count > 0 Then
                    Dim settings As AppSettings = New AppSettings()

                    Dim markForShowingHelp As Action = Sub()
                                                           actionToInvoke = Nothing
                                                           settings = Nothing
                                                       End Sub

                    If dirs.Count = 1 Then
                        '' use single directory as destination and
                        '' the current one as source

                        settings.SourceDirectory = New DirectoryInfo(Environment.CurrentDirectory)
                        settings.DestionationDirectories.Add(New DirectoryInfo(dirs(0)))
                    Else
                        settings.SourceDirectory = New DirectoryInfo(dirs(0))

                        For i As Integer = 2 To dirs.Count
                            settings.DestionationDirectories.Add(New DirectoryInfo(dirs(i - 1)))
                        Next
                    End If

                    For Each o As String In opts
                        If o.ToLower() = "/d" Or o.ToLower() = "/dec" Or o.ToLower() = "/decrypt" Then
                            '' decrypt
                            settings.Type = CryptoOperationType.Decrypt
                        ElseIf o.ToLower() = "/e" Or o.ToLower() = "/enc" Or o.ToLower() = "/encrypt" Then
                            '' encrypt
                            settings.Type = CryptoOperationType.Encrypt
                        ElseIf o.ToLower() = "/?" Or o.ToLower() = "/h" Or o.ToLower() = "/help" Then
                            '' long help screen

                            actionToInvoke = New Action(AddressOf ShowLongHelp)
                            settings = Nothing

                            Exit For
                        ElseIf o.ToLower().StartsWith("/p:") Or o.ToLower().StartsWith("/pwd:") Or o.ToLower().StartsWith("/password:") Then
                            '' password

                            Dim pwd = o.Substring(o.IndexOf(":") + 1)
                            If Not String.IsNullOrEmpty(pwd) Then
                                settings.Password = Console.InputEncoding.GetBytes(pwd)
                            Else
                                settings.Password = Nothing
                            End If
                        ElseIf o.ToLower().StartsWith("/p64:") Or o.ToLower().StartsWith("/pwd64:") Or o.ToLower().StartsWith("/password64:") Then
                            '' password (Base64)

                            Dim b64Pwd = o.Substring(o.IndexOf(":") + 1).Trim()
                            If Not String.IsNullOrWhiteSpace(b64Pwd) Then
                                settings.Password = Convert.FromBase64String(b64Pwd)
                            Else
                                settings.Password = Nothing
                            End If
                        ElseIf o.ToLower().StartsWith("/s:") Or o.ToLower().StartsWith("/salt:") Then
                            '' salt

                            Dim salt = o.Substring(o.IndexOf(":") + 1)
                            If Not String.IsNullOrEmpty(salt) Then
                                settings.Salt = Console.InputEncoding.GetBytes(salt)
                            Else
                                settings.Salt = Nothing
                            End If
                        ElseIf o.ToLower().StartsWith("/s64:") Or o.ToLower().StartsWith("/salt64:") Then
                            '' salt (Base64)

                            Dim b64Salt = o.Substring(o.IndexOf(":") + 1).Trim()
                            If Not String.IsNullOrWhiteSpace(b64Salt) Then
                                settings.Salt = Convert.FromBase64String(b64Salt)
                            Else
                                settings.Salt = Nothing
                            End If
                        Else
                            '' unknown option

                            markForShowingHelp()

                            Exit For
                        End If
                    Next

                    '' check if settings are valid
                    Select Case settings.Type
                        Case CryptoOperationType.Decrypt
                            If settings.Password Is Nothing Then
                                '' no password for decryption

                                markForShowingHelp()
                            Else
                                If Not settings.DestionationDirectories.Count = 1 Then
                                    '' only one destionation directory required

                                    markForShowingHelp()
                                End If
                            End If
                            Exit Select
                    End Select

                    If Not settings Is Nothing Then
                        '' set action for doing crypto operation

                        actionToInvoke = Sub()
                                             If settings.Password Is Nothing Then
                                                 If settings.Type = CryptoOperationType.Encrypt Then
                                                     Dim rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider()

                                                     settings.Password = New Byte(15) {}
                                                     rng.GetBytes(settings.Password)

                                                     settings.ShowPassword = True
                                                     settings.ShowSalt = True
                                                 End If
                                             End If

                                             If settings.Salt Is Nothing Then
                                                 '' default salt

                                                 settings.ShowSalt = False
                                                 Using md5 As New MD5CryptoServiceProvider()
                                                     settings.Salt = md5.ComputeHash(Encoding.UTF8.GetBytes("gwsTMV4lY+4an8XMK4aSk"))
                                                 End Using
                                             End If

                                             Dim operation As CryptoOperation = New CryptoOperation(settings)
                                             operation.Start()
                                         End Sub
                    End If
                End If
            End If

            If actionToInvoke Is Nothing Then
                '' define default
                actionToInvoke = New Action(AddressOf ShowShortHelp)
            End If

            actionToInvoke()
        Catch ex As Exception
            '' TODO
        Finally
#If DEBUG Then
            Global.System.Console.WriteLine("===== ENTER =====")
            Global.System.Console.ReadLine()
#End If
        End Try
    End Sub

    Private Sub ShowLongHelp()
        Console.WriteLine()
        Console.WriteLine()

        '' TODO
    End Sub

    Private Sub ShowShortHelp()
        Console.WriteLine()
        Console.WriteLine()

        '' TODO
    End Sub

#End Region

End Module