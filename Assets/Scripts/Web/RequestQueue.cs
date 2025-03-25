using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RequestQueue : MonoBehaviour
{
    private class QueuedTask
    {
        public Func<CancellationToken, UniTask> taskFunc;
        public string taskId;
        public QueuedTask(Func<CancellationToken, UniTask> func, string id)
        {
            taskFunc = func;
            taskId = id;
        }
    }
    private Queue<QueuedTask> _queue = new();
    private CancellationTokenSource _cts;
    private bool _isProcessing = false;


    public void Enqueue(Func<CancellationToken, UniTask> taskFunc, string taskId)
    {
        QueuedTask task = new QueuedTask(taskFunc, taskId);
        _queue.Enqueue(task);
        if (!_isProcessing)
            ProcessQueueAsync().Forget();
    }
    
    public void CancelAndRemove(string taskId)
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        _queue = new Queue<QueuedTask>(_queue.Where(t => t.taskId != taskId)); // Удаляем только задачи с заданным ID
        _queue.Clear();
        _isProcessing = false;
    }
    
    private async UniTaskVoid ProcessQueueAsync()
    {
        _isProcessing = true;
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        while (_queue.Count > 0)
        {
            var currentTask = _queue.Dequeue();
            try
            {
                await currentTask.taskFunc(_cts.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Задача с ID {currentTask.taskId} была отменена");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка в задаче {currentTask.taskId}: {ex.Message}");
            }
        }

        _isProcessing = false;
    }
    
    private void OnDestroy()
    {
        _cts?.Dispose();
    }
}

