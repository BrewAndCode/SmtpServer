using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using SmtpServer.IO;
using SmtpServer.Protocol;

namespace SmtpServer
{
    internal sealed class SmtpSessionContext : ISessionContext
    {
        /// <summary>
        /// Fired when a command is about to execute.
        /// </summary>
        public event EventHandler<SmtpCommandExecutingEventArgs> CommandExecuting;

        /// <summary>
        /// Fired when the session has been authenticated.
        /// </summary>
        public event EventHandler<EventArgs> SessionAuthenticated;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tcpClient">The TCP client that the session is connected with.</param>
        internal SmtpSessionContext(TcpClient tcpClient)
        {
            Transaction = new SmtpMessageTransaction();
            RemoteEndPoint = tcpClient.Client.RemoteEndPoint;
            Client = new NetworkClient(tcpClient.GetStream());
            Properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Indicates to the session that it should quit.
        /// </summary>
        internal void Quit()
        {
            IsQuitRequested = true;
        }

        /// <summary>
        /// Raise the command executing event.
        /// </summary>
        /// <param name="command">The command that is executing.</param>
        internal void RaiseCommandExecuting(SmtpCommand command)
        {
            CommandExecuting?.Invoke(this, new SmtpCommandExecutingEventArgs(this, command));
        }

        /// <summary>
        /// Raise the session authenticated event.
        /// </summary>
        internal void RaiseSessionAuthenticated()
        {
            SessionAuthenticated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets the text stream to read from and write to.
        /// </summary>
        public INetworkClient Client { get; }

        /// <summary>
        /// Gets the current transaction.
        /// </summary>
        public SmtpMessageTransaction Transaction { get; }

        /// <summary>
        /// Gets the remote endpoint of the client.
        /// </summary>
        public EndPoint RemoteEndPoint { get; }

        /// <summary>
        /// Returns a value indicating whether or not the current session is secure.
        /// </summary>
        public bool IsSecure { get; internal set; }

        /// <summary>
        /// Returns a value indicating whether or nor the current session is authenticated.
        /// </summary>
        public bool IsAuthenticated { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether a quit has been requested.
        /// </summary>
        public bool IsQuitRequested { get; private set; }

        /// <summary>
        /// Returns a set of propeties for the current session.
        /// </summary>
        public IDictionary<string, object> Properties { get; }
    }
}