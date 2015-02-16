'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.IO

''' <summary>
''' Describes an object that encrypts and decrypts data.
''' </summary>
Public Interface ICrypter

#Region "Methods (2)"

    ''' <summary>
    ''' Decrypts a source stream to a destion stream.
    ''' </summary>
    ''' <param name="src">The source stream.</param>
    ''' <param name="dest">The target stream.</param>
    Sub Decrypt(src As Stream, dest As Stream)

    ''' <summary>
    ''' Encrypts a source stream to a destion stream.
    ''' </summary>
    ''' <param name="src">The source stream.</param>
    ''' <param name="dest">The target stream.</param>
    Sub Encrypt(src As Stream, dest As Stream)

#End Region

End Interface