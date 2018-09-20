
String instring; 
void setup() 
{ 
Serial.begin(115200); 
Serial.setTimeout(100); 
} 
void loop() 
{ 
 
instring = Serial.readString();
if(instring=="a")
{
digitalWrite(13,HIGH);
}
if(instring=="b"){
digitalWrite(13,LOW);
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
