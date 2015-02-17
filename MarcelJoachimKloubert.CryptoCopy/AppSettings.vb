'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

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

#Region "Properties (7)"

    ''' <summary>
    ''' Gets or sets the destionation directory.
    ''' </summary>
    Public ReadOnly Property DestionationDirectories As List(Of DirectoryInfo)
        Get
            Return Me._DESTIONATION_DIRS
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the iterations.
    ''' </summary>
    Public Property Iterations As Integer?

    ''' <summary>
    ''' Gets or sets the password.
    ''' </summary>
    Public Property Password As Byte()

    ''' <summary>
    ''' Gets or sets the salt.
    ''' </summary>
    Public Property Salt As Byte()

    ''' <summary>
    ''' Gets or sets if password should be shown or not.
    ''' </summary>
    Public Property ShowPassword As Boolean

    ''' <summary>
    ''' Gets or sets the source directory.
    ''' </summary>
    Public Property SourceDirectory As DirectoryInfo

    ''' <summary>
    ''' Gets or sets the operation type.
    ''' </summary>
    Public Property Type As CryptoOperationType = CryptoOperationType.Encrypt

#End Region

#Region "Methods (1)"

    ''' <summary>
    ''' Creates a new crypter instance based on the current settings.
    ''' </summary>
    ''' <returns>The new crypter instance.</returns>
    Public Function CreateCrypter() As ICrypter
        Dim salt As Byte() = Me.Salt
        If salt IsNot Nothing Then
            '' default salt

            Using md5 As New MD5CryptoServiceProvider()
                salt = md5.ComputeHash(Encoding.UTF8.GetBytes("gwsTMV4lY+4an8XMK4aSk"))
            End Using
        End If

        Dim iterations As Integer? = Me.Iterations
        If Not iterations.HasValue Then
            iterations = 1000
        End If

        Return New RijndaelCrypter(Me.Password, _
                                   salt, _
                                   iterations.Value)
    End Function

#End Region

End Class