@echo off
echo Creating code signing certificate for CarProject...
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "$cert = New-SelfSignedCertificate -Type CodeSigning -Subject 'CN=CarProject Dev, O=CarProject' -CertStoreLocation 'Cert:\CurrentUser\My' -NotAfter (Get-Date).AddYears(3);" ^
    "Write-Host 'Cert created with thumbprint:' $cert.Thumbprint;" ^
    "$pwd = ConvertTo-SecureString -String 'CarProject123' -Force -AsPlainText;" ^
    "Export-PfxCertificate -Cert ('Cert:\CurrentUser\My\' + $cert.Thumbprint) -FilePath '%~dp0CarProject\build\CarProjectCert.pfx' -Password $pwd;" ^
    "Import-PfxCertificate -FilePath '%~dp0CarProject\build\CarProjectCert.pfx' -CertStoreLocation 'Cert:\CurrentUser\TrustedPublisher' -Password $pwd;" ^
    "Write-Host 'Done! Certificate installed.'"

echo You also need admin rights to install to LocalMachine root.
echo Run this as Administrator if you have WDAC/Smart App Control enabled:
echo   certutil -addstore Root "CarProject\build\CarProjectCert.pfx"
echo   certutil -addstore TrustedPublisher "CarProject\build\CarProjectCert.pfx"
pause
