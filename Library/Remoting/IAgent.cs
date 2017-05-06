using System;

using Regulus.Utility;

namespace Regulus.Remoting
{
    /// <summary>
    ///     �N�z��
    /// </summary>
    public interface IAgent : IUpdatable
    {
        /// <summary>
        ///     �P���ݵo���_�u
        ///     �I�sDisconnect���|�o�ͦ��ƥ�
        /// </summary>
        event Action BreakEvent;

        /// <summary>
        ///     �s�u���\�ƥ�
        /// </summary>
        event Action ConnectEvent;

        /// <summary>
        ///     Ping
        /// </summary>
        long Ping { get; }

        /// <summary>
        ///     �O�_���s�u���A
        /// </summary>
        bool Connected { get; }

        /// <summary>
        ///     �d�ߤ�������q����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        INotifier<T> QueryNotifier<T>();

        /// <summary>
        ///     �s�u
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="port"></param>
        /// <returns>�p�G�s�u���\�|�o��OnValue�Ǧ^true</returns>
        Value<bool> Connect(string ipaddress, int port);

        /// <summary>
        ///     �_�u
        /// </summary>
        void Disconnect();

        /// <summary>
        /// ���~����k�I�s
        /// �p�G�I�s����k�ѼƦ��~�h�|�^�Ǧ��T��.
        /// �ƥ�Ѽ�:
        ///     1.��k�W��
        ///     2.���~�T��
        /// �|�o�ͦ��T���q�`�O�]��client�Pserver�������ۮe�ҭP.
        /// </summary>
        event Action<string , string> ErrorMethodEvent;
    }
}