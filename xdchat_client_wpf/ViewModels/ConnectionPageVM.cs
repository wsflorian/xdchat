﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using xdchat_client_wpf.Models;
using XdChatShared;
using XdChatShared.Scheduler;


namespace xdchat_client_wpf
{
    public class ConnectionPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _serverAddress;
        private string _nickname;
        private List<ServerLogMessage> _serverLog;
        public string ServerAdress
        {
            get => _serverAddress;
            set
            {
                _serverAddress = value;
                PropChanged(nameof(_serverAddress));
                ConnectButtonActionCommand.RaiseCanExecuteChanged();
            }
        }

        public string Nickname
        {
            get => _nickname;
            set
            {
                _nickname = value;
                PropChanged(nameof(Nickname));
                ConnectButtonActionCommand.RaiseCanExecuteChanged();
            }
        }

        public List<ServerLogMessage> ServerLog
        {
            get => _serverLog;
            set
            {
                _serverLog = value;
                PropChanged(nameof(ServerLog));
            }
        }
        
        public ActionCommand ConnectButtonActionCommand { get; private set; }
        private Action ClientConnectedFunc { get; set; }

        public ConnectionPageVM(Action clientConnectedFunc)
        {
            ConnectButtonActionCommand = new ActionCommand(ClickConnectFunc, ConnectButtonClickable);
            ClientConnectedFunc = clientConnectedFunc;
            ServerLog = new List<ServerLogMessage>();

            Nickname = XdClient.Instance.Nickname;
            ServerAdress = XdClient.Instance.HostName != null ? $"{XdClient.Instance.HostName}:{XdClient.Instance.PortName}" : "";
        }

        private void ClickConnectFunc()
        {
            XdClient.Instance.Nickname = Nickname;
            if (XdClient.Instance.Uuid == null) 
            {
                XdClient.Instance.Uuid = Guid.NewGuid().ToString();
            }

            if(XdConnection.TryParseEndpoint(ServerAdress, Constants.DefaultPort, out string host, out ushort port))
            {
                XdClient.Instance.HostName = host;
                XdClient.Instance.PortName = port;
            }
            
            XdScheduler.QueueAsyncTask(XdClient.Instance.Connect);

            // should be executed after client receives auth pack of server
            ClientConnectedFunc();
        }

        private bool ConnectButtonClickable()
        {
            return XdChatShared.Validation.IsValidNickname(Nickname) && XdChatShared.Validation.IsValidHostPort(ServerAdress);
        }
        
        protected virtual void PropChanged( string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}