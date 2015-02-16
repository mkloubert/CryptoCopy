'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.IO

''' <summary>
''' A basic crypter
''' </summary>
Public MustInherit Class CrypterBase
    Implements ICrypter

#Region "Fields (1)"

    ''' <summary>
    ''' Stores the object for thread safe operations.
    ''' </summary>
    Protected ReadOnly _SYNC As Object

#End Region

#Region "Constructors (2)"

    ''' <summary>
    ''' Initializes a new instance of the <see cref="CrypterBase" /> class.
    ''' </summary>
    Protected Sub New()
        Me.New(New Object())
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the <see cref="CrypterBase" /> class.
    ''' </summary>
    ''' <param name="sync">The value for the <see cref="CrypterBase._SYNC" /> field.</param>
    Protected Sub New(sync As Object)
        Me._SYNC = sync
    End Sub

#End Region

#Region "Methods (2)"

    ''' <inheriteddoc />
    Public MustOverride Sub Decrypt(src As Stream, dest As Stream) Implements ICrypter.Decrypt

    ''' <inheriteddoc />
    Public MustOverride Sub Encrypt(src As Stream, dest As Stream) Implements ICrypter.Encrypt

#End Region

End Class