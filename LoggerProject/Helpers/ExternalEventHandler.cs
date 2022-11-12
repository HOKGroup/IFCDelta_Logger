using Autodesk.Revit.UI;

namespace Helpers
	{
	/// <summary>
	///  A class to manage the safe execution of events in a modeless dialog.
	/// </summary>
	/// <seealso cref="Autodesk.Revit.UI.IExternalEventHandler" />
	public class ExternalEventHandler : IExternalEventHandler
		{
		public static ExternalEvent ExternalEventInstance;
		public static ExternalEventHandler HandlerInstance;
		public ExternalEventInfo EventInfo;

		/// <summary>
		/// This method is called to handle the external event.
		/// </summary>
		/// <param name="app"></param>
		void IExternalEventHandler.Execute(UIApplication app)
			{
			if (EventInfo != null)
				{
				EventInfo.Execute();
				}
			EventInfo = null;
			}

		/// <summary>
		/// String identification of the event handler.
		/// </summary>
		/// <returns>
		/// The event's name
		/// </returns>
		string IExternalEventHandler.GetName()
			{
			return "External Event";
			}

		/// <summary>
		///	Creates external events.
		/// </summary>
		public static void CreateEvent()
			{
			if (HandlerInstance == null)
				{
				HandlerInstance = new ExternalEventHandler();
				ExternalEventInstance = ExternalEvent.Create(HandlerInstance);
				}
			}

		/// <summary>
		///	Raise the external event.
		/// </summary>
		public void Raise()
			{
			ExternalEventInstance.Raise();
			}
		}
	}