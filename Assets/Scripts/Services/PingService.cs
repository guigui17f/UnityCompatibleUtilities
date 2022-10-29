using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace GUIGUI17F
{
    public struct PingResult
    {
        public string Address;
        public long RoundtripTime;
        public float PacketLossRate;
    }

    public struct FastPingResult
    {
        public string Address;
        public long RoundtripTime;
    }

    public class PingService
    {
        /// <summary>
        /// start a ping test
        /// </summary>
        /// <param name="addressList">address list to ping</param>
        /// <param name="pingCount">package sent count for every address</param>
        /// <param name="timeout">timeout in milliseconds for every ping</param>
        public async Task<PingResult[]> Ping(List<string> addressList, int pingCount = 4, int timeout = 800)
        {
            List<Task<PingResult>> taskList = new List<Task<PingResult>>();
            foreach (string address in addressList)
            {
                taskList.Add(RunPing(address, pingCount, timeout));
            }
            return await Task.WhenAll(taskList);
        }

        /// <summary>
        /// start a fast ping test (return as soon as a valid data gotten)
        /// </summary>
        /// <param name="addressList">address list to ping</param>
        /// <param name="maxRetry">max retry count for every address</param>
        /// <param name="timeout">timeout in milliseconds for every ping</param>
        public async Task<FastPingResult[]> FastPing(List<string> addressList, int maxRetry = 4, int timeout = 800)
        {
            List<Task<FastPingResult>> taskList = new List<Task<FastPingResult>>();
            foreach (string address in addressList)
            {
                taskList.Add(RunFastPing(address, maxRetry, timeout));
            }
            return await Task.WhenAll(taskList);
        }

        private async Task<PingResult> RunPing(string address, int pingCount, int timeout)
        {
            int successCount = 0;
            long totalRoundtripTime = 0;
            using (Ping pingSender = new Ping())
            {
                for (int i = 0; i < pingCount; i++)
                {
                    PingReply reply = await pingSender.SendPingAsync(address, timeout);
                    if (reply.Status == IPStatus.Success)
                    {
                        successCount++;
                        totalRoundtripTime += reply.RoundtripTime;
                    }
                }
            }
            PingResult result = new PingResult { Address = address };
            if (successCount > 0)
            {
                result.RoundtripTime = totalRoundtripTime / successCount;
                result.PacketLossRate = successCount * 100f / pingCount;
            }
            else
            {
                result.RoundtripTime = -1;
                result.PacketLossRate = 100;
            }
            return result;
        }

        private async Task<FastPingResult> RunFastPing(string address, int maxRetry, int timeout)
        {
            long roundtripTime = -1;
            using (Ping pingSender = new Ping())
            {
                for (int i = 0; i < maxRetry; i++)
                {
                    PingReply reply = await pingSender.SendPingAsync(address, timeout);
                    if (reply.Status == IPStatus.Success)
                    {
                        roundtripTime = reply.RoundtripTime;
                        break;
                    }
                }
            }
            return new FastPingResult { Address = address, RoundtripTime = roundtripTime };
        }
    }
}