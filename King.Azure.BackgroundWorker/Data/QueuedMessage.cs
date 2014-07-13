﻿namespace King.Azure.BackgroundWorker.Data
{
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;

    public class QueuedMessage<T> : IQueued<T>
    {
        #region Members
        private readonly IQueue queue = null;
        private readonly CloudQueueMessage message = null;
        #endregion

        #region Constructors
        public QueuedMessage(IQueue queue, CloudQueueMessage message)
        {
            if (null == queue)
            {
                throw new ArgumentNullException("queue");
            }
            if (null == message)
            {
                throw new ArgumentNullException("message");
            }

            this.queue = queue;
            this.message = message;
        }
        #endregion

        #region Methods
        public async Task Delete()
        {
            await this.queue.Delete(this.message);
        }

        public Task Abandon()
        {
            return null; //No Abandon?
        }

        public async Task<T> Data()
        {
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(this.message.AsString));
        }
        #endregion
    }
}