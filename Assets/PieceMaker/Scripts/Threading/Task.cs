using System.Collections.Generic;
using System;
using System.Threading;

namespace UnityThreading
{
    public abstract class TaskBase
    {
        private ManualResetEvent abortEvent = new ManualResetEvent(false);
        private ManualResetEvent endedEvent = new ManualResetEvent(false);
		private bool hasStarted = false;

		protected abstract void Do();

		/// <summary>
		/// Returns true if the task should abort. If a Task should abort and has not yet been started
		/// it will never start but indicate an end and failed state.
		/// </summary>
        public bool ShouldAbort
        {
            get
			{
				return abortEvent.WaitOne(0); 
			}
        }

		/// <summary>
		/// Returns true when processing of this task has been ended or has been skipped due early abortion.
		/// </summary>
        public bool HasEnded
        {
            get 
			{
				return endedEvent.WaitOne(0); 
			}
        }

		/// <summary>
		/// Returns true when the task has successfully been processed. Tasks which throw exceptions will
		/// not be set to a failed state, also any exceptions will not be catched, the user needs to add
		/// checks for these kind of situation.
		/// </summary>
        public bool IsSucceeded
        {
            get
            {
				return endedEvent.WaitOne(0) && !abortEvent.WaitOne(0);
            }
        }

		/// <summary>
		/// Returns true if the task should abort and has been ended. This value will not been set to true
		/// in case of an exception while processing this task. The user needs to add checks for these kind of situation.
		/// </summary>
        public bool IsFailed
        {
            get
            {
				return endedEvent.WaitOne(0) && abortEvent.WaitOne(0);
            }
        }

		/// <summary>
		/// Notifies the task to abort and sets the task state to failed. The task needs to check ShouldAbort if the task should  abort.
		/// </summary>
        public void Abort()
        {
			abortEvent.Set();
        }

		/// <summary>
		/// Notifies the task to abort and sets the task state to failed. The task needs to check ShouldAbort if the task should abort.
		/// This method will wait until the task has been aborted/ended.
		/// </summary>
        public void AbortWait()
		{
			Abort();
			Wait();
        }

		/// <summary>
		/// Notifies the task to abort and sets the task state to failed. The task needs to check ShouldAbort if the task should abort.
		/// This method will wait until the task has been aborted/ended or the given timeout has been reached.
		/// </summary>
		/// <param name="seconds">Time in seconds this method will max wait.</param>
        public void AbortWaitForSeconds(float seconds)
        {
			Abort();
			WaitForSeconds(seconds);
        }

		/// <summary>
		/// Blocks the calling thread until the task has been ended.
		/// </summary>
        public void Wait()
        {
			endedEvent.WaitOne();
        }

		/// <summary>
		/// Blocks the calling thread until the task has been ended or the given timeout value has been reached.
		/// </summary>
		/// <param name="seconds">Time in seconds this method will max wait.</param>
        public void WaitForSeconds(float seconds)
        {
			endedEvent.WaitOne(TimeSpan.FromSeconds(seconds));
        }

		/// <summary>
		/// Blocks the calling thread until the task has been ended and returns the return value of the task as the given type.
		/// Use this method only for Tasks with return values (functions)!
		/// </summary>
		/// <returns>The return value of the task as the given type.</returns>
		public abstract TResult Wait<TResult>();

		/// <summary>
		/// Blocks the calling thread until the task has been ended and returns the return value of the task as the given type.
		/// Use this method only for Tasks with return values (functions)!
		/// </summary>
		/// <param name="seconds">Time in seconds this method will max wait.</param>
		/// <returns>The return value of the task as the given type.</returns>
		public abstract TResult WaitForSeconds<TResult>(float seconds);

		/// <summary>
		/// Blocks the calling thread until the task has been ended and returns the return value of the task as the given type.
		/// Use this method only for Tasks with return values (functions)!
		/// </summary>
		/// <param name="seconds">Time in seconds this method will max wait.</param>
		/// <param name="defaultReturnValue">The default return value which will be returned when the task has failed.</param>
		/// <returns>The return value of the task as the given type.</returns>
		public abstract TResult WaitForSeconds<TResult>(float seconds, TResult defaultReturnValue);

        internal void DoInternal()
        {
			hasStarted = true;
            if (!ShouldAbort)
            {
                try
                {
                    Do();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e);
                }
            }
            endedEvent.Set();
        }

		/// <summary>
		/// Disposes this task and waits for completion if its still running.
		/// </summary>
        public void Dispose()
        {
			if (hasStarted)
				Wait();
			endedEvent.Close();
			abortEvent.Close();
        }
    }

    public class Task : TaskBase
    {
        private Action action;
		
        public Task(Action action)
        {
            this.action = action;
        }

		protected override void Do()
        {
            action();
        }

		public override TResult Wait<TResult>()
		{
			throw new InvalidOperationException("This task type does not support return values.");
		}

		public override TResult WaitForSeconds<TResult>(float seconds)
		{
			throw new InvalidOperationException("This task type does not support return values.");
		}

		public override TResult WaitForSeconds<TResult>(float seconds, TResult defaultReturnValue)
		{
			throw new InvalidOperationException("This task type does not support return values.");
		}
    }

    public class Task<T> : TaskBase
    {
        private Func<T> function;
        private T result;

        public Task(Func<T> function)
        {
            this.function = function;
        }

		protected override void Do()
        {
            result = function();
        }

		public override TResult Wait<TResult>()
		{
			return (TResult)(object)Result;
		}

		public override TResult WaitForSeconds<TResult>(float seconds)
		{
			return WaitForSeconds(seconds, default(TResult));
		}

		public override TResult WaitForSeconds<TResult>(float seconds, TResult defaultReturnValue)
		{
			if (!HasEnded)
				WaitForSeconds(seconds);
			if (IsSucceeded)
				return (TResult)(object)result;
			return defaultReturnValue;
		}
        
		/// <summary>
		/// Waits till completion and returns the operation result.
		/// </summary>
        public T Result
        {
            get
            {
                if (!HasEnded)
                    Wait();
                return result;
            }
        }
    }
}

