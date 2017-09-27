/* PC Monitor V02 by Fegyi */

/*TODO-LIST:  error handling, maximized lines, custom characters, pinpad */

#include <LiquidCrystal.h>
#include <SimpleTimer.h>

/* LONG COMMAND STATES */
#define CMD_CODE    0 
#define CMD_LEN      1
#define CMD_DATA    2
#define CMD_END      3

/* COMMAND CODES */
#define C_WRITELN0    'a'
#define C_WRITELN1    'b'
#define C_WRITELN2    'c'
#define C_WRITELN3    'd'
#define C_CLEARSCR    'C'
#define C_BACKLGHT    'B'
//TODO
#define C_ANSWER      'A'
#define C_CUSTCH0     '0'
#define C_CUSTCH1     '1'
#define C_CUSTCH2     '2'
#define C_CUSTCH3     '3'
#define C_CUSTCH4     '4'
#define C_CUSTCH5     '5'
#define C_CUSTCH6     '6'
#define C_CUSTCH7     '7'

/* FIRMWARE CONFIGURATION */
#define ENABLE_SERIAL_DEBUGGING 1

#define CONNECTION_TIMEOUT 30000

#define UART_BAUDRATE 19200

#define CONNECTED_BACKLIGHT 100
#define SLEEP_BACKLIGHT 10

boolean isConnected;
int commandState;
int connectTimerID;
/* --- LCD ------------------------------------------------------------------------------------------------------------------------ */
LiquidCrystal lcd(12, 11, 5, 4, 3, 2);

void initLCD()
{
  lcd.begin(20, 4);
  pinMode(6,OUTPUT); /* Backlight connected to D6 */
}

void setBacklight(char value)
{
  if(0 < value && 255 > value)
  {
    analogWrite(6, value);
  }
}

void writeLine(int line_num, String s)
{
  lcd.setCursor(0,line_num);
  lcd.print(s);
}

/* --- Graphics ------------------------------------------------------------------------------------------------------------------- */

/* --- Peripherals ---------------------------------------------------------------------------------------------------------------- */
void beginUART(int baud)
{
  writeLine(1, "Program started...");
  Serial.begin(baud);
  delay(500);
  lcd.clear(); 
}

/* --- Setup --------------------------------------------------------------------------------------------------------------------- */
void setup() 
{
  initLCD();
  setBacklight(10);
  beginUART(UART_BAUDRATE);
  isConnected = false;
  commandState = CMD_END;
}

void connectionTimeoutCallback()
{
  if(ENABLE_SERIAL_DEBUGGING)
  {
    Serial.write("Connection timeout.\n");
  }
  isConnected = false;
  setBacklight(SLEEP_BACKLIGHT);
}


/* --- Command interpreter -------------------------------------------------------------------------------------------------------- */
SimpleTimer connectTimer;
unsigned long connectTimerTimestamp;

char act_cmd;
int data_idx;

int timeout_val_old;

/* Returns 'false' if command not recognized, otherwise returns 'true'      */
boolean processLongCmd(char c)
{
    boolean retval = false;
    int numeric_attr;
    switch(c)
    {
        /* Connect or ping (in case no ongoing command processing!) */
        case '+':
        {
            if((isConnected == false) && (commandState == CMD_END))
            {
                if(ENABLE_SERIAL_DEBUGGING)
                {
                    Serial.write("connect command\n");
                    Serial.write("connected, start ");
                    Serial.print(String(CONNECTION_TIMEOUT/1000));
                    Serial.write(" s timer...\n");
                }
                setBacklight(CONNECTED_BACKLIGHT);
                //lcd.clear();
                isConnected = true;
                connectTimerID = connectTimer.setTimeout(CONNECTION_TIMEOUT,connectionTimeoutCallback);
                connectTimerTimestamp = millis();
            }
            else if((isConnected == true) && (commandState == CMD_END))
            {
                if(ENABLE_SERIAL_DEBUGGING)
                {
                    Serial.write("connected yet, restart ");
                    Serial.print(String(CONNECTION_TIMEOUT/1000));
                    Serial.write(" s timer...\n");                    
                }
                connectTimer.restartTimer(connectTimerID);
                connectTimerTimestamp = millis();
            }
        }
        break;

        /* Disconnect if connect (in case no ongoing command processing!) */
        case '-':
        {
            if((isConnected == true) && (commandState == CMD_END))
            {
                if(ENABLE_SERIAL_DEBUGGING)
                {
                    Serial.write("disconnect command\n");
                }
                connectTimer.setTimeout(1,connectionTimeoutCallback);
            }
        }
        break;
        
        /* Begin command */
        case '[':
        {
            if(commandState == CMD_END)
            {
                if(ENABLE_SERIAL_DEBUGGING)
                {
                    Serial.write("command begin character detected!\n");
                }
                /* (re)initialize command parameters */
                act_cmd = CMD_CODE;
                commandState = CMD_CODE;    
            }
        }
        break;
        
        /* End command */
        case ']':
        {
            if(ENABLE_SERIAL_DEBUGGING)
            {
                Serial.write("command end character detected!\n");
            }
            commandState = CMD_END;
        }
        break;
        
        /* Process command */
        default:
        {
            if(isConnected == true)
            {
                /* restart connection timer */
                connectTimer.restartTimer(connectTimerID);
                connectTimerTimestamp = millis();
                if(ENABLE_SERIAL_DEBUGGING)
                {
                    Serial.write("character belongs to a longer command!\n");
                }
                switch(commandState)
                {         
                    case CMD_CODE:
                    {
                       if(ENABLE_SERIAL_DEBUGGING)
                       {
                           Serial.write("command code: ");
                           Serial.write(c);
                           Serial.write(" recognized!\n");
                       }
                       /* Get command */
                       act_cmd = c; 
                       data_idx = 0;
                       commandState = CMD_DATA;
                    }
                    break;
                    
                    case CMD_DATA:
                    {                   
                        if(ENABLE_SERIAL_DEBUGGING)
                        {
                            Serial.write("command data:\n");
                            Serial.write(c);
                            Serial.write(" = ");
                        }
                        /* Process data */
                        switch(act_cmd)
                        {
                            /* Write character to the 1st line */
                            case C_WRITELN0:
                            {
                                if(ENABLE_SERIAL_DEBUGGING)
                                {
                                    Serial.write("write data to Line0 ");
                                    Serial.write("( ");
                                    Serial.write(data_idx);
                                    Serial.write(" )\n");
                                }
                                /* avoid overflow to other line */
                                if(data_idx < 20)
                                {
                                    lcd.setCursor(data_idx,0);
                                    lcd.write(c);
                                    data_idx++;
                                }
                            }
                            break;
                            /* Write character to the 2nd line */
                            case C_WRITELN1:
                            {
                                if(ENABLE_SERIAL_DEBUGGING)
                                {
                                    Serial.write("write data to Line1 ");
                                    Serial.write("( ");
                                    Serial.write(data_idx);
                                    Serial.write(" )\n");
                                }
                                /* avoid overflow to other line */
                                if(data_idx < 20)
                                {
                                    lcd.setCursor(data_idx,1);
                                    lcd.write(c);
                                    data_idx++;
                                }
                            }
                            break;
                            /* Write character to the 3rd line */
                            case C_WRITELN2:
                            {
                                if(ENABLE_SERIAL_DEBUGGING)
                                {
                                    Serial.write("write data to Line2 ");
                                    Serial.write("( ");
                                    Serial.write(data_idx);
                                    Serial.write(" )\n");
                                }
                                /* avoid overflow to other line */
                                if(data_idx < 20)
                                {
                                    lcd.setCursor(data_idx,2);
                                    lcd.write(c);
                                    data_idx++;
                                }
                            }
                            break;
                            /* Write character to the 4th line */
                            case C_WRITELN3:
                            {
                                if(ENABLE_SERIAL_DEBUGGING)
                                {
                                    Serial.write("write data to Line3 ");
                                    Serial.write("( ");
                                    Serial.write(data_idx);
                                    Serial.write(" )\n");
                                }
                                /* avoid overflow to other line */
                                if(data_idx < 20)
                                {
                                    lcd.setCursor(data_idx,3);
                                    lcd.write(c);
                                    data_idx++;
                                }
                            }
                            break;
                            /* Clear screen */
                            case C_CLEARSCR:
                            {
                                if(ENABLE_SERIAL_DEBUGGING)
                                {
                                    Serial.write("clear screen\n");
                                }
                                lcd.clear();
                            }
                            break;
                            /* Set backlight */
                            case C_BACKLGHT:
                            {
                                if(ENABLE_SERIAL_DEBUGGING)
                                {
                                    Serial.write("set backlight value to ");
                                    Serial.write("( ");
                                    Serial.write((uint8_t)c);
                                    Serial.write(" )\n");
                                }
                                setBacklight((uint8_t)c);
                            }
                            break;
                        }
                    }
                    break;
                    
                    case CMD_END:
                    {
                        //TODO: send response - unknown command
                    }
                    break;
                }
            }
        }
        break;
    }
    
    return retval;
}

/* --- Connection checker -------------------------------------------------------- */
void connectionCheck()
{
  unsigned long timeout_val;
  String str;
  if(isConnected)
  {
    connectTimer.run();
    timeout_val = CONNECTION_TIMEOUT - ((millis() - connectTimerTimestamp));
    timeout_val = (timeout_val/1000);
    
    if(ENABLE_SERIAL_DEBUGGING && (timeout_val != timeout_val_old) && ( ((timeout_val % 5) == 0) || (timeout_val < 5)))
    {
        
        Serial.write("Timeout in: ");
        Serial.print(String(timeout_val));
        Serial.write(" s\n");
    }
    
    timeout_val_old = timeout_val;
  }
}

/* --- MAIN LOOP ----------------------------------------------------------------------------------------------------------------- */
char c;
String str;
int cnt;
void loop() 
{
  c = Serial.read();
  if( c != -1)
  {
    if(ENABLE_SERIAL_DEBUGGING)
    {
        Serial.write("Detected character: ");
        Serial.write(c);
        Serial.write("\n"); 
    }
    (void)processLongCmd(c);   
  }
  connectionCheck();
}

