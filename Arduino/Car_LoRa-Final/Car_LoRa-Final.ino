#include <RadioLib.h>
#include <stdio.h>

SX1262 radio = new Module(D36, D40, D44, D39);
char ChrBufferRfsr[6] = "";

void setup() {
  //Serial.begin(9600);
  Serial1.begin(115200);
  radio.begin(920.0, 250.0, 7, 5, 0x34, 20, 10, 0, false);
}

void loop() {
  String str;
  int state = radio.receive(str);
  if (state == ERR_NONE) {
    int snrInt = radio.getSNR() * 100;
    String snr = String(snrInt);
    while(snr.length() <= 5)
      snr.concat("0");
    while(str.length() < 38 )
      str.concat("0");
    str.concat(snr);
    Serial1.println(str);
  }
  delay(100);
}
