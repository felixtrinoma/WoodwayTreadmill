/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

using UnityEngine;
using System;
using System.Threading;
using System.Linq;
using System.Collections;

/**
 * This class allows a Unity program to continually check for messages from a
 * serial device.
 *
 * It creates a Thread that communicates with the serial port and continually
 * polls the messages on the wire.
 * That Thread puts all the messages inside a Queue, and this SerialController
 * class polls that queue by means of invoking SerialThread.GetSerialMessage().
 *
 * The serial device must send its messages separated by a newline character.
 * Neither the SerialController nor the SerialThread perform any validation
 * on the integrity of the message. It's up to the one that makes sense of the
 * data.
 */
public class SerialController : MonoBehaviour
{
    [Tooltip("Port name with which the SerialPort object will be created.")]
    public string portName = "COM3";

    [Tooltip("Baud rate that the serial device is using to transmit data.")]
    public int baudRate = 9600;

    [Tooltip("Reference to an scene object that will receive the events of connection, " +
             "disconnection and the messages from the serial device.")]
    public GameObject messageListener;

    [Tooltip("After an error in the serial communication, or an unsuccessful " +
             "connect, how many milliseconds we should wait.")]
    public int reconnectionDelay = 1000;

    [Tooltip("Maximum number of unread data messages in the queue. " +
             "New or old (depending on \"Drop Old Message\" configuration) messages will be discarded.")]
    public int maxUnreadMessages = 1;

    [Tooltip("When the queue is full, prefer dropping the oldest message in the queue " +
             "instead of the new incoming message. Use this if you prefer to keep the " +
             "newest messages from the port.")]
    public bool dropOldMessage;

    public object[] message;
    public byte[] numMessage;

    // Constants used to mark the start and end of a connection. There is no
    // way you can generate clashing messages from your serial device, as I
    // compare the references of these strings, no their contents. So if you
    // send these same strings from the serial device, upon reconstruction they
    // will have different reference ids.
    public const string SERIAL_DEVICE_CONNECTED = "__Connected__";
    public const string SERIAL_DEVICE_DISCONNECTED = "__Disconnected__";

    //state byte arrays
    private byte[] enable = { 169, 0 }, stop = { 168, 0 }, getSpeed = { 199, 0 }, getSlope = { 200, 0 };
    // set byte arrays used to send messages to treadmill. First and last values remain unchanged
    private byte[] setSpeed = { 163, 48, 48, 48, 48, 13 }, setSlope = { 164, 48, 48, 48, 48, 13 };
    // byte arrays of response from treadmill to a given command
    private byte[] enabledMsg = { 169, 13 }, stoppedMsg = { 168, 13 }, speedSetMsg = { 163, 13 }, slopeSetMsg = { 164, 13 };
    // speed and slope responses from treadmill. 
    private byte[] tempSpeedMsg = { 48, 48, 48, 48 }, tempSlopeMsg = { 48, 48, 48, 48};
    public byte[] speedMsg = { 48, 48, 48, 48 }, slopeMsg = { 48, 48, 48, 48 };
    // O value used to stop treadmill
    private byte[] speedStop = { 48, 48, 48, 48 };

    private byte spByte = 199;
    private byte slByte = 200;
    private byte stopByte = 13;

    public byte[] toSend;

    //state bools
    public bool connect = false, isEnabled = false;
    // read/write state bools
    public bool rSpeed = false, rSlope = false, wSpeed = false, wSlope = false;
    // serial response bools used to check is answer to a command has been received
    public bool speedSR = true, slopeSR = true, stopSR;


    // Internal reference to the Thread and the object that runs in it.
    protected Thread thread;
    protected SerialThreadLines serialThread;


    // ------------------------------------------------------------------------
    // Invoked whenever the SerialController gameobject is activated.
    // It creates a new thread that tries to connect to the serial device
    // and start reading from it.
    // ------------------------------------------------------------------------
    void OnEnable()
    {
        serialThread = new SerialThreadLines(portName,
                                             baudRate,
                                             reconnectionDelay,
                                             maxUnreadMessages,
                                             dropOldMessage);
        thread = new Thread(new ThreadStart(serialThread.RunForever));
        thread.Start();
    }

    // ------------------------------------------------------------------------
    // Invoked whenever the SerialController gameobject is deactivated.
    // It stops and destroys the thread that was reading from the serial device.
    // ------------------------------------------------------------------------
    void OnDisable()
    {
        // If there is a user-defined tear-down function, execute it before
        // closing the underlying COM port.
        if (userDefinedTearDownFunction != null)
            userDefinedTearDownFunction();

        // The serialThread reference should never be null at this point,
        // unless an Exception happened in the OnEnable(), in which case I've
        // no idea what face Unity will make.
        if (serialThread != null)
        {
            serialThread.RequestStop();
            serialThread = null;
        }

        // This reference shouldn't be null at this point anyway.
        if (thread != null)
        {
            thread.Join();
            thread = null;
        }
    }

    // ------------------------------------------------------------------------
    // Polls messages from the queue that the SerialThread object keeps. Once a
    // message has been polled it is removed from the queue. There are some
    // special messages that mark the start/end of the communication with the
    // device.
    // ------------------------------------------------------------------------


    void Start()
    {
        object initMessage = ReadSerialMessage();
        if (ReferenceEquals(initMessage, SERIAL_DEVICE_CONNECTED))
        {
            connect = true;
            Debug.Log("Connection established");
        }
        else if (ReferenceEquals(initMessage, SERIAL_DEVICE_DISCONNECTED))
            Debug.Log("Connection attempt failed or disconnection detected");

        toSend = getSpeed;
        toSend = ResizeArray(toSend, getSlope);
    }

    void FixedUpdate()
    {
        toSend = getSpeed;
        toSend = ResizeArray(toSend, getSlope);

        if (connect)
        {          
            message = (object[]) ReadSerialMessage();

            if(message != null)
            {
                numMessage = new byte[message.Length];

                int i = 0;
                foreach (object obj in message)
                {
                    numMessage[i] = Convert.ToByte(obj);
                    i++;
                }

                if (!isEnabled)
                {
                    if (numMessage.SequenceEqual(enabledMsg))
                        isEnabled = true;
                }
                else
                {
                    byte[] testMsg = new byte[2];
                    for(int j = 0; j < numMessage.Length-1; j++)
                    {
                        Array.Copy(numMessage, j, testMsg, 0, 2);
                        if (testMsg.SequenceEqual(stoppedMsg))
                        {
                            isEnabled = false;
                            wSpeed = false;
                            wSlope = false;

                            speedSR = true;
                            slopeSR = true;
                        }
                           
                            
                        if (testMsg.SequenceEqual(speedSetMsg))
                        {
                            wSpeed = false;
                            speedSR = true;
                        }
                            
                        if (testMsg.SequenceEqual(slopeSetMsg))
                        {
                            wSlope = false;
                            slopeSR = true;
                        }
                            
                    }

                    for (int k = 0; k < numMessage.Length - 5; k++)
                    {
                        if (numMessage[k].Equals(spByte) && numMessage[k + 5].Equals(stopByte))
                        {
                            tempSpeedMsg[0] = numMessage[k + 1];
                            tempSpeedMsg[1] = numMessage[k + 2];
                            tempSpeedMsg[2] = numMessage[k + 3];
                            tempSpeedMsg[3] = numMessage[k + 4];
                        }
                        if (numMessage[k].Equals(slByte) && numMessage[k + 5].Equals(stopByte))
                        {
                            tempSlopeMsg[0] = numMessage[k + 1];
                            tempSlopeMsg[1] = numMessage[k + 2];
                            tempSlopeMsg[2] = numMessage[k + 3];
                            tempSlopeMsg[3] = numMessage[k + 4];
                        }
                    }

                    speedMsg = tempSpeedMsg;
                    slopeMsg = tempSlopeMsg;

                }

            }

            if (isEnabled)
            {
                if (wSpeed)
                {
                    toSend = ResizeArray(toSend, setSpeed);
                    speedSR = false;
                }

                if (wSlope)
                {
                    toSend = ResizeArray(toSend, setSlope);
                    slopeSR = false;
                }


                SendSerialMessage(toSend);
            }
                
        }  
    }

    /// <summary>sends the unable message to treadmill</summary>
    public void EnableTreadmill()
    {
        SendSerialMessage(enable);
    }

    /// <summary>sends to stop message to treadmill</summary>
    public void StopTreadmill()
    {
        if (isEnabled)// && !(speedMsg.SequenceEqual(speedStop)))
        {
            SendSerialMessage(stop);
            speedMsg = speedStop;
        }
         
        
    }

    /// <summary>write the speed value to the setSpeed byte array</summary>
    /// <param><c>speedToSet</c>byte of the speed value to set</param>
    public void SendSpeedTreadmill(byte[] speedToSet)
    {
        if (speedSR)
        {
            setSpeed[1] = speedToSet[0];
            setSpeed[2] = speedToSet[1];
            setSpeed[3] = speedToSet[2];
            setSpeed[4] = speedToSet[3];

            wSpeed = true;
        }
        
    }

    /// <summary>write the slope value to the setSlope byte array</summary>
    /// <param><c>slopeToSet</c>byte of the slope value to set</param>
    public void SendSlopeTreadmill(byte[] slopeToSet)
    {
        if (slopeSR)
        {
            setSlope[1] = slopeToSet[0];
            setSlope[2] = slopeToSet[1];
            setSlope[3] = slopeToSet[2];
            setSlope[4] = slopeToSet[3];

            wSlope = true;
        }
        
    }

    // ------------------------------------------------------------------------
    // Returns a new byte array by fusioning the argument byte arrays
    // ------------------------------------------------------------------------
    private byte[] ResizeArray(byte[] first, byte[] second)
    {
        Array.Resize(ref first, first.Length + second.Length);
        Array.Copy(second, 0, first, first.Length - second.Length, second.Length);
        return first;
    }

    // ------------------------------------------------------------------------
    // Returns a new unread message from the serial device. You only need to
    // call this if you don't provide a message listener.
    // ------------------------------------------------------------------------
    public object ReadSerialMessage()
    {
        // Read the next message from the queue
        object[] response = serialThread.ReadMessage();
        if (response != null)
        {
            if (response.Length == 1)
                return response[0];
            return response;
        }
        else
            return null;
        
    }

    // ------------------------------------------------------------------------
    // Puts a message in the outgoing queue. The thread object will send the
    // message to the serial device when it considers it's appropriate.
    // ------------------------------------------------------------------------
    public void SendSerialMessage(byte[] message)
    {
        serialThread.SendMessage(message,message.Length);
    }

    // ------------------------------------------------------------------------
    // Executes a user-defined function before Unity closes the COM port, so
    // the user can send some tear-down message to the hardware reliably.
    // ------------------------------------------------------------------------
    public delegate void TearDownFunction();

    private TearDownFunction userDefinedTearDownFunction;

    public void SetTearDownFunction(TearDownFunction userFunction)
    {
        this.userDefinedTearDownFunction = userFunction;
    }

    

}
