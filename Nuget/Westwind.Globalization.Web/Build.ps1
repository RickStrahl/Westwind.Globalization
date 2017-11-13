Set-Location "$PSScriptRoot" 

Copy-Item ..\..\src\Net45\Westwind.Globalization.Web\bin\Release\Westwind.Globalization.Web.dll .\lib\net45
Copy-Item ..\..\src\Net45\Westwind.Globalization.Web\bin\Release\Westwind.Globalization.Web.xml .\lib\net45

remove-item content\LocalizationAdmin -Recurse -Force
# mkdir content\LocalizationAdmin


Copy-Item ..\..\src\Net45\Westwind.Globalization.Sample\LocalizationAdmin  content\LocalizationAdmin -Recurse -Force
Copy-Item ..\..\src\Net45\Westwind.Globalization.Sample\Properties\LocalizationForm.resx content\Properties\LocalizationForm.resx
Copy-Item ..\..\src\Net45\Westwind.Globalization.Sample\Properties\LocalizationForm.de.resx content\Properties\LocalizationForm.de.resx
pause