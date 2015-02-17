# CryptoCopy

Console application that encrypts / decrypts files.

## Requirements

* [Microsoft .NET](https://en.wikipedia.org/wiki/.NET_Framework) 4+ for [Windows](https://en.wikipedia.org/wiki/Microsoft_Windows) systems **-OR-**
* [Mono framework](https://en.wikipedia.org/wiki/Mono_%28software%29) for [Linux](https://en.wikipedia.org/wiki/Linux) or [MacOS](https://en.wikipedia.org/wiki/Mac_OS)

## How to use

### Syntax

```dos
CryptoCopy OPERATION SOURCE DESTINATIONS OPTIONS
```

### Encrypt

The following example shows how to encrypt a directory and its sub-directories:

```dos
CryptoCopy /e C:\My_Uncrypted_Files C:\Store_The_Crypted_Files_Here /p:mySecretPassword
```

HINT: If you do not set a password, the application will generate a random one for you!

### Decrypt

Use the `/d` option to decrypt a crypted directory.

```dos
CryptoCopy /d C:\Files_That_Are_Crypted C:\My_Uncrypted_Files /p:mySecretPassword
```

### Operations

Operation |  Aliases  | Description  
------------ | ------------- | ------------- 
/?  | /h, /help  | Shows the detailed help screen.  
/d  | /dec, /decrypt  | Decrypt directory. 
/e  | /enc, /encrypt  | Encrypt directory. 

### Options

Option |  Aliases  | Description  | Example  
------------ | ------------- | ------------- | -------------
/i  | /iterations  | The number of iterations to do (default: 1000).  | `/i:5979`
/p  | /pwd, /password  | Defines the password.  | `/p:myPassword`
/p64  | /pwd64, /password64  | Defines the password as Base64 string.  | `/p64:bXlQYXNzd29yZA==`
/s  | /salt  | Defines the salt.  | `/s:mySalt`
/s64  | /salt64  | Defines the salt as Base64 string.  | `/s64:bXlTYWx0`
