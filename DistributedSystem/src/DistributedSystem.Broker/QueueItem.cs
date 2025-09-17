﻿using System.Net.Sockets;
using DistributedSystem.Broker.Messages;

namespace DistributedSystem.Broker;

public class QueueItem
{
    public string SenderId { get; }
    public Message Message { get; }

    public QueueItem(string senderId, Message message)
    {
        SenderId = senderId;
        Message = message;
    }
}