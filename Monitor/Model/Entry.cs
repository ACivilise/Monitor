using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using log4net;

namespace Monitor
{
    /// <summary>
    /// Represents an IP packet.
    /// </summary>
    public class Entry : INotifyPropertyChanged
    {
        #region private variables

        private byte[] m_Raw;
        private DateTime m_Time;
        private int m_Version;
        private int m_HeaderLength;
        private Precedence m_Precedence;
        private Delay m_Delay;
        private Throughput m_Throughput;
        private Reliability m_Reliability;
        private int m_TotalLength;
        private int m_Identification;
        private int m_TimeToLive;
        private byte[] m_Checksum;
        private int m_SourcePort;
        private int m_DestinationPort;
        #endregion

        private static readonly ILog log = LogManager.GetLogger(typeof(Entry));

        private IPAddress m_SourceAddress;
        public IPAddress SourceAddress
        {
            get { return m_SourceAddress; }
            set
            {
                m_SourceAddress = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("SourceAddress"));
            }
        }

        private IPAddress m_DestinationAddress;
        public IPAddress DestinationAddress
        {
            get { return m_DestinationAddress; }
            set
            {
                m_DestinationAddress = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("DestinationAddress"));
            }
        }

        private string m_SourceURL;
        public string SourceURL
        {
            get { return m_SourceURL; }
            set
            {
                m_SourceURL = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("SourceURL"));
            }
        }

        private string m_DestinationURL;
        public string DestinationURL
        {
            get { return m_DestinationURL; }
            set
            {
                m_DestinationURL = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("DestinationURL"));
            }
        }

        private int m_NbofPackets;
        public int NbofPackets
        {
            get { return m_NbofPackets; }
            set
            {
                m_NbofPackets = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("NbofPackets"));
            }
        }

        private Protocol m_Protocol;
        public Protocol Protocol
        {
            get { return m_Protocol; }
            set
            {
                m_Protocol = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Protocol"));
            }
        }

        /// <summary>
        /// Initializes a new version of the Packet class.
        /// </summary>
        /// <param name="raw">The raw bytes of the IP packet.</param>
        /// <exception cref="ArgumentNullException"><paramref name="raw"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="raw"/> represents an invalid IP packet.</exception>
        /// <remarks>The intercept time will be set to DateTime.Now.</remarks>
        public Entry(byte[] raw) : this(raw, DateTime.Now) { }
        /// <summary>
        /// Initializes a new version of the Packet class.
        /// </summary>
        /// <param name="raw">The raw bytes of the IP packet.</param>
        /// <param name="time">The time when the IP packet was intercepted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="raw"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="raw"/> represents an invalid IP packet.</exception>
        public Entry(byte[] raw, DateTime time)
        {
            try
            {
                if (raw == null)
                    throw new ArgumentNullException();
                if (raw.Length < 20)
                    throw new ArgumentException(); // invalid IP packet
                m_Raw = raw;
                m_Time = time;
                m_Version = (raw[0] & 0xF0) >> 4;
                m_HeaderLength = (raw[0] & 0x0F) * 4 /* size of(int) */;
                if ((raw[0] & 0x0F) < 5)
                    throw new ArgumentException(); // invalid header of packet
                m_Precedence = (Precedence)((raw[1] & 0xE0) >> 5);
                m_Delay = (Delay)((raw[1] & 0x10) >> 4);
                m_Throughput = (Throughput)((raw[1] & 0x8) >> 3);
                m_Reliability = (Reliability)((raw[1] & 0x4) >> 2);
                m_TotalLength = raw[2] * 256 + raw[3];
                if (m_TotalLength != raw.Length)
                    throw new ArgumentException(); // invalid size of packet
                m_Identification = raw[4] * 256 + raw[5];
                m_TimeToLive = raw[8];
                if (Enum.IsDefined(typeof(Protocol), (int)raw[9]))
                    m_Protocol = (Protocol)raw[9];
                else
                    m_Protocol = Protocol.Other;
                m_Checksum = new byte[2];
                m_Checksum[0] = raw[11];
                m_Checksum[1] = raw[10];
                m_SourceAddress = new IPAddress(BitConverter.ToUInt32(raw, 12));
                m_DestinationAddress = new IPAddress(BitConverter.ToUInt32(raw, 16));
                if (m_Protocol == Protocol.Tcp || m_Protocol == Protocol.Udp)
                {
                    m_SourcePort = raw[m_HeaderLength] * 256 + raw[m_HeaderLength + 1];
                    m_DestinationPort = raw[m_HeaderLength + 2] * 256 + raw[m_HeaderLength + 3];
                }
                else
                {
                    m_SourcePort = -1;
                    m_DestinationPort = -1;
                }


                //Gets the URL if it exists
                IPHostEntry ipHostEntry = null;
                try
                {
                    ipHostEntry = Dns.GetHostEntry(m_SourceAddress);
                    m_SourceURL = ipHostEntry.HostName;
                }
                catch (Exception e)
                {
                    //log.Error(e);
                    m_SourceURL = e.Message;
                }
                try
                {
                    ipHostEntry = Dns.GetHostEntry(m_DestinationAddress);
                    m_DestinationURL = ipHostEntry.HostName;
                }
                catch (Exception e)
                {
                    //log.Error(e);
                    m_DestinationURL = e.Message;
                }
                
                m_NbofPackets = 1;
                
            }
            catch (Exception e)
            {
                log.Error(e);
                throw e;
            }
        }

        /// <summary>
        /// Gets the raw bytes of the IP packet.
        /// </summary>
        /// <value>An array of bytes.</value>
        protected byte[] Raw
        {
            get
            {
                return m_Raw;
            }
        }
        /// <summary>
        /// Gets the time when the IP packet was intercepted.
        /// </summary>
        /// <value>A <see cref="DateTime"/> value.</value>
        public DateTime Time
        {
            get
            {
                return m_Time;
            }
        }
        /// <summary>
        /// Gets the version of the IP protocol used.
        /// </summary>
        /// <value>A 32-bits signed integer.</value>
        public int Version
        {
            get
            {
                return m_Version;
            }
        }
        /// <summary>
        /// Gets the length of the IP header [in bytes].
        /// </summary>
        /// <value>A 32-bits signed integer.</value>
        public int HeaderLength
        {
            get
            {
                return m_HeaderLength;
            }
        }
        /// <summary>
        /// Gets the precedence parameter.
        /// </summary>
        /// <value>A <see cref="Precedence"/> instance.</value>
        public Precedence Precedence
        {
            get
            {
                return m_Precedence;
            }
        }
        /// <summary>
        /// Gets the delay parameter.
        /// </summary>
        /// <value>A <see cref="Delay"/> instance.</value>
        public Delay Delay
        {
            get
            {
                return m_Delay;
            }
        }
        /// <summary>
        /// Gets the throughput parameter.
        /// </summary>
        /// <value>A <see cref="Throughput"/> instance.</value>
        public Throughput Throughput
        {
            get
            {
                return m_Throughput;
            }
        }
        /// <summary>
        /// Gets the reliability parameter.
        /// </summary>
        /// <value>A <see cref="Reliability"/> instance.</value>
        public Reliability Reliability
        {
            get
            {
                return m_Reliability;
            }
        }
        /// <summary>
        /// Gets the total length of the IP packet.
        /// </summary>
        /// <value>A 32-bits signed integer.</value>
        public int TotalLength
        {
            get
            {
                return m_TotalLength;
            }
        }
        /// <summary>
        /// Gets the identification number of the IP packet.
        /// </summary>
        /// <value>A 32-bits signed integer.</value>
        public int Identification
        {
            get
            {
                return m_Identification;
            }
        }
        /// <summary>
        /// Gets the time-to-live [hop count] of the IP packet.
        /// </summary>
        /// <value>A 32-bits signed integer.</value>
        public int TimeToLive
        {
            get
            {
                return m_TimeToLive;
            }
        }
        /// <summary>
        /// Gets the checksum of the IP packet.
        /// </summary>
        /// <value>An array of two bytes.</value>
        public byte[] Checksum
        {
            get
            {
                return m_Checksum;
            }
        }

        /// <summary>
        /// Gets the source port of the packet.
        /// </summary>
        /// <value>A 32-bits signed integer.</value>
        /// <remarks>
        /// This property will only return meaningful data if the IP packet encapsulates either a TCP or a UDP packet.
        /// If the IP address encapsulates a packet of another protocol, the returned source port will be set to minus one.
        /// </remarks>
        public int SourcePort
        {
            get
            {
                return m_SourcePort;
            }
        }
        /// <summary>
        /// Gets the destination port of the packet.
        /// </summary>
        /// <value>A 32-bits signed integer.</value>
        /// <remarks>
        /// This property will only return meaningful data if the IP packet encapsulates either a TCP or a UDP packet.
        /// If the IP address encapsulates a packet of another protocol, the returned destination port will be set to minus one.
        /// </remarks>
        public int DestinationPort
        {
            get
            {
                return m_DestinationPort;
            }
        }
        /// <summary>
        /// Gets a string representation of the source.
        /// </summary>
        /// <value>An <see cref="String"/> instance.</value>
        /// <remarks>
        /// If the encapsulated packet is a TCP or UDP packet, the returned string will consist of the IP address and the port number.
        /// If the IP packet does not encapsulate a TCP or UDP packet, the returned string will consist of the IP address.
        /// </remarks>
        public string Source
        {
            get
            {
                if (m_SourcePort != -1)
                    return SourceAddress.ToString() + ":" + m_SourcePort.ToString();
                else
                    return SourceAddress.ToString();
            }
        }
        /// <summary>
        /// Gets a string representation of the destination.
        /// </summary>
        /// <value>An <see cref="String"/> instance.</value>
        /// <remarks>
        /// If the encapsulated packet is a TCP or UDP packet, the returned string will consist of the IP address and the port number.
        /// If the IP packet does not encapsulate a TCP or UDP packet, the returned string will consist of the IP address.
        /// </remarks>
        public string Destination
        {
            get
            {
                if (m_DestinationPort != -1)
                    return DestinationAddress.ToString() + ":" + m_DestinationPort.ToString();
                else
                    return DestinationAddress.ToString();
            }
        }
        /// <summary>
        /// Returns a string representation of the Packet 
        /// </summary>
        /// <returns>An instance of the <see cref="String"/> class.</returns>
        public override string ToString()
        {
            return this.ToString(false);
        }
        /// <summary>
        /// Returns a string representation of the Packet 
        /// </summary>
        /// <param name="raw"><b>true</b> if the returned string should only contain the raw bytes, <b>false</b> if the returned string should also contain a hexadecimal representation.</param>
        /// <returns>An instance of the <see cref="String"/> class.</returns>
        public string ToString(bool raw)
        {
            var sb = new StringBuilder(Raw.Length);
            try
            {
                if (raw)
                {
                    for (int i = 0; i < Raw.Length; i++)
                    {
                        if (Raw[i] > 31)
                            sb.Append((char)Raw[i]);
                        else
                            sb.Append(".");
                    }
                }
                else
                {
                    var rawString = this.ToString(true);
                    for (int i = 0; i < Raw.Length; i += 16)
                    {
                        for (int j = i; j < Raw.Length && j < i + 16; j++)
                        {
                            sb.Append(Raw[j].ToString("X2") + " ");
                        }
                        if (rawString.Length < i + 16)
                        {
                            sb.Append(' ', ((16 - (rawString.Length % 16)) % 16) * 3);
                            sb.Append(" " + rawString.Substring(i) + "\r\n");
                        }
                        else
                        {
                            sb.Append(" " + rawString.Substring(i, 16) + "\r\n");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return sb.ToString();
        }



        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            try
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

}
