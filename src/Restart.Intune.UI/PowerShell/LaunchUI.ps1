$RootFolder = "C:\DEV\Restart.Intune\src\artifacts\windows"

$ServiceUIFile = "$($RootFolder)\Dependencies\serviceui.exe"
$ExeFile = "$($RootFolder)\Restart.Intune.App.exe"

$Deadline = ""
$Theme = ""
$TotalTime = ""
$Version = "v2.0.0"
$Window = ""

$Arguments = "-nowait -process:explorer.exe `"$ExeFile`" --Theme ""\""$($Theme)""\"" --Deadline ""\""$($Deadline)""\"" --TotalTime ""\""$($TotalTime)""\"" --Version ""\""$($Version)""\"" --Window ""\""$($Window)""\"""

Write-Host "ServiceUI: $ServiceUIFile"
Write-Host "Arguments: $Arguments"

Start-Process -FilePath $ServiceUIFile -ArgumentList $Arguments