set inputP=%1
set outputP=%2
set quality=%3

type %inputP% | %0\..\cjpeg.exe -quality %quality% -optimize > %outputP%