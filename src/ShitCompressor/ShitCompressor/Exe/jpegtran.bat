set inputP=%1
set outputP=%2

type %inputP% | %0\..\jpegtran.exe -copy none -optimize > %OutputP%