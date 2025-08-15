// Messaging.Registration.cs
using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging
{
    public static class MessagingRegistration
    {
        public static IServiceCollection AddRabbitMqMessaging(
            this IServiceCollection services,
            Assembly[] scanAssemblies,
            Action<MessagingOptions>? configure = null,
            Action<IRabbitMqBusFactoryConfigurator>? busCfg = null)
        {
            var options = new MessagingOptions();
            configure?.Invoke(options);
            services.AddSingleton(options);

            // Idempotency default
            services.AddMemoryCache();
            services.AddSingleton<IIdempotencyStore, MemoryIdempotencyStore>();

            services.AddMassTransit(x =>
            {
                // Consumer tiplerini topla
                var allTypes = scanAssemblies.SelectMany(a => a.GetTypes())
                    .Where(t => !t.IsAbstract && !t.IsInterface)
                    .ToArray();

                var cmdConsumers = allTypes.Where(t => InheritsOpenGeneric(t, typeof(CommandConsumerBase<>))).ToList();
                var rpcConsumers = allTypes.Where(t => InheritsOpenGeneric(t, typeof(RequestConsumerBase<,>))).ToList();
                var evtConsumers = allTypes.Where(t => InheritsOpenGeneric(t, typeof(EventConsumerBase<>))).ToList();

                foreach (var t in cmdConsumers.Concat(rpcConsumers).Concat(evtConsumers))
                    x.AddConsumer(t);

                // RPC request client’ları: RequestConsumerBase<TRq,TRes> => AddRequestClient<TRq>(rpcQueue)
                var rqTypes = rpcConsumers
                    .Select(t => t.BaseType!) // RequestConsumerBase<,>
                    .Select(bt => bt.GetGenericArguments()[0])
                    .Distinct();

                foreach (var rq in rqTypes)
                {
                    var rpcQueue = Naming.QueueName(rq, MessagePattern.SendAndWait);
                    //x.AddRequestClient(rq, Naming.QueueUri(rpcQueue));
                    x.AddRequestClient(rq, Naming.ExchangeUri(rpcQueue));
                }

                // Scheduler (RabbitMQ Delayed Exchange varsa)
                if (options.UseDelayedMessageScheduler)
                    x.AddDelayedMessageScheduler();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(options.Host, options.VHost, h =>
                    {
                        h.Username(options.Username);
                        h.Password(options.Password);
                    });

                    if (options.UseDelayedMessageScheduler)
                        cfg.UseDelayedMessageScheduler();

                    // Global consume filter (idempotency)
                    cfg.UseConsumeFilter(typeof(IdempotencyConsumeFilter<>), context);

                    // Her consumer türü için uygun endpoint
                    void ConfigureCommonEndpoint(IRabbitMqReceiveEndpointConfigurator e)
                    {
                        e.PrefetchCount = options.PrefetchCount;
                        if (options.ConcurrentMessageLimit.HasValue)
                            e.ConcurrentMessageLimit = options.ConcurrentMessageLimit;

                        // Retry + Redelivery
                        e.UseMessageRetry(r => r.Exponential(
                            retryLimit: options.RetryAttempts,
                            minInterval: options.RetryMin,
                            maxInterval: options.RetryMax,
                            intervalDelta: TimeSpan.FromSeconds(2)));

                        e.UseDelayedRedelivery(rd => rd.Intervals(options.RedeliveryIntervals));

                        // DLQ (MassTransit default _error / _skipped); ekstra ayar istersen:
                        if (options.ConfigureDeadLetter)
                        {
                            // Rabbit tarafında ilave argümanlar verebilirsin
                            // (MT zaten faulted mesajları *_error kuyruğuna taşır)
                        }

                        // Queue özellikleri
                        if (options.UseQuorumQueues)
                            e.SetQueueArgument("x-queue-type", "quorum");

                        if (options.EnablePriority)
                            e.SetQueueArgument("x-max-priority", options.MaxPriority);
                    }

                    // Command endpoints (.cmd + .pubq) – aynı consumer sınıfı ikisini de dinler
                    foreach (var t in cmdConsumers)
                    {
                        var msgType = t.BaseType!.GetGenericArguments()[0];
                        var cmdQueue = Naming.QueueName(msgType, MessagePattern.SendAndForget);
                        cfg.ReceiveEndpoint(cmdQueue, e =>
                        {
                            ConfigureCommonEndpoint(e);
                            e.ConfigureConsumer(context, t);
                        });

                        var pubQueue = Naming.QueueName(msgType, MessagePattern.PublishToQueue);
                        cfg.ReceiveEndpoint(pubQueue, e =>
                        {
                            ConfigureCommonEndpoint(e);
                            e.ConfigureConsumer(context, t);
                        });
                    }

                    // RPC endpoints (.rpc)
                    foreach (var t in rpcConsumers)
                    {
                        var rqType = t.BaseType!.GetGenericArguments()[0];
                        var rpcQueue = Naming.QueueName(rqType, MessagePattern.SendAndWait);
                        cfg.ReceiveEndpoint(rpcQueue, e =>
                        {
                            ConfigureCommonEndpoint(e);
                            e.ConfigureConsumer(context, t);
                        });
                    }

                    // Event endpoints (.evtq) – yalnızca EventEnvelope<TPayload> dinler
                    foreach (var t in evtConsumers)
                    {
                        var payloadType = t.BaseType!.GetGenericArguments()[0];
                        var evtQueue = Naming.QueueName(payloadType, MessagePattern.PublishToAll);
                        cfg.ReceiveEndpoint(evtQueue, e =>
                        {
                            ConfigureCommonEndpoint(e);
                            e.ConfigureConsumer(context, t);
                        });
                    }

                    // Dışarıdan ek Rabbit ayarlamak istersen
                    busCfg?.Invoke(cfg);
                });
            });

            return services;
        }

        private static bool InheritsOpenGeneric(Type type, Type openGeneric)
        {
            while (type is not null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == openGeneric)
                    return true;
                type = type.BaseType!;
            }
            return false;
        }
    }
}