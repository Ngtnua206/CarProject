' Chay ngam CarProject + Cloudflare khi mo may
Dim WShell
Set WShell = CreateObject("WScript.Shell")

' Doi Docker + SQL start xong
WScript.Sleep 15000

' Chay CarProject
WShell.Run "cmd /c ""D:\Code\Code\WebMVC\run\run-carproject.cmd""", 0, False

' Doi app chay
WScript.Sleep 10000

' Chay cloudflared tunnel (kem --url de khoi loi 503)
WShell.Run "cmd /c ""C:\Program Files (x86)\cloudflared\cloudflared.exe tunnel --url http://localhost:5001 run car-tunnel""", 0, False
