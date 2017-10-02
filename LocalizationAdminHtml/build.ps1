Set-Location "$PSScriptRoot" 


$sampleRoot = "..\src\NetCore\WestWind.Globalization.Sample.AspNetCore"
remove-item content -Recurse -Force

mkdir content\wwwroot\LocalizationAdmin    
mkdir content\Properties

Copy-Item $sampleRoot\wwwroot\LocalizationAdmin  content\wwwroot\LocalizationAdmin -Recurse -Force
Copy-Item $sampleRoot\Properties\LocalizationForm.resx content\Properties\LocalizationForm.resx
Copy-Item $sampleRoot\Properties\LocalizationForm.de.resx content\Properties\LocalizationForm.de.resx

"Zipping up portable setup file..."
Remove-Item "LocalizationAdministrationHtml_AspnetCore.zip"
7z a -tzip -r "LocalizationAdministrationHtml_AspNetCore.zip" ".\content\*.*"