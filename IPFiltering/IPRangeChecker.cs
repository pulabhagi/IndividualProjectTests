using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class Program
{
    public static void Main()
    {
        List<string> ipRanges = new List<string>
        {
            "192.168.0.1",
            "192.168.1.0-192.168.1.255",
            "192.168.2.0/24"
        };

        Console.Write("Enter an IPv4 address to check: ");
        string ipAddress = Console.ReadLine();

        bool isInRange = IsIpAddressInRange(ipAddress, ipRanges);
        Console.WriteLine(isInRange ? "The IP address is within the range." : "The IP address is not within the range.");
    }

    private static bool IsIpAddressInRange(string ipAddress, List<string> ipRanges)
    {
        IPAddress address = IPAddress.Parse(ipAddress);

        foreach (string range in ipRanges)
        {
            if (range.Contains("-"))
            {
                string[] rangeParts = range.Split('-');
                IPAddress startAddress = IPAddress.Parse(rangeParts[0]);
                IPAddress endAddress = IPAddress.Parse(rangeParts[1]);
                if (IsIpAddressInRange(address, startAddress, endAddress))
                    return true;
            }
            else if (range.Contains("/"))
            {
                string[] cidrParts = range.Split('/');
                IPAddress cidrAddress = IPAddress.Parse(cidrParts[0]);
                int cidrPrefixLength = int.Parse(cidrParts[1]);
                if (IsIpAddressInCidrRange(address, cidrAddress, cidrPrefixLength))
                    return true;
            }
            else
            {
                IPAddress singleAddress = IPAddress.Parse(range);
                if (address.Equals(singleAddress))
                    return true;
            }
        }

        return false;
    }

    private static bool IsIpAddressInRange(IPAddress address, IPAddress startAddress, IPAddress endAddress)
    {
        byte[] addressBytes = address.GetAddressBytes();
        byte[] startBytes = startAddress.GetAddressBytes();
        byte[] endBytes = endAddress.GetAddressBytes();

        bool greaterThanOrEqualStart = true;
        bool lessThanOrEqualEnd = true;

        for (int i = 0; i < addressBytes.Length; i++)
        {
            if (addressBytes[i] < startBytes[i])
            {
                greaterThanOrEqualStart = false;
                break;
            }

            if (addressBytes[i] > endBytes[i])
            {
                lessThanOrEqualEnd = false;
                break;
            }
        }

        return greaterThanOrEqualStart && lessThanOrEqualEnd;
    }

    private static bool IsIpAddressInCidrRange(IPAddress address, IPAddress cidrAddress, int cidrPrefixLength)
    {
        byte[] addressBytes = address.GetAddressBytes();
        byte[] cidrBytes = cidrAddress.GetAddressBytes();

        if (address.AddressFamily != cidrAddress.AddressFamily)
            return false;

        int numBits = addressBytes.Length * 8;
        byte[] mask = new byte[addressBytes.Length];

        for (int i = 0; i < numBits; i++)
        {
            if (i < cidrPrefixLength)
                mask[i / 8] |= (byte)(1 << (7 - (i % 8)));
            else
                mask[i / 8] &= (byte)~(1 << (7 - (i % 8)));
        }

        for (int i = 0; i < addressBytes.Length; i++)
        {
            if ((addressBytes[i] & mask[i]) != (cidrBytes[i] & mask[i]))
                return false;
        }

        return true;
    }
}
