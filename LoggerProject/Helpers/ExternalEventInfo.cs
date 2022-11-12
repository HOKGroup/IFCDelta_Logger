using Autodesk.Revit.DB;

namespace Helpers
{
    /// <summary>
    ///  Information used by ExternalEventHandler
    /// </summary>
    public abstract class ExternalEventInfo
    {
        /// <summary>
        ///  The name of the transaction
        /// </summary>
        public string TransactionName { get; set; }

        /// <summary>
        /// Provides access to an object that represents the currently active project.
        /// </summary>
        public Document CurrentDocument { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the execution fails or succeeds.
        /// </summary>
        /// <value>
        /// <c>true</c> if this execution is success; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Overload this method to implement external event.
        /// </summary>
        public abstract void Execute();
    }
}
