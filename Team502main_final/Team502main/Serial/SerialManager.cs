using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace Team502main.Serial
{
    class SerialManager
    {
        private SerialDevice serialPort = null;
        DataWriter dataWriteObject = null;
        DataReader dataReaderObject = null;

        private ObservableCollection<DeviceInformation> listOfDevices;
        private CancellationTokenSource ReadCancellationTokenSource;
        private ISerialReadNotification serialReadNotification;
        public SerialManager(ISerialReadNotification serialReadNotification)
        {
            listOfDevices = new ObservableCollection<DeviceInformation>();
            this.serialReadNotification = serialReadNotification;
            ListAvailablePorts();
        }
        /// <summary>
        /// ListAvailablePorts
        /// - Use SerialDevice.GetDeviceSelector to enumerate all serial devices
        /// - Attaches the DeviceInformation to the ListBox source so that DeviceIds are displayed
        /// </summary>
        private async void ListAvailablePorts()
        {
            try
            {
                string aqs = SerialDevice.GetDeviceSelector();
                DeviceInformationCollection dis = await DeviceInformation.FindAllAsync(aqs);

                for (int i = 0; i < dis.Count; i++)
                {
                    listOfDevices.Add(dis[i]);
                    //Debug.WriteLine(dis[i].Id);
                }

                //DeviceListSource.Source = listOfDevices;
            }
            catch (Exception)
            {
                //status.Text = ex.Message;
            }
            ConnectToUsbSerial();
        }

        /// <summary>
        /// comPortInput_Click: Action to take when 'Connect' button is clicked
        /// - Get the selected device index and use Id to create the SerialDevice object
        /// - Configure default settings for the serial port
        /// - Create the ReadCancellationTokenSource token
        /// - Start listening on the serial port input
        /// </summary>

        private async void ConnectToUsbSerial()
        {
            DeviceInformation entry = null;
            foreach (var item in listOfDevices)
            {
                if (item.Id.Contains("USB"))
                {
                    entry = item;
                }
            }

            try
            {
                //string aqs = SerialDevice.GetDeviceSelector();
                //var dis = await DeviceInformation.FindAllAsync(aqs);
                /*
                Debug.WriteLine(entry.Id);
                Debug.WriteLine(entry.IsEnabled);
                Debug.WriteLine(entry.EnclosureLocation);
                Debug.WriteLine(entry.Kind);
                Debug.WriteLine(entry.Name);
                Debug.WriteLine(entry.Properties);
                */
                serialPort = await SerialDevice.FromIdAsync(entry.Id);

                if (serialPort == null)
                {
                    Debug.WriteLine("Serial null");
                    return;
                }
                Debug.WriteLine("Connect to Serial");

                // Disable the 'Connect' button 
                // Configure serial settings
                serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                serialPort.BaudRate = 115200;
                serialPort.Parity = SerialParity.Even;
                serialPort.StopBits = SerialStopBitCount.Two;
                serialPort.DataBits = 8;
                serialPort.Handshake = SerialHandshake.None;

                // Create cancellation token object to close I/O operations when closing the device
                ReadCancellationTokenSource = new CancellationTokenSource();

                Listen();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// - Create a DataReader object
        /// - Create an async task to read from the SerialDevice InputStream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Listen()
        {
            try
            {
                if (serialPort != null)
                {
                    dataReaderObject = new DataReader(serialPort.InputStream);

                    // keep reading the serial input
                    while (true)
                    {
                        await ReadAsync(ReadCancellationTokenSource.Token, serialReadNotification);
                    }
                }
            }
            catch (TaskCanceledException tce)
            {
                CloseDevice();
            }
            catch (Exception ex)
            {
                //status.Text = ex.Message;
            }
            finally
            {
                // Cleanup once complete
                if (dataReaderObject != null)
                {
                    dataReaderObject.DetachStream();
                    dataReaderObject = null;
                }
            }
        }

        /// <summary>
        /// ReadAsync: Task that waits on data and reads asynchronously from the serial device InputStream
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task ReadAsync(CancellationToken cancellationToken, ISerialReadNotification serialReadNotification)
        {
            Task<UInt32> loadAsyncTask;

            uint ReadBufferLength = 1;

            // If task cancellation was requested, comply
            cancellationToken.ThrowIfCancellationRequested();

            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            dataReaderObject.InputStreamOptions = InputStreamOptions.ReadAhead;

            using (var childCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                try
                {
                    // Create a task object to wait for data on the serialPort.InputStream
                    loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(childCancellationTokenSource.Token);

                    // Launch the task and wait
                    uint bytesRead = await loadAsyncTask;
                    if (bytesRead == ReadBufferLength)
                    {
                        try
                        {
                            var data = dataReaderObject.ReadString(bytesRead);
                            // if(data.EndsWith('\n'))
                            serialReadNotification.OnMessageReceive(data);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);

                        }
                    }
                }
                catch (Exception ex) {
                    Debug.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// CancelReadTask:
        /// - Uses the ReadCancellationTokenSource to cancel read operations
        /// </summary>
        private void CancelReadTask()
        {
            if (ReadCancellationTokenSource != null)
            {
                if (!ReadCancellationTokenSource.IsCancellationRequested)
                {
                    ReadCancellationTokenSource.Cancel();
                }
            }
        }

        /// <summary>
        /// CloseDevice:
        /// - Disposes SerialDevice object
        /// - Clears the enumerated device Id list
        /// </summary>
        private void CloseDevice()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
            }
            serialPort = null;

            //comPortInput.IsEnabled = true;
            //sendTextButton.IsEnabled = false;
            //rcvdText.Text = "";
            listOfDevices.Clear();
        }
    }
}
