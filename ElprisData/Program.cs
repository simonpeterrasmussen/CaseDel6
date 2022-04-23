using Confluent.Kafka;
using System.Net;
using ElprisData;
using Newtonsoft.Json;
//ElprisData:
//Is waiting for requests on requesteldata.
//Reply is send bask on ReplyTopic of received request.

bool testmode = false;

var produceConfig = new ProducerConfig
{
    //BootstrapServers = "192.168.126.128",
    BootstrapServers = "Ubuntu2004",
    ClientId = Dns.GetHostName(),
};

var consumeConfig = new ConsumerConfig
{
    //BootstrapServers = "192.168.126.128:9092",
    BootstrapServers = "Ubuntu2004:9092",
    GroupId = "Gruppe",
    AutoOffsetReset = AutoOffsetReset.Latest,
};

var logit = new SimpleLogging();

using (var consumer = new ConsumerBuilder<Ignore, string>(consumeConfig).Build())
{
    consumer.Subscribe("requesteldata");

    Console.WriteLine("ElprisData klar til at modtage beskeder");
    logit.Log("ElprisData klar til at modtage beskeder");
    while (true)
    {
        try
        {
            var consumeResult = consumer.Consume();
            string message = consumeResult.Message.Value;
            Console.WriteLine($"I ElprisData er besked modtaget: {message}");
            logit.Log($"I ElprisData er besked modtaget: {message}");
            var request = new MessageRequest();
            request = JsonConvert.DeserializeObject<MessageRequest>(message);

            if (request.RequestType == "ElprisDataRequest" || testmode)
            {
                Console.WriteLine($"ElprisData - modtog request: {request.MessageId} / {request.CreationDateTime}");
                logit.Log($"ElprisData - modtog request: {request.MessageId} / {request.CreationDateTime}");
                string ? json = JsonCache.Get();
                if (json == null)
                {
                    json = await PriceDownloader.GetTodaysPricesJsonAsync();
                    JsonCache.Put(json);
                }

                MessageReply replyMessage = new MessageReply()
                {
                    MessageId = Guid.NewGuid(),
                    ReplyOnRequestId = request.MessageId,
                    CreationDateTime = DateTime.Now,
                    Replydata = json
                };

                Console.WriteLine($"ElprisData - svarer på {replyMessage.ReplyOnRequestId} med {replyMessage.MessageId}");
                logit.Log($"ElprisData - svarer på {replyMessage.ReplyOnRequestId} med {replyMessage.MessageId}");

                var jsonMessageReply = JsonConvert.SerializeObject(replyMessage);
                
                using (var producer = new ProducerBuilder<Null, string>(produceConfig).Build())
                {
                    //????request.ReplyTopic = "publisheldata";
                    await producer.ProduceAsync(request.ReplyTopic, new Message<Null, string> { Value = jsonMessageReply });
                }
                Console.WriteLine("ElprisData - Ended on reply");
                logit.Log($"ElprisData - Ended on reply {request.MessageId} / {request.CreationDateTime}");
            }
        }
        catch (Exception)
        {
            Console.WriteLine("ElprisData har bøvl");
            logit.Log($"ElprisData har bøvl!!");
        }
    }
    consumer.Close();
}