# CryptoCopy

Console application that encrypts / decrypts files.

Enter `CryptopCopy /?` to display detail help screen.

## Requirements

* [Microsoft .NET](https://en.wikipedia.org/wiki/.NET_Framework) 4+ for [Windows](https://en.wikipedia.org/wiki/Microsoft_Windows) systems **-OR-**
* [Mono framework](https://en.wikipedia.org/wiki/Mono_%28software%29) for [Linux](https://en.wikipedia.org/wiki/Linux) or [MacOS](https://en.wikipedia.org/wiki/Mac_OS)

## How to use

### Encrypt

The following examples shows how to encrypt a directory and its sub-directories:

```dos
CryptopCopy /e C:\My_Uncrypted_Files C:\Store_The_Crypted_Files_Here /p:mySecretPassword
```

HINT: If you do not set a password, the application will generate a random one for you!

### Decrypt

Use the `/d` option to decrypt a crypted directory.

```dos
CryptopCopy /d C:\Files_That_Are_Crypted C:\My_Uncrypted_Files /p:mySecretPassword
```

