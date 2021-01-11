$InstallerDir = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer"
$VsWhere = "$InstallerDir\vswhere.exe"
$MSBuildPath = (&"$VsWhere" -latest -products * -find "\MSBuild\Current\Bin\MSBuild.exe").Trim()
$Root = (Get-Item -Path $PSScriptRoot).Parent.FullName
$DesktopPath = [Environment]::GetFolderPath("Desktop")
$WixDir = "C:\Program Files (x86)\WiX Toolset v3.11\bin\"
$Version = "1.3.0.0"

if ([string]::IsNullOrWhiteSpace($MSBuildPath) -or !(Test-Path -Path $MSBuildPath)) {
    Write-Host
    Write-Host "ERROR: Unable to find the path to MSBuild.exe." -ForegroundColor Red
    Write-Host
    pause
    return
}



& "$MSBuildPath" "$Root\Toems\Toems-FrontEnd" /t:webpublish /p:WebPublishMethod=FileSystem /p:publishUrl="$DesktopPath\Theopenem Installer\Toems Application\Program Files\Toems-UI" /p:DeleteExistingFiles=True /p:Configuration=Release /p:Platform=x64
& "$MSBuildPath" "$Root\Toems\Toems-ApplicationApi" /t:webpublish /p:WebPublishMethod=FileSystem /p:publishUrl="$DesktopPath\Theopenem Installer\Toems Application\Program Files\Toems-API" /p:DeleteExistingFiles=True /p:Configuration=Release /p:Platform=x64
& "$MSBuildPath" "$Root\Toems\Toems-ClientApi" /t:webpublish /p:WebPublishMethod=FileSystem /p:publishUrl="$DesktopPath\Theopenem Installer\Toems Client API\Program Files\Toec-API" /p:DeleteExistingFiles=True /p:Configuration=Release /p:Platform=x64

Invoke-WebRequest https://github.com/theopenem/Toems-MSI/archive/main.zip -OutFile "$DesktopPath\Theopenem Installer\msisrc.zip"
Expand-Archive -Path "$DesktopPath\Theopenem Installer\msisrc.zip" -DestinationPath "$DesktopPath\Theopenem Installer\" -Force


cd "$DesktopPath\Theopenem Installer\"
Copy-Item Toems-MSI-main\* -Destination .\ -Force -Recurse
del Toems-MSI-main -Force -Recurse
del msisrc.zip -Force

mkdir "Toems Application\Program Files\Toems-API\private\logs"
mkdir "Toems Application\Program Files\Toems-API\private\agent"
mkdir "Toems Application\Program Files\Toems-UI\private\logs"
mkdir "Toems Client API\Program Files\Toec-API\private\logs"

cd "$Root\Toec\Toec-Installer"

& "$MSBuildPath" "$Root\Toec\Toec" /t:build /p:Configuration=Release /p:Platform=x64
& "$MSBuildPath" "$Root\Toec\Toec" /t:build /p:Configuration=Release /p:Platform=x86

& "$MSBuildPath" "$Root\Toec\Toec-InstallHelper" /t:build /p:Configuration=Release /p:Platform=x64
& "$MSBuildPath" "$Root\Toec\Toec-InstallHelper" /t:build /p:Configuration=Release /p:Platform=x86

& "$MSBuildPath" "$Root\Toec\Toec-UI" /t:build /p:Configuration=Release /p:Platform=x64
& "$MSBuildPath" "$Root\Toec\Toec-UI" /t:build /p:Configuration=Release /p:Platform=x86

& "$WixDir\candle.exe" -dSolutionDir="$Root\Toec\" `
-dSolutionExt=".sln" `
-dSolutionFileName="theopenem-client.sln" `
-dSolutionName="theopenem-client" `
-dSolutionPath="$root\Toec\theopenem-client.sln" `
-dConfiguration=Release `
-dOutDir=bin\x64\Release\ `
-dPlatform=x64 `
-dProjectDir="$Root\Toec\Toec-Installer\" `
-dProjectExt=".wixproj" `
-dProjectFileName="Toec-Installer.wixproj" `
-dProjectName=Toec-Installer `
-dProjectPath="$Root\Toec\Toec-Installer\Toec-Installer.wixproj" `
-dTargetDir="$DesktopPath\Theopenem Installer" `
-dTargetExt=".msi" `
-dTargetFileName="Toec-$Version-x64.msi" `
-dTargetName="Toec-$Version-x64" `
-dTargetPath="$DesktopPath\Theopenem Installer\Toec-$Version-x64.msi" `
-out obj\Release\ `
-arch x64 `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixUIExtension.dll" `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixNetFxExtension.dll" `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixUtilExtension.dll" `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixIIsExtension.dll" Product.wxs

& "$WixDir\Light.exe" -out "$DesktopPath\Theopenem Installer\Toec-$Version-x64.msi" `
-pdbout C:\Development\Toec\Toec-Installer\bin\x64\Release\Installer.wixpdb `
-cultures:null `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixUIExtension.dll" `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixNetFxExtension.dll" `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixUtilExtension.dll" `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixIIsExtension.dll" `
-contentsfile obj\Release\Toec-Installer.wixproj.BindContentsFileListnull.txt `
-outputsfile obj\Release\Toec-Installer.wixproj.BindOutputsFileListnull.txt `
-builtoutputsfile obj\Release\Toec-Installer.wixproj.BindBuiltOutputsFileListnull.txt `
-wixprojectfile C:\Development\Toec\Toec-Installer\Toec-Installer.wixproj obj\Release\Product.wixobj


#x86 Client

& "$WixDir\candle.exe" -dSolutionDir="$Root\Toec\" `
-dSolutionExt=".sln" `
-dSolutionFileName="theopenem-client.sln" `
-dSolutionName="theopenem-client" `
-dSolutionPath="$root\Toec\theopenem-client.sln" `
-dConfiguration=Release `
-dOutDir=bin\x86\Release\ `
-dPlatform=x86 `
-dProjectDir="$Root\Toec\Toec-Installer\" `
-dProjectExt=".wixproj" `
-dProjectFileName="Toec-Installer.wixproj" `
-dProjectName=Toec-Installer `
-dProjectPath="$Root\Toec\Toec-Installer\Toec-Installer.wixproj" `
-dTargetDir="$DesktopPath\Theopenem Installer" `
-dTargetExt=".msi" `
-dTargetFileName="Toec-$Version-x86.msi" `
-dTargetName="Toec-$Version-x86" `
-dTargetPath="$DesktopPath\Theopenem Installer\Toec-$Version-x86.msi" `
-out obj\Release\ `
-arch x86 `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixUIExtension.dll" `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixNetFxExtension.dll" `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixUtilExtension.dll" `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixIIsExtension.dll" Product.wxs

& "$WixDir\Light.exe" -out "$DesktopPath\Theopenem Installer\Toec-$Version-x86.msi" `
-pdbout C:\Development\Toec\Toec-Installer\bin\x86\Release\Installer.wixpdb `
-cultures:null `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixUIExtension.dll" `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixNetFxExtension.dll" `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixUtilExtension.dll" `
-ext "C:\Program Files (x86)\WiX Toolset v3.11\bin\\WixIIsExtension.dll" `
-contentsfile obj\Release\Toec-Installer.wixproj.BindContentsFileListnull.txt `
-outputsfile obj\Release\Toec-Installer.wixproj.BindOutputsFileListnull.txt `
-builtoutputsfile obj\Release\Toec-Installer.wixproj.BindBuiltOutputsFileListnull.txt `
-wixprojectfile C:\Development\Toec\Toec-Installer\Toec-Installer.wixproj obj\Release\Product.wixobj

mv "$DesktopPath\Theopenem Installer\Toec-$Version-x64.msi" "$DesktopPath\Theopenem Installer\Toems Application\Program Files\Toems-API\private\agent\"
mv "$DesktopPath\Theopenem Installer\Toec-$Version-x86.msi" "$DesktopPath\Theopenem Installer\Toems Application\Program Files\Toems-API\private\agent\"