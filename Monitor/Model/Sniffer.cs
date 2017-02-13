using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Globalization;
using log4net;

namespace Monitor
{
    public class Sniffer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Sniffer));
        
        /// <summary>
        /// Holds all the listeners for the NewPacket event.
        /// </summary>
        public event NewPacketEventHandler NewPacket;
        /// <summary>
        /// Represents the method that will handle the NewPacket event.
        /// </summary>
        /// <param name="pm">The <see cref="Sniffer"/> that intercepted the <see cref="Entry"/>.</param>
        /// <param name="p">The newly arrived <see cref="Entry"/>.</param>
        public delegate void NewPacketEventHandler(Sniffer pm, Entry p);

        public Sniffer()
        {
            //this.NewPacket += this.OnNewPacket;
            Sniffer.Start(this);
        }

        private IPAddress m_IP;
        /// <summary>
        /// The interface used to intercept IP packets.
        /// </summary>
        /// <value>An <see cref="IPAddress"/> instance.</value>
        public IPAddress Local_IP
        {
            get
            {
                return m_IP;
            }
        }
        private const int IOC_VENDOR = 0x18000000;
        private const int IOC_IN = -2147483648; //0x80000000; /* copy in parameters */
        public const int SIO_RCVALL = IOC_IN | IOC_VENDOR | 1;
        private Socket sniffer;
        private byte[] m_Buffer;

        /// <summary>
        /// Raises an event that indicates a new packet has arrived.
        /// </summary>
        /// <param name="p">The arrived <see cref="Entry"/>.</param>
        protected void OnNewPacket(Entry p)
        {
            if (NewPacket != null)
                NewPacket(this, p);
        }

        /// <summary>
        /// Called when the socket intercepts an IP packet.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                var received = sniffer.EndReceive(ar);
                {
                    if (sniffer != null)
                    {
                        var packet = new byte[received];
                        Array.Copy(m_Buffer, 0, packet, 0, received);
                        OnNewPacket(new Entry(packet, received));
                    }
                }
                sniffer.BeginReceive(m_Buffer, 0, m_Buffer.Length, SocketFlags.None, new AsyncCallback(this.OnReceive), null);
            }
            catch (Exception)
            {
                Stop();
            }
        }
        static void Start(Sniffer instance)
        {
            var hosts = Dns.Resolve(Dns.GetHostName()).AddressList;
            foreach (var item in hosts)
            {
            }
            if (hosts.Length == 0)
                throw new NotSupportedException("This computer does not have non-loopback interfaces installed!");

            instance.m_IP = hosts[0];
            instance.m_Buffer = new byte[65535];

            if (instance.sniffer == null)
            {
                try
                {

                    instance.sniffer = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
                    instance.sniffer.Bind(new IPEndPoint(instance.Local_IP, 0));
                    instance.sniffer.IOControl(SIO_RCVALL, BitConverter.GetBytes(1), null);
                    instance.sniffer.BeginReceive(instance.m_Buffer, 0, instance.m_Buffer.Length, SocketFlags.None, new AsyncCallback(instance.OnReceive), null);
                }
                catch (Exception ex)
                {
                    instance.sniffer = null;
                    throw new SocketException();
                }
            }
        }

        /// <summary>
        /// Stops listening on the specified interface.
        /// </summary>
        public void Stop()
        {
            if (sniffer != null)
            {
                sniffer.Close();
                sniffer = null;
            }
        }

        private void OnNewPacket(Sniffer pm, Entry p)
        {
            StringBuilder sb = new StringBuilder("Entry : ");
            sb.Append("Time : ").Append(p.Time.ToString());
            sb.Append(" ");
            sb.Append("Protocol : ").Append(p.Protocol.ToString());
            sb.Append(" ");
            sb.Append("Source : ").Append(p.Source.ToString());
            sb.Append(" ");
            sb.Append("Destination : ").Append(p.Destination.ToString());
            sb.Append(" ");
            sb.Append("TotalLength : ").Append(p.LastPacketLenght.ToString());
            sb.Append(" ");
            Console.WriteLine(sb.ToString());

        }

    }
}
