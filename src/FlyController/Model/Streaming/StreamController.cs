using System;
using System.Collections.Generic;

namespace FlyController.Streaming
{
    /// <summary>
    /// The stream controller.
    /// </summary>
    /// <typeparam name="TWriter">
    /// The type of the writer.
    /// </typeparam>
    public class StreamController<TWriter> : IStreamController<TWriter>
    {
        #region Fields

        /// <summary>
        ///     The _action.
        /// </summary>
        private readonly Action<TWriter, Queue<Action>> _action;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamController{TWriter}"/> class.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="action"/> is null.
        /// </exception>
        public StreamController(Action<TWriter, Queue<Action>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this._action = action;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Stream XML output to <paramref name="writer"/>
        /// </summary>
        /// <param name="writer">
        ///     The writer to write the output to
        /// </param>
        /// <param name="actions"></param>
        public void StreamTo(TWriter writer, Queue<Action> actions)
        {
            this._action(writer, actions);
        }

        #endregion
    }
}