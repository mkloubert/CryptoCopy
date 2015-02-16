'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.IO

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
        '' TODO
    End Sub

    Private Function FindNextDir(baseDir As DirectoryInfo)

    End Function

    Private Sub EncryptDirectory(src As DirectoryInfo, dest As DirectoryInfo, isFirst As Boolean)
        Dim crypter As ICrypter = Me.Settings.CreateCrypter()

        Dim mf As MetaFile = New MetaFile(dest.FullName, crypter)

        If isFirst Then
            For Each file As FileInfo In src.GetFiles()
                mf.EncryptFile(file.FullName)
            Next
        End If

        For Each subDir As DirectoryInfo In src.GetDirectories()
            Dim newDirEntry As MetaFileDirectoryEntry = mf.EncryptDirectory(subDir.FullName)

            Me.EncryptDirectory(subDir, _
                                newDirEntry.File.File.Directory, _
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