using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    /// <summary>
    /// The Network Control precedence designation is intended to be used within a network only. The actual use and control of that designation is up to each network. The Internetwork Control designation is intended for use by gateway control originators only. If the actual use of these precedence designations is of concern to a particular network, it is the responsibility of that network to control the access to, and use of, those precedence designations.
    /// </summary>
    public enum Precedence
    {
        Routine = 0,
        Priority = 1,
        Immediate = 2,
        Flash = 3,
        FlashOverride = 4,
        CRITICECP = 5,
        InternetworkControl = 6,
        NetworkControl = 7
    }
    /// <summary>
    /// The use of the Delay, Throughput, and Reliability indications may increase the cost (in some sense) of the service. In many networks better performance for one of these parameters is coupled with worse performance on another.
    /// </summary>
    public enum Delay
    {
        NormalDelay = 0,
        LowDelay = 1
    }
    /// <summary>
    /// The use of the Delay, Throughput, and Reliability indications may increase the cost (in some sense) of the service. In many networks better performance for one of these parameters is coupled with worse performance on another.
    /// </summary>
    public enum Throughput
    {
        NormalThroughput = 0,
        HighThroughput = 1
    }
    /// <summary>
    /// The use of the Delay, Throughput, and Reliability indications may increase the cost (in some sense) of the service. In many networks better performance for one of these parameters is coupled with worse performance on another.
    /// </summary>
    public enum Reliability
    {
        NormalReliability = 0,
        HighReliability = 1
    }
    /// <summary>
    /// This field indicates the next level protocol used in the data portion of the Internet datagram.
    /// </summary>
    public enum Protocol
    {
        Ggp = 3,
        Icmp = 1,
        Idp = 22,
        Igmp = 2,
        IP = 4,
        ND = 77,
        Pup = 12,
        Tcp = 6,
        Udp = 17,
        Other = -1
    }
}
