int analogPin = 0; // A0 on the board
float r1 = 10000; // 10 kOhm
int vin = 5;

int raw = 0;
float vout = 0;
float r2 = 0;
float buf = 0;

void setup() {
  Serial.begin(9600);
}

void loop() {
  raw = analogRead(analogPin);
  //Serial.println(raw);
  buf = raw * vin;
  vout = buf/1024.0;
  buf = vin/vout - 1;
  r2 = r1 * buf;
  if(vout == 0) {
    Serial.println("10000000"); // FSR-406 has max(R) = 10Mohm
  } else {
    Serial.println((int) r2);
  }
  
  delay(200);
}
