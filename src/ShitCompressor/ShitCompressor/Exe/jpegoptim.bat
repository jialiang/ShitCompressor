set inputP=%1
set outputP=%2

set arguments=%*
call set arguments=%%arguments:*%2=%%

copy %inputP% %outputP%
%0\..\jpegoptim.exe %arguments% %outputP%