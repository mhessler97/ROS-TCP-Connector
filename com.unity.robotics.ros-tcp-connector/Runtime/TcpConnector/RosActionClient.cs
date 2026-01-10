using System;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace Unity.Robotics.ROSTCPConnector
{
    /// <summary>
    /// Lightweight wrapper around <see cref="ROSConnection"/> to manage a ROS action client.
    /// </summary>
    /// <typeparam name="TGoal">Action goal type.</typeparam>
    /// <typeparam name="TFeedback">Action feedback type.</typeparam>
    /// <typeparam name="TResult">Action result type.</typeparam>
    public class RosActionClient<TGoal, TFeedback, TResult>
        where TGoal : Message
        where TFeedback : Message
        where TResult : Message
    {
        readonly ROSConnection m_Connection;

        public string ActionName { get; }
        public string ActionType { get; }

        public RosActionClient(ROSConnection connection, string actionName, string actionType)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (string.IsNullOrEmpty(actionName)) throw new ArgumentException("Action name must be set.", nameof(actionName));
            if (string.IsNullOrEmpty(actionType)) throw new ArgumentException("Action type must be set.", nameof(actionType));

            m_Connection = connection;
            ActionName = actionName;
            ActionType = actionType;

            m_Connection.RegisterRosActionClient(ActionName, ActionType);
        }

        public void Listen(Action<string, TFeedback> onFeedback, Action<string, TResult> onResult)
        {
            m_Connection.ListenForAction(ActionName, onFeedback, onResult);
        }

        public ROSConnection.ActionGoalSendResult SendGoal(TGoal goal, string goalId = null, float timeoutSeconds = 30.0f)
        {
            return m_Connection.SendActionGoal(ActionName, goal, goalId, timeoutSeconds);
        }

        public void CancelGoal(string goalId)
        {
            if (string.IsNullOrEmpty(goalId))
            {
                throw new ArgumentException("Goal ID is required to cancel an action goal.", nameof(goalId));
            }

            m_Connection.CancelActionGoal(ActionName, goalId);
        }
    }
}
