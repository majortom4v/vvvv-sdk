﻿using System;
using System.Collections;
using System.Net.Security;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace VVVV.Utils.Network
{
    #region RemotingUtils

    public static class RemotingUtils
    {
        //remove channel if it exists
        public static void RemoveChannel(string name)
        {
            var ch = ChannelServices.GetChannel(name);
            if (ch != null) ChannelServices.UnregisterChannel(ch);
        }

        //check if channel present
        public static bool ChannelExists(string name)
        {
            return  ChannelServices.GetChannel(name) != null;
        }
    }

    #endregion RemotingUtils

    #region TCP

    public class RemotingManagerTCP : IRemotingManager
    {
        //setup TCP channel
        public void InitializeChannel(string name, int port, bool enableSecurity)
        {

            RemotingUtils.RemoveChannel(name);

            var sinkProviderSrv = new BinaryServerFormatterSinkProvider();
            var sinkProviderClt = new BinaryClientFormatterSinkProvider();
            sinkProviderSrv.TypeFilterLevel = TypeFilterLevel.Full;

            IDictionary channelSettings = new Hashtable();
            channelSettings["name"] = name;
            channelSettings["port"] = port;
            channelSettings["secure"] = enableSecurity;

            IChannel channel = new TcpChannel(channelSettings, sinkProviderClt, sinkProviderSrv);
            ChannelServices.RegisterChannel(channel, enableSecurity);

        }

        //setup TCP server channel
        public void InitializeServerChannel(string name, int port, bool enableSecurity)
        {

            RemotingUtils.RemoveChannel(name);

            var sinkProvider = new BinaryServerFormatterSinkProvider();
            sinkProvider.TypeFilterLevel = TypeFilterLevel.Full;

            IDictionary channelSettings = new Hashtable();
            channelSettings["name"] = name;
            channelSettings["port"] = port;
            channelSettings["secure"] = enableSecurity;

            TcpServerChannel channel = new TcpServerChannel(channelSettings, sinkProvider);
            ChannelServices.RegisterChannel(channel, enableSecurity);

        }

        //setup TCP client channel
        public void InitializeClientChannel(string name, bool enableSecurity)
        {
            RemotingUtils.RemoveChannel(name);

            IDictionary channelSettings = new Hashtable();
            channelSettings["name"] = name;
            channelSettings["secure"] = enableSecurity;

            var sinkProvider = new BinaryClientFormatterSinkProvider();

            IChannel channel = new TcpClientChannel(channelSettings, sinkProvider);
            ChannelServices.RegisterChannel(channel, enableSecurity);
        }

        //object publishing
        public void PublishObject(MarshalByRefObject instance, string publicName)
        {
            RemotingServices.Marshal(instance, publicName);
        }

        public void PublishSingleton<T>(string publicName) where T : MarshalByRefObject
        {
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(T), publicName, WellKnownObjectMode.Singleton);
        }

        public void PublishSingleCall<T>(string publicName) where T : MarshalByRefObject
        {
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(T), publicName, WellKnownObjectMode.SingleCall);
        }

        //object request
        public T GetRemoteObject<T>(string publicName, string server, int port)
        {
            StringBuilder uriBuilder = new StringBuilder(@"tcp://");
            uriBuilder.Append(server);
            uriBuilder.Append(':');
            uriBuilder.Append(port);
            uriBuilder.Append('/');
            uriBuilder.Append(publicName);

            return (T)Activator.GetObject(typeof(T), uriBuilder.ToString());
        }
    }


    #endregion TCP

    #region HTTP

    public class RemotingManagerHTTP : IRemotingManager
    {

        //setup HTTP server channel
        public void InitializeChannel(string name, int port, bool enableSecurity)
        {
            RemotingUtils.RemoveChannel(name);

            var sinkProviderSrv = new BinaryServerFormatterSinkProvider();
            var sinkProviderClt = new BinaryClientFormatterSinkProvider();
            sinkProviderSrv.TypeFilterLevel = TypeFilterLevel.Full;

            IDictionary channelSettings = new Hashtable();
            channelSettings["name"] = name;
            channelSettings["port"] = port;

            var channel = new HttpChannel(channelSettings, sinkProviderClt, sinkProviderSrv);

            ChannelServices.RegisterChannel(channel, enableSecurity);
        }

        //setup HTTP server channel
        public void InitializeServerChannel(string name, int port, bool enableSecurity)
        {
            RemotingUtils.RemoveChannel(name);

            var sinkProvider = new BinaryServerFormatterSinkProvider();
            sinkProvider.TypeFilterLevel = TypeFilterLevel.Full;

            var channel = new HttpServerChannel(name, port, sinkProvider);

            ChannelServices.RegisterChannel(channel, enableSecurity);
        }

        //setup HTTP client channel
        public void InitializeClientChannel(string name, bool enableSecurity)
        {
            RemotingUtils.RemoveChannel(name);

            var sinkProvider = new BinaryClientFormatterSinkProvider();

            var channel = new HttpClientChannel(name, sinkProvider);

            ChannelServices.RegisterChannel(channel, enableSecurity);
        }

        //object publishing
        public void PublishObject(MarshalByRefObject instance, string publicName)
        {
            RemotingServices.Marshal(instance, publicName);
        }

        public void PublishSingleton<T>(string publicName) where T : MarshalByRefObject
        {
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(T), publicName, WellKnownObjectMode.Singleton);
        }

        public void PublishSingleCall<T>(string publicName) where T : MarshalByRefObject
        {
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(T), publicName, WellKnownObjectMode.SingleCall);
        }

        //object request
        public T GetRemoteObject<T>(string publicName, string server, int port)
        {
            StringBuilder uriBuilder = new StringBuilder(@"http://");
            uriBuilder.Append(server);
            uriBuilder.Append(':');
            uriBuilder.Append(port);
            uriBuilder.Append('/');
            uriBuilder.Append(publicName);

            return (T)Activator.GetObject(typeof(T), uriBuilder.ToString());
        }
    }

    #endregion HTTP

    #region IPC

    public class RemotingManagerIPC : IRemotingManagerIPC
    {

        //setup ICP server channel
        public void InitializeIpcChannel(string name, string portName, bool enableSecurity)
        {
            RemotingUtils.RemoveChannel(name);

            IDictionary channelSettings = new Hashtable();
            channelSettings["name"] = name;
            channelSettings["portName"] = portName;
            channelSettings["secure"] = enableSecurity;

            var sinkProviderSrv = new BinaryServerFormatterSinkProvider();
            var sinkProviderClt = new BinaryClientFormatterSinkProvider();
            sinkProviderSrv.TypeFilterLevel = TypeFilterLevel.Full;

            IChannel channel = new IpcChannel(channelSettings, sinkProviderClt, sinkProviderSrv);
            ChannelServices.RegisterChannel(channel, enableSecurity);
        }

        //setup ICP server channel
        public void InitializeIpcServerChannel(string name, string portName, bool enableSecurity)
        {
            RemotingUtils.RemoveChannel(name);

            IDictionary channelSettings = new Hashtable();
            channelSettings["name"] = name;
            channelSettings["portName"] = portName;
            channelSettings["secure"] = enableSecurity;

            var sinkProvider = new BinaryServerFormatterSinkProvider();
            sinkProvider.TypeFilterLevel = TypeFilterLevel.Full;

            IChannel channel = new IpcServerChannel(channelSettings, sinkProvider);
            ChannelServices.RegisterChannel(channel, enableSecurity);
        }

        //setup ICP client channel
        public void InitializeIpcClientChannel(string name, bool enableSecurity)
        {
            RemotingUtils.RemoveChannel(name);

            IDictionary channelSettings = new Hashtable();
            channelSettings["name"] = name;
            channelSettings["secure"] = enableSecurity;

            var sinkProvider = new BinaryClientFormatterSinkProvider();

            IChannel channel = new IpcClientChannel(channelSettings, sinkProvider);
            ChannelServices.RegisterChannel(channel, enableSecurity);

        }

        //object publishing
        public void PublishObject(MarshalByRefObject instance, string publicName)
        {
            RemotingServices.Marshal(instance, publicName);
        }

        public void PublishSingleton<T>(string publicName) where T : MarshalByRefObject
        {
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(T), publicName, WellKnownObjectMode.Singleton);
        }

        public void PublishSingleCall<T>(string publicName) where T : MarshalByRefObject
        {
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(T), publicName, WellKnownObjectMode.SingleCall);
        }

        //object request
        public T GetRemoteObjectIpc<T>(string publicName, string portName)
        {
            StringBuilder uriBuilder = new StringBuilder(@"ipc://");
            uriBuilder.Append(portName);
            uriBuilder.Append('/');
            uriBuilder.Append(publicName);

            return (T)Activator.GetObject(typeof(T), uriBuilder.ToString());
        }
    }

    #endregion IPC
}

