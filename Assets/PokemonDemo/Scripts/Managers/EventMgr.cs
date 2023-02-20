using System;
using System.Collections.Generic;

namespace PokemonDemo.Scripts.Managers
{
    public class EventListenHandler
    {
        public bool Complete = false;
    }
    
    public class EventListenHandler<T> : EventListenHandler
    {
        public T Result;
    }
    
    public class EventListenHandler<T1, T2> : EventListenHandler
    {
        public Tuple<T1, T2> Result;
    }
    
    public class EventListenHandler<T1, T2, T3> : EventListenHandler
    {
        public Tuple<T1, T2, T3> Result;
    }
    
    public class EventListenHandler<T1, T2, T3, T4> : EventListenHandler
    {
        public Tuple<T1, T2, T3, T4> Result;
    }
    public class EventMgr
    {
        private static EventMgr _instance;

        public static EventMgr Instance
        {
            get { return _instance ??= new EventMgr(); }
        }

        private readonly Dictionary<string, Delegate> _listeners = new Dictionary<string, Delegate>();
        private readonly Dictionary<string, EventListenHandler> _listenHandlers = new Dictionary<string, EventListenHandler>();

        public void AddListener<T1, T2, T3, T4>(string evt, Action<T1, T2, T3, T4> callback)
        {
            AddListener(evt, (Delegate)callback);
        }

        public void AddListener<T1, T2, T3>(string evt, Action<T1, T2, T3> callback)
        {
            AddListener(evt, (Delegate)callback);
        }

        public void AddListener<T1, T2>(string evt, Action<T1, T2> callback)
        {
            AddListener(evt, (Delegate)callback);
        }

        public void AddListener<T>(string evt, Action<T> callback)
        {
            AddListener(evt, (Delegate)callback);
        }

        public void AddListener(string evt, Action callback)
        {
            AddListener(evt, (Delegate)callback);
        }

        public void AddListener(string evt, Delegate callback)
        {
            if (_listeners.TryGetValue(evt, out var listener))
            {
                _listeners[evt] = Delegate.Combine(listener, callback);
            }
            else
            {
                _listeners[evt] = callback;
            }
        }

        public void RemoveListener<T1, T2, T3, T4>(string evt, Action<T1, T2, T3, T4> callback)
        {
            RemoveListener(evt, (Delegate)callback);
        }

        public void RemoveListener<T1, T2, T3>(string evt, Action<T1, T2, T3> callback)
        {
            RemoveListener(evt, (Delegate)callback);
        }

        public void RemoveListener<T1, T2>(string evt, Action<T1, T2> callback)
        {
            RemoveListener(evt, (Delegate)callback);
        }

        public void RemoveListener<T>(string evt, Action<T> callback)
        {
            RemoveListener(evt, (Delegate)callback);
        }

        public void RemoveListener(string evt, Action callback)
        {
            RemoveListener(evt, (Delegate)callback);
        }

        private void RemoveListener(string evt, Delegate callback)
        {
            if (_listeners.TryGetValue(evt, out var listener))
            {
                listener = Delegate.Remove(listener, callback);
                if (listener == null)
                {
                    _listeners.Remove(evt);
                }
                else
                {
                    _listeners[evt] = listener;
                }
            }
        }

        public void Dispatch<T1, T2, T3, T4>(string evt, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var methods = GetMethods(evt);
            if (methods != null)
            {
                foreach (var m in methods)
                {
                    try
                    {
                        if (m.Target != null) ((Action<T1, T2, T3, T4>)m)(arg1, arg2, arg3, arg4);
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                }
            }
            if (_listenHandlers.ContainsKey(evt))
            {
                if (_listenHandlers[evt] is EventListenHandler<T1, T2, T3, T4> handler)
                {
                    handler.Complete = true;
                    handler.Result = new Tuple<T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
                }               
                else
                {
                    UnityEngine.Debug.LogError("Can cast like this: " + evt);
                }

                _listenHandlers.Remove(evt);
            }
        }

        public void Dispatch<T1, T2, T3>(string evt, T1 arg1, T2 arg2, T3 arg3)
        {
            var methods = GetMethods(evt);
            if (methods != null)
            {
                foreach (var m in methods)
                {
                    try
                    {
                        if (m.Target != null) ((Action<T1, T2, T3>)m)(arg1, arg2, arg3);
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                }
            }
            if (_listenHandlers.ContainsKey(evt))
            {
                if (_listenHandlers[evt] is EventListenHandler<T1, T2, T3> handler)
                {
                    handler.Complete = true;
                    handler.Result = new Tuple<T1, T2, T3>(arg1, arg2, arg3);
                }                
                else
                {
                    UnityEngine.Debug.LogError("Can cast like this: " + evt);
                }

                _listenHandlers.Remove(evt);
            }
        }

        public void Dispatch<T1, T2>(string evt, T1 arg1, T2 arg2)
        {
            var methods = GetMethods(evt);
            if (methods != null)
            {
                foreach (var m in methods)
                {
                    try
                    {
                        if (m.Target != null) ((Action<T1, T2>)m)(arg1, arg2);
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                }
            }
            if (_listenHandlers.ContainsKey(evt))
            {
                if (_listenHandlers[evt] is EventListenHandler<T1, T2> handler)
                {
                    handler.Complete = true;
                    handler.Result = new Tuple<T1, T2>(arg1, arg2);
                }                
                else
                {
                    UnityEngine.Debug.LogError("Can cast like this: " + evt);
                }

                _listenHandlers.Remove(evt);
            }
        }

        public void Dispatch<T>(string evt, T arg)
        {
            var methods = GetMethods(evt);
            if (methods != null)
            {
                foreach (var m in methods)
                {
                    try
                    {
                        if (m.Target != null) ((Action<T>)m)(arg);
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                }
            }
            if (_listenHandlers.ContainsKey(evt))
            {
                if (_listenHandlers[evt] is EventListenHandler<T> handler)
                {
                    handler.Complete = true;
                    handler.Result = arg;
                }
                else
                {
                    UnityEngine.Debug.LogError("Can cast like this: " + evt);
                }

                _listenHandlers.Remove(evt);
            }
        }

        public void Dispatch(string evt)
        {
            var methods = GetMethods(evt);
            if (methods != null)
            {
                foreach (var m in methods)
                {
                    try
                    {
                        if (m.Target != null) ((Action)m)();
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                }
            }
            if (_listenHandlers.ContainsKey(evt))
            {
                _listenHandlers[evt].Complete = true;
                _listenHandlers.Remove(evt);
            }
        }

        public EventListenHandler ListenTo(string evt)
        {
            if (_listenHandlers.ContainsKey(evt))
                return _listenHandlers[evt];
            EventListenHandler temp = new EventListenHandler();
            _listenHandlers.Add(evt, temp);
            return temp;
        }
        
        public EventListenHandler<T> ListenTo<T>(string evt)
        {
            if (_listenHandlers.ContainsKey(evt))
                return (EventListenHandler<T>)_listenHandlers[evt];
            var temp = new EventListenHandler<T>();
            _listenHandlers.Add(evt, temp);
            return temp;
        }
        
        public EventListenHandler<T1, T2> ListenTo<T1, T2>(string evt)
        {
            if (_listenHandlers.ContainsKey(evt))
                return (EventListenHandler<T1, T2>)_listenHandlers[evt];
            var temp = new EventListenHandler<T1, T2>();
            _listenHandlers.Add(evt, temp);
            return temp;
        }
        
        public EventListenHandler<T1, T2, T3> ListenTo<T1, T2, T3>(string evt)
        {
            if (_listenHandlers.ContainsKey(evt))
                return (EventListenHandler<T1, T2, T3>)_listenHandlers[evt];
            var temp = new EventListenHandler<T1, T2, T3>();
            _listenHandlers.Add(evt, temp);
            return temp;
        }
        
        public EventListenHandler<T1, T2, T3, T4> ListenTo<T1, T2, T3, T4>(string evt)
        {
            if (_listenHandlers.ContainsKey(evt))
                return (EventListenHandler<T1, T2, T3, T4>)_listenHandlers[evt];
            var temp = new EventListenHandler<T1, T2, T3, T4>();
            _listenHandlers.Add(evt, temp);
            return temp;
        }


        private Delegate[] GetMethods(string evt)
        {
            return !_listeners.TryGetValue(evt, out var listener) ? null : listener.GetInvocationList();
        }

        private static void LogError(Exception e)
        {
            UnityEngine.Debug.LogError(e);
        }
    }
}