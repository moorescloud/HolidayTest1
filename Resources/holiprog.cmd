avrdude.exe -q -pm328 -Pusb -cavrispmkII -u -Uhfuse:w:0xDE:m -Ulfuse:w:0xFF:m -Uefuse:w:0x04:m -Uflash:w:HolidayDuino04_plus_optiboot_holiday328_20MHz.hex:i
