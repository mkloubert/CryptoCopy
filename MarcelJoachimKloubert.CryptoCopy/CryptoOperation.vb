'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.IO
Imports MarcelJoachimKloubert.CryptoCopy.Extensions

''' <summary>
''' Handles a crypto operation.
''' </summary>
Public NotInheritable Class CryptoOperation

#Region "Fields (1)"

    Private ReadOnly _SETTINGS As AppSettings

#End Region

#Region "Constructors (1)"

    ''' <summary>
    ''' Initializes a new instance of the <see cref="CryptoOperation" /> class.
    ''' </summary>
    Sub New(settings As AppSettings)
        Me._SETTINGS = settings
    End Sub

#End Region

#Region "Properties (1)"

    ''' <summary>
    ''' Gets the underlying application settings.
    ''' </summary>
    Public ReadOnly Property Settings As AppSettings
        Get
            Return Me._SETTINGS
        End Get
    End Property

#End Region

#Region "Methods (6)"

    Private Sub DecryptDirectory(src As DirectoryInfo, dest As DirectoryInfo, isFirst As Boolean)
        Dim crypter As ICrypter = Me.Settings.CreateCrypter()

        Dim mf As MetaFile = Nothing
        Try
            mf = New MetaFile(src.FullName, crypter)

            If Not mf.File.Exists Then
                Return
            End If
        Catch ex As Exception
            Return
        End Try

        For Each file As MetaFileFileEntry In mf.Files
            file.DecryptTo(dest.FullName)
        Next

        For Each dir As MetaFileDirectoryEntry In mf.Directories
            Dim decryptedDir = dir.DecryptTo(dest.FullName)

            Me.DecryptDirectory(New DirectoryInfo(dir.RealDirectory.FullName), _
                                decryptedDir, _
                                False)
        Next
    End Sub

    Private Sub EncryptDirectory(src As DirectoryInfo, dest As DirectoryInfo, isFirst As Boolean)
        Dim crypter As ICrypter = Me.Settings.CreateCrypter()

        Dim mf As MetaFile = New MetaFile(dest.FullName, crypter)
        If mf.File.Exists Then
            mf.File.Delete()
            mf.File.Refresh()
        End If

        If isFirst Then
            '' make random ordered list of files to encrypt
            Dim filesToEncrypt As List(Of FileInfo) = New List(Of FileInfo)(src.EnumerateFiles())
            filesToEncrypt.Shuffle()

            For Each file As FileInfo In filesToEncrypt
                mf.EncryptFile(file.FullName)
            Next
        End If

        '' make random ordered list of directories to encrypt
        Dim dirsToEncrypt As List(Of DirectoryInfo) = New List(Of DirectoryInfo)(src.EnumerateDirectories())
        dirsToEncrypt.Shuffle()

        For Each subDir As DirectoryInfo In dirsToEncrypt
            Dim newDirEntry As MetaFileDirectoryEntry = mf.EncryptDirectory(subDir.FullName)
            newDirEntry.File.UpdateAndSave()

            Me.EncryptDirectory(subDir, _
                                New DirectoryInfo(newDirEntry.File.File.Directory.FullName), _
                                False)
        Next

        mf.UpdateAndSave()
    End Sub

    ''' <summary>
    ''' Starts the operation.
    ''' </summary>
    Public Sub Start()
        Dim actionToInvoke As Action = Nothing

        Select Case Me.Settings.Type
            Case CryptoOperationType.Decrypt
                actionToInvoke = New Action(AddressOf Me.Start_Decrypt)
                Exit Select

            Case CryptoOperationType.Encrypt
                actionToInvoke = New Action(AddressOf Me.Start_Encrypt)
                Exit Select
        End Select

        If Me.Settings.ShowPassword Then
            Console.WriteLine("Password (Base64): {0}", _
                              Convert.ToBase64String(Me.Settings.Password))
        End If

        If Me.Settings.ShowSalt Then
            Console.WriteLine("Salt (Base64)    : {0}", _
                              Convert.ToBase64String(Me.Settings.Salt))
        End If

        If Not actionToInvoke Is Nothing Then
            actionToInvoke()
        End If
    End Sub

    Private Sub Start_Decrypt()
        For Each dest As DirectoryInfo In Me.Settings.DestionationDirectories.ToArray()
            Try
                Me.DecryptDirectory(Me.Settings.SourceDirectory, _
                                    dest, _
                                    True)
            Catch ex As Exception
                '' TODO
            End Try
        Next
    End Sub

    Private Sub Start_Encrypt()
        For Each dest As DirectoryInfo In Me.Settings.DestionationDirectories.ToArray()
            Try
                Me.EncryptDirectory(Me.Settings.SourceDirectory, _
                                    dest, _
                                    True)
            Catch ex As Exception
                '' TODO
            End Try
        Next
    End Sub

#End Region

End Class