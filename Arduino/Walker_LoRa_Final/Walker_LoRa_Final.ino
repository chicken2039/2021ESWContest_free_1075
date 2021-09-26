#include <RadioLib.h>

SX1262 radio = new Module(D36, D40, D44, D39);
String serial1 = "";
bool serial1Complete;
char inChar = ' ';
String str = "";

String Lat = "0000000000";
String Long = "00000000000";
String content = "pFFFFABCDABCDAAAA";

void setup() {
  Serial1.begin(9600);
  int state = radio.begin(920.0, 250.0, 7, 5, 0x34, 20, 10, 0, false);
}

void loop() {
  int state = radio.transmit(content);
  if(Serial1.available())
  {
      inChar=Serial1.read();
      if(inChar == '\n') {
        serial1Complete = true;
        if(serial1.startsWith("$GPGGA")){
          int firstData = serial1.indexOf(","); //첫 번째 콤마 전까지의 내용을 파싱 
          int secondData = serial1.indexOf(",", firstData+1); 
          int thirdData = serial1.indexOf(",", secondData+1);  
          int fourthData = serial1.indexOf(",", thirdData+1); 
          int fifthData = serial1.indexOf(",", fourthData+1); //data 추출 
          if(secondData+1 != thirdData)
            Lat = serial1.substring(secondData+1, thirdData);
          else
            Lat = "0000000000";
          if(fourthData+1 != fifthData)
            Long = serial1.substring(fourthData+1, fifthData);
          else
            Long = "00000000000";
              
          while(Lat.length() < 10)
          {
            Lat.concat("0");
          }
          while(Long.length() < 11)
          {
            Long.concat("0");
          }
          String tmpContent = "pFFFFABCDABCDAAAA";
          tmpContent.concat(Lat);
          tmpContent.concat(Long);
          while(tmpContent.length() < 38)
          {
            Long.concat("0");
          }
          content = tmpContent;
      }
      serial1 = "";
      serial1Complete = false;
    } else {
      serial1 += inChar;
    }
  } else {
     String str;
     int state = radio.receive(str);
     if (state == ERR_NONE) {
       if(radio.getRSSI() > -60){
         digitalWrite(17,HIGH);
       }
     }
  }
}
