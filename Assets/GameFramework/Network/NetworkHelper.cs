//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #网络帮助类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年8月2日 13点42分 </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace GameFramework.Taurus
{
    public class NetworkHelper
    {
        /// <summary>
        /// 获取局域网的IP
        /// </summary>
        /// <returns></returns>
        public static List<string> Lanips
        {
            get
            {
                List<string> lanIps = new List<string>();

                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    if (adapter.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
                        adapter.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
                        continue;

                    if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                    {
                        UnicastIPAddressInformationCollection uniCast = adapter.GetIPProperties().UnicastAddresses;
                        if (uniCast.Count > 0)
                        {
                            foreach (UnicastIPAddressInformation uni in uniCast)
                            {
                                //得到IPv4的地址。 AddressFamily.InterNetwork指的是IPv4
                                if (uni.Address.AddressFamily == AddressFamily.InterNetwork)
                                {
                                    lanIps.Add(uni.Address.ToString());
                                }
                            }
                        }
                    }
                }
                return lanIps;
            }
           
        }
    }
}
