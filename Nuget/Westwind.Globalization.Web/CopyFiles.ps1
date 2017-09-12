Set-Location "$PSScriptRoot" 

Copy-Item ..\..\Westwind.Globalization.Web\bin\Release\Westwind.Globalization.Web.dll .\lib\net45
Copy-Item ..\..\Westwind.Globalization.Web\bin\Release\Westwind.Globalization.Web.xml .\lib\net45

remove-item content\LocalizationAdmin -Recurse -Force
# mkdir content\LocalizationAdmin


Copy-Item ..\..\Westwind.Globalization.Sample\LocalizationAdmin  content\LocalizationAdmin -Recurse -Force
Copy-Item ..\..\Westwind.Globalization.Sample\Properties\LocalizationForm.resx content\Properties\LocalizationForm.resx
Copy-Item ..\..\Westwind.Globalization.Sample\Properties\LocalizationForm.de.resx content\Properties\LocalizationForm.de.resx
