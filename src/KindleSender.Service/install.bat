SET BINPATH=%~dp0

sc create KindleSenderService binpath=%BINPATH%KindleSender.Service.exe displayname= "Kindle Sender Service" start=auto
sc description KindleSenderService "Sends files to the Kindle device."
sc start KindleSenderService