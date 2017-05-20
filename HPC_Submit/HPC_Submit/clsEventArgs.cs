using System;

namespace HPC_Submit
{
	public delegate void MessageEventHandler(object sender, MessageEventArgs e);
	public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);

	public class MessageEventArgs : EventArgs
	{
		public readonly string Message;

		public MessageEventArgs(string message)
		{
			Message = message;
		}
	}

	public class ProgressEventArgs : EventArgs
	{
		public readonly string CurrentTask;

		/// <summary>
		/// Value between 0 and 100
		/// </summary>
		public readonly double PercentComplete;

		public readonly double HoursElapsed;

		public ProgressEventArgs(string currentTask, double percentComplete, double hoursElapsed)
		{
			CurrentTask = currentTask;
			PercentComplete = percentComplete;
			HoursElapsed = hoursElapsed;
		}
	}

}
