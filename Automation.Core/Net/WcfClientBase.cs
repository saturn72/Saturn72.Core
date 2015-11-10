using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Automation.Core.Net
{
    public abstract class WcfClientBase<TService> where TService : class
    {
        protected abstract Uri UriOrIp { get; }

        protected virtual void ServiceRequest(Action action)
        {
            using (var channelFactory = CreateChannelFactory())
            {
                ServiceInstance = channelFactory.CreateChannel();
                action();
            }
        }

        protected virtual TResult ServiceRequest<TResult>(Func<TResult> function)
        {
            using (var channelFactory = CreateChannelFactory())
            {
                ServiceInstance = channelFactory.CreateChannel();
                return function();
            }
        }

        protected ChannelFactory<TService> CreateChannelFactory()
        {
            ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;

            var address = new Uri(UriOrIp,ServiceName);

            var binding = BuildBinding();
            var ePoint = new EndpointAddress(address);

            return new ChannelFactory<TService>(binding, ePoint);
        }

        protected abstract Binding BuildBinding();


        protected virtual bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #region Properties

        /// <summary>
        ///     Gets the service name.
        /// </summary>
        protected  virtual string ServiceName
        {
            get { return typeof (TService).Name; }
        }

        protected TService ServiceInstance { get; private set; }

        #endregion
    }
}