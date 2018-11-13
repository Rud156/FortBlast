using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

namespace FortBlast.Extras
{
    public class ThreadedDataRequester : MonoBehaviour
    {
        #region Singleton

        private static ThreadedDataRequester _instance;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            if (_instance == null)
                _instance = this;

            if (_instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        private ConcurrentQueue<ThreadInfo> _dataQueue;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() => _dataQueue = new ConcurrentQueue<ThreadInfo>();


        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
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
            ThreadStart threadStart = new ThreadStart(() => _instance.DataThread(generateData, callback));
            new Thread(threadStart).Start();

        }

        private void DataThread(Func<object> generateData, Action<object> callback)
        {
            object data = generateData.Invoke();
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
    }
}