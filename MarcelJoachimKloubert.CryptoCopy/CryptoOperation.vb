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

#Region "Methods (5)"

    Private Sub DecryptDirectory(src As DirectoryInfo, dest As DirectoryInfo)
        '' TODO
    End Sub

    Private Sub EncryptDirectory(src As DirectoryInfo, dest As DirectoryInfo)
        '' TODO
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
        Try
            Me.DecryptDirectory(Me.Settings.SourceDirectory, _
                                Me.Settings.DestionationDirectories(0))
        Catch ex As Exception
            '' TODO
        End Try
    End Sub

    Private Sub Start_Encrypt()
        For Each dest As DirectoryInfo In Me.Settings.DestionationDirectories.ToArray()
            Try
                Me.DecryptDirectory(Me.Settings.SourceDirectory, _
                                    dest)
            Catch ex As Exception
                '' TODO
            End Try
        Next
    End Sub

#End Region

End Class