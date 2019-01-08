using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

namespace FortBlast.Extras
{
    public class ThreadedDataRequester : MonoBehaviour
    {
        private ConcurrentQueue<ThreadInfo> _dataQueue;

        private void Start() => _dataQueue = new ConcurrentQueue<ThreadInfo>();


        /// <summary>
        ///     Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            if (_dataQueue.Count > 0)
            {
                ThreadInfo threadInfo;
                while (_dataQueue.TryDequeue(out threadInfo))
                    threadInfo.callback(threadInfo.parameter);
            }
        }

        public static void RequestData(Func<object> generateData, Action<object> callback)
        {
            ThreadStart threadStart = () => _instance.DataThread(generateData, callback);
            new Thread(threadStart).Start();
        }

        private void DataThread(Func<object> generateData, Action<object> callback)
        {
            var data = generateData.Invoke();
            _dataQueue.Enqueue(new ThreadInfo(callback, data));
        }

        private class ThreadInfo
        {
            public readonly Action<object> callback;
            public readonly object parameter;

            public ThreadInfo(Action<object> callback, object parameter)
            {
                this.callback = callback;
                this.parameter = parameter;
            }
        }

        #region Singleton

        private static ThreadedDataRequester _instance;

        /// <summary>
        ///     Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton
    }
}