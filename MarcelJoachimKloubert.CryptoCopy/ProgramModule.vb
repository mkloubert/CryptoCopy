'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Security.Cryptography
Imports System.Reflection

''' <summary>
''' The program module.
''' </summary>
Module ProgramModule

#Region "Methods (4)"

    ''' <summary>
    ''' Entry point.
    ''' </summary>
    ''' <param name="args">Command line arguments.</param>
    Sub Main(args As String())
        PrintHeader()

        Dim actionToInvoke As Action = Nothing

        Dim normalizedArgs As String() = args.Select(Function(x)
                                                         Return x.Trim()
                                                     End Function) _
                                             .Where(Function(x)
                                                        Return x <> String.Empty
                                                    End Function) _
                                             .ToArray()

        If normalizedArgs.Length > 0 Then
            Dim settings As AppSettings = New AppSettings()

            Dim markForShowingHelp As Action = Sub()
                                                   actionToInvoke = Nothing
                                                   settings = Nothing
                                               End Sub

            Dim op = normalizedArgs(0).ToLower().Trim()
            Select Case op
                Case "/d", "/dec", "/decrypt"
                    '' decrypt files
                    settings.Type = CryptoOperationType.Decrypt
                    Exit Select

                Case "/e", "/enc", "/encrypt"
                    '' encrypt files
                    settings.Type = CryptoOperationType.Encrypt
                    Exit Select

                Case "/?", "/h", "/help"
                    '' show long help
                    actionToInvoke = New Action(AddressOf ShowLongHelp)
                    settings = Nothing
                    Exit Select

                Case Else
                    '' unknown operation
                    markForShowingHelp()
                    Exit Select
            End Select

            If Not settings Is Nothing Then
                '' extract directories and options
                Dim dirs As List(Of String) = New List(Of String)
                Dim opts As List(Of String) = New List(Of String)
                For i As Integer = 2 To normalizedArgs.Length
                    Dim a As String = normalizedArgs(i - 1).TrimStart()

                    If a.StartsWith("/") And a.Contains(":") Then
                        opts.Add(a)
                    Else
                        dirs.Add(a)
                    End If
                Next

                '' process options
                For Each o As String In opts
                    If o.ToLower().StartsWith("/p:") Or o.ToLower().StartsWith("/pwd:") Or o.ToLower().StartsWith("/password:") Then
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
                    ElseIf o.ToLower().StartsWith("/i:") Or o.ToLower().StartsWith("/iterations:") Then
                        '' iterations

                        Dim itStr = o.Substring(o.IndexOf(":") + 1).Trim()
                        If Not String.IsNullOrWhiteSpace(itStr) Then
                            settings.Iterations = Convert.ToInt32(itStr)
                        Else
                            settings.Iterations = Nothing
                        End If
                    Else
                        '' unknown option

                        markForShowingHelp()

                        Exit For
                    End If
                Next

                If Not settings Is Nothing Then
                    '' validate by operation types
                    Select Case settings.Type
                        Case CryptoOperationType.Decrypt
                            '' decrypt
                            If dirs.Count = 1 Then
                                settings.SourceDirectory = New DirectoryInfo(Environment.CurrentDirectory)
                                settings.DestionationDirectories _
                                        .Add(New DirectoryInfo(dirs(0)))
                            ElseIf dirs.Count > 1 Then
                                settings.SourceDirectory = New DirectoryInfo(dirs(0))

                                For i As Integer = 2 To dirs.Count
                                    settings.DestionationDirectories _
                                            .Add(New DirectoryInfo(dirs(i - 1)))
                                Next
                            Else
                                markForShowingHelp()
                            End If

                            If settings.Password Is Nothing Then
                                '' no password
                                markForShowingHelp()
                            End If
                            Exit Select

                        Case CryptoOperationType.Encrypt
                            '' encrypt
                            If dirs.Count = 1 Then
                                settings.SourceDirectory = New DirectoryInfo(Environment.CurrentDirectory)
                                settings.DestionationDirectories _
                                        .Add(New DirectoryInfo(dirs(0)))
                            ElseIf dirs.Count > 1 Then
                                settings.SourceDirectory = New DirectoryInfo(dirs(0))

                                For i As Integer = 2 To dirs.Count
                                    settings.DestionationDirectories _
                                            .Add(New DirectoryInfo(dirs(i - 1)))
                                Next
                            Else
                                markForShowingHelp()
                            End If

                            If settings.Password Is Nothing Then
                                '' random password

                                Dim rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider()

                                settings.Password = New Byte(15) {}
                                rng.GetBytes(settings.Password)

                                settings.ShowPassword = True
                            End If
                            Exit Select
                    End Select
                End If
            End If

            If Not settings Is Nothing Then
                '' define action for starting operation

                actionToInvoke = Sub()
                                     If settings.Salt Is Nothing Then
                                         '' default salt

                                         Using md5 As New MD5CryptoServiceProvider()
                                             settings.Salt = md5.ComputeHash(Encoding.UTF8.GetBytes("gwsTMV4lY+4an8XMK4aSk"))
                                         End Using
                                     End If

                                     Dim operation As CryptoOperation = New CryptoOperation(settings)
                                     operation.Start()
                                 End Sub
            End If
        End If

        If actionToInvoke Is Nothing Then
            actionToInvoke = New Action(AddressOf ShowShortHelp)
        End If

        actionToInvoke()

#If DEBUG Then
        Console.WriteLine()
        Console.WriteLine()

        Console.WriteLine("===== ENTER =====")
        Console.ReadLine()
#End If
    End Sub

    Private Sub PrintHeader()
        Dim title As String = String.Format("CryptoCopy {0}", _
                                            Assembly.GetExecutingAssembly().GetName().Version)

        Console.WriteLine(title)
        Console.WriteLine(String.Concat(Enumerable.Repeat("=", _
                                                          title.Length + 5)))
        Console.WriteLine()
    End Sub

    Private Sub ShowLongHelp()
        Console.WriteLine("Encrypts or decrypts files and directories.")
        Console.WriteLine()
        Console.WriteLine()

        Console.WriteLine("CryptoCopy OPERATION SOURCE DESTS OPTS")
        Console.WriteLine()
        Console.WriteLine("  OPERATION         /d    Decrypt directory")
        Console.WriteLine("                    /e    Encrypt directory")
        Console.WriteLine("                    /?    Show this help")
        Console.WriteLine()
        Console.WriteLine("  SOURCE            Source directory")
        Console.WriteLine()
        Console.WriteLine("  DESTS             One or more destionation directories")
        Console.WriteLine()
        Console.WriteLine("  OPTS              One or more options")
        Console.WriteLine()
        Console.WriteLine("    /i:[NR]           The custom iteration value to use.")
        Console.WriteLine()
        Console.WriteLine("    /p:[PASSOWRD]     The password to use.")
        Console.WriteLine("    /p64:[BASE64]     The password as Base64 string to use.")
        Console.WriteLine()
        Console.WriteLine("    /s:[SALT]         The optional, custom salt to use.")
        Console.WriteLine("    /s64:[BASE64]     The optional, custom salt (as Base64 string) to use.")
    End Sub

    Private Sub ShowShortHelp()
        Console.WriteLine("Encrypts or decrypts files and directories.")
        Console.WriteLine()
        Console.WriteLine()

        Console.WriteLine("Encrypt a directory:")
        Console.WriteLine("    CryptoCopy /e SourceDirectory DestinationDirectory /p:myPassword")
        Console.WriteLine()

        Console.WriteLine("Decrypt a directory:")
        Console.WriteLine("    CryptoCopy /d SourceDirectory DestinationDirectory /p:myPassword")
        Console.WriteLine()


    End Sub

#End Region

End Module