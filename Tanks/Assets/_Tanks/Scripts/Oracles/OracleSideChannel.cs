using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using System.Text;
using System;
using System.Security.Cryptography;

namespace Tanks.Complete
{
    public class OracleSideChannel : SideChannel
    {
        public static event Action<int> OnStuckResetReceived;
        public static event Action OnResetReceived;

        public OracleSideChannel()
        {
            string runID = GetRunID();
            ChannelId = new Guid(runID);
            //ChannelId = new Guid("621f0a70-4f87-11ea-a6bf-784f4387d1f7");
        }

        protected override void OnMessageReceived(IncomingMessage msg)
        {
            var receivedString = msg.ReadString();
            
            if (receivedString.StartsWith("RESET"))
            {
                int envId = 0;
                var parts = receivedString.Split(':');
                if (parts.Length > 1)
                {
                    int.TryParse(parts[1], out envId);
                    OnStuckResetReceived?.Invoke(envId);
                }
                else
                {
                    OnResetReceived?.Invoke();
                }
            }
        }

        public void SendStringToPython(string msg)
        {
            var stringToSend = msg;
            using (var msgOut = new OutgoingMessage())
            {
                msgOut.WriteString(stringToSend);
                QueueMessageToSend(msgOut);
            }
        }

        private string GetRunID()
        {
            string oralceId = System.Environment.GetEnvironmentVariable("ORACLE_HASH");
            if (!string.IsNullOrEmpty(oralceId))
                return oralceId;

            return "0";
        }
    }
}