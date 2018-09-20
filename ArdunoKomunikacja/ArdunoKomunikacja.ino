#define L_for 2
#define L_bck 3
#define R_for 4
#define R_bck 5
String instring; 
void setup() 
{ 
  pinMode(L_for,OUTPUT);
  pinMode(L_bck,OUTPUT);
  pinMode(R_for,OUTPUT);
  pinMode(R_bck,OUTPUT);
Serial.begin(115200); 
Serial.setTimeout(100); 
} 
void loop() 
{ 
 
instring = Serial.readString();


//REAKCJA ARDUINO NA BODŹCE
if(instring=="w")
{
digitalWrite(L_for,HIGH);
digitalWrite(R_for,HIGH);
}
if(instring=="s"){
digitalWrite(L_bck,HIGH);
digitalWrite(R_bck,HIGH);
}
if(instring=="s_stop"){
digitalWrite(L_bck,LOW);
digitalWrite(R_bck,LOW);
}

if(instring=="w_stop"){
digitalWrite(L_for,LOW);
digitalWrite(R_for,LOW);
}




if(instring.length()>0)
{
  wyslij(instring.substring(0, instring.length()-2));
  instring="";
}


}
void wyslij(String co) 
{ 
Serial.print(co); 
} 
