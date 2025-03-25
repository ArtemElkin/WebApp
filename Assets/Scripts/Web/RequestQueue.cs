using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

// Очередь для управления асинхронными задачами с возможностью отмены
public class RequestQueue
{
    // Внутренний класс для хранения задачи и её идентификатора
    private class QueuedTask
    {
        public Func<CancellationToken, UniTask> taskFunc; // Функция задачи
        public string taskId;                             // Уникальный ID задачи
        public QueuedTask(Func<CancellationToken, UniTask> func, string id)
        {
            taskFunc = func;
            taskId = id;
        }
    }

    private Queue<QueuedTask> _queue = new();         // Очередь задач
    private CancellationTokenSource _cts;             // Источник токена для отмены задач
    private bool _isProcessing = false;               // Флаг выполнения очереди

    // Добавляет задачу в очередь и запускает обработку, если она не активна
    public void Enqueue(Func<CancellationToken, UniTask> taskFunc, string taskId)
    {
        QueuedTask task = new QueuedTask(taskFunc, taskId);
        _queue.Enqueue(task);
        if (!_isProcessing)
            ProcessQueueAsync().Forget(); // Запускаем обработку асинхронно
    }
    
    // Удаляет задачу с указанным ID из очереди
    public void CancelAndRemove(string taskId)
    {
        _cts?.Cancel();  // Отменяем текущие задачи
        _cts?.Dispose(); // Очищаем старый токен
        _cts = new CancellationTokenSource(); // Создаём новый токен
        // Фильтруем очередь
        //_queue.Clear();
        _isProcessing = false; // Сбрасываем флаг обработки
    }
    
    // Асинхронно обрабатывает задачи из очереди
    private async UniTaskVoid ProcessQueueAsync()
    {
        _isProcessing = true;
        _cts?.Dispose();
        _cts = new CancellationTokenSource(); // Обновляем токен для новых задач

        while (_queue.Count > 0)
        {
            var currentTask = _queue.Dequeue();
            try
            {
                // Выполняем задачу с токеном отмены
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

        _isProcessing = false; // Очередь завершена
    }
    
    // Очищает ресурсы при уничтожении объекта
    private void OnDestroy()
    {
        _cts?.Dispose();
    }
}