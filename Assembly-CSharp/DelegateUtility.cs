using System;
using System.Linq.Expressions;
using System.Reflection;

public static class DelegateUtility
{
	public static Delegate AddEventHandler(object target, string eventHandler, Action callback)
	{
		EventInfo eventInfo;
		eventInfo = target.GetType().GetEvent(eventHandler);
		Delegate obj;
		obj = DelegateUtility.CreateDelegate(eventInfo, callback);
		eventInfo.AddEventHandler(target, obj);
		return obj;
	}

	public static void RemoveEventHandler(object target, string eventHandler, Delegate handler)
	{
		target.GetType().GetEvent(eventHandler).RemoveEventHandler(target, handler);
	}

	public static Delegate CreateDelegate(EventInfo ev, Action action)
	{
		return Expression.Lambda(parameters: Array.ConvertAll(ev.EventHandlerType.GetMethod("Invoke").GetParameters(), (ParameterInfo x) => Expression.Parameter(x.ParameterType, x.Name)), body: Expression.Call(Expression.Constant(action.Target), action.Method), delegateType: ev.EventHandlerType).Compile();
	}
}
