#define L_for 2
#define L_bck 3
#define R_for 4
#define R_bck 5
int rfs=0; // Received From Server
void setup() 
{ 
  pinMode(L_for,OUTPUT);
  pinMode(L_bck,OUTPUT);
  pinMode(R_for,OUTPUT);
  pinMode(R_bck,OUTPUT);
Serial.begin(115200); 
Serial.setTimeout(10); 
} 
void loop() 
{ 

if (Serial.available() > 0) { // WCZYTUJ DANE
  
int odczyt = Serial.parseInt();

serialFlush();

if(odczyt!=0){
  rfs = odczyt;
}
Serial.println(rfs);     // NA TYM POZIOMIE MAMY INTEGERA !

//TU WYKONUJ REAKCJE ARDUINO!

if(rfs==1 || rfs==2 || rfs==3 || rfs==4 || rfs==9 || rfs==10 || rfs==11 || rfs==12){
  digitalWrite(13,HIGH);
  digitalWrite(2,HIGH);
}
else{
  digitalWrite(13,LOW);
  digitalWrite(2,LOW);
}



}



}

void serialFlush(){ // Funkcja zdejmuje resztę wiszących na serial porcie bitów bo dane i tak zostały sparsowane.
  while(Serial.available() > 0) {
    char t = Serial.read();
  }
}  
