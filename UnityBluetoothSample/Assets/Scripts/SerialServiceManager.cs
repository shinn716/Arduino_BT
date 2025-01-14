﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Text;

public class SerialServiceManager
{
    private SerialPort stream;
    private Thread thread;

    public delegate void Callback();
    public Callback callback;

    public string stringData { get; set; }

    public SerialServiceManager(string port = "COM4", int baudrate = 9600)
    {
        // Get a list of serial port names.
        string[] ports = SerialPort.GetPortNames();
        Debug.Log("[Log]The following serial ports were found:");
        
        foreach (var i in ports)
            Debug.Log(i);

        Debug.Log($"[Log]Connect to {port} / {baudrate}");

        Task task01 = Task.Run(() =>
        {
            stream = new SerialPort(port, baudrate);
            stream.ReadTimeout = 50;
            stream.Open();
        });

        task01.Wait();

        Task task02 = Task.Run(() =>
        {
            thread = new Thread(new ThreadStart(Read));
            thread.Start();
        });
    }

    public void SendMessage(string message)
    {
        //stream.Write(message);
        byte[] bytes = Encoding.ASCII.GetBytes(message);
        stream.Write(bytes, 0, bytes.Length);
    }

    public void Dispose()
    {
        stream.Close();
        thread.Join();
    }

    private void Read()
    {
        while (stream.IsOpen)
        {
            try
            {
                stringData = stream.ReadLine();
                callback?.Invoke();                     // net 4.0
            }

            catch (Exception)
            {
                stringData = null;
                //Debug.Log("[LOG]Serial is not ready." + e);
            }
        }
    }
}
