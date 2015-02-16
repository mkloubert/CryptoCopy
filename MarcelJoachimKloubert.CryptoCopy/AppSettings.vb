'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.IO

''' <summary>
''' Stores application settings.
''' </summary>
Public NotInheritable Class AppSettings

#Region "Fields (1)"

    Private ReadOnly _DESTIONATION_DIRS As List(Of DirectoryInfo)

#End Region

#Region "Constructors (1)"

    ''' <summary>
    ''' Initializes a new instance of the <see cref="CryptoOperation" /> class.
    ''' </summary>
    Sub New()
        _DESTIONATION_DIRS = New List(Of DirectoryInfo)
    End Sub

#End Region

#Region "Properties (5)"

    ''' <summary>
    ''' Gets or sets the destionation directory.
    ''' </summary>
    Public ReadOnly Property DestionationDirectories As List(Of DirectoryInfo)
        Get
            Return Me._DESTIONATION_DIRS
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the password.
    ''' </summary>
    Public Property Password As Byte()

    ''' <summary>
    ''' Gets or sets the salt.
    ''' </summary>
    Public Property Salt As Byte()

    ''' <summary>
    ''' Gets or sets the source directory.
    ''' </summary>
    Public Property SourceDirectory As DirectoryInfo

    ''' <summary>
    ''' Gets or sets the operation type.
    ''' </summary>
    Public Property Type As CryptoOperationType = CryptoOperationType.Encrypt

#End Region

End Class